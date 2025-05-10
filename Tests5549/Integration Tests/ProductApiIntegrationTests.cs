using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace Tests5549.Integration_Tests
{
    public class ProductApiIntegrationTests : IClassFixture<WebApplicationFactory<App5549.Program>>
    {
        private readonly HttpClient _client;

        public ProductApiIntegrationTests(WebApplicationFactory<App5549.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostProduct_WithValidData_ReturnsCreated()
        {
            var product = new
            {
                name = "Intel's Core i9-9900K",
                description = "High-end CPU",
                price = 475.99,
                categories = new[] { "CPU" }
            };
            var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/products", content);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("Intel's Core i9-9900K", body);
        }

        [Fact]
        public async Task PostProduct_WithoutName_ReturnsBadRequest()
        {
            var product = new
            {
                name = "",
                description = "Missing name",
                price = 100,
                categories = new[] { "CPU" }
            };
            var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/products", content);

            Assert.False(response.IsSuccessStatusCode);

            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("name", body, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
