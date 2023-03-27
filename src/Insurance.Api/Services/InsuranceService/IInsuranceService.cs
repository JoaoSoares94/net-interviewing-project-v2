
using Insurance.Api.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public interface IInsuranceService
{
    InsuranceDto CalculateInsurance(int productId);
    OrderDto CalculateOrder(OrderDto orderDto);
}

