
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
    public class GetCustomerRepositoryTests
    { 
        
        [Fact]
        public void GetCustomer_ParametersMatchExpectedValues()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"CustomerDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeCustomer = new FakeCustomer { }.Generate();

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Customers.AddRange(fakeCustomer);
                context.SaveChanges();

                var service = new CustomerRepository(context, new SieveProcessor(sieveOptions));

                //Assert
                var customerById = service.GetCustomer(fakeCustomer.CustomerId);
                                customerById.CustomerId.Should().Be(fakeCustomer.CustomerId);
                customerById.FirstName.Should().Be(fakeCustomer.FirstName);
                customerById.LastName.Should().Be(fakeCustomer.LastName);
                customerById.Address1.Should().Be(fakeCustomer.Address1);
                customerById.Address2.Should().Be(fakeCustomer.Address2);
                customerById.City.Should().Be(fakeCustomer.City);
                customerById.State.Should().Be(fakeCustomer.State);
                customerById.PostalCode.Should().Be(fakeCustomer.PostalCode);
                customerById.PhoneNumber.Should().Be(fakeCustomer.PhoneNumber);
                customerById.StartDate.Should().Be(fakeCustomer.StartDate);
            }
        }
        
        [Fact]
        public void GetCustomers_CountMatchesAndContainsEquivalentObjects()
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
                context.SaveChanges();

                var service = new CustomerRepository(context, new SieveProcessor(sieveOptions));

                var customerRepo = service.GetCustomers(new CustomerParametersDto());

                //Assert
                customerRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(3);

                customerRepo.Should().ContainEquivalentOf(fakeCustomerOne);
                customerRepo.Should().ContainEquivalentOf(fakeCustomerTwo);
                customerRepo.Should().ContainEquivalentOf(fakeCustomerThree);

                context.Database.EnsureDeleted();
            }
        }
        
        [Fact]
        public void GetCustomers_ReturnExpectedPageSize()
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
                context.SaveChanges();

                var service = new CustomerRepository(context, new SieveProcessor(sieveOptions));

                var customerRepo = service.GetCustomers(new CustomerParametersDto { PageSize = 2 });

                //Assert
                customerRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(2);

                customerRepo.Should().ContainEquivalentOf(fakeCustomerOne);
                customerRepo.Should().ContainEquivalentOf(fakeCustomerTwo);

                context.Database.EnsureDeleted();
            }
        }
        
        [Fact]
        public void GetCustomers_ReturnExpectedPageNumberAndSize()
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
                context.SaveChanges();

                var service = new CustomerRepository(context, new SieveProcessor(sieveOptions));

                var customerRepo = service.GetCustomers(new CustomerParametersDto { PageSize = 1, PageNumber = 2 });

                //Assert
                customerRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(1);

                customerRepo.Should().ContainEquivalentOf(fakeCustomerTwo);

                context.Database.EnsureDeleted();
            }
        }
        
        
        [Fact]
        public void GetCustomers_FilterCustomerIdListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"CustomerDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeCustomerOne = new FakeCustomer { }.Generate();
            fakeCustomerOne.CustomerId = 1;

            var fakeCustomerTwo = new FakeCustomer { }.Generate();
            fakeCustomerTwo.CustomerId = 2;

            var fakeCustomerThree = new FakeCustomer { }.Generate();
            fakeCustomerThree.CustomerId = 3;

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Customers.AddRange(fakeCustomerOne, fakeCustomerTwo, fakeCustomerThree);
                context.SaveChanges();

                var service = new CustomerRepository(context, new SieveProcessor(sieveOptions));

                var customerRepo = service.GetCustomers(new CustomerParametersDto { Filters = $"CustomerId == 2" });

                //Assert
                customerRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetCustomers_FilterFirstNameListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"CustomerDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeCustomerOne = new FakeCustomer { }.Generate();
            fakeCustomerOne.FirstName = "alpha";

            var fakeCustomerTwo = new FakeCustomer { }.Generate();
            fakeCustomerTwo.FirstName = "bravo";

            var fakeCustomerThree = new FakeCustomer { }.Generate();
            fakeCustomerThree.FirstName = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Customers.AddRange(fakeCustomerOne, fakeCustomerTwo, fakeCustomerThree);
                context.SaveChanges();

                var service = new CustomerRepository(context, new SieveProcessor(sieveOptions));

                var customerRepo = service.GetCustomers(new CustomerParametersDto { Filters = $"FirstName == bravo" });

                //Assert
                customerRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetCustomers_FilterLastNameListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"CustomerDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeCustomerOne = new FakeCustomer { }.Generate();
            fakeCustomerOne.LastName = "alpha";

            var fakeCustomerTwo = new FakeCustomer { }.Generate();
            fakeCustomerTwo.LastName = "bravo";

            var fakeCustomerThree = new FakeCustomer { }.Generate();
            fakeCustomerThree.LastName = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Customers.AddRange(fakeCustomerOne, fakeCustomerTwo, fakeCustomerThree);
                context.SaveChanges();

                var service = new CustomerRepository(context, new SieveProcessor(sieveOptions));

                var customerRepo = service.GetCustomers(new CustomerParametersDto { Filters = $"LastName == bravo" });

                //Assert
                customerRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetCustomers_FilterAddress1ListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"CustomerDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeCustomerOne = new FakeCustomer { }.Generate();
            fakeCustomerOne.Address1 = "alpha";

            var fakeCustomerTwo = new FakeCustomer { }.Generate();
            fakeCustomerTwo.Address1 = "bravo";

            var fakeCustomerThree = new FakeCustomer { }.Generate();
            fakeCustomerThree.Address1 = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Customers.AddRange(fakeCustomerOne, fakeCustomerTwo, fakeCustomerThree);
                context.SaveChanges();

                var service = new CustomerRepository(context, new SieveProcessor(sieveOptions));

                var customerRepo = service.GetCustomers(new CustomerParametersDto { Filters = $"Address1 == bravo" });

                //Assert
                customerRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetCustomers_FilterAddress2ListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"CustomerDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeCustomerOne = new FakeCustomer { }.Generate();
            fakeCustomerOne.Address2 = "alpha";

            var fakeCustomerTwo = new FakeCustomer { }.Generate();
            fakeCustomerTwo.Address2 = "bravo";

            var fakeCustomerThree = new FakeCustomer { }.Generate();
            fakeCustomerThree.Address2 = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Customers.AddRange(fakeCustomerOne, fakeCustomerTwo, fakeCustomerThree);
                context.SaveChanges();

                var service = new CustomerRepository(context, new SieveProcessor(sieveOptions));

                var customerRepo = service.GetCustomers(new CustomerParametersDto { Filters = $"Address2 == bravo" });

                //Assert
                customerRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetCustomers_FilterCityListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"CustomerDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeCustomerOne = new FakeCustomer { }.Generate();
            fakeCustomerOne.City = "alpha";

            var fakeCustomerTwo = new FakeCustomer { }.Generate();
            fakeCustomerTwo.City = "bravo";

            var fakeCustomerThree = new FakeCustomer { }.Generate();
            fakeCustomerThree.City = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Customers.AddRange(fakeCustomerOne, fakeCustomerTwo, fakeCustomerThree);
                context.SaveChanges();

                var service = new CustomerRepository(context, new SieveProcessor(sieveOptions));

                var customerRepo = service.GetCustomers(new CustomerParametersDto { Filters = $"City == bravo" });

                //Assert
                customerRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetCustomers_FilterStateListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"CustomerDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeCustomerOne = new FakeCustomer { }.Generate();
            fakeCustomerOne.State = "alpha";

            var fakeCustomerTwo = new FakeCustomer { }.Generate();
            fakeCustomerTwo.State = "bravo";

            var fakeCustomerThree = new FakeCustomer { }.Generate();
            fakeCustomerThree.State = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Customers.AddRange(fakeCustomerOne, fakeCustomerTwo, fakeCustomerThree);
                context.SaveChanges();

                var service = new CustomerRepository(context, new SieveProcessor(sieveOptions));

                var customerRepo = service.GetCustomers(new CustomerParametersDto { Filters = $"State == bravo" });

                //Assert
                customerRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetCustomers_FilterPostalCodeListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"CustomerDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeCustomerOne = new FakeCustomer { }.Generate();
            fakeCustomerOne.PostalCode = "alpha";

            var fakeCustomerTwo = new FakeCustomer { }.Generate();
            fakeCustomerTwo.PostalCode = "bravo";

            var fakeCustomerThree = new FakeCustomer { }.Generate();
            fakeCustomerThree.PostalCode = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Customers.AddRange(fakeCustomerOne, fakeCustomerTwo, fakeCustomerThree);
                context.SaveChanges();

                var service = new CustomerRepository(context, new SieveProcessor(sieveOptions));

                var customerRepo = service.GetCustomers(new CustomerParametersDto { Filters = $"PostalCode == bravo" });

                //Assert
                customerRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetCustomers_FilterPhoneNumberListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"CustomerDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeCustomerOne = new FakeCustomer { }.Generate();
            fakeCustomerOne.PhoneNumber = "alpha";

            var fakeCustomerTwo = new FakeCustomer { }.Generate();
            fakeCustomerTwo.PhoneNumber = "bravo";

            var fakeCustomerThree = new FakeCustomer { }.Generate();
            fakeCustomerThree.PhoneNumber = "charlie";

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Customers.AddRange(fakeCustomerOne, fakeCustomerTwo, fakeCustomerThree);
                context.SaveChanges();

                var service = new CustomerRepository(context, new SieveProcessor(sieveOptions));

                var customerRepo = service.GetCustomers(new CustomerParametersDto { Filters = $"PhoneNumber == bravo" });

                //Assert
                customerRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetCustomers_FilterStartDateListWithExact()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<BespokedBikesDbContext>()
                .UseInMemoryDatabase(databaseName: $"CustomerDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeCustomerOne = new FakeCustomer { }.Generate();
            fakeCustomerOne.StartDate = DateTime.Now.AddDays(1);

            var fakeCustomerTwo = new FakeCustomer { }.Generate();
            fakeCustomerTwo.StartDate = DateTime.Parse(DateTime.Now.AddDays(2).ToString("MM/dd/yyyy"));

            var fakeCustomerThree = new FakeCustomer { }.Generate();
            fakeCustomerThree.StartDate = DateTime.Now.AddDays(3);

            //Act
            using (var context = new BespokedBikesDbContext(dbOptions))
            {
                context.Customers.AddRange(fakeCustomerOne, fakeCustomerTwo, fakeCustomerThree);
                context.SaveChanges();

                var service = new CustomerRepository(context, new SieveProcessor(sieveOptions));

                var customerRepo = service.GetCustomers(new CustomerParametersDto { Filters = $"StartDate == {DateTime.Now.AddDays(2).ToString("MM/dd/yyyy")}" });

                //Assert
                customerRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

    } 
}