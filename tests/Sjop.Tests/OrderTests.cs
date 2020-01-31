using System;
using Xunit;
using Sjop.Models;

namespace sjop.tests
{
    public class OrderTests
    {
        [Fact]
        public void OrderTotalIncVatCorrect()
        {
            var order = new Order() { Id = 1 };
            var p1 = new Product() { Id = 1, Price = 10, VatPercent = 25 };
            var p2 = new Product() { Id = 2, Price = 15, VatPercent = 25 };
            var ol1 = new OrderLine() { OrderId = 1, ProductId = 1, Product = p1, Quantity = 1, Price = p1.Price };
            var ol2 = new OrderLine() { OrderId = 1, ProductId = 2, Product = p2, Quantity = 2, Price = p2.Price };
            order.OrderLines.Add(ol1);
            order.OrderLines.Add(ol2);

            Assert.Equal(50, order.OrderTotalprice);

        }
    }
}
