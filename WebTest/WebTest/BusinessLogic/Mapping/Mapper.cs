using WebTest.Models;
using WebTest.ViewModels.Cart;
using WebTest.ViewModels.DTOs.Responses;
using WebTest.ViewModels.Manager;
using WebTest.ViewModels.Order;
using WebTest.ViewModels.Product;

namespace WebTest.BusinessLogic.Mapping;

public static class Mapper
{
    public static ProductViewModel ToViewModel(this Product product)
    {
        return new ProductViewModel
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Price = product.Price,
            Category = product.Category
        };
    }

    public static ProductResponseDto ToResponseDto(this Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Price = product.Price,
            Category = product.Category
        };
    }

    public static CartItemViewModel ToCartItemViewModel(this Product product, int quantity)
    {
        return new CartItemViewModel
        {
            ProductId = product.Id,
            ProductName = product.Name,
            ProductCode = product.Code,
            Price = product.Price,
            Quantity = quantity,
            Subtotal = product.Price * quantity
        };
    }

    public static OrderViewModel ToViewModel(this Order order)
    {
        return new OrderViewModel
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            ShipmentDate = order.ShipmentDate,
            Status = order.Status,
            CustomerName = order.Customer?.Name ?? string.Empty,
            Items = order.Items.Select(ToViewModel).ToList(),
            Total = order.Items.Sum(i => i.ItemPrice * i.ItemsCount)
        };
    }

    public static OrderItemViewModel ToViewModel(this OrderItem orderItem)
    {
        return new OrderItemViewModel
        {
            Id = orderItem.Id,
            ProductId = orderItem.ItemId,
            ProductName = orderItem.Product?.Name ?? string.Empty,
            ProductCode = orderItem.Product?.Code ?? string.Empty,
            Quantity = orderItem.ItemsCount,
            Price = orderItem.ItemPrice,
            Subtotal = orderItem.ItemPrice * orderItem.ItemsCount
        };
    }

    public static OrderResponseDto ToResponseDto(this Order order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            ShipmentDate = order.ShipmentDate,
            Status = order.Status,
            Items = order.Items.Select(ToResponseDto).ToList()
        };
    }

    public static OrderItemResponseDto ToResponseDto(this OrderItem orderItem)
    {
        return new OrderItemResponseDto
        {
            Id = orderItem.Id,
            ProductId = orderItem.ItemId,
            ProductName = orderItem.Product?.Name ?? string.Empty,
            Quantity = orderItem.ItemsCount,
            Price = orderItem.ItemPrice
        };
    }

    public static UserViewModel ToViewModel(this AppUser user)
    {
        return new UserViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Role = user.Role
        };
    }
}
