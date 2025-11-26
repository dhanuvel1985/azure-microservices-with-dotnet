using Clinic_Management_Api.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Management_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultationController(ClinicApplicationService service) : ControllerBase
    {
        [HttpPost("/start")]
        public async Task<IActionResult> Start(StartConsulationCommand command)
        {
            var result = await service.Handle(command);
            return Ok(result);
        }
    }

    public record StartConsulationCommand(int PatientId);
}
