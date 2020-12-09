using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrderApi.Data.Database;
using OrderApi.Data.Repository.v1;
using OrderApi.Data.Test.Infrastructure;
using OrderApi.Domain;
using Xunit;

namespace OrderApi.Data.Test.Repository
{
    /// <summary>
    /// https://fakeiteasy.readthedocs.io/en/stable/specifying-a-call-to-configure/
    /// </summary>
    public class RepositoryTests : DatabaseTestBase
    {
        private readonly OrderContext _orderContext;
        private readonly Repository<Order> _testee;
        private readonly Repository<Order> _testeeFake;
        private readonly Order _newOrder;

        public RepositoryTests()
        {
            // Create Fake object
            _orderContext = A.Fake<OrderContext>();
            _testeeFake = new Repository<Order>(_orderContext);
            _testee = new Repository<Order>(Context);
            _newOrder = new Order()
            {
                CustomerGuid = Guid.Parse("a4498698-bda1-4241-b3b7-4c8fe9649c54"),
                CustomerFullName = "Son Goku",
                OrderState = OrderState.PaidOrder
            };

        }

        [Theory]
        [InlineData("Changed")]
        public async void UpdateOrderAsync_WhenOrderIsNotNull_ShouldReturnOrder(string newOrderFullName)
        {
            // Arrange
            var order = Context.Orders.First();
            order.CustomerFullName = newOrderFullName;

            // Act
            var result = await _testee.UpdateAsync(order);

            // Assert
            result.Should().BeOfType<Order>(because: "O pedido retornado do metodo updateAsync tem que ser um objeto Order");
            result.CustomerFullName.Should().Be(expected: newOrderFullName, because: $"O nome tem que sofrer a alteraçao, deve ser igual a {newOrderFullName}");
        }

        [Fact]
        public void UpdateAsync_QuandoPedidoForNull_DeveLancarUmaExcecao()
        {
            // Assert
            _testee.Invoking(repository => repository.UpdateAsync(null))
                .Should()
                .Throw<ArgumentNullException>(because: "Lança exceção pois o objeto pedido é null");
        }

        [Fact]
        public void AddAsync_WhenEntityIsNull_ThrowsException()
        {
            // Assert
            _testee.Invoking(repository => repository.AddAsync(null))
                .Should()
                .Throw<ArgumentNullException>(because: "Se a entidade recebida for null deve lançar um Null Exception.");
        }

        [Fact]
        public void AddAsync_WhenExceptionOccurs_ThrowsException()
        {
            // Act
            // Configure a fake object behaviour
            A.CallTo(() => _orderContext.SaveChangesAsync(default)).Throws<Exception>();

            // Assert
            _testeeFake.Invoking(r => r.AddAsync(new Order()))
                .Should()
                .Throw<Exception>()
                .WithMessage("entity could not be saved");
        }

        [Fact]
        public async void AddAsync_CreateOrderAsync_WhenOrderIsNotNull_ShouldReturnOrder()
        {
            // Act
            var result = await _testee.AddAsync(_newOrder);

            // assert
            result.Should()
                .BeOfType<Order>(because: "O pedido adicinado deve retornar o mesmo objeto pedido");
        }

        [Fact]
        public async void AddAsync_CreateOrderAsync_WhenOrderIsNotNull_ShouldAddOrder()
        {
            // arrange
            var orderCount = Context.Orders.Count();

            // act
            await _testee.AddAsync(_newOrder);

            // assert
            Context.Orders.Count().Should().Be(orderCount + 1);
        }

        [Fact]
        public void GetAll_WhenExceptionOccurs_ThrowsException()
        {
            // A.CallTo -- method call or property get
            // One of the first steps in configuring a fake object's behavior is to specify which call to configure.
            // Like most FakeItEasy actions, this is done using a method on the A class: A.CallTo.
            // Act
            A.CallTo(() => _orderContext.Set<Order>()).Throws<Exception>();

            // assert
            _testeeFake.Invoking(r => r.GetAll())
                .Should()
                .Throw<Exception>()
                .WithMessage("Couldn't retrieve entities");
        }

        [Fact]
        public void UpdateAsync_WhenEntityIsNull_ThrowsException()
        {
            _testee.Invoking(r => r.UpdateAsync(null))
                .Should()
                .Throw<ArgumentNullException>(because: "Lança uma execeçao em caso de objeto null");
        }

        [Fact]
        public void UpdateAsync_WhenExceptionOccurs_ThrowsException()
        {
            // Act
            A.CallTo(() => _orderContext.SaveChangesAsync(default)).Throws<Exception>();

            // Asserrt
            _testeeFake.Invoking(r => r.UpdateAsync(new Order()))
                .Should()
                .Throw<Exception>()
                .WithMessage("entity could not be updated");

        }

        [Fact]
        public void UpdateRangeAsync_WhenEntityIsNull_ThrowsException()
        {
            _testee.Invoking(x => x.UpdateRangeAsync(null))
                .Should()
                .Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateRangeAsync_WhenExceptionOccurs_ThrowsException()
        {
            // Act
            A.CallTo(() => _orderContext.SaveChangesAsync(default)).Throws<Exception>();

            // Assert
            _testeeFake.Invoking(x => x.UpdateRangeAsync(new List<Order>()))
                .Should()
                .Throw<Exception>()
                .WithMessage("Entidade nao pode ser alterada");
        }

        [Theory]
        [InlineData("Changed")]
        public async void UpdateOrderAsync_WhenOrdersIsNotNull_ShouldUpdateOrders(string newOrderFullName)
        {
            // arrange
            var orders = _testee.GetAll().ToList();
            orders.ForEach(o => o.CustomerFullName = newOrderFullName);

            // act
            await _testee.UpdateRangeAsync(orders);

            // assert
            (Context.Orders.All(o => o.CustomerFullName == newOrderFullName)).Should().BeTrue();

        }


    }
}
