using Insurance.Api.Data;
using Insurance.Api.Model;
using Insurance.Api.Services.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace Insurance.Api.Repositories.InsuranceRepository
{
    //In a real Repo scenario, all this calls would be async.
    public class InsuranceRepo : BaseRepository<InsuredProduct>, IInsuranceRepo
    {
        public InsuranceRepo(DataContext context) : base(context.InsuredProducts)
        {
        }


        public async Task<List<InsuredProduct>> FindByProductsByTypeId(int productTypeId)
        {
            return await _objs.Where(item => item.ProductTypeId.Equals(productTypeId)).ToListAsync();
        }

        public async Task Update(List<InsuredProduct> toUpdate)
        {
            List<InsuredProduct> updatedProducts = new();
            if (toUpdate == null)
            {
                throw new NullReferenceException();
            }
            foreach (var product in toUpdate)
            {
                var toUpdateProduct = await Get(product.Id);
                toUpdateProduct.InsuranceValue = product.InsuranceValue;

            }
            
        }
    }
}
