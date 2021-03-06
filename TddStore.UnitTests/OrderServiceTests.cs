﻿using NUnit.Framework;
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
        private ICustomerService _customerService;
        private IOrderFulfillmentService _orderFulfillmentService;

        [SetUp]
        public void SetupTestFixture()
        {
            _orderDataService = Mock.Create<IOrderDataService>();
            _customerService = Mock.Create<ICustomerService>();
            _orderFulfillmentService = Mock.Create<IOrderFulfillmentService>();
            _orderService = new OrderService(_orderDataService, _customerService);
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
            try
            {
                _orderService.PlaceOrder(customerId, shoppingCart);
            }
            catch (InvalidOrderException ex)
            {

                // Assert
                //Assert.Throws<InvalidOrderException>(() => _orderService.PlaceOrder(customerId, shoppingCart));
                Mock.Assert(_orderDataService);
                Assert.Pass();
            }

            Assert.Fail();

        }

        [Test]
        public void WhenAValidCustomerPlacesAValidOrder_OrderServiceShouldGetACustomerFromCustomerService()
        {
            // Arrange
            var shoppingCart = new ShoppingCart();
            shoppingCart.Items.Add(new ShoppingCartItem { ItemId = Guid.NewGuid(), Quantity = 1 });
            var customerId = Guid.NewGuid();

            var customerToReturn = new Customer { Id = customerId, FirstName = "Fred", LastName = "Flinstone" };

            Mock.Arrange(() => _customerService.GetCustomer(customerId))
                .Returns(customerToReturn)
                .OccursOnce();

            // Act
            _orderService.PlaceOrder(customerId, shoppingCart);

            // Assert
            Mock.Assert(_customerService);
        }
        [Test]
        public void WhenUserPlacesOrderWithItemThatIsInInventoryOrderFulfillmentWorkflowShouldComplete()
        {
            //Arrange
            var shoppingCart = new ShoppingCart();
            shoppingCart.Items.Add(new ShoppingCartItem { ItemId = Guid.NewGuid(), Quantity = 1 });
            var customerId = Guid.NewGuid();
            var customer = new Customer { Id = customerId };
            var orderFulfillmentSessionId = Guid.NewGuid();

             Mock.Arrange(() => _customerService.GetCustomer(customerId)).Returns(customer).OccursOnce();

            Mock.Arrange(() => _orderFulfillmentService.OpenSession(Arg.IsAny<string>(), Arg.IsAny<string>()))
                .Returns(orderFulfillmentSessionId)
                .InOrder();
            Mock.Arrange(() => _orderFulfillmentService.IsInInventory(orderFulfillmentSessionId, itemId, 1))
                .Returns(true)
                .InOrder();
            Mock.Arrange(() => 
                _orderFulfillmentService.
                    PlaceOrder(orderFulfillmentSessionId, Arg.IsAny<IDictionary<Guid, int>>(), Arg.IsAny<string>()))
                .Returns(true)
                .InOrder();
            Mock.Arrange(() => _orderFulfillmentService.CloseSession(orderFulfillmentSessionId))
                .InOrder();

            //Act
            _orderService.PlaceOrder(customerId, shoppingCart);

            //Assert
            Mock.Assert(_orderFulfillmentService);
        }


    }
}
