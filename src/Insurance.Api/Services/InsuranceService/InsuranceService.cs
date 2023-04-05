using Insurance.Api.Data;
using Insurance.Api.Model;
using Insurance.Api.Repositories.InsuranceRepository;
using Insurance.Api.Repositories.OrderRepo;
using Insurance.Api.Repositories.SurchargeRateRepo;
using Insurance.Api.Services.Dto;
using Insurance.Api.Services.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

public class InsuranceService : IInsuranceService
{
    private readonly IBusinessRules _bussinessRules;
    private readonly IInsuranceRepo _insuranceRepo;
    private readonly IOrderRepo _orderRepo;
    private readonly ISurchargeRateRepo _surchargeRateRepo;
    private readonly IUnitOfWork _unitOfWork;


    public InsuranceService(IBusinessRules bussinessRules, IInsuranceRepo insuranceRepo, ISurchargeRateRepo surchargeRateRepo, IUnitOfWork unitOfWork, IOrderRepo orderRepo)
    {
        _bussinessRules = bussinessRules;
        _insuranceRepo = insuranceRepo;
        _surchargeRateRepo = surchargeRateRepo;
        _unitOfWork = unitOfWork;
        _orderRepo = orderRepo;
    }



    public async Task<InsuranceDto> CalculateInsurance(int productId)
    {   //Outgoing call to fetch information about a product
        var productDto = _bussinessRules.GetProductById(productId);
        if (productDto is null)
        {
            return null;
        }
        //Outgoing call to fetch information about a product type
        var productTypeDto = _bussinessRules.GetProductType(productDto.productTypeId);
        if (productTypeDto is null)
        {
            return null;
        }

        var toBeInsured = new InsuredProduct(productDto, productTypeDto);
        //We can check whether to perform Insurance calculation first. Its cleaner and more perfomant. Always good to check if our product is not null.
        if (toBeInsured is not null)
        {
            if (toBeInsured.ProductTypeHasInsurance)
            {
                //Calculates InsuranceValue 
                var calculateInsurance = await CalculateInsuranceValue(toBeInsured);
                //Adds Insured Product to storage
                var insuredProduct = await _insuranceRepo.Add(calculateInsurance);
                await _unitOfWork.CommitAsync();
                //Returns a dto of the Insured Product
                return insuredProduct.toDto(); ;
            }
            //Returns dto from product that can't be insured
            return new InsuranceDto(productDto, productTypeDto);
        }

        return null;
    }

    public async Task<OrderDto> CalculateOrder(OrderDto orderDto)
    {
        // Get product types for the products in the order
        //Because the order might end up having more than one ProductType, it's better to call the API just one time and get all the product types
        try
        {
            var productTypes = _bussinessRules.GetProductTypes();
            //Get the productDto for all the products in the order
            var productsDto = orderDto.Items
                                   .Select(item => _bussinessRules.GetProductById(item.ProductId))
                                   .ToList();


            List<InsuredProduct> notInsuredList = new();
            foreach (var productDto in productsDto)
            {
                //Matching each product to its product type and creating an InsuranceDto
                var productType = productTypes.FirstOrDefault(item => item.id == productDto.productTypeId);
                notInsuredList.Add(new InsuredProduct(productDto, productType));
            }
            var insuredList = new List<InsuredProduct>();
            //Calculating insurance for every product on the list
            foreach (var item in notInsuredList)
            {
                var product = await CalculateInsuranceValue(item);
                insuredList.Add(product);
            }

            Order order = new()
            {
                //Updating order with the insurance calculations
                OrderItems = insuredList
            };

            //Calculating the total for order insurance 
            order.CalculateInsurance();

            //Checking if order contains product from type DigitalCameras
            //Rule - If an order has one or more digital cameras, add € 500 to the insured value of the order.

            if (DslCheck(order.OrderItems))
            {
                order.OrderInsurance += 500;
            }
            //Add order to db
            var insuredOrder = await _orderRepo.Add(order);
            //Convert Order into dto
            await _unitOfWork.CommitAsync();
            var insuredOrderDto = insuredOrder.toDto();
            return insuredOrderDto;

        }
        catch (Exception)
        {

            return null;
        }

    }

