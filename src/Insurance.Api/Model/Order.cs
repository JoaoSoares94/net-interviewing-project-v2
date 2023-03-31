using System.Collections.Generic;
using System.Linq;

namespace Insurance.Api.Model
{
    public class Order
    {
        public int Id { get; set; }
        public List<InsuredProduct> OrderItems { get; set; }

        public double? OrderInsurance { get; set; }

        public Order(int id)
        {
            Id = id;
        }

        public Order() { }
        public void CalculateInsurance()
        {
            OrderInsurance = OrderItems.Sum(item => item.InsuranceValue);
        }

        public OrderDto toDto()
        {

            OrderDto dto = new OrderDto()
            {
                OrderId = Id,
                OrderInsurance = (float)OrderInsurance,
                Items = OrderItems.Select(x => x.toDto()).ToList()
            };
            return dto;
        }
    }

}
