using Insurance.Api.Model;
using Insurance.Api.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Insurance.Tests.UnitTests.Controller
{
    public class InsuranceControllerTests
    {
        private Mock<IInsuranceService> _mockInsuranceService = new Mock<IInsuranceService>();
        private InsuranceController _controller;
        public InsuranceControllerTests()
        {
            _controller = new InsuranceController(_mockInsuranceService.Object);
        }

        [Fact]
        public async Task GetById_Returns_Ok_When_Product_Exists()
        {
            // Arrange
            int productId = 1;
            var insuranceDto = new InsuranceDto { ProductId = productId };
            _mockInsuranceService.Setup(product => product.GetById(productId)).ReturnsAsync(insuranceDto);

            // Act
            var result = await _controller.GetById(productId);
            var okResult = result.Result as OkObjectResult;


            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(insuranceDto, okResult.Value);
        }

        [Fact]
        public async Task GetById_Returns_NotFound_When_Product_Does_Not_Exist()
        {
            // Arrange
            int productId = 1;
            var expected = "The product does not exist";
            _mockInsuranceService.Setup(x => x.GetById(productId)).ReturnsAsync(() => null);

            // Act
            var result = await _controller.GetById(productId);
            var notFoundResult = result.Result as NotFoundObjectResult;
            //Get the message from the object created
            object message = notFoundResult.Value?.GetType().GetProperty("message")?.GetValue(notFoundResult.Value, null);

            // Assert

            // Actually NotFoundObject 
            Assert.IsType<NotFoundObjectResult>(result.Result);


            //The status message should be 404
            Assert.Equal(404, notFoundResult.StatusCode);

            //The message should be the one passed in the constructor
            Assert.Equal(expected, message);

        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenInsuranceDtosExist()
        {
            // Arrange
            var expected = 2;
            var insuranceDtos = new List<InsuranceDto>
        {
            new InsuranceDto { ProductId = 1, ProductTypeName = "ProductType 1" },
            new InsuranceDto { ProductId = 2, ProductTypeName = "ProductType 2" },
        };
            _mockInsuranceService.Setup(x => x.GetAll()).ReturnsAsync(insuranceDtos);


            // Act
            var result = await _controller.GetAll();
            var okResult = result.Result as OkObjectResult;
            var model = okResult.Value as List<InsuranceDto>;

            // Assert

            //Object shouldn't be null
            Assert.NotNull(okResult);

            //Controller should generator 200(OK) status code
            Assert.Equal(200, okResult.StatusCode);


            //Response should have 2 Insurance Dto instances 
            Assert.NotNull(model);
            Assert.Equal(expected, model.Count);
        }

        [Fact]
        public async Task GetAll_ReturnsNotFound_WhenInsuranceDtosDoNotExist()
        {
            // Arrange
            _mockInsuranceService.Setup(x => x.GetAll()).ReturnsAsync(() => null);

            // Act
            var result = await _controller.GetAll();
            var notFoundResult = result.Result as NotFoundObjectResult;
            object message = notFoundResult.Value?.GetType().GetProperty("message")?.GetValue(notFoundResult.Value, null);

            // Assert

            // Actually NotFoundObject 
            Assert.IsType<NotFoundObjectResult>(result.Result);

            //NotFoundObject should have a message logging the error
            Assert.NotNull(notFoundResult);

            //The status code should be 404
            Assert.Equal(404, notFoundResult.StatusCode);

            //The message should be the one passed in the constructor
            Assert.NotNull(message);

            Assert.Equal("The database doesn't have any product registered!", message);
        }

        [Theory]
        [InlineData(1, 100)]
        [InlineData(2, 500)]
        [InlineData(3, 2500)]

        public async Task CalculateInsurance_WithValidProduct_ReturnsCreatedResult(int productId, int insuranceValue)
        {
            // Arrange
            var product = new InsuranceDto() { ProductId = productId };
            var expectedResult = new InsuranceDto { ProductId = productId, ProductTypeHasInsurance = true, InsuranceValue = insuranceValue };

            _mockInsuranceService.Setup(x => x.CalculateInsurance(productId))
                .Returns(Task.FromResult(expectedResult));

            // Act
            var result = await _controller.CalculateInsurance(product);
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            var insuranceDto = createdAtActionResult.Value as InsuranceDto;

            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);

            Assert.Equal(expectedResult, insuranceDto);
        }

        [Fact]
        public async Task CalculateInsurance_WithInvalidProduct_ReturnsBadRequest()
        {
            // Arrange
            InsuranceDto product = null;
            string expected = "Please provide a valid product";


            // Act
            var result = await _controller.CalculateInsurance(product);
            var badRequestResult = result.Result as BadRequestObjectResult;
            object message = badRequestResult.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null);



            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);

            Assert.Equal(expected, message);
        }

        [Fact]
        public async Task CalculateInsurance_WithNullResult_ReturnsBadRequest()
        {
            // Arrange
            var product = new InsuranceDto { ProductId = 1 };
            string expected = "Couldn't calculate insurance";


            _mockInsuranceService.Setup(x => x.CalculateInsurance(product.ProductId))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.CalculateInsurance(product);
            var badRequestResult = result.Result as BadRequestObjectResult;

            object message = badRequestResult.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null);


            // Assert
            Assert.IsType<BadRequestObjectResult>(badRequestResult);

            Assert.Equal(expected, message);
        }
        [Fact]
        public async Task CalculateInsurance_WithProductHasInsuranceFalseResult_ReturnsBadRequest()
        {
            // Arrange
            var product = new InsuranceDto { ProductId = 1, ProductTypeHasInsurance = false };
            string expected = "Product cannot be insured";


            _mockInsuranceService.Setup(x => x.CalculateInsurance(product.ProductId))
                .ReturnsAsync(product);

            // Act
            var result = await _controller.CalculateInsurance(product);
            var badRequestResult = result.Result as BadRequestObjectResult;

            object message = badRequestResult.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null);


            // Assert
            Assert.IsType<BadRequestObjectResult>(badRequestResult);

            Assert.Equal(expected, message);
        }

        [Theory]
        [InlineData(1, 20.7f)]
        public async Task UploadSurcharge_Returns_CreatedAtAction_With_SurchargeDto(int productTypeId, float surcharge)
        {
            // Arrange
            List<InsuranceDto> list = new List<InsuranceDto>()
            {
                new InsuranceDto {ProductId = 1}
            };

            var surchargeDto = new SurchargeDto { ProductTypeId = productTypeId, SurchargeRate = surcharge };
            _mockInsuranceService.Setup(x => x.UploadSurchargeRate(productTypeId, surcharge))
                                 .ReturnsAsync(surchargeDto);
            _mockInsuranceService.Setup(x => x.UpdateInsuranceValue(productTypeId, surcharge))
                                 .ReturnsAsync(list);

            // Act
            var result = await _controller.UploadSurcharge(productTypeId, surcharge);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var model = Assert.IsAssignableFrom<SurchargeDto>(createdAtActionResult.Value);
            Assert.Equal(productTypeId, model.ProductTypeId);
            Assert.Equal(surcharge, model.SurchargeRate);
        }

        [Fact]
        public async Task UploadSurcharge_Returns_BadRequest_When_InsuranceService_Returns_Null()
        {
            // Arrange
            int id = 1;
            float surcharge = 0.5f;
            _mockInsuranceService.Setup(x => x.UploadSurchargeRate(id, surcharge))
                                 .ReturnsAsync(() => null);

            // Act
            var result = await _controller.UploadSurcharge(id, surcharge);
            var badRequestResult = result.Result as BadRequestObjectResult;


            // Assert
            Assert.IsType<BadRequestObjectResult>(badRequestResult);
            object message = badRequestResult.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null);

            Assert.Equal("Couldn't upload surcharge", message);
        }

        [Fact]
        public async Task UploadSurcharge_Returns_Problem_When_UpdateInsuranceValue_Returns_Null()
        {
            // Arrange
            int id = 1;
            float surcharge = 0.5f;
            var surchargeDto = new SurchargeDto { ProductTypeId = id, SurchargeRate = surcharge };
            _mockInsuranceService.Setup(x => x.UploadSurchargeRate(id, surcharge))
                                 .ReturnsAsync(surchargeDto);
            _mockInsuranceService.Setup(x => x.UpdateInsuranceValue(id, surcharge))
                                 .ReturnsAsync(() => null);

            // Act
            var result = await _controller.UploadSurcharge(id, surcharge);
            var problemResult = result.Result as ObjectResult;
            object message = problemResult.Value.GetType().GetProperty("Detail")?.GetValue(problemResult.Value, null);

            // Assert
            Assert.IsType<ObjectResult>(problemResult);
            Assert.Equal("Couldn't update the products already in the database", message);
        }

        [Fact]
        public async Task UploadSurcharge_WhenUpdateInsuranceValueFails_ReturnsProblem()
        {
            // Arrange
            int id = 1;
            float surcharge = 0.1f;
            var mockInsuranceService = new Mock<IInsuranceService>();
            var surchargeDto = new SurchargeDto { ProductTypeId = id, SurchargeRate = surcharge };
            _mockInsuranceService.Setup(service => service.UploadSurchargeRate(id, surcharge)).ReturnsAsync(surchargeDto);
            _mockInsuranceService.Setup(service => service.UpdateInsuranceValue(id, surcharge)).ReturnsAsync(It.IsAny<List<InsuranceDto>>);

            // Act
            var result = await _controller.UploadSurcharge(id, surcharge);
            var problemResult = result.Result as ObjectResult;
            object message = problemResult.Value.GetType().GetProperty("Detail")?.GetValue(problemResult.Value, null);
            // Assert
            Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, problemResult.StatusCode);
            Assert.Equal("Couldn't update the products already in the database", message);
        }

        [Theory]
        [InlineData(1,-0.5)]
        [InlineData(1,0)]
        [InlineData(1,-500)]
        public async Task UploadSurcharge_WhenSurchargeIsInvalid_ReturnsBadRequest(int id, float surcharge)
        {
            // Arrange

            var mockInsuranceService = new Mock<IInsuranceService>();
            var surchargeDto = new SurchargeDto { ProductTypeId = id, SurchargeRate = surcharge };
            _mockInsuranceService.Setup(service => service.UploadSurchargeRate(id, surcharge)).ReturnsAsync(surchargeDto);
            _mockInsuranceService.Setup(service => service.UpdateInsuranceValue(id, surcharge)).ReturnsAsync(It.IsAny<List<InsuranceDto>>);

            // Act
            var result = await _controller.UploadSurcharge(id, surcharge);
            var problemResult = result.Result as BadRequestObjectResult;
            object message = problemResult.Value.GetType().GetProperty("message")?.GetValue(problemResult.Value, null);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, problemResult.StatusCode);
            Assert.Equal("Please provide a valid surcharge value greater than 0", message);

        }
    }
}


