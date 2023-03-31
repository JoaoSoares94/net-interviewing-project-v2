using Xunit;
using Insurance.Api.Model;

namespace Insurance.Tests.Model.Tests
{
    public class SurchargeRateTests
    {
        [Fact]
        public void Constructor_InitializesProperties()
        {
            // Arrange
            int productTypeId = 1;
            string productTypeName = "Car";
            float rate = 1.5f;

            // Act
            var surchargeRate = new SurchargeRate(productTypeId, productTypeName, rate);

            // Assert
            Assert.Equal(productTypeId, surchargeRate.ProductTypeId);
            Assert.Equal(productTypeName, surchargeRate.ProductTypeName);
            Assert.Equal(rate, surchargeRate.Rate);
        }

        [Fact]
        public void toDto_ConvertsSurchargeRateToDto()
        {
            // Arrange
            var surchargeRate = new SurchargeRate
            {
                ProductTypeId = 2,
                ProductTypeName = "Home",
                Rate = 2.0f
            };

            // Act
            var dto = surchargeRate.toDto();

            // Assert
            Assert.Equal(2, dto.ProductTypeId);
            Assert.Equal("Home", dto.ProductTypeName);
            Assert.Equal(2.0f, dto.SurchargeRate);
        }
    }
}