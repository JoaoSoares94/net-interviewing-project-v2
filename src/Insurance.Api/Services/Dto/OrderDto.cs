
using Insurance.Api.Services.Dto;
using System.Collections.Generic;

public class OrderDto
{
    public int OrderId { get; set; }
    public List<InsuranceDto> Items { get; set; }

    public float OrderInsurance { get; set; } = 0;
}
