using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Insurance.Api.Services.Dto
{
    public class SurchargeDto
    {
        [JsonRequired]
        public int ProductTypeId { get; set; }
        [JsonRequired]
        public float SurchargeRate { get; set; }

        public string ProductTypeName { get; set; }

        public SurchargeDto()
        {
            
        }
    }
    
}
