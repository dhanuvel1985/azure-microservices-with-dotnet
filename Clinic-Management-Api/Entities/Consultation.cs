namespace Clinic_Management_Api.Entities
{
    public record Consultation(Guid Id, int PatientId, string PatientName, int Age, DateTime StartDate)
    {
    }
}
