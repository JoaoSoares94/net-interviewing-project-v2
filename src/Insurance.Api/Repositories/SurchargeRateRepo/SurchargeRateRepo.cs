using Insurance.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Insurance.Api.Repositories.SurchargeRateRepo
{
    public class SurchargeRateRepo : ISurchargeRateRepo
    {
        private static List<SurchargeRate> list = new()
        {
            new SurchargeRate()
            {
                Id= 1,
                ProductTypeId= 1,
                Rate = 21.3f,
                ProductTypeName = "Laptops"
            }
        };

        //Adds a SurchargeRate
        public async Task<SurchargeRate> Add(SurchargeRate surcharge)
        {
            try
            {
                surcharge.Id = list.Count();
                list.Add(surcharge);
            }
            catch (Exception)
            {

                return null;
            }
            return surcharge;
        }

        //Retrieves a Surcharge Rate based on its productTypeId
        public async Task<SurchargeRate> GetByProductTypeId(int productTypeId)
        {
            try
            {
                var surchargeRate = list.FirstOrDefault(x => x.ProductTypeId == productTypeId);
                return surchargeRate;
            }
            catch (Exception)
            {

                return null;
            }
        }
        //Updates the rate of SurchargeRate based on its productTypeId
        public async Task<SurchargeRate> Update(int productTypeId,float surchargeRate)
        {
            try
            {
                foreach (var item in list)
                {

                    if (item.ProductTypeId == productTypeId)
                    {
                        item.Rate = surchargeRate;
                        return item;

                    }
                }
                return null;
            }

            catch (Exception)
            {

                return null;
            }


        }
        //This methods checks if there is any SurchargeRate with this productTypeId
        public async Task<bool> Exists(int productTypeId)
        {
            try
            {
                var surchargeRate = list.Any(x => x.ProductTypeId == productTypeId);
                return surchargeRate;
            }
            catch (Exception)
            {

                return false ;
            }
        }
    }
}
