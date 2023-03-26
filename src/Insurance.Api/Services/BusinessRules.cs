using Insurance.Api.Services.Dto;
using Newtonsoft.Json;
using System;
using System.Net.Http;

public static class BusinessRules
{
    public static void GetProductType(string baseAddress, int productID, ref InsuranceDto insurance)
    {
        HttpClient client = new HttpClient { BaseAddress = new Uri(baseAddress) };
        string json = client.GetAsync("/product_types").Result.Content.ReadAsStringAsync().Result;
        var collection = JsonConvert.DeserializeObject<dynamic>(json);

        json = client.GetAsync(string.Format("/products/{0:G}", productID)).Result.Content.ReadAsStringAsync().Result;
        var product = JsonConvert.DeserializeObject<dynamic>(json);

        int productTypeId = product.productTypeId;
        string productTypeName = null;
        bool hasInsurance = false;

        insurance = new InsuranceDto();

        for (int i = 0; i < collection.Count; i++)
        {
            if (collection[i].id == productTypeId && collection[i].canBeInsured == true)
            {
                insurance.ProductTypeName = collection[i].name;
                insurance.ProductTypeHasInsurance = true;
            }
        }
    }

    public static void GetSalesPrice(string baseAddress, int productID, ref InsuranceDto insurance)
    {
        HttpClient client = new HttpClient { BaseAddress = new Uri(baseAddress) };
        string json = client.GetAsync(string.Format("/products/{0:G}", productID)).Result.Content.ReadAsStringAsync().Result;
        var product = JsonConvert.DeserializeObject<dynamic>(json);

        insurance.SalesPrice = product.salesPrice;
    }

    public static void CalculateInsurance(ref InsuranceDto toInsure)
    {
        //We can check whether to perform Insurance calculation first. Its cleaner and more perfomant. Always good to check if our product is not null.
        if (toInsure != null && toInsure.ProductTypeHasInsurance)
        {
            // Check for rule - If the type of the product is a smartphone or a laptop, add € 500 more to the insurance cost.

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
        }
    }

    public static float CalculateInsurance( InsuranceDto toInsure)
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
          orderInsuranceValue +=  CalculateInsurance( item);
        }

    }
}
