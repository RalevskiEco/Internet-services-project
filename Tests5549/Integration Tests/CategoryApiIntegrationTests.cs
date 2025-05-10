using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace Tests5549.Integration_Tests
{
    public class CategoryApiIntegrationTests : IClassFixture<WebApplicationFactory<App5549.Program>>
    {
        private readonly HttpClient _client;

        public CategoryApiIntegrationTests(WebApplicationFactory<App5549.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostCategory_WithValidData_ReturnsCreated()
        {
            var category = new { name = "Networking", description = "Switches and routers" };
            var content = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/categories", content);

            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("Networking", body);
        }
    }
}