    public async Task<InsuranceDto> GetById(int id)
    {
        try
        {
            var insuredProduct = await _insuranceRepo.Get(id);
            return insuredProduct.toDto();

        }
        catch (Exception)
        {

            return null;
        }

    }
    public async Task<IEnumerable<InsuranceDto>> GetAll()
    {
        try
        {
            var insuredProducts = await _insuranceRepo.GetAll();
            var insuredDtos = insuredProducts.Select(x => x.toDto());
            return insuredDtos;
        }
        catch (Exception)
        {

            return null;
        }

    }

    private async Task<InsuredProduct> CalculateInsuranceValue(InsuredProduct toInsure)
    {
        {
            toInsure.InsuranceValue = 0;
            // Check for rule - If the type of the product is a smartphone or a laptop, add € 500 more to the insurance cost.
            //Static values instead of magic strings is a far less error prone approach

            if (toInsure.ProductTypeName == ProductTypeName.Laptops.ToString() || toInsure.ProductTypeName == ProductTypeName.Smartphones.ToString())
            {
                toInsure.InsuranceValue += 500;
            }

            //Using a switch expression is cleaner than having nested if conditions.
            int calculateInsuranceValue = toInsure.SalesPrice switch
            {
                // Check for rule - If the product sales price is less than € 500, no insurance required
                < 500 => 0,
                // Check for rule - If the product sales price => € 500 but < € 2000, insurance cost is € 1000
                >= 500 and < 2000 => 1000,
                // Check for rule - If the product sales price => € 2000, insurance cost is €2000
                > 2000 => 2000,
                //default 
                _ => throw new NotFiniteNumberException()
            };
            toInsure.InsuranceValue += calculateInsuranceValue;

            //Checks if the productType of this product has a surcharge rate or not. Returns surcharge if it has
            var hasSurcharge = await HasSurcharge(toInsure.ProductTypeId);
            if (hasSurcharge is not null)
            {
                //Applies Surcharge to the product
                toInsure.ApplySurcharge(hasSurcharge.Rate);
            }
        }
        return toInsure;

    }
    //Verifies if any product of a product list is a Digital Camera
    private bool DslCheck(List<InsuredProduct> insuredProducts)
    {
        var hasDSL = insuredProducts.Exists(item => item.ProductTypeName == ProductTypeName.DigitalCameras.ToString());
        return hasDSL;
    }


    public async Task<SurchargeDto> UploadSurchargeRate(int productTypeId, float surchargeRate)
    {
        //Checks if surcharge with same Product Type id already exists
        //If it exists updates its surchargeRate property to the one passed as parameter
        var alreadyHasRate = await _surchargeRateRepo.Exists(productTypeId);
        if (alreadyHasRate)
        {
            var updatedRate = await _surchargeRateRepo.Update(productTypeId, surchargeRate);
            await _unitOfWork.CommitAsync();

            //Returns dto of updatedRate
            return updatedRate.toDto();
        }
        //Fetches information about the Product Type
        var productTypeDto = _bussinessRules.GetProductType(productTypeId);
        if (productTypeDto == null)
        {
            return null;
        }
        //Creates new SurchargeRate and adds its to the db
        SurchargeRate rate = new(productTypeDto.id, productTypeDto.name, surchargeRate);
        var uploadedSurcharge = await _surchargeRateRepo.Add(rate);
        await _unitOfWork.CommitAsync();
        //Returns dto of created rate
        var surchargeDto = uploadedSurcharge.toDto();
        return surchargeDto;

    }

    public async Task<List<InsuranceDto>> UpdateInsuranceValue(int productTypeId, float rate)
    {
        // Finds any product that have the Product Type Id equal to the one passed as parameter
        var products = await _insuranceRepo.FindByProductsByTypeId(productTypeId);

        //If List is empty, means no updates where made
        if (!products.Any())
        {
            return new List<InsuranceDto>();
        }
        try
        {

            //For each product of the least, applies the surcharge rate.
            var newProducts = products.Select(product => CalculateInsuranceValue(product).Result).ToList();
            //Updates db 
            await _insuranceRepo.Update(products);
            await _unitOfWork.CommitAsync();
            //creates a dto for each product of the list 
            var updatedDtos = newProducts.Select(product => product.toDto()).ToList();
            return updatedDtos;
        }
        catch (Exception)
        {

            return null;
        }

    }

    // Checks if the product has any surcharge rate associated 
    private async Task<SurchargeRate> HasSurcharge(int productTypeId)
    {
        var rate = await _surchargeRateRepo.GetByProductTypeId(productTypeId);
        if (rate is null)
        {
            return null;
        }
        return rate;
    }

}

