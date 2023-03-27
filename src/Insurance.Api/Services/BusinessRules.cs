using Insurance.Api.Services;
using Insurance.Api.Services.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

public class BusinessRules
{
    private readonly HttpClient _httpClient;
    private readonly MySettings _mySettings;

    public BusinessRules(HttpClient httpClient, MySettings mySettings)
    {
        _httpClient = httpClient;
        _mySettings = mySettings;
        _httpClient.BaseAddress = new Uri(mySettings.ProductApi);
    }


    public ProductDto GetProductById(int productId)
    {
        try
        {
            string json = _httpClient.GetAsync($"/products/{productId}").Result.Content.ReadAsStringAsync().Result;
            var product = JsonConvert.DeserializeObject<ProductDto>(json);
            return product;
        }
        catch (Exception)
        {

            throw;
        }


    }

    public ProductTypeDto GetProductType(int productTypeId)
    {
        try
        {
            var json = _httpClient.GetAsync(string.Format("/product_types/{0:G}", productTypeId)).Result.Content.ReadAsStringAsync().Result;
            var productTypeDto = JsonConvert.DeserializeObject<ProductTypeDto>(json);
            return productTypeDto;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public List<ProductTypeDto> GetProductTypes()
    {
        try
        {
            var json = _httpClient.GetAsync("/product_types").Result.Content.ReadAsStringAsync().Result;
            var productTypeDto = JsonConvert.DeserializeObject<List<ProductTypeDto>>(json);
            return productTypeDto;
        }
        catch (Exception)
        {
            throw;
        }
    }


    //I dont find this method particularly necessary.
    public void GetSalesPrice(int productID, ref InsuranceDto insurance)
    {
        string json = _httpClient.GetAsync(string.Format("/products/{0:G}", productID)).Result.Content.ReadAsStringAsync().Result;
        var product = JsonConvert.DeserializeObject<dynamic>(json);

        insurance.SalesPrice = product.salesPrice;
    }



    public static float CalculateInsurance(InsuranceDto toInsure)
    {
        float insuranceValue = toInsure.InsuranceValue;
        //We can check whether to perform Insurance calculation first. Its cleaner and more perfomant. Always good to check if our product is not null.
        if (toInsure != null && toInsure.ProductTypeHasInsurance)
        {
            // Check for rule - If the type of the product is a smartphone or a laptop, add € 500 more to the insurance cost.
            // TODO : create Enum Class, not good to have magic strings
            if (toInsure.ProductTypeName == "Laptops" || toInsure.ProductTypeName == "Smartphones")
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
            toInsure.InsuranceValue = +calculateInsuranceValue;
            return toInsure.InsuranceValue;
        }
        else
            return insuranceValue;
    }
    public static void CalculateOrderInsurance(ref OrderDto toInsure)
    {
        float orderInsuranceValue = 0;
        foreach (InsuranceDto item in toInsure.Items)
        {
            orderInsuranceValue += CalculateInsurance(item);
        }

    }
}
