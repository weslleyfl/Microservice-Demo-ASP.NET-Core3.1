using CustomerApi.Domain.Entities;
using CustomerApi.Data.Repository.v1;
using CustomerApi.Service.v1.Command;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace CustomerApi.Service.Test.v1.Command
{
    public class CreateCustomerCommandHandlerTests
    {
        private readonly IRepository<Customer> _repository;
        private readonly CreateCustomerCommandHandler _testee;

        public CreateCustomerCommandHandlerTests()
        {
            _repository = A.Fake<IRepository<Customer>>();
            _testee = new CreateCustomerCommandHandler(_repository);
        }

        [Fact]
        public async void Handle_ShouldCallAddAsync()
        {
            await _testee.Handle(new CreateCustomerCommand(), default);

            A.CallTo(() => _repository.AddAsync(A<Customer>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void Handle_ShouldReturnCreatedCustomer()
        {
            // arrange - crio um objeto fake - com o metodo e seu retorno -os Cenarios
            A.CallTo(() => _repository.AddAsync(A<Customer>.Ignored)).Returns(new Customer()
            {
                FirstName = "Weslley"
            });

            // act - realizo a açao
            var result = await _testee.Handle(new CreateCustomerCommand(), default);

            // assert - valido dois resultdos 
            result.Should().BeOfType<Customer>();
            result.FirstName.Should().Be("Weslley");
        }

    }
}
