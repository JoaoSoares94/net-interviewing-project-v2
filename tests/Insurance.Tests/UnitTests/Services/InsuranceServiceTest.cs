using Insurance.Api.Data;
using Insurance.Api.Model;
using Insurance.Api.Repositories.InsuranceRepository;
using Insurance.Api.Repositories.OrderRepo;
using Insurance.Api.Repositories.SurchargeRateRepo;
using Insurance.Api.Services.Dto;
using Insurance.Api.Services.Shared;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Insurance.Tests.UnitTests.Services
{
    public class InsuranceServiceTests
    {
        private readonly Mock<IBusinessRules> _businessRulesMock = new Mock<IBusinessRules>();
        private readonly Mock<IInsuranceRepo> _insuranceRepoMock = new Mock<IInsuranceRepo>();
        private readonly Mock<IOrderRepo> _orderRepoMock = new Mock<IOrderRepo>();
        private readonly Mock<ISurchargeRateRepo> _surchargeRepoMock = new Mock<ISurchargeRateRepo>();
        private readonly Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork>();

        private readonly InsuranceService _insuranceService;


        public InsuranceServiceTests()
        {
            _insuranceService = new InsuranceService(_businessRulesMock.Object, _insuranceRepoMock.Object, _surchargeRepoMock.Object,_unitOfWork.Object, _orderRepoMock.Object);
        }

        [Theory]
        [InlineData(1, 1, "Laptops")]
        [InlineData(2, 2, "Smartphones")]
        public async Task CalculateInsurance_WithValidProductId_ShouldCalculateInsurance(int productId, int productTypeId, string name)
        {
            // Arrange

            ProductDto productDto = new() { id = productId, productTypeId = productTypeId, salesPrice = 1000 };
            ProductTypeDto productTypeDto = new() { id = productTypeId, name = name, canBeInsured = true };
            _businessRulesMock.Setup(x => x.GetProductById(productId)).Returns(productDto);
            _businessRulesMock.Setup(x => x.GetProductType(productDto.productTypeId)).Returns(productTypeDto);
            InsuredProduct insured = new(productDto, productTypeDto);
            insured.InsuranceValue = 1500;
            _insuranceRepoMock.Setup(x => x.Add(It.IsAny<InsuredProduct>())).Returns(Task.FromResult(insured));

            // Act
            var result = await _insuranceService.CalculateInsurance(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal(productTypeDto.name, result.ProductTypeName);
            Assert.True(result.ProductTypeHasInsurance);
            // Expected insurance value is 1500 ( 1000 base + 500 smartphone/laptop rule).
            Assert.Equal(1500, result.InsuranceValue);
            _insuranceRepoMock.Verify(x => x.Add(It.Is<InsuredProduct>(i => i.ProductId == productId)), Times.Once);
        }

        [Fact]
        public async Task CalculateInsurance_WithInvalidProductId_ShouldReturnNull()
        {
            // Arrange
            int productId = -1;
            _businessRulesMock.Setup(x => x.GetProductById(productId)).Returns(() => null);

            // Act
            var result = await _insuranceService.CalculateInsurance(productId);

            // Assert
            Assert.Null(result);
            _insuranceRepoMock.Verify(x => x.Add(It.IsAny<InsuredProduct>()), Times.Never);
        }

        [Fact]
        public async Task CalculateInsurance_WithProductTypeWithoutInsurance_ShouldReturnInsuranceDtoWithoutAddingToRepo()
        {
            // Arrange
            int productId = 1;
            var productDto = new ProductDto { id = productId, productTypeId = 1, salesPrice = 1000 };
            var productTypeDto = new ProductTypeDto { id = 1, name = "Miner", canBeInsured = false };
            _businessRulesMock.Setup(x => x.GetProductById(productId)).Returns(productDto);
            _businessRulesMock.Setup(x => x.GetProductType(productDto.productTypeId)).Returns(productTypeDto);

            // Act
            var result = await _insuranceService.CalculateInsurance(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal(productTypeDto.name, result.ProductTypeName);
            Assert.False(result.ProductTypeHasInsurance);
            Assert.Equal(productDto.salesPrice, result.SalesPrice);
            _insuranceRepoMock.Verify(x => x.Add(It.IsAny<InsuredProduct>()), Times.Never);
        }


        [Fact]
        public async Task CalculateOrder_ShouldCalculateOrder_WithValidOrder()
        {
            // Arrange
            var orderDto = new OrderDto
            {
                OrderId = 1,
                Items = new List<InsuranceDto>
                {
                    new InsuranceDto
                    {
                        ProductId = 1,
                        ProductTypeName = ProductTypeName.Smartphones.ToString(),
                        ProductTypeHasInsurance = true,
                        SalesPrice = 1000
                    },
                    new InsuranceDto
                    {
                        ProductId = 2,
                        ProductTypeName = ProductTypeName.DigitalCameras.ToString(),
                        ProductTypeHasInsurance = true,
                        SalesPrice = 800
                    },
                    new InsuranceDto
                    {
                        ProductId = 3,
                        ProductTypeName = ProductTypeName.Laptops.ToString(),
                        ProductTypeHasInsurance = true,
                        SalesPrice = 2500
                    }
                }
            };

            var order = new Order
            {
                Id = 1,
                OrderItems = new List<InsuredProduct>
                {
                    new InsuredProduct
                    {
                        Id = 1,
                        ProductTypeName = ProductTypeName.Smartphones.ToString(),
                        ProductTypeHasInsurance = true,
                        SalesPrice = 1000,
                        InsuranceValue = 1500,
                    },
                    new InsuredProduct
                    {
                        Id = 2,
                        ProductTypeName = ProductTypeName.DigitalCameras.ToString(),
                        ProductTypeHasInsurance = true,
                        SalesPrice = 800,
                        InsuranceValue = 1000
                    },
                    new InsuredProduct
                    {
                        Id = 3,
                        ProductTypeName = ProductTypeName.Laptops.ToString(),
                        ProductTypeHasInsurance = true,
                        SalesPrice = 2500,
                        InsuranceValue = 2000
                    }
                },
                OrderInsurance = 5000
            };

            _businessRulesMock.Setup(mock => mock.GetProductTypes())
                .Returns(new List<ProductTypeDto>
                {
                    new ProductTypeDto { id = 1, name = "Smartphones", canBeInsured = true },
                    new ProductTypeDto { id = 2, name = "DigitalCameras", canBeInsured = true },
                    new ProductTypeDto { id = 3, name = "Laptops", canBeInsured = true }
                });

            _businessRulesMock.Setup(mock => mock.GetProductById(1))
                .Returns(new ProductDto { id = 1, name = "Product 1", productTypeId = 1, salesPrice = 1000 });

            _businessRulesMock.Setup(mock => mock.GetProductById(2))
                .Returns(new ProductDto { id = 2, name = "Product 2", productTypeId = 2, salesPrice = 800 });

            _businessRulesMock.Setup(mock => mock.GetProductById(3))
                .Returns(new ProductDto { id = 3, name = "Product 3", productTypeId = 3, salesPrice = 2500 });

            _orderRepoMock.Setup(x => x.Add(It.IsAny<Order>())).Returns(Task.FromResult(order));

            // Act
            var result = await _insuranceService.CalculateOrder(orderDto);

            // Assert
            _businessRulesMock.Verify(mock => mock.GetProductTypes(), Times.Once);
            _businessRulesMock.Verify(mock => mock.GetProductById(1), Times.Once);
            _businessRulesMock.Verify(mock => mock.GetProductById(2), Times.Once);
            _businessRulesMock.Verify(mock => mock.GetProductById(3), Times.Once);
            _orderRepoMock.Verify(mock => mock.Add(It.IsAny<Order>()), Times.Once);

            Assert.NotNull(result);
            Assert.Equal(5000, result.OrderInsurance);
        }


    }

}