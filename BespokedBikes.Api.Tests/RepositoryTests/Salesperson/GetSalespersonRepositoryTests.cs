
namespace BespokedBikes.Api.Tests.RepositoryTests.Salesperson
{
    using Application.Dtos.Salesperson;
    using FluentAssertions;
    using BespokedBikes.Api.Tests.Fakes.Salesperson;
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
    public class GetSalespersonRepositoryTests
    { 
        
        [Fact]
        public void GetSalesperson_ParametersMatchExpectedValues()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SalespersonDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSalesperson = new FakeSalesperson { }.Generate();

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Salespersons.AddRange(fakeSalesperson);
                context.SaveChanges();

                var service = new SalespersonRepository(context, new SieveProcessor(sieveOptions));

                //Assert
                var salespersonById = service.GetSalesperson(fakeSalesperson.SalespersonId);
                                salespersonById.SalespersonId.Should().Be(fakeSalesperson.SalespersonId);
                salespersonById.FirstName.Should().Be(fakeSalesperson.FirstName);
                salespersonById.LastName.Should().Be(fakeSalesperson.LastName);
                salespersonById.Address1.Should().Be(fakeSalesperson.Address1);
                salespersonById.Address2.Should().Be(fakeSalesperson.Address2);
                salespersonById.City.Should().Be(fakeSalesperson.City);
                salespersonById.State.Should().Be(fakeSalesperson.State);
                salespersonById.PostalCode.Should().Be(fakeSalesperson.PostalCode);
                salespersonById.PhoneNumber.Should().Be(fakeSalesperson.PhoneNumber);
                salespersonById.StartDate.Should().Be(fakeSalesperson.StartDate);
                salespersonById.TerminationDate.Should().Be(fakeSalesperson.TerminationDate);
                salespersonById.Manager.Should().Be(fakeSalesperson.Manager);
            }
        }
        
        [Fact]
        public void GetSalespersons_CountMatchesAndContainsEquivalentObjects()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SalespersonDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSalespersonOne = new FakeSalesperson { }.Generate();
            var fakeSalespersonTwo = new FakeSalesperson { }.Generate();
            var fakeSalespersonThree = new FakeSalesperson { }.Generate();

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Salespersons.AddRange(fakeSalespersonOne, fakeSalespersonTwo, fakeSalespersonThree);
                context.SaveChanges();

                var service = new SalespersonRepository(context, new SieveProcessor(sieveOptions));

                var salespersonRepo = service.GetSalespersons(new SalespersonParametersDto());

                //Assert
                salespersonRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(3);

                salespersonRepo.Should().ContainEquivalentOf(fakeSalespersonOne);
                salespersonRepo.Should().ContainEquivalentOf(fakeSalespersonTwo);
                salespersonRepo.Should().ContainEquivalentOf(fakeSalespersonThree);

                context.Database.EnsureDeleted();
            }
        }
        
        [Fact]
        public void GetSalespersons_ReturnExpectedPageSize()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SalespersonDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSalespersonOne = new FakeSalesperson { }.Generate();
            var fakeSalespersonTwo = new FakeSalesperson { }.Generate();
            var fakeSalespersonThree = new FakeSalesperson { }.Generate();

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Salespersons.AddRange(fakeSalespersonOne, fakeSalespersonTwo, fakeSalespersonThree);
                context.SaveChanges();

                var service = new SalespersonRepository(context, new SieveProcessor(sieveOptions));

                var salespersonRepo = service.GetSalespersons(new SalespersonParametersDto { PageSize = 2 });

                //Assert
                salespersonRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(2);

                salespersonRepo.Should().ContainEquivalentOf(fakeSalespersonOne);
                salespersonRepo.Should().ContainEquivalentOf(fakeSalespersonTwo);

                context.Database.EnsureDeleted();
            }
        }
        
        [Fact]
        public void GetSalespersons_ReturnExpectedPageNumberAndSize()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SalespersonDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSalespersonOne = new FakeSalesperson { }.Generate();
            var fakeSalespersonTwo = new FakeSalesperson { }.Generate();
            var fakeSalespersonThree = new FakeSalesperson { }.Generate();

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Salespersons.AddRange(fakeSalespersonOne, fakeSalespersonTwo, fakeSalespersonThree);
                context.SaveChanges();

                var service = new SalespersonRepository(context, new SieveProcessor(sieveOptions));

                var salespersonRepo = service.GetSalespersons(new SalespersonParametersDto { PageSize = 1, PageNumber = 2 });

                //Assert
                salespersonRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(1);

                salespersonRepo.Should().ContainEquivalentOf(fakeSalespersonTwo);

                context.Database.EnsureDeleted();
            }
        }
        
        
        [Fact]
        public void GetSalespersons_FilterSalespersonIdListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SalespersonDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSalespersonOne = new FakeSalesperson { }.Generate();
            fakeSalespersonOne.SalespersonId = 1;

            var fakeSalespersonTwo = new FakeSalesperson { }.Generate();
            fakeSalespersonTwo.SalespersonId = 2;

            var fakeSalespersonThree = new FakeSalesperson { }.Generate();
            fakeSalespersonThree.SalespersonId = 3;

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Salespersons.AddRange(fakeSalespersonOne, fakeSalespersonTwo, fakeSalespersonThree);
                context.SaveChanges();

                var service = new SalespersonRepository(context, new SieveProcessor(sieveOptions));

                var salespersonRepo = service.GetSalespersons(new SalespersonParametersDto { Filters = $"SalespersonId == 2" });

                //Assert
                salespersonRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetSalespersons_FilterFirstNameListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SalespersonDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSalespersonOne = new FakeSalesperson { }.Generate();
            fakeSalespersonOne.FirstName = "alpha";

            var fakeSalespersonTwo = new FakeSalesperson { }.Generate();
            fakeSalespersonTwo.FirstName = "bravo";

            var fakeSalespersonThree = new FakeSalesperson { }.Generate();
            fakeSalespersonThree.FirstName = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Salespersons.AddRange(fakeSalespersonOne, fakeSalespersonTwo, fakeSalespersonThree);
                context.SaveChanges();

                var service = new SalespersonRepository(context, new SieveProcessor(sieveOptions));

                var salespersonRepo = service.GetSalespersons(new SalespersonParametersDto { Filters = $"FirstName == bravo" });

                //Assert
                salespersonRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetSalespersons_FilterLastNameListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SalespersonDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSalespersonOne = new FakeSalesperson { }.Generate();
            fakeSalespersonOne.LastName = "alpha";

            var fakeSalespersonTwo = new FakeSalesperson { }.Generate();
            fakeSalespersonTwo.LastName = "bravo";

            var fakeSalespersonThree = new FakeSalesperson { }.Generate();
            fakeSalespersonThree.LastName = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Salespersons.AddRange(fakeSalespersonOne, fakeSalespersonTwo, fakeSalespersonThree);
                context.SaveChanges();

                var service = new SalespersonRepository(context, new SieveProcessor(sieveOptions));

                var salespersonRepo = service.GetSalespersons(new SalespersonParametersDto { Filters = $"LastName == bravo" });

                //Assert
                salespersonRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetSalespersons_FilterAddress1ListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SalespersonDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSalespersonOne = new FakeSalesperson { }.Generate();
            fakeSalespersonOne.Address1 = "alpha";

            var fakeSalespersonTwo = new FakeSalesperson { }.Generate();
            fakeSalespersonTwo.Address1 = "bravo";

            var fakeSalespersonThree = new FakeSalesperson { }.Generate();
            fakeSalespersonThree.Address1 = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Salespersons.AddRange(fakeSalespersonOne, fakeSalespersonTwo, fakeSalespersonThree);
                context.SaveChanges();

                var service = new SalespersonRepository(context, new SieveProcessor(sieveOptions));

                var salespersonRepo = service.GetSalespersons(new SalespersonParametersDto { Filters = $"Address1 == bravo" });

                //Assert
                salespersonRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetSalespersons_FilterAddress2ListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SalespersonDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSalespersonOne = new FakeSalesperson { }.Generate();
            fakeSalespersonOne.Address2 = "alpha";

            var fakeSalespersonTwo = new FakeSalesperson { }.Generate();
            fakeSalespersonTwo.Address2 = "bravo";

            var fakeSalespersonThree = new FakeSalesperson { }.Generate();
            fakeSalespersonThree.Address2 = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Salespersons.AddRange(fakeSalespersonOne, fakeSalespersonTwo, fakeSalespersonThree);
                context.SaveChanges();

                var service = new SalespersonRepository(context, new SieveProcessor(sieveOptions));

                var salespersonRepo = service.GetSalespersons(new SalespersonParametersDto { Filters = $"Address2 == bravo" });

                //Assert
                salespersonRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetSalespersons_FilterCityListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SalespersonDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSalespersonOne = new FakeSalesperson { }.Generate();
            fakeSalespersonOne.City = "alpha";

            var fakeSalespersonTwo = new FakeSalesperson { }.Generate();
            fakeSalespersonTwo.City = "bravo";

            var fakeSalespersonThree = new FakeSalesperson { }.Generate();
            fakeSalespersonThree.City = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Salespersons.AddRange(fakeSalespersonOne, fakeSalespersonTwo, fakeSalespersonThree);
                context.SaveChanges();

                var service = new SalespersonRepository(context, new SieveProcessor(sieveOptions));

                var salespersonRepo = service.GetSalespersons(new SalespersonParametersDto { Filters = $"City == bravo" });

                //Assert
                salespersonRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetSalespersons_FilterStateListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SalespersonDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSalespersonOne = new FakeSalesperson { }.Generate();
            fakeSalespersonOne.State = "alpha";

            var fakeSalespersonTwo = new FakeSalesperson { }.Generate();
            fakeSalespersonTwo.State = "bravo";

            var fakeSalespersonThree = new FakeSalesperson { }.Generate();
            fakeSalespersonThree.State = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Salespersons.AddRange(fakeSalespersonOne, fakeSalespersonTwo, fakeSalespersonThree);
                context.SaveChanges();

                var service = new SalespersonRepository(context, new SieveProcessor(sieveOptions));

                var salespersonRepo = service.GetSalespersons(new SalespersonParametersDto { Filters = $"State == bravo" });

                //Assert
                salespersonRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetSalespersons_FilterPostalCodeListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SalespersonDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSalespersonOne = new FakeSalesperson { }.Generate();
            fakeSalespersonOne.PostalCode = "alpha";

            var fakeSalespersonTwo = new FakeSalesperson { }.Generate();
            fakeSalespersonTwo.PostalCode = "bravo";

            var fakeSalespersonThree = new FakeSalesperson { }.Generate();
            fakeSalespersonThree.PostalCode = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Salespersons.AddRange(fakeSalespersonOne, fakeSalespersonTwo, fakeSalespersonThree);
                context.SaveChanges();

                var service = new SalespersonRepository(context, new SieveProcessor(sieveOptions));

                var salespersonRepo = service.GetSalespersons(new SalespersonParametersDto { Filters = $"PostalCode == bravo" });

                //Assert
                salespersonRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetSalespersons_FilterPhoneNumberListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SalespersonDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSalespersonOne = new FakeSalesperson { }.Generate();
            fakeSalespersonOne.PhoneNumber = "alpha";

            var fakeSalespersonTwo = new FakeSalesperson { }.Generate();
            fakeSalespersonTwo.PhoneNumber = "bravo";

            var fakeSalespersonThree = new FakeSalesperson { }.Generate();
            fakeSalespersonThree.PhoneNumber = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Salespersons.AddRange(fakeSalespersonOne, fakeSalespersonTwo, fakeSalespersonThree);
                context.SaveChanges();

                var service = new SalespersonRepository(context, new SieveProcessor(sieveOptions));

                var salespersonRepo = service.GetSalespersons(new SalespersonParametersDto { Filters = $"PhoneNumber == bravo" });

                //Assert
                salespersonRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetSalespersons_FilterStartDateListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SalespersonDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSalespersonOne = new FakeSalesperson { }.Generate();
            fakeSalespersonOne.StartDate = DateTime.Now.AddDays(1);

            var fakeSalespersonTwo = new FakeSalesperson { }.Generate();
            fakeSalespersonTwo.StartDate = DateTime.Parse(DateTime.Now.AddDays(2).ToString("MM/dd/yyyy"));

            var fakeSalespersonThree = new FakeSalesperson { }.Generate();
            fakeSalespersonThree.StartDate = DateTime.Now.AddDays(3);

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Salespersons.AddRange(fakeSalespersonOne, fakeSalespersonTwo, fakeSalespersonThree);
                context.SaveChanges();

                var service = new SalespersonRepository(context, new SieveProcessor(sieveOptions));

                var salespersonRepo = service.GetSalespersons(new SalespersonParametersDto { Filters = $"StartDate == {DateTime.Now.AddDays(2).ToString("MM/dd/yyyy")}" });

                //Assert
                salespersonRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetSalespersons_FilterTerminationDateListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SalespersonDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSalespersonOne = new FakeSalesperson { }.Generate();
            fakeSalespersonOne.TerminationDate = DateTime.Now.AddDays(1);

            var fakeSalespersonTwo = new FakeSalesperson { }.Generate();
            fakeSalespersonTwo.TerminationDate = DateTime.Parse(DateTime.Now.AddDays(2).ToString("MM/dd/yyyy"));

            var fakeSalespersonThree = new FakeSalesperson { }.Generate();
            fakeSalespersonThree.TerminationDate = DateTime.Now.AddDays(3);

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Salespersons.AddRange(fakeSalespersonOne, fakeSalespersonTwo, fakeSalespersonThree);
                context.SaveChanges();

                var service = new SalespersonRepository(context, new SieveProcessor(sieveOptions));

                var salespersonRepo = service.GetSalespersons(new SalespersonParametersDto { Filters = $"TerminationDate == {DateTime.Now.AddDays(2).ToString("MM/dd/yyyy")}" });

                //Assert
                salespersonRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetSalespersons_FilterManagerListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"SalespersonDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeSalespersonOne = new FakeSalesperson { }.Generate();
            fakeSalespersonOne.Manager = "alpha";

            var fakeSalespersonTwo = new FakeSalesperson { }.Generate();
            fakeSalespersonTwo.Manager = "bravo";

            var fakeSalespersonThree = new FakeSalesperson { }.Generate();
            fakeSalespersonThree.Manager = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Salespersons.AddRange(fakeSalespersonOne, fakeSalespersonTwo, fakeSalespersonThree);
                context.SaveChanges();

                var service = new SalespersonRepository(context, new SieveProcessor(sieveOptions));

                var salespersonRepo = service.GetSalespersons(new SalespersonParametersDto { Filters = $"Manager == bravo" });

                //Assert
                salespersonRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

    } 
}