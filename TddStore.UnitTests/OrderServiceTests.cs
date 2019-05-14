using NUnit.Framework;
using System;
using System.Linq;
using TddStore.Core;
using Telerik.JustMock;


namespace TddStore.UnitTests
{
    [TestFixture]
    public class OrderServiceTests
    {
        [Test]
        public void WhenUserPlacesACorrectOrderThenAnOrderNumberShouldBeReturned()
        {
            var shoppingCart = new ShoppingCart();
            shoppingCart.Items.Add(new ShoppingCartItem { ItemId = Guid.NewGuid(), Quantity = 1});
            var customerId = Guid.NewGuid();
            var expectedOrderId = Guid.NewGuid();

            // creates the mocked interface to use in OrderService
            var orderDataService = Mock.Create<IOrderDataService>();

            /*
            makes mock object into stub that responds to calls to Save method
            --> getting 'Matcher' which tells arrangement that I'm not concerned with
            specifics of a parameter, I just want to define a behvior for params that 
            meet a certain pattern
            --> elevates Dummy mock object to a Stub responding to a method call
            */
            Mock.Arrange(() => orderDataService.Save(Arg.IsAny<Order>())).Returns(expectedOrderId);

            OrderService orderService = new OrderService(orderDataService);

            var result = orderService.PlaceOrder(customerId, shoppingCart);

            Assert.AreEqual(expectedOrderId, result);
        }



    }
}
