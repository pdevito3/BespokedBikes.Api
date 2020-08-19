
namespace BespokedBikes.Api.Tests.RepositoryTests.Sale
{
    using Application.Dtos.Sale;
    using FluentAssertions;
    using BespokedBikes.Api.Tests.Fakes.Sale;
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
    public class DeleteSaleRepositoryTests
    { 
        
        [Fact]
        public void DeleteSale_ReturnsProperCount()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SaleDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSaleOne = new FakeSale { }.Generate();
            var fakeSaleTwo = new FakeSale { }.Generate();
            var fakeSaleThree = new FakeSale { }.Generate();

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Sales.AddRange(fakeSaleOne, fakeSaleTwo, fakeSaleThree);

                var service = new SaleRepository(context, new SieveProcessor(sieveOptions));
                service.DeleteSale(fakeSaleTwo);

                context.SaveChanges();

                //Assert
                var saleList = context.Sales.ToList();

                saleList.Should()
                    .NotBeEmpty()
                    .And.HaveCount(2);

                saleList.Should().ContainEquivalentOf(fakeSaleOne);
                saleList.Should().ContainEquivalentOf(fakeSaleThree);
                Assert.DoesNotContain(saleList, s => s == fakeSaleTwo);

                context.Database.EnsureDeleted();
            }
        }
    } 
}