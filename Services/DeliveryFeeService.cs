using DependencyStore.Services.Contracts;
using RestSharp;

namespace DependencyStore.Services
{
    public class DeliveryFeeService : IDeliveryFeeService
    {
        public async Task<decimal> GetDeliveryFeeAsync(string zipCode)
        {
            var client = new RestClient("https://api.myorg.com"); // indo na api
            var request = new RestRequest()
                .AddJsonBody(new
                {
                    ZipCode = zipCode
                });
            var response = await client.PostAsync<decimal>(request);
            return response < 5 ? 5 : response; //if 
        }
    }
}
