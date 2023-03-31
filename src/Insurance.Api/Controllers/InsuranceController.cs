using Insurance.Api.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/insurance/insurance")]
[ApiController]
public class InsuranceController : ControllerBase
{
    private readonly IInsuranceService _insuranceService;

    public InsuranceController(IInsuranceService insuranceService)
    {
        _insuranceService = insuranceService;
    }

    [HttpGet]
    [Route("product/{id}")]

    //Just for easibility of testing 
    public async Task<ActionResult<InsuranceDto>> GetById(int id)
    {
        var insuranceDto = await _insuranceService.GetById(id);
        if (insuranceDto == null)
        {
            return NotFound(new { message = "The product does not exist" });
        }
        return Ok(insuranceDto);


    }

    //Just for easibility of testing 
    [HttpGet]
    [Route("products")]
    public async Task<ActionResult<IEnumerable<InsuranceDto>>> GetAll()
    {
        var insuranceDtos = await _insuranceService.GetAll();
        if (insuranceDtos == null)
        {
            return NotFound(new { message = "The database doesn't have any product registered!" });

        }
        return Ok(insuranceDtos);
    }


    [HttpPost]
    [Route("product")]
    public async Task<ActionResult<InsuranceDto>> CalculateInsurance([FromBody] InsuranceDto toInsure)
    {
        if (toInsure is null)
        {
            return BadRequest(new { message = "Please provide a valid product" });
        }
        int productId = toInsure.ProductId;

        var result = await _insuranceService.CalculateInsurance(productId);
        if (result is null)
        {
            return BadRequest(new { message = "Couldn't calculate insurance" });
        }

        if (!result.ProductTypeHasInsurance)
        {
            return BadRequest(new { message = "Product cannot be insured" });
        }

        return CreatedAtAction(nameof(CalculateInsurance), result);
    }


    [HttpPost]
    [Route("orders")]
    public async Task<ActionResult<OrderDto>> CalculateInsurance([FromBody] OrderDto toInsure)
    {
        if (toInsure is null)
        {
            return BadRequest(new { message = "Please provide a valid order" });
        }
        var insuredOrder = await _insuranceService.CalculateOrder(toInsure);

        if (insuredOrder is null)
        {
            return BadRequest(new { message = "Couldn't insure order" });
        }

        return CreatedAtAction(nameof(CalculateInsurance), toInsure);
    }

    [HttpPost]
    [Route("surcharge/{productTypeId:int:min(0)}/{surcharge:float}")]
    public async Task<ActionResult<SurchargeDto>> UploadSurcharge(int productTypeId, float surcharge)
    {
        if (surcharge <= 0)
        {
            return BadRequest(new { message = "Please provide a valid surcharge value greater than 0" });
        }
       var surchargeDto =  await _insuranceService.UploadSurchargeRate(productTypeId, surcharge);

        if (surchargeDto is null)
        {
            return BadRequest(new { message = "Couldn't upload surcharge" });
        }
        var updateEveryProduct = await _insuranceService.UpdateInsuranceValue(productTypeId, surcharge);
        if(updateEveryProduct is null)
        {
            return Problem( "Couldn't update the products already in the database" );
        }
        return CreatedAtAction(nameof(UploadSurcharge), surchargeDto);
    }
}