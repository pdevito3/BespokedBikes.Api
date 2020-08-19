
namespace BespokedBikes.Api.Tests.IntegrationTests.Salesperson
{
    using Application.Dtos.Salesperson;
    using FluentAssertions;
    using BespokedBikes.Api.Tests.Fakes.Salesperson;
    using Microsoft.AspNetCore.Mvc.Testing;
    using System.Threading.Tasks;
    using Xunit;
    using Newtonsoft.Json;
    using System.Net.Http;
    using WebApi;
    using System.Collections.Generic;
    using Infrastructure.Persistence.Contexts;
    using Microsoft.Extensions.DependencyInjection;

    [Collection("Sequential")]
    public class GetSalespersonIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    { 
        private readonly CustomWebApplicationFactory _factory;

        public GetSalespersonIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        
        [Fact]
        public async Task GetSalespersons_ReturnsSuccessCodeAndResourceWithAccurateFields()
        {
            var fakeSalespersonOne = new FakeSalesperson { }.Generate();
            var fakeSalespersonTwo = new FakeSalesperson { }.Generate();

            var appFactory = _factory;
            using (var scope = appFactory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BespokedBikesDbContext>();
                context.Database.EnsureCreated();

                //context.Salespersons.RemoveRange(context.Salespersons);
                context.Salespersons.AddRange(fakeSalespersonOne, fakeSalespersonTwo);
                context.SaveChanges();
            }

            var client = appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var result = await client.GetAsync("api/Salespersons")
                .ConfigureAwait(false);
            var responseContent = await result.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<IEnumerable<SalespersonDto>>(responseContent);

            // Assert
            result.StatusCode.Should().Be(200);
            response.Should().ContainEquivalentOf(fakeSalespersonOne, options =>
                options.ExcludingMissingMembers());
            response.Should().ContainEquivalentOf(fakeSalespersonTwo, options =>
                options.ExcludingMissingMembers());
        }
    } 
}