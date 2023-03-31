using Insurance.Api.Services.Dto;
using System;

namespace Insurance.Api.Model
{
    public class InsuredProduct
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string Name { get; set; }

        public int ProductTypeId { get; set; }

        public string ProductTypeName { get; set; }

        public bool ProductTypeHasInsurance { get; set; }

        public double SalesPrice { get; set; }

        public double InsuranceValue { get; set; }


        public InsuredProduct(ProductDto productDto, ProductTypeDto productTypeDto)
        {

            ProductId = productDto.id;

            Name = productDto.name;

            ProductTypeId = productTypeDto.id;

            ProductTypeName = productTypeDto.name;

            ProductTypeHasInsurance = productTypeDto.canBeInsured;

            SalesPrice = productDto.salesPrice;
        }
        public InsuredProduct()
        {

        }

        public InsuranceDto toDto()
        {
            InsuranceDto insuranceDto = new()
            {
                InsuranceValue = (float)this.InsuranceValue,
                ProductId = this.ProductId,
                ProductTypeName = this.ProductTypeName,
                ProductTypeHasInsurance = this.ProductTypeHasInsurance,
                SalesPrice = this.SalesPrice,

            };
            return insuranceDto;
        }
        public void ApplySurcharge(float rate)
        {
            InsuranceValue = Math.Round(InsuranceValue * (1 + (rate / 100)), 2);

        }

    }

}
