using Insurance.Api.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

public class InsuranceService : IInsuranceService
{
    private readonly BusinessRules _bussinessRules;

    public InsuranceService(BusinessRules bussinessRules)
    {
        _bussinessRules = bussinessRules;
    }

    public InsuranceDto CalculateInsurance(int productId)
    {
        var productDto = _bussinessRules.GetProductById(productId);
        var productTypeDto = _bussinessRules.GetProductType(productDto.productTypeId);

        InsuranceDto toInsure = new InsuranceDto(productDto.id, productTypeDto.name, productTypeDto.canBeInsured, productDto.salesPrice);
        //We can check whether to perform Insurance calculation first. Its cleaner and more perfomant. Always good to check if our product is not null.
        if (toInsure != null && toInsure.ProductTypeHasInsurance)
        {
            var insuredDto = CalculateInsuranceValue(toInsure);
            return insuredDto;

        }

        return toInsure;
    }

    public OrderDto CalculateOrder(OrderDto orderDto)
    {
        // Get product types for the products in the order
        //Because the order might end up having more than one ProductType, it's better to call the API just one time and get all the product types
        var productTypes = _bussinessRules.GetProductTypes();

        //Get the productDto for all the products in the order
        var productsDto = orderDto.Items
                               .Select(item => _bussinessRules.GetProductById(item.ProductId))
                               .ToList();


        List<InsuranceDto> list = new();
        foreach (var productDto in productsDto)
        {
            //Matching each product to its product type and creating and InsuranceDto
            var productType = productTypes.FirstOrDefault(item => item.id == productDto.productTypeId);
            list.Add(new InsuranceDto(productDto.id, productType.name, productType.canBeInsured, productDto.salesPrice));
        }

        //Calculating insurance for every product on the list
        orderDto.Items = list
            .Select(item => CalculateInsurance(item.ProductId))
            .ToList();

        //Calculating the order insurance 
        orderDto.OrderInsurance = orderDto.Items
            .Sum(item => item.InsuranceValue);

        //Checking if order contains product from type DigitalCameras
        //Rule - If an order has one or more digital cameras, add € 500 to the insured value of the order.
        var dslCheck = orderDto.Items.Exists(item => item.ProductTypeName == ProductTypeName.DigitalCameras.ToString());
        if (dslCheck)
        {
            orderDto.OrderInsurance += 500;
        }
        return orderDto;
    }


    private InsuranceDto CalculateInsuranceValue(InsuranceDto toInsure)
    {
        {
            // Check for rule - If the type of the product is a smartphone or a laptop, add € 500 more to the insurance cost.
            //Enum instead of magic strings is a far less error prone approach

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
        }
        return toInsure;

    }
}

