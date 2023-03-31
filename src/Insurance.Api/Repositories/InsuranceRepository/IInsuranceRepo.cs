using Insurance.Api.Model;
using Insurance.Api.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Insurance.Api.Repositories.InsuranceRepository
{
    public interface IInsuranceRepo
    {
        //CRUD
        Task<InsuredProduct> GetById(int id);

        Task<IEnumerable<InsuredProduct>> GetAll();

        Task<InsuredProduct> Add(InsuredProduct insuranceProduct);

        Task<Order> AddOrder(Order order);

        Task<List<InsuredProduct>> Update(List<InsuredProduct> products);

        Task<InsuredProduct> Delete(int id);

        Task<List<InsuredProduct>> FindByProductsByTypeId(int productTypeId);


    }
}
