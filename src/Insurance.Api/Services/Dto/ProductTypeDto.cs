using Newtonsoft.Json;

namespace Insurance.Api.Services.Dto
{
    public class ProductTypeDto
    {
        [JsonRequired]
        public int id { get; set; }
        public string name { get; set; }
        public bool canBeInsured{ get; set; }
    }
}
