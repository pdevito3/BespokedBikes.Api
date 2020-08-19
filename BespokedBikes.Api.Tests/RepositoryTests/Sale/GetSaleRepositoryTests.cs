
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
    public class GetSaleRepositoryTests
    { 
        
        [Fact]
        public void GetSale_ParametersMatchExpectedValues()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SaleDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSale = new FakeSale { }.Generate();

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Sales.AddRange(fakeSale);
                context.SaveChanges();

                var service = new SaleRepository(context, new SieveProcessor(sieveOptions));

                //Assert
                var saleById = service.GetSale(fakeSale.SaleId);
                                saleById.SaleId.Should().Be(fakeSale.SaleId);
                saleById.ProductId.Should().Be(fakeSale.ProductId);
                saleById.SalespersonId.Should().Be(fakeSale.SalespersonId);
                saleById.SaleDate.Should().Be(fakeSale.SaleDate);
            }
        }
        
        [Fact]
        public void GetSales_CountMatchesAndContainsEquivalentObjects()
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
                context.SaveChanges();

                var service = new SaleRepository(context, new SieveProcessor(sieveOptions));

                var saleRepo = service.GetSales(new SaleParametersDto());

                //Assert
                saleRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(3);

                saleRepo.Should().ContainEquivalentOf(fakeSaleOne);
                saleRepo.Should().ContainEquivalentOf(fakeSaleTwo);
                saleRepo.Should().ContainEquivalentOf(fakeSaleThree);

                context.Database.EnsureDeleted();
            }
        }
        
        [Fact]
        public void GetSales_ReturnExpectedPageSize()
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
                context.SaveChanges();

                var service = new SaleRepository(context, new SieveProcessor(sieveOptions));

                var saleRepo = service.GetSales(new SaleParametersDto { PageSize = 2 });

                //Assert
                saleRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(2);

                saleRepo.Should().ContainEquivalentOf(fakeSaleOne);
                saleRepo.Should().ContainEquivalentOf(fakeSaleTwo);

                context.Database.EnsureDeleted();
            }
        }
        
        [Fact]
        public void GetSales_ReturnExpectedPageNumberAndSize()
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
                context.SaveChanges();

                var service = new SaleRepository(context, new SieveProcessor(sieveOptions));

                var saleRepo = service.GetSales(new SaleParametersDto { PageSize = 1, PageNumber = 2 });

                //Assert
                saleRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(1);

                saleRepo.Should().ContainEquivalentOf(fakeSaleTwo);

                context.Database.EnsureDeleted();
            }
        }
        
        
        [Fact]
        public void GetSales_FilterSaleIdListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SaleDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSaleOne = new FakeSale { }.Generate();
            fakeSaleOne.SaleId = 1;

            var fakeSaleTwo = new FakeSale { }.Generate();
            fakeSaleTwo.SaleId = 2;

            var fakeSaleThree = new FakeSale { }.Generate();
            fakeSaleThree.SaleId = 3;

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Sales.AddRange(fakeSaleOne, fakeSaleTwo, fakeSaleThree);
                context.SaveChanges();

                var service = new SaleRepository(context, new SieveProcessor(sieveOptions));

                var saleRepo = service.GetSales(new SaleParametersDto { Filters = $"SaleId == 2" });

                //Assert
                saleRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetSales_FilterProductIdListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SaleDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSaleOne = new FakeSale { }.Generate();
            fakeSaleOne.ProductId = 1;

            var fakeSaleTwo = new FakeSale { }.Generate();
            fakeSaleTwo.ProductId = 2;

            var fakeSaleThree = new FakeSale { }.Generate();
            fakeSaleThree.ProductId = 3;

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Sales.AddRange(fakeSaleOne, fakeSaleTwo, fakeSaleThree);
                context.SaveChanges();

                var service = new SaleRepository(context, new SieveProcessor(sieveOptions));

                var saleRepo = service.GetSales(new SaleParametersDto { Filters = $"ProductId == 2" });

                //Assert
                saleRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetSales_FilterSalespersonIdListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SaleDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSaleOne = new FakeSale { }.Generate();
            fakeSaleOne.SalespersonId = 1;

            var fakeSaleTwo = new FakeSale { }.Generate();
            fakeSaleTwo.SalespersonId = 2;

            var fakeSaleThree = new FakeSale { }.Generate();
            fakeSaleThree.SalespersonId = 3;

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Sales.AddRange(fakeSaleOne, fakeSaleTwo, fakeSaleThree);
                context.SaveChanges();

                var service = new SaleRepository(context, new SieveProcessor(sieveOptions));

                var saleRepo = service.GetSales(new SaleParametersDto { Filters = $"SalespersonId == 2" });

                //Assert
                saleRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetSales_FilterSaleDateListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SaleDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSaleOne = new FakeSale { }.Generate();
            fakeSaleOne.SaleDate = DateTime.Now.AddDays(1);

            var fakeSaleTwo = new FakeSale { }.Generate();
            fakeSaleTwo.SaleDate = DateTime.Parse(DateTime.Now.AddDays(2).ToString("MM/dd/yyyy"));

            var fakeSaleThree = new FakeSale { }.Generate();
            fakeSaleThree.SaleDate = DateTime.Now.AddDays(3);

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Sales.AddRange(fakeSaleOne, fakeSaleTwo, fakeSaleThree);
                context.SaveChanges();

                var service = new SaleRepository(context, new SieveProcessor(sieveOptions));

                var saleRepo = service.GetSales(new SaleParametersDto { Filters = $"SaleDate == {DateTime.Now.AddDays(2).ToString("MM/dd/yyyy")}" });

                //Assert
                saleRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

    } 
}