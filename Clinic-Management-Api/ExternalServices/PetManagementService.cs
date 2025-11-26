using System.Dynamic;

namespace Clinic_Management_Api.ExternalServices
{
    public class PetManagementService(HttpClient client)
    {
        public async Task<PetInfo> GetPetInfo(int Id)
        {
            var petInfo = await client.GetFromJsonAsync<PetInfo>($"/api/Pets/{Id}");
            return petInfo;
        }
    }

    public record PetInfo(int Id, string Name, int Age, int BreedId);
}
