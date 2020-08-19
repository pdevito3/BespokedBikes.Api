
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
    public class GetDiscountRepositoryTests
    { 
        
        [Fact]
        public void GetDiscount_ParametersMatchExpectedValues()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"DiscountDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeDiscount = new FakeDiscount { }.Generate();

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Discounts.AddRange(fakeDiscount);
                context.SaveChanges();

                var service = new DiscountRepository(context, new SieveProcessor(sieveOptions));

                //Assert
                var discountById = service.GetDiscount(fakeDiscount.DiscountId);
                                discountById.DiscountId.Should().Be(fakeDiscount.DiscountId);
                discountById.ProductId.Should().Be(fakeDiscount.ProductId);
                discountById.BeginDate.Should().Be(fakeDiscount.BeginDate);
                discountById.EndDate.Should().Be(fakeDiscount.EndDate);
                discountById.DiscountPercentage.Should().Be(fakeDiscount.DiscountPercentage);
            }
        }
        
        [Fact]
        public void GetDiscounts_CountMatchesAndContainsEquivalentObjects()
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
                context.SaveChanges();

                var service = new DiscountRepository(context, new SieveProcessor(sieveOptions));

                var discountRepo = service.GetDiscounts(new DiscountParametersDto());

                //Assert
                discountRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(3);

                discountRepo.Should().ContainEquivalentOf(fakeDiscountOne);
                discountRepo.Should().ContainEquivalentOf(fakeDiscountTwo);
                discountRepo.Should().ContainEquivalentOf(fakeDiscountThree);

                context.Database.EnsureDeleted();
            }
        }
        
        [Fact]
        public void GetDiscounts_ReturnExpectedPageSize()
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
                context.SaveChanges();

                var service = new DiscountRepository(context, new SieveProcessor(sieveOptions));

                var discountRepo = service.GetDiscounts(new DiscountParametersDto { PageSize = 2 });

                //Assert
                discountRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(2);

                discountRepo.Should().ContainEquivalentOf(fakeDiscountOne);
                discountRepo.Should().ContainEquivalentOf(fakeDiscountTwo);

                context.Database.EnsureDeleted();
            }
        }
        
        [Fact]
        public void GetDiscounts_ReturnExpectedPageNumberAndSize()
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
                context.SaveChanges();

                var service = new DiscountRepository(context, new SieveProcessor(sieveOptions));

                var discountRepo = service.GetDiscounts(new DiscountParametersDto { PageSize = 1, PageNumber = 2 });

                //Assert
                discountRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(1);

                discountRepo.Should().ContainEquivalentOf(fakeDiscountTwo);

                context.Database.EnsureDeleted();
            }
        }
        
        
        [Fact]
        public void GetDiscounts_FilterDiscountIdListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"DiscountDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeDiscountOne = new FakeDiscount { }.Generate();
            fakeDiscountOne.DiscountId = 1;

            var fakeDiscountTwo = new FakeDiscount { }.Generate();
            fakeDiscountTwo.DiscountId = 2;

            var fakeDiscountThree = new FakeDiscount { }.Generate();
            fakeDiscountThree.DiscountId = 3;

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Discounts.AddRange(fakeDiscountOne, fakeDiscountTwo, fakeDiscountThree);
                context.SaveChanges();

                var service = new DiscountRepository(context, new SieveProcessor(sieveOptions));

                var discountRepo = service.GetDiscounts(new DiscountParametersDto { Filters = $"DiscountId == 2" });

                //Assert
                discountRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetDiscounts_FilterProductIdListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"DiscountDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeDiscountOne = new FakeDiscount { }.Generate();
            fakeDiscountOne.ProductId = 1;

            var fakeDiscountTwo = new FakeDiscount { }.Generate();
            fakeDiscountTwo.ProductId = 2;

            var fakeDiscountThree = new FakeDiscount { }.Generate();
            fakeDiscountThree.ProductId = 3;

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Discounts.AddRange(fakeDiscountOne, fakeDiscountTwo, fakeDiscountThree);
                context.SaveChanges();

                var service = new DiscountRepository(context, new SieveProcessor(sieveOptions));

                var discountRepo = service.GetDiscounts(new DiscountParametersDto { Filters = $"ProductId == 2" });

                //Assert
                discountRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetDiscounts_FilterBeginDateListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"DiscountDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeDiscountOne = new FakeDiscount { }.Generate();
            fakeDiscountOne.BeginDate = DateTime.Now.AddDays(1);

            var fakeDiscountTwo = new FakeDiscount { }.Generate();
            fakeDiscountTwo.BeginDate = DateTime.Parse(DateTime.Now.AddDays(2).ToString("MM/dd/yyyy"));

            var fakeDiscountThree = new FakeDiscount { }.Generate();
            fakeDiscountThree.BeginDate = DateTime.Now.AddDays(3);

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Discounts.AddRange(fakeDiscountOne, fakeDiscountTwo, fakeDiscountThree);
                context.SaveChanges();

                var service = new DiscountRepository(context, new SieveProcessor(sieveOptions));

                var discountRepo = service.GetDiscounts(new DiscountParametersDto { Filters = $"BeginDate == {DateTime.Now.AddDays(2).ToString("MM/dd/yyyy")}" });

                //Assert
                discountRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetDiscounts_FilterEndDateListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"DiscountDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeDiscountOne = new FakeDiscount { }.Generate();
            fakeDiscountOne.EndDate = DateTime.Now.AddDays(1);

            var fakeDiscountTwo = new FakeDiscount { }.Generate();
            fakeDiscountTwo.EndDate = DateTime.Parse(DateTime.Now.AddDays(2).ToString("MM/dd/yyyy"));

            var fakeDiscountThree = new FakeDiscount { }.Generate();
            fakeDiscountThree.EndDate = DateTime.Now.AddDays(3);

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Discounts.AddRange(fakeDiscountOne, fakeDiscountTwo, fakeDiscountThree);
                context.SaveChanges();

                var service = new DiscountRepository(context, new SieveProcessor(sieveOptions));

                var discountRepo = service.GetDiscounts(new DiscountParametersDto { Filters = $"EndDate == {DateTime.Now.AddDays(2).ToString("MM/dd/yyyy")}" });

                //Assert
                discountRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetDiscounts_FilterDiscountPercentageListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"DiscountDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeDiscountOne = new FakeDiscount { }.Generate();
            fakeDiscountOne.DiscountPercentage = 1;

            var fakeDiscountTwo = new FakeDiscount { }.Generate();
            fakeDiscountTwo.DiscountPercentage = 2;

            var fakeDiscountThree = new FakeDiscount { }.Generate();
            fakeDiscountThree.DiscountPercentage = 3;

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Discounts.AddRange(fakeDiscountOne, fakeDiscountTwo, fakeDiscountThree);
                context.SaveChanges();

                var service = new DiscountRepository(context, new SieveProcessor(sieveOptions));

                var discountRepo = service.GetDiscounts(new DiscountParametersDto { Filters = $"DiscountPercentage == 2" });

                //Assert
                discountRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

    } 
}