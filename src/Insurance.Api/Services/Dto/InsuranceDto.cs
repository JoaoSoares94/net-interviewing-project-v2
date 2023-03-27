using Newtonsoft.Json;

namespace Insurance.Api.Services.Dto
{
    public class InsuranceDto
    {
        public int ProductId { get; set; }

        public float InsuranceValue { get; set; }

        public string ProductTypeName { get; set; }

        public bool ProductTypeHasInsurance { get; set; }

        public double SalesPrice { get; set; }

        public InsuranceDto(int productId, string productTypeName, bool productTypeHasInsurance, double salesPrice)
        {
            ProductId = productId;
            ProductTypeName = productTypeName;
            ProductTypeHasInsurance = productTypeHasInsurance;
            SalesPrice = salesPrice;
        }

        public InsuranceDto()
        {
        }
    }


}
