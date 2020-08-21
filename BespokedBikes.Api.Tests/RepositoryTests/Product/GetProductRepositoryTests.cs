
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
    public class GetProductRepositoryTests
    { 
        
        [Fact]
        public void GetProduct_ParametersMatchExpectedValues()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"ProductDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeProduct = new FakeProduct { }.Generate();

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Products.AddRange(fakeProduct);
                context.SaveChanges();

                var service = new ProductRepository(context, new SieveProcessor(sieveOptions));

                //Assert
                var productById = service.GetProduct(fakeProduct.ProductId);
                                productById.ProductId.Should().Be(fakeProduct.ProductId);
                productById.Name.Should().Be(fakeProduct.Name);
                productById.Manufacturer.Should().Be(fakeProduct.Manufacturer);
                productById.Style.Should().Be(fakeProduct.Style);
                productById.PurchasePrice.Should().Be(fakeProduct.PurchasePrice);
                productById.SalePrice.Should().Be(fakeProduct.SalePrice);
                productById.QuantityOnHand.Should().Be(fakeProduct.QuantityOnHand);
                productById.CommissionPercentage.Should().Be(fakeProduct.CommissionPercentage);
            }
        }
        
        [Fact]
        public void GetProducts_CountMatchesAndContainsEquivalentObjects()
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
                context.SaveChanges();

                var service = new ProductRepository(context, new SieveProcessor(sieveOptions));

                var productRepo = service.GetProducts(new ProductParametersDto());

                //Assert
                productRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(3);

                productRepo.Should().ContainEquivalentOf(fakeProductOne);
                productRepo.Should().ContainEquivalentOf(fakeProductTwo);
                productRepo.Should().ContainEquivalentOf(fakeProductThree);

                context.Database.EnsureDeleted();
            }
        }
        
        [Fact]
        public void GetProducts_ReturnExpectedPageSize()
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
                context.SaveChanges();

                var service = new ProductRepository(context, new SieveProcessor(sieveOptions));

                var productRepo = service.GetProducts(new ProductParametersDto { PageSize = 2 });

                //Assert
                productRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(2);

                productRepo.Should().ContainEquivalentOf(fakeProductOne);
                productRepo.Should().ContainEquivalentOf(fakeProductTwo);

                context.Database.EnsureDeleted();
            }
        }
        
        [Fact]
        public void GetProducts_ReturnExpectedPageNumberAndSize()
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
                context.SaveChanges();

                var service = new ProductRepository(context, new SieveProcessor(sieveOptions));

                var productRepo = service.GetProducts(new ProductParametersDto { PageSize = 1, PageNumber = 2 });

                //Assert
                productRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(1);

                productRepo.Should().ContainEquivalentOf(fakeProductTwo);

                context.Database.EnsureDeleted();
            }
        }
        
        
        [Fact]
        public void GetProducts_FilterProductIdListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"ProductDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeProductOne = new FakeProduct { }.Generate();
            fakeProductOne.ProductId = 1;

            var fakeProductTwo = new FakeProduct { }.Generate();
            fakeProductTwo.ProductId = 2;

            var fakeProductThree = new FakeProduct { }.Generate();
            fakeProductThree.ProductId = 3;

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Products.AddRange(fakeProductOne, fakeProductTwo, fakeProductThree);
                context.SaveChanges();

                var service = new ProductRepository(context, new SieveProcessor(sieveOptions));

                var productRepo = service.GetProducts(new ProductParametersDto { Filters = $"ProductId == 2" });

                //Assert
                productRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetProducts_FilterNameListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"ProductDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeProductOne = new FakeProduct { }.Generate();
            fakeProductOne.Name = "alpha";

            var fakeProductTwo = new FakeProduct { }.Generate();
            fakeProductTwo.Name = "bravo";

            var fakeProductThree = new FakeProduct { }.Generate();
            fakeProductThree.Name = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Products.AddRange(fakeProductOne, fakeProductTwo, fakeProductThree);
                context.SaveChanges();

                var service = new ProductRepository(context, new SieveProcessor(sieveOptions));

                var productRepo = service.GetProducts(new ProductParametersDto { Filters = $"Name == bravo" });

                //Assert
                productRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetProducts_FilterManufacturerListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"ProductDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeProductOne = new FakeProduct { }.Generate();
            fakeProductOne.Manufacturer = "alpha";

            var fakeProductTwo = new FakeProduct { }.Generate();
            fakeProductTwo.Manufacturer = "bravo";

            var fakeProductThree = new FakeProduct { }.Generate();
            fakeProductThree.Manufacturer = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Products.AddRange(fakeProductOne, fakeProductTwo, fakeProductThree);
                context.SaveChanges();

                var service = new ProductRepository(context, new SieveProcessor(sieveOptions));

                var productRepo = service.GetProducts(new ProductParametersDto { Filters = $"Manufacturer == bravo" });

                //Assert
                productRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetProducts_FilterStyleListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"ProductDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeProductOne = new FakeProduct { }.Generate();
            fakeProductOne.Style = "alpha";

            var fakeProductTwo = new FakeProduct { }.Generate();
            fakeProductTwo.Style = "bravo";

            var fakeProductThree = new FakeProduct { }.Generate();
            fakeProductThree.Style = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Products.AddRange(fakeProductOne, fakeProductTwo, fakeProductThree);
                context.SaveChanges();

                var service = new ProductRepository(context, new SieveProcessor(sieveOptions));

                var productRepo = service.GetProducts(new ProductParametersDto { Filters = $"Style == bravo" });

                //Assert
                productRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetProducts_FilterPurchasePriceListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"ProductDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeProductOne = new FakeProduct { }.Generate();
            fakeProductOne.PurchasePrice = 1;

            var fakeProductTwo = new FakeProduct { }.Generate();
            fakeProductTwo.PurchasePrice = 2;

            var fakeProductThree = new FakeProduct { }.Generate();
            fakeProductThree.PurchasePrice = 3;

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Products.AddRange(fakeProductOne, fakeProductTwo, fakeProductThree);
                context.SaveChanges();

                var service = new ProductRepository(context, new SieveProcessor(sieveOptions));

                var productRepo = service.GetProducts(new ProductParametersDto { Filters = $"PurchasePrice == 2" });

                //Assert
                productRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetProducts_FilterSalePriceListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"ProductDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeProductOne = new FakeProduct { }.Generate();
            fakeProductOne.SalePrice = 1;

            var fakeProductTwo = new FakeProduct { }.Generate();
            fakeProductTwo.SalePrice = 2;

            var fakeProductThree = new FakeProduct { }.Generate();
            fakeProductThree.SalePrice = 3;

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Products.AddRange(fakeProductOne, fakeProductTwo, fakeProductThree);
                context.SaveChanges();

                var service = new ProductRepository(context, new SieveProcessor(sieveOptions));

                var productRepo = service.GetProducts(new ProductParametersDto { Filters = $"SalePrice == 2" });

                //Assert
                productRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetProducts_FilterQuantityOnHandListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"ProductDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeProductOne = new FakeProduct { }.Generate();
            fakeProductOne.QuantityOnHand = 1;

            var fakeProductTwo = new FakeProduct { }.Generate();
            fakeProductTwo.QuantityOnHand = 2;

            var fakeProductThree = new FakeProduct { }.Generate();
            fakeProductThree.QuantityOnHand = 3;

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Products.AddRange(fakeProductOne, fakeProductTwo, fakeProductThree);
                context.SaveChanges();

                var service = new ProductRepository(context, new SieveProcessor(sieveOptions));

                var productRepo = service.GetProducts(new ProductParametersDto { Filters = $"QuantityOnHand == 2" });

                //Assert
                productRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetProducts_FilterCommissionPercentageListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"ProductDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeProductOne = new FakeProduct { }.Generate();
            fakeProductOne.CommissionPercentage = 1;

            var fakeProductTwo = new FakeProduct { }.Generate();
            fakeProductTwo.CommissionPercentage = 2;

            var fakeProductThree = new FakeProduct { }.Generate();
            fakeProductThree.CommissionPercentage = 3;

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Products.AddRange(fakeProductOne, fakeProductTwo, fakeProductThree);
                context.SaveChanges();

                var service = new ProductRepository(context, new SieveProcessor(sieveOptions));

                var productRepo = service.GetProducts(new ProductParametersDto { Filters = $"CommissionPercentage == 2" });

                //Assert
                productRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

    } 
}