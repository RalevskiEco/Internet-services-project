using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;

namespace Tests5549.Integration_Tests
{
    public class DiscountApiIntegrationTests : IClassFixture<WebApplicationFactory<App5549.Program>>
    {
        private readonly HttpClient _client;

        public DiscountApiIntegrationTests(WebApplicationFactory<App5549.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostDiscount_WithValidBasket_ReturnsDiscount()
        {
            var basket = new[]
            {
                new { Name = "Intel's Core i9-9900K", Quantity = 2 },
                new { Name = "Razer BlackWidow Keyboard", Quantity = 1 }
            };
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(basket), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/discount/calculate", content);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("discount", body, System.StringComparison.OrdinalIgnoreCase);
        }

        
    }
}
