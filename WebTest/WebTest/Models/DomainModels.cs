using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebTest.Models;

public enum UserRole
{
    Customer = 0,
    Manager = 1
}

public class AppUser
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }

    public Customer? Customer { get; set; }
}

/// <summary>
/// Заказчик (Customer)
/// </summary>
public class Customer
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Код заказчика в формате XXXX-ГГГГ
    /// </summary>
    [Required, MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    public string? Address { get; set; }

    /// <summary>
    /// % скидки, 0 или null – скидки нет
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal? Discount { get; set; }

    /// <summary>
    /// Связанный аккаунт (для входа в систему)
    /// </summary>
    public int? AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    public List<Order> Orders { get; set; } = new();
}

/// <summary>
/// Товар (Product)
/// </summary>
public class Product
{
    public Guid Id { get; set; }

    /// <summary>
    /// Код товара, формат «XX-XXXX-YYXX»
    /// </summary>
    [Required, MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [MaxLength(30)]
    public string? Category { get; set; }

    public List<OrderItem> OrderItems { get; set; } = new();
}

public static class OrderStatuses
{
    public const string New = "Новый.";
    public const string InProgress = "Выполняется.";
    public const string Completed = "Выполнен.";

    public static readonly string[] All = { New, InProgress, Completed };
}

/// <summary>
/// Заказ (Order)
/// </summary>
public class Order
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }

    /// <summary>
    /// ORDER_DATE
    /// </summary>
    [Required]
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// SHIPMENT_DATE
    /// </summary>
    public DateTime? ShipmentDate { get; set; }

    /// <summary>
    /// ORDER_NUMBER
    /// </summary>
    public int OrderNumber { get; set; }

    /// <summary>
    /// STATUS (string)
    /// </summary>
    [Required]
    public string Status { get; set; } = OrderStatuses.New;

    public List<OrderItem> Items { get; set; } = new();
}

/// <summary>
/// Элемент заказа (OrderItem)
/// </summary>
public class OrderItem
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }
    public Order? Order { get; set; }

    public Guid ItemId { get; set; }
    public Product? Product { get; set; }

    /// <summary>
    /// ITEMS_COUNT
    /// </summary>
    [Range(1, 1000)]
    public int ItemsCount { get; set; }

    /// <summary>
    /// ITEM_PRICE
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal ItemPrice { get; set; }
}

