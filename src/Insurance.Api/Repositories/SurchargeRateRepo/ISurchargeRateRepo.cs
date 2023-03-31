using Insurance.Api.Model;
using System.Threading.Tasks;

namespace Insurance.Api.Repositories.SurchargeRateRepo
{
    public interface ISurchargeRateRepo
    {
        Task<SurchargeRate> Add(SurchargeRate surcharge);

        Task<SurchargeRate> GetByProductTypeId(int productTypeId);

        Task<SurchargeRate> Update(int productTypeId,float surchargeRate);

        Task<bool> Exists(int productTypeId);
    }
}
