using NUnit.Framework;
using System;
using System.Linq;
using TddStore.Core;
using TddStore.Core.Exceptions;
using Telerik.JustMock;


namespace TddStore.UnitTests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private OrderService _orderService;
        private IOrderDataService _orderDataService;

        [SetUp]
        public void SetupTestFixture()
        {
            _orderDataService = Mock.Create<IOrderDataService>();
            _orderService = new OrderService(_orderDataService);
        }

        [Test]
        public void WhenUserPlacesACorrectOrderThenAnOrderNumberShouldBeReturned()
        {
            var shoppingCart = new ShoppingCart();
            shoppingCart.Items.Add(new ShoppingCartItem { ItemId = Guid.NewGuid(), Quantity = 1});
            var customerId = Guid.NewGuid();
            var expectedOrderId = Guid.NewGuid();

            Mock.Arrange(() => _orderDataService.Save(Arg.IsAny<Order>()))
                .Returns(expectedOrderId)
                .OccursOnce();

            //OrderService orderService = new OrderService(orderDataService);

            var result = _orderService.PlaceOrder(customerId, shoppingCart);

            Assert.AreEqual(expectedOrderId, result);
            // calls to Mock to verify that its rules have been followed
            Mock.Assert(_orderDataService);
        }

        [Test]
        public void WhenAUserAttemptsToOrderAnItemWithAQuantityOfZero_ThrowInvalidOrderException()
        {
            // Arrange
            var shoppingCart = new ShoppingCart();
            shoppingCart.Items.Add(new ShoppingCartItem { ItemId = Guid.NewGuid(), Quantity = 0 });
            var customerId = Guid.NewGuid();
            var expectedOrderId = Guid.NewGuid();

            Mock.Arrange(() => _orderDataService.Save(Arg.IsAny<Order>()))
                .Returns(expectedOrderId)
                .OccursNever();

            // Act
           // _orderService.PlaceOrder(customerId, shoppingCart);
            
            // Assert
            Assert.Throws<InvalidOrderException>(() => _orderService.PlaceOrder(customerId, shoppingCart));
            Mock.Assert(_orderDataService);

        }



    }
}
