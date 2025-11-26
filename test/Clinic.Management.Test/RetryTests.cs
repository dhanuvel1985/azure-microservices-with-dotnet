using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Polly;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

public class RetryTests
{

    private readonly WireMockServer _mockServer;

    public RetryTests()
    {
        _mockServer = WireMockServer.Start();
    }

    public async ValueTask DisposeAsync()
    {
        _mockServer.Stop();
        _mockServer.Dispose();
    }

    [Fact]
    public async Task Endpoint_Should_Retry_3_Times_When_Downstream_Fails()
    {
        // Arrange
        // 1st and 2nd calls return 500, 3rd call returns 200
        _mockServer
            .Given(Request.Create().WithPath("/Pets/1").UsingGet())
            .InScenario("RetryScenario")
            .WillSetStateTo("SecondCall")
            .RespondWith(
                Response.Create()
                .WithStatusCode(500)
            );

        _mockServer
            .Given(Request.Create().WithPath("/Pets/1").UsingGet())
            .InScenario("RetryScenario")
            .WhenStateIs("SecondCall")
            .WillSetStateTo("ThirdCall")
            .RespondWith(
                Response.Create()
                .WithStatusCode(500)
            );

        _mockServer
            .Given(Request.Create().WithPath("/Pets/1").UsingGet())
            .InScenario("RetryScenario")
            .WhenStateIs("ThirdCall")
            .RespondWith(
                Response.Create()
                .WithStatusCode(200)
                .WithBody("{ \"id\": 1, \"name\": \"Gianni\" }")
            );

        var appFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    // inject mock server base URL
                    Environment.SetEnvironmentVariable("ExternalApiBaseUrl", _mockServer.Url);
                });
            });

        var client = appFactory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/Pets/1");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Gianni", content);

        // MOST IMPORTANT: validate number of requests (retry attempts)
        Assert.Equal(3, _mockServer.LogEntries.Count);
    }

    //[Fact]
    //public async Task PollyRetry_Should_Retry_Twice_And_Then_Succeed()
    //{
    //    // Arrange
    //    var handler = new FakeRetryHandler();
    //    var client = new HttpClient(handler)
    //    {
    //        BaseAddress = new Uri("https://localhost:7124")
    //    };

    //    var policy = Policy
    //        .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
    //        .RetryAsync(2);  // Retry twice

    //    // Act
    //    var result = await policy.ExecuteAsync(() => client.GetAsync("/api/Pets/1"));

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    //    Assert.Equal(3, handler.CallCount); // 1 original + 2 retries
    //}
}
