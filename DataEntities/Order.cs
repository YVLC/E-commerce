using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataEntities;

public record Order
{
    [Key]
    public Guid OrderNumber { get; init; }

    public DateTime Date { get; init; } = DateTime.UtcNow;

    [Required]
    public string Status { get; init; } = string.Empty;

    public string? Description { get; init; } // Nullable, since it may not always have a value

    [Required]
    public string Street { get; init; } = string.Empty;

    [Required]
    public string City { get; init; } = string.Empty;

    [Required]
    public string Country { get; init; } = string.Empty;

    [Required]
    public string PostalCode { get; init; } = string.Empty;
    [Required]
    public Guid ClientId { get; init; } 

    public List<OrderItem> OrderItems { get; set; } = new(); // Updated to use `OrderItem`

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }
}

public record OrderItem
{
    [Key]
    public Guid OrderItemId { get; init; } = new Guid();
    public int Id { get; init; } // Primary key for OrderItem

    [Required]
    public string ProductName { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Units { get; init; } // Ensures at least 1 unit is ordered

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; init; } // Use `decimal` for monetary values to avoid floating-point issues

    public string? PictureUrl { get; init; } // Nullable, in case there’s no picture

    [ForeignKey(nameof(Order))] // Establishing the relationship
    public Guid OrderNumber { get; set; }
}