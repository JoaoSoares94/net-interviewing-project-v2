using Insurance.Api.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using Insurance.Api.Services;
using Microsoft.Extensions.Options;

public class HomeController : Controller
    {
    private readonly MySettings _settings;

    public HomeController(MySettings settings)
    {
        _settings = settings;
    }
   

    [HttpPost]
        [Route("api/insurance/product")]
        public InsuranceDto CalculateInsurance([FromBody] InsuranceDto toInsure)
        {
            int productId = toInsure.ProductId;

            BusinessRules.GetProductType(_settings.ProductApi, productId, ref toInsure);
            BusinessRules.GetSalesPrice(_settings.ProductApi, productId, ref toInsure);
            BusinessRules.CalculateInsurance(ref toInsure);

            return toInsure;
        }

 }