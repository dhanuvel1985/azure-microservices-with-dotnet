using Clinic_Management_Api.Controllers;
using Clinic_Management_Api.DataAccess;
using Clinic_Management_Api.Entities;
using Clinic_Management_Api.ExternalServices;

namespace Clinic_Management_Api.Application
{
    public class ClinicApplicationService(ClinicDbContext context, PetManagementService service)
    {
        public async Task<Consultation> Handle(StartConsulationCommand command)
        {
            var petInfo = await service.GetPetInfo(command.PatientId);

            var newConsulation = new Consultation(Guid.NewGuid(), command.PatientId,petInfo.Name, petInfo.Age, DateTime.UtcNow);
            await context.Consultations.AddAsync(newConsulation);
            await context.SaveChangesAsync();

            return newConsulation;
        }
    }
}
