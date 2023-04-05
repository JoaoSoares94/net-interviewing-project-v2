using Insurance.Api.Services.Dto;

namespace Insurance.Api.Model
{
    public class SurchargeRate : Entity
    {
        public override int Id { get; set; }

        public int ProductTypeId { get; set; }

        public string ProductTypeName { get; set; }

        public float Rate { get; set; }

        public SurchargeRate( int productTypeId,string productTypeName, float rate)
        {
            ProductTypeId = productTypeId;
            ProductTypeName = productTypeName;
            Rate = rate;
        }

        public SurchargeRate() { }

    public SurchargeDto toDto()
    {
            SurchargeDto dto = new()
            {
                ProductTypeId = ProductTypeId,
                SurchargeRate = Rate,
                ProductTypeName = ProductTypeName

            };
            return dto;
    }

    }

}
