using Insurance.Api.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using Insurance.Api.Services;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

[Route("api/insurance/insurance")]
[ApiController]
public class HomeController : ControllerBase
{
    private readonly IInsuranceService _insuranceService;
    private static List<InsuranceDto> insuranceDtos = new List<InsuranceDto>()
    {
        new InsuranceDto()
        {
            ProductId = 572770,
        },
         new InsuranceDto()
        {
            ProductId = 715990,
        },
          new InsuranceDto()
        {
            ProductId = 827074,
        },
           new InsuranceDto()
        {
            ProductId = 828519,
        },
            new InsuranceDto()
        {
            ProductId = 837856,
        }

    };


    public HomeController(IInsuranceService insuranceService)
    {
        _insuranceService = insuranceService;
    }

    [HttpGet]
    [Route("product/{id}")]

    public InsuranceDto GetById(int id)
    {

        return insuranceDtos.Find(x => x.ProductId == id);
    }

    [HttpGet]
    [Route("products")]
    public List<InsuranceDto> GetAll()
    {
        return insuranceDtos;
    }


    [HttpPost]
    [Route("product")]
    public ActionResult<InsuranceDto> CalculateInsurance([FromBody] InsuranceDto toInsure)
    {
        int productId = toInsure.ProductId;
        var result = _insuranceService.CalculateInsurance(productId);
        insuranceDtos.Add(result);

        return result;
    }


    [HttpPost]
    [Route("orders")]
    public ActionResult<InsuranceDto> CalculateInsurance([FromBody] OrderDto toInsure)
    {
        toInsure.Items = insuranceDtos;
        _insuranceService.CalculateOrder(toInsure);

        return null;
    }

}