using BuckeyeMarketplaceAPI.Models;
using Xunit;

namespace BuckeyeMarketplaceAPI.Tests;

/// <summary>
/// Pure unit tests for Order.CalculateTotal().
/// No database. No HTTP. Just new Order() + Assert.
/// </summary>
public class OrderTotalTests
{
    [Fact]
    public void SingleItem_ReturnsUnitPriceTimesQuantity()
    {
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem { UnitPrice = 25.00m, Quantity = 2 }
            }
        };

        Assert.Equal(50.00m, order.CalculateTotal());
    }

    [Fact]
    public void MultipleItems_ReturnsSumOfLineItems()
    {
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem { UnitPrice = 10.00m, Quantity = 3 },  // 30.00
                new OrderItem { UnitPrice = 5.50m,  Quantity = 2 },  // 11.00
                new OrderItem { UnitPrice = 99.99m, Quantity = 1 }   // 99.99
            }
        };

        Assert.Equal(140.99m, order.CalculateTotal());
    }

    [Fact]
    public void EmptyOrder_ReturnsZero()
    {
        var order = new Order();

        Assert.Equal(0m, order.CalculateTotal());
    }

    [Fact]
    public void SingleItemQuantityOne_ReturnsUnitPrice()
    {
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem { UnitPrice = 49.99m, Quantity = 1 }
            }
        };

        Assert.Equal(49.99m, order.CalculateTotal());
    }

    [Fact]
    public void LargeQuantity_CalculatesCorrectly()
    {
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem { UnitPrice = 0.99m, Quantity = 1000 }
            }
        };

        Assert.Equal(990.00m, order.CalculateTotal());
    }

    [Fact]
    public void FractionalPrices_NoRoundingErrors()
    {
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem { UnitPrice = 19.99m, Quantity = 3 },  // 59.97
                new OrderItem { UnitPrice = 7.49m,  Quantity = 4 }   // 29.96
            }
        };

        Assert.Equal(89.93m, order.CalculateTotal());
    }

    [Fact]
    public void CalculateTotal_DoesNotMutateTotalAmount()
    {
        var order = new Order
        {
            TotalAmount = 0m,
            Items = new List<OrderItem>
            {
                new OrderItem { UnitPrice = 20.00m, Quantity = 2 }
            }
        };

        var result = order.CalculateTotal();

        // CalculateTotal returns the value; it should NOT change TotalAmount
        Assert.Equal(40.00m, result);
        Assert.Equal(0m, order.TotalAmount);
    }
}
