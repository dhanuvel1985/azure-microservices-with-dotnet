using Clinic_Management_Api.Application;
using Clinic_Management_Api.DataAccess;
using Clinic_Management_Api.ExternalServices;
using Microsoft.EntityFrameworkCore;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<PetManagementService>();
builder.Services.AddScoped<ClinicApplicationService>();
builder.Services.AddDbContext<ClinicDbContext>(option =>
{
    option.UseInMemoryDatabase("ClinicManagement");
});

builder.Services.AddHttpClient<PetManagementService>(client =>
{
    var uri = builder.Configuration.GetValue<string>("Pet__ManagementUri") ??
              builder.Configuration.GetValue<string>("Pet:ManagementUri");
    client.BaseAddress = new Uri(uri);
})
.ConfigurePrimaryHttpMessageHandler(() =>
        new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        })
.AddResilienceHandler("pet-management-pipeline", builder =>
        {
            builder.AddRetry(new Polly.Retry.RetryStrategyOptions<HttpResponseMessage>()
            {
                BackoffType = DelayBackoffType.Exponential,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(10)
            });
          }); 
var app = builder.Build();

app.EnsureDbIsCreated();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
