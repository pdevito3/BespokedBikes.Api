
namespace BespokedBikes.Api.Tests.RepositoryTests.Customer
{
    using Application.Dtos.Customer;
    using FluentAssertions;
    using BespokedBikes.Api.Tests.Fakes.Customer;
    using Infrastructure.Persistence.Contexts;
    using Infrastructure.Persistence.Repositories;
    using Infrastructure.Shared.Shared;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Sieve.Models;
    using Sieve.Services;
    using System;
    using System.Linq;
    using Xunit;

    [Collection("Sequential")]
    public class DeleteCustomerRepositoryTests
    { 
        
        [Fact]
        public void DeleteCustomer_ReturnsProperCount()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"CustomerDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeCustomerOne = new FakeCustomer { }.Generate();
            var fakeCustomerTwo = new FakeCustomer { }.Generate();
            var fakeCustomerThree = new FakeCustomer { }.Generate();

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Customers.AddRange(fakeCustomerOne, fakeCustomerTwo, fakeCustomerThree);

                var service = new CustomerRepository(context, new SieveProcessor(sieveOptions));
                service.DeleteCustomer(fakeCustomerTwo);

                context.SaveChanges();

                //Assert
                var customerList = context.Customers.ToList();

                customerList.Should()
                    .NotBeEmpty()
                    .And.HaveCount(2);

                customerList.Should().ContainEquivalentOf(fakeCustomerOne);
                customerList.Should().ContainEquivalentOf(fakeCustomerThree);
                Assert.DoesNotContain(customerList, c => c == fakeCustomerTwo);

                context.Database.EnsureDeleted();
            }
        }
    } 
}