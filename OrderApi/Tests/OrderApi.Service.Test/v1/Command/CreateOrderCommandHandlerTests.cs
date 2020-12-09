using FakeItEasy;
using FluentAssertions;
using OrderApi.Data.Repository.v1;
using OrderApi.Domain;
using OrderApi.Service.v1.Command;
using Xunit;

namespace OrderApi.Service.Test.v1.Command
{
    public class CreateOrderCommandHandlerTests
    {
        private readonly IRepository<Order> _repository;
        private readonly CreateOrderCommandHandler _testee;

        public CreateOrderCommandHandlerTests()
        {
            // Arrange
            _repository = A.Fake<IRepository<Order>>();
            _testee = new CreateOrderCommandHandler(_repository);
        }

        [Fact]
        public async void Handle_ShouldReturnCreatedOrder()
        {
           
            // Arrange           
            // Para quem chamar este metodo retorne uma Order com um CustomerFullName = "Bruce Wayne"
            A.CallTo(() => _repository.AddAsync(A<Order>.Ignored)).Returns(new Order { CustomerFullName = "Bruce Wayne" });

            // Act
            // Aqui estou chamando o metodo _repository.AddAsync
            var orderResult = await _testee.Handle(new CreateOrderCommand(), default);

            // Assert
            orderResult.Should().BeOfType<Order>();
            orderResult.CustomerFullName.Should().Be("Bruce Wayne");

           
        }

        [Fact]
        public async void Handle_ShouldCallRepositoryAddAsync()
        {

            await _testee.Handle(new CreateOrderCommand(), default);

            // Assert
            A.CallTo(() => _repository.AddAsync(A<Order>._)).MustHaveHappenedOnceExactly();


        }
    }
}
