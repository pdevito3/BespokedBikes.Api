
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
    public class DeleteSalespersonRepositoryTests
    { 
        
        [Fact]
        public void DeleteSalesperson_ReturnsProperCount()
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

                var service = new SalespersonRepository(context, new SieveProcessor(sieveOptions));
                service.DeleteSalesperson(fakeSalespersonTwo);

                context.SaveChanges();

                //Assert
                var salespersonList = context.Salespersons.ToList();

                salespersonList.Should()
                    .NotBeEmpty()
                    .And.HaveCount(2);

                salespersonList.Should().ContainEquivalentOf(fakeSalespersonOne);
                salespersonList.Should().ContainEquivalentOf(fakeSalespersonThree);
                Assert.DoesNotContain(salespersonList, s => s == fakeSalespersonTwo);

                context.Database.EnsureDeleted();
            }
        }
    } 
}