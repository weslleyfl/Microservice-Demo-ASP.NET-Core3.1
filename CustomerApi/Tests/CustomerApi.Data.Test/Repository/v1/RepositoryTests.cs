using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomerApi.Data.Database;
using CustomerApi.Domain.Entities;
using CustomerApi.Data.Repository.v1;
using CustomerApi.Data.Test.Infrastructure;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace CustomerApi.Data.Test.Repository.v1
{
    public class RepositoryTests : DatabaseTestBase
    {
        private readonly CustomerContext _customerContext;
        private readonly Repository<Customer> _testee;
        private readonly Repository<Customer> _testeeFake;
        private readonly Customer _newCustomer;

        public RepositoryTests()
        {
            _customerContext = A.Fake<CustomerContext>();
            _testeeFake = new Repository<Customer>(_customerContext);
            _testee = new Repository<Customer>(Context);
            _newCustomer = new Customer
            {
                FirstName = "Son",
                LastName = "Goku",
                Birthday = new DateTime(737, 04, 16),
                Age = 1282
            };
        }
        [Theory]
        [InlineData("Changed")]
        public async void UpdateCustomerAsync_WhenCustomerIsNotNull_ShouldReturnCustomer(string firstName)
        {
            // arrange -- Preparaçao do cenario
            var customer = Context.Customers.FirstOrDefault();
            customer.FirstName = firstName;


            // act -- Simulando a açao , chamada do metodo
            var result = await _testee.UpdateAsync(customer);

            // assert -- Validaçao
            result.Should().BeOfType<Customer>(because: "O tipo retornado deve ser um Customer");
            result.FirstName.Should().Be(firstName);

        }

        [Fact]
        public void AddAsync_WhenEntityIsNull_ThrowsException()
        {
            _testee.Invoking(x => x.AddAsync(null)).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddAsync_WhenExceptionOccurs_ThrowsException()
        {
            // Arrange and act -- preparaçao e simulaçao de chamada fake
            A.CallTo(() => _customerContext.SaveChangesAsync(default))
                .Throws<Exception>();

            // assert
            _testeeFake.Invoking(x => x.AddAsync(new Customer()))
                .Should()
                .Throw<Exception>()
                .WithMessage("entity could not be saved");

        }

        [Fact]
        public async void CreateCustomerAsync_WhenCustomerIsNotNull_ShouldReturnCustomer()
        {
            // act
            var result = await _testee.AddAsync(_newCustomer);

            // assert
            result.Should().BeOfType<Customer>();

        }

        [Fact]
        public async void CreateCustomerAsync_WhenCustomerIsNotNull_ShouldShouldAddCustomer()
        {
            // arrange
            var customerCount = Context.Customers.Count();

            // act
            await _testee.AddAsync(_newCustomer);

            // assert
            Context.Customers.Count().Should().Be(customerCount + 1);

        }

        [Fact]
        public void GetAll_WhenExceptionOccurs_ThrowsException()
        {
            // _customerContext.Set<TEntity>();
            A.CallTo(() => _customerContext.Set<Customer>()).Throws<Exception>();

            // assert
            _testeeFake.Invoking(x => x.GetAll())
                .Should()
                .Throw<Exception>()
                .WithMessage("Couldn't retrieve entities");
        }

        [Fact]
        public void UpdateAsync_WhenEntityIsNull_ThrowsException()
        {
            // o que estou testando 
            // e a chamada deste metdodo public async Task<TEntity> UpdateAsync(TEntity entity)
            _testee.Invoking(x => x.UpdateAsync(null)).Should().Throw<ArgumentNullException>();
        }



        [Fact]
        public void UpdateAsync_WhenExceptionOccurs_ThrowsException()
        {
            // arrange - simulaçao fake
            A.CallTo(() => _customerContext.SaveChangesAsync(default)).Throws<Exception>();

            // assert - validar o retorno fake
            _testeeFake.Invoking(x => x.UpdateAsync(new Customer()))
                .Should()
                .Throw<Exception>()
                .WithMessage("entity could not be updated");
        }



    }
}
