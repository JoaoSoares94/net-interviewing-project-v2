using Insurance.Api.Model;
using Insurance.Api.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Insurance.Api.Repositories.InsuranceRepository
{
    public interface IInsuranceRepo : IRepository<InsuredProduct>
    {
        Task Update(List<InsuredProduct> products);

        Task<List<InsuredProduct>> FindByProductsByTypeId(int productTypeId);

    }
}
