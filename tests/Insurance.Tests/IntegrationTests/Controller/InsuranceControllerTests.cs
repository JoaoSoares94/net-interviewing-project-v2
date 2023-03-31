using Insurance.Api.Model;
using Insurance.Api.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace Insurance.Api.IntegrationTests.Controllers
{
    public class InsuranceControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public InsuranceControllerTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CalculateInsurance_ValidProduct_ReturnsCreated()
        {
            // Arrange
            var product = new InsuranceDto { ProductId = 572770 };
            var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/insurance/insurance/product", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CalculateInsurance_EmptyProduct_ReturnsInternalServerError()
        {
            // Arrange
            var product = new InsuranceDto();
            var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/insurance/insurance/product", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task CalculateOrder_ValidOrder_ReturnsCreated()
        {
            // Arrange
            List<InsuranceDto> list = new List<InsuranceDto>();
            var dto = new InsuranceDto() { ProductId = 572770 };
            list.Add(dto);
            var order = new OrderDto
            {
                Items = list
            };
            var content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/insurance/insurance/orders", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CalculateOrder_InvalidOrder_ReturnsBadRequest()
        {
            // Arrange
            var order = new OrderDto();
            var content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/insurance/insurance/orders", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UploadSurcharge_ValidSurcharge_ReturnsCreated()
        {
            // Arrange
            var content = new StringContent("", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/insurance/insurance/surcharge/1/0.5", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }


        [Fact]
        public async Task GetById_ExistingId_ReturnsInsuranceDto()
        {
            // Arrange
            // Id= 1, ProductId = 572770, Name = "Samsung WW80J6400CW EcoBubble", SalesPrice = 475, ProductTypeHasInsurance = true, ProductTypeId = 1, ProductTypeName = "Washing machines"
            var id = 1;
            var productId = 572770;

            // Act
            var response = await _client.GetAsync($"/api/insurance/insurance/product/{id}");
            var responseContent = await response.Content.ReadAsStringAsync();
            var insuranceDto = JsonConvert.DeserializeObject<InsuranceDto>(responseContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(insuranceDto);
            Assert.Equal(productId, insuranceDto.ProductId);
        }

        [Fact]
        public async Task GetById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            //Does not exist
            var id = 999;

            // Act
            var response = await _client.GetAsync($"/api/insurance/insurance/product/{id}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAll_ExistingProducts_ReturnsListOfInsuranceDto()
        {
            // Arrange
            // Id= 1, ProductId = 572770, Name = "Samsung WW80J6400CW EcoBubble", SalesPrice = 475, ProductTypeHasInsurance = true, ProductTypeId = 1, ProductTypeName = "Washing machines"
            //Id = 2,  ProductId = 715990,ProductTypeId = 2,Name = "Canon Powershot SX620 HS Black",SalesPrice = 195,ProductTypeHasInsurance = true,ProductTypeName = "Digital cameras",InsuranceValue = 500

            var expectedCount = 2;

            // Act
            var response = await _client.GetAsync("/api/insurance/insurance/products");
            var responseContent = await response.Content.ReadAsStringAsync();
            var insuranceDtos = JsonConvert.DeserializeObject<IEnumerable<InsuranceDto>>(responseContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(insuranceDtos);
            Assert.Equal(expectedCount, insuranceDtos.Count());
        }
    }
}
