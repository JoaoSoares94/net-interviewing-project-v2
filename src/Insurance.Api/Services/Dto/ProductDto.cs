using Newtonsoft.Json;

namespace Insurance.Api.Services.Dto
{
    public class ProductDto
    {
        [JsonRequired]
        public int id { get; set; }

        public string name { get; set; }

        public double salesPrice { get; set; }
        [JsonRequired]
        public int productTypeId { get; set; }
    }
}
