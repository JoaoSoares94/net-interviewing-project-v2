﻿using Insurance.Api.Model;
using Insurance.Api.Repositories.InsuranceRepository;
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
    private readonly ISurchargeRateRepo _surchargeRateRepo;

    public InsuranceService(IBusinessRules bussinessRules, IInsuranceRepo insuranceRepo, ISurchargeRateRepo surchargeRateRepo)
    {
        _bussinessRules = bussinessRules;
        _insuranceRepo = insuranceRepo;
        _surchargeRateRepo = surchargeRateRepo;
    }



    public async Task<InsuranceDto> CalculateInsurance(int productId)
    {
        var productDto = _bussinessRules.GetProductById(productId);
        if (productDto is null)
        {
            return null;
        }
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
                var calculateInsurance = await CalculateInsuranceValue(toBeInsured);
                var insuredProduct = await _insuranceRepo.Add(calculateInsurance);
                return insuredProduct.toDto(); ;
            }

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


            List<InsuredProduct> list = new();
            foreach (var productDto in productsDto)
            {
                //Matching each product to its product type and creating an InsuranceDto
                var productType = productTypes.FirstOrDefault(item => item.id == productDto.productTypeId);
                list.Add(new InsuredProduct(productDto, productType));
            }
            var totalList = new List<InsuredProduct>();
            //Calculating insurance for every product on the list
            foreach (var item in list)
            {
                var product = await CalculateInsuranceValue(item);
                totalList.Add(product);
            }

            Order order = new Order(orderDto.OrderId);
            //Updating order with the insurance calculations
            order.OrderItems = totalList;

            //Calculating the total for order insurance 
            order.CalculateInsurance();

            //Checking if order contains product from type DigitalCameras
            //Rule - If an order has one or more digital cameras, add € 500 to the insured value of the order.

            if (DslCheck(order.OrderItems))
            {
                order.OrderInsurance += 500;
            }

            var insuredOrder = await _insuranceRepo.AddOrder(order);
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
            var insuredProduct = await _insuranceRepo.GetById(id);
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
            var hasSurcharge = await HasSurcharge(toInsure.ProductTypeId);
            if (hasSurcharge is not null)
            {
                toInsure.ApplySurcharge(hasSurcharge.Rate);
            }
        }
        return toInsure;

    }

    private bool DslCheck(List<InsuredProduct> insuredProducts)
    {
        var hasDSL = insuredProducts.Exists(item => item.ProductTypeName == ProductTypeName.DigitalCameras.ToString());
        return hasDSL;
    }

    public async Task<SurchargeDto> UploadSurchargeRate(int productTypeId, float surchargeRate)
    {
        var alreadyHasRate = await _surchargeRateRepo.Exists(productTypeId);
        if (alreadyHasRate)
        {
            var updatedRate = await _surchargeRateRepo.Update(productTypeId,surchargeRate);
            return updatedRate.toDto();
        }

        var productTypeDto = _bussinessRules.GetProductType(productTypeId);
        if (productTypeDto == null)
        {
            return null;
        }
        SurchargeRate rate = new(productTypeDto.id, productTypeDto.name, surchargeRate);
        var uploadedSurcharge = await _surchargeRateRepo.Add(rate);
        var surchargeDto = uploadedSurcharge.toDto();
        return surchargeDto;

    }

    public async Task<List<InsuranceDto>> UpdateInsuranceValue(int productTypeId, float rate)
    {
       
        var products = await _insuranceRepo.FindByProductsByTypeId(productTypeId);
        if (!products.Any())
        {
            return  new List<InsuranceDto>();
        }
        try
        {
            products.ForEach(product =>  product.ApplySurcharge(rate) );
            var updatedProducts = await _insuranceRepo.Update(products);
            var updatedDtos = updatedProducts.Select(product => product.toDto()).ToList();
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

