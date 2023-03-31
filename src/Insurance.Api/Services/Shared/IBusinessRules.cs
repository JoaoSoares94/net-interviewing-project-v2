using Insurance.Api.Services.Dto;
using System.Collections.Generic;

namespace Insurance.Api.Services.Shared
{
    public interface IBusinessRules
    {

        ProductDto GetProductById(int productId);

        ProductTypeDto GetProductType(int productTypeId);

        List<ProductTypeDto> GetProductTypes();

        void GetSalesPrice(int productID, ref InsuranceDto insurance);

        public static float CalculateInsurance(InsuranceDto toInsure) => 0;

    }
}
