using Xunit;
using Moq;
using System.Collections.Generic;
using Insurance.Api.Model;

namespace Insurance.Tests.Model.Tests
{
    public class OrderTests
    {
        [Fact]
        public void CalculateInsurance_SumsInsuranceValueOfOrderItems()
        {
            // Arrange
            var order = new Order();
            order.OrderItems = new List<InsuredProduct>
            {
                new InsuredProduct { InsuranceValue = 100.0 },
                new InsuredProduct { InsuranceValue = 50.0 },
                new InsuredProduct { InsuranceValue = 25.0 }
            };

            // Act
            order.CalculateInsurance();

            // Assert
            Assert.Equal(175.0, order.OrderInsurance);
        }

        [Fact]
        public void toDto_ConvertsOrderToDto()
        {
            // Arrange
            var order = new Order(123);
            order.OrderItems = new List<InsuredProduct>
            {
                new InsuredProduct { ProductId = 1, ProductTypeName = "Gaming", SalesPrice = 5000.0, InsuranceValue = 1000.0 },
                new InsuredProduct { ProductId = 2, ProductTypeName = "Car", SalesPrice = 100000.0, InsuranceValue = 5000.0 }
            };
            order.CalculateInsurance();

            // Act
            var dto = order.toDto();

            // Assert
            Assert.Equal(123, dto.OrderId);
            Assert.Equal(6000.0f, dto.OrderInsurance);
            Assert.Collection(dto.Items,
                item => Assert.Equal(1, item.ProductId),
                item => Assert.Equal(2, item.ProductId)
            );
        }
    }
}