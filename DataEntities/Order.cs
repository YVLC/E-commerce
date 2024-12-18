using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DataEntities;

public record Order
{
    [Key]
    public int OrderNumber { get; init; }
    public DateTime Date { get; init; }
    public string Status { get; init; }
    public string Description { get; init; }
    public string Street { get; init; }
    public string City { get; init; }
    public string Country { get; init; }
    public List<Orderitem> OrderItems { get; set; }
    public decimal Total { get; set; }
}

[Keyless]
public record Orderitem
{
    public string ProductName { get; init; }
    public int Units { get; init; }
    public double UnitPrice { get; init; }
    public string PictureUrl { get; init; }
}