using Insurance.Api.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Tests
{
    //Could've generated a TestData file to use as a generic case for most tests
    public  class TestData
    {
        public static InsuranceDto insuranceDto1 = new()
        {
            ProductId = 1,
            ProductTypeName = "Digital cameras",
            ProductTypeHasInsurance = true,
            SalesPrice = 100,
        };

        static InsuranceDto insuranceDto2 = new(2, "Smartphones", true, 501.99);

        static InsuranceDto insuranceDto3 = new(3, "Laptops", true, 2000);

        static InsuranceDto insuranceDto4 = new();

    }
}
