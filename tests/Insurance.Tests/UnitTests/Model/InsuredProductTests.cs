using Insurance.Api.Model;
using Insurance.Api.Services.Dto;
using Moq;
using System;
using Xunit;

namespace Insurance.Test.UnitTests.Model
{
    public class InsuredProductTests
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            var productDto = new ProductDto
            {
                id = 1,
                name = "Product 1",
                salesPrice = 100
            };

            var productTypeDto = new ProductTypeDto
            {
                id = 1,
                name = "Product Type 1",
                canBeInsured = true
            };

            // Act
            var insuredProduct = new InsuredProduct(productDto, productTypeDto);

            // Assert
            Assert.Equal(productDto.id, insuredProduct.ProductId);
            Assert.Equal(productDto.name, insuredProduct.Name);
            Assert.Equal(productTypeDto.id, insuredProduct.ProductTypeId);
            Assert.Equal(productTypeDto.name, insuredProduct.ProductTypeName);
            Assert.Equal(productTypeDto.canBeInsured, insuredProduct.ProductTypeHasInsurance);
            Assert.Equal(productDto.salesPrice, insuredProduct.SalesPrice);
        }

        [Fact]
        public void ToDto_ReturnsCorrectDto()
        {
            // Arrange
            var insuredProduct = new InsuredProduct
            {
                ProductId = 1,
                Name = "Product 1",
                ProductTypeId = 1,
                ProductTypeName = "Product Type 1",
                ProductTypeHasInsurance = true,
                SalesPrice = 100,
                InsuranceValue = 10
            };

            var expectedDto = new InsuranceDto
            {
                ProductId = insuredProduct.ProductId,
                ProductTypeName = insuredProduct.ProductTypeName,
                ProductTypeHasInsurance = insuredProduct.ProductTypeHasInsurance,
                SalesPrice = insuredProduct.SalesPrice,
                InsuranceValue = (float)insuredProduct.InsuranceValue
            };

            // Act
            var actualDto = insuredProduct.toDto();

            // Assert
            Assert.Equal(expectedDto.ProductId, actualDto.ProductId);
            Assert.Equal(expectedDto.ProductTypeName, actualDto.ProductTypeName);
            Assert.Equal(expectedDto.ProductTypeHasInsurance, actualDto.ProductTypeHasInsurance);
            Assert.Equal(expectedDto.SalesPrice, actualDto.SalesPrice);
            Assert.Equal(expectedDto.InsuranceValue, actualDto.InsuranceValue);
        }

        [Theory]
        [InlineData(5, 50.5)]
        [InlineData(10, 10.7)]
        [InlineData(2500, 5.1)]
        public void ApplySurcharge_AppliesCorrectSurcharge(double insuranceValue, float surchargeRate)
        {
            // Arrange
            var insuredProduct = new InsuredProduct
            {
                InsuranceValue = insuranceValue
            };

            var expectedInsuranceValue = Math.Round(insuranceValue * (1 + (surchargeRate / 100)), 2);

            // Act
            insuredProduct.ApplySurcharge(surchargeRate);

            // Assert
            Assert.Equal(expectedInsuranceValue, insuredProduct.InsuranceValue);
        }
    }
}