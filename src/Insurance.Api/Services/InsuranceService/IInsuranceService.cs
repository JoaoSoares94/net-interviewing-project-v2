
using Insurance.Api.Model;
using Insurance.Api.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IInsuranceService
{
    Task<InsuranceDto> GetById(int id);

    Task<IEnumerable<InsuranceDto>> GetAll();

    Task<InsuranceDto> CalculateInsurance(int productId);

    Task<OrderDto> CalculateOrder(OrderDto orderDto);

    Task<SurchargeDto> UploadSurchargeRate(int productId, float surchargeRate);

    Task<List<InsuranceDto>> UpdateInsuranceValue(int productTypeId, float rate);
}

