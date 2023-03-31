using Insurance.Api.Model;
using Insurance.Api.Services.Dto;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace Insurance.Api.Repositories.InsuranceRepository
{
    //In a real Repo scenario, all this calls would be async.
    public class InsuranceRepo : IInsuranceRepo
    {
        private static List<InsuredProduct> insuredProducts = new List<InsuredProduct>()
    {
        new InsuredProduct()
        {
            Id= 1,
            ProductId = 572770,
            Name= "Samsung WW80J6400CW EcoBubble",
            SalesPrice = 475,
            ProductTypeHasInsurance = true,
            ProductTypeId = 1,
            ProductTypeName = "Washing machines",
            InsuranceValue = 500
        },
        new InsuredProduct()
        {
            Id= 2,
            ProductId =715990,
            ProductTypeId=2,
            Name="Canon Powershot SX620 HS Black",
             SalesPrice =  195,
             ProductTypeHasInsurance= true,
             ProductTypeName = "Digital cameras",
             InsuranceValue = 500

        }

    };

        private static List<Order> orders = new()
        {
            new Order()
            {
                Id = 1,
                OrderInsurance = 0,
                OrderItems= insuredProducts
            }
        };

        public async Task<InsuredProduct> Add(InsuredProduct insuredProduct)
        {
            try
            {
                insuredProduct.Id = insuredProducts.Count();
                insuredProducts.Add(insuredProduct);
            }
            catch (System.Exception)
            {

                return null;
            }


            return insuredProduct;

        }

        public async Task<Order> AddOrder(Order order)
        {
            try
            {
                //In a db scenario, you don't need this as it is created automatically.
                order.Id = orders.Count();
                orders.Add(order);
            }
            catch (System.Exception)
            {

                return null;
            }


            return order;

        }

        public Task<InsuredProduct> Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<InsuredProduct>> GetAll()
        {

            return insuredProducts.ToList();
        }

        public async Task<InsuredProduct> GetById(int id)
        {
            var insuredProduct = insuredProducts.Find(x => x.Id == id);

            return insuredProduct;
        }
        public async Task<List<InsuredProduct>> FindByProductsByTypeId(int productTypeId)
        {
            return insuredProducts.Where(item => item.ProductTypeId == productTypeId).ToList();
        }

        public async Task<List<InsuredProduct>> Update(List<InsuredProduct> toUpdate)
        {
            List<InsuredProduct> updatedProducts= new();
            if (toUpdate == null)
            {
                return null;
            }
            foreach (var product in toUpdate)
            {
                foreach (var item in insuredProducts)
                {
                    if (product.Id == item.Id)
                    {
                        item.InsuranceValue = product.InsuranceValue;
                        updatedProducts.Add(item);
                    }
                }

            }
            return updatedProducts;
        }
    }
}
