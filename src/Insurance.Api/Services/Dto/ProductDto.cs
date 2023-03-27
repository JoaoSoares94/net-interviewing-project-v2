namespace Insurance.Api.Services.Dto
{
    public class ProductDto
    {
        public int id { get; set; }

        //Property is marked as nullable in the API
        public string name { get; set; }

        public double salesPrice { get; set; }

        public int productTypeId { get; set; }
    }
}
