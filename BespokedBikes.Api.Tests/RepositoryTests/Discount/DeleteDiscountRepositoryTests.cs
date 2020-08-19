
namespace BespokedBikes.Api.Tests.RepositoryTests.Discount
{
    using Application.Dtos.Discount;
    using FluentAssertions;
    using BespokedBikes.Api.Tests.Fakes.Discount;
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
    public class DeleteDiscountRepositoryTests
    { 
        
        [Fact]
        public void DeleteDiscount_ReturnsProperCount()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"DiscountDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeDiscountOne = new FakeDiscount { }.Generate();
            var fakeDiscountTwo = new FakeDiscount { }.Generate();
            var fakeDiscountThree = new FakeDiscount { }.Generate();

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Discounts.AddRange(fakeDiscountOne, fakeDiscountTwo, fakeDiscountThree);

                var service = new DiscountRepository(context, new SieveProcessor(sieveOptions));
                service.DeleteDiscount(fakeDiscountTwo);

                context.SaveChanges();

                //Assert
                var discountList = context.Discounts.ToList();

                discountList.Should()
                    .NotBeEmpty()
                    .And.HaveCount(2);

                discountList.Should().ContainEquivalentOf(fakeDiscountOne);
                discountList.Should().ContainEquivalentOf(fakeDiscountThree);
                Assert.DoesNotContain(discountList, d => d == fakeDiscountTwo);

                context.Database.EnsureDeleted();
            }
        }
    } 
}