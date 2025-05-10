using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace Tests5549.Integration_Tests
{
    public class StockImportIntegrationTests : IClassFixture<WebApplicationFactory<App5549.Program>>
    {
        private readonly HttpClient _client;

        public StockImportIntegrationTests(WebApplicationFactory<App5549.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostStockImport_WithNewItems_ReturnsSuccessMessage()
        {
            var stock = new[]
            {
                new { Name = "Intel's Core i9-9900K", Categories = new[] { "CPU" }, Price = 475.99, Quantity = 2 },
                new { Name = "Razer BlackWidow Keyboard", Categories = new[] { "Keyboard", "Periphery" }, Price = 89.99, Quantity = 10 }
            };
            var content = new StringContent(JsonConvert.SerializeObject(stock), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/stock/import", content);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("Stock imported successfully", body);
        }
    }
}
