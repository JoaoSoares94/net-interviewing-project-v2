using Insurance.Api.Data;
using Insurance.Api.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Insurance.Api.Repositories.SurchargeRateRepo
{
    public class SurchargeRateRepo : BaseRepository<SurchargeRate>, ISurchargeRateRepo
    {
        public SurchargeRateRepo(DataContext dataContext) : base(dataContext.SurchargeRates)
        {
        }


        //Retrieves a Surcharge Rate based on its productTypeId
        public async Task<SurchargeRate> GetByProductTypeId(int productTypeId)
        {
            try
            {
                var surchargeRate = await _objs.FirstOrDefaultAsync(x => x.ProductTypeId == productTypeId);
                return surchargeRate;
            }
            catch (Exception)
            {

                return null;
            }
        }
        //Updates the rate of SurchargeRate based on its productTypeId
        public async Task<SurchargeRate> Update(int productTypeId, float surchargeRate)
        {
            try
            {
                var rate = await _objs.Where(x => x.ProductTypeId == productTypeId).FirstOrDefaultAsync();
                if(rate.Rate == surchargeRate)
                {
                    throw new ArgumentException("Please provide a different rate than the one is currently attributed");
                }
                rate.Rate = surchargeRate;
                return rate;
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
                var surchargeRate = await _objs.AnyAsync(x => x.ProductTypeId == productTypeId);
                return surchargeRate;
            }
            catch (Exception)
            {

                throw new NullReferenceException (nameof(SurchargeRate));
            }
        }
    }
}
