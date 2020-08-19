
namespace BespokedBikes.Api.Tests.RepositoryTests.Product
{
    using Application.Dtos.Product;
    using FluentAssertions;
    using BespokedBikes.Api.Tests.Fakes.Product;
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
    public class DeleteProductRepositoryTests
    { 
        
        [Fact]
        public void DeleteProduct_ReturnsProperCount()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"ProductDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeProductOne = new FakeProduct { }.Generate();
            var fakeProductTwo = new FakeProduct { }.Generate();
            var fakeProductThree = new FakeProduct { }.Generate();

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Products.AddRange(fakeProductOne, fakeProductTwo, fakeProductThree);

                var service = new ProductRepository(context, new SieveProcessor(sieveOptions));
                service.DeleteProduct(fakeProductTwo);

                context.SaveChanges();

                //Assert
                var productList = context.Products.ToList();

                productList.Should()
                    .NotBeEmpty()
                    .And.HaveCount(2);

                productList.Should().ContainEquivalentOf(fakeProductOne);
                productList.Should().ContainEquivalentOf(fakeProductThree);
                Assert.DoesNotContain(productList, p => p == fakeProductTwo);

                context.Database.EnsureDeleted();
            }
        }
    } 
}