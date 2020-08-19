
namespace BespokedBikes.Api.Tests.IntegrationTests.Sale
{
    using Application.Dtos.Sale;
    using FluentAssertions;
    using BespokedBikes.Api.Tests.Fakes.Sale;
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
    public class GetSaleIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    { 
        private readonly CustomWebApplicationFactory _factory;

        public GetSaleIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        
        [Fact]
        public async Task GetSales_ReturnsSuccessCodeAndResourceWithAccurateFields()
        {
            var fakeSaleOne = new FakeSale { }.Generate();
            var fakeSaleTwo = new FakeSale { }.Generate();

            var appFactory = _factory;
            using (var scope = appFactory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BespokedBikesDbContext>();
                context.Database.EnsureCreated();

                //context.Sales.RemoveRange(context.Sales);
                context.Sales.AddRange(fakeSaleOne, fakeSaleTwo);
                context.SaveChanges();
            }

            var client = appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var result = await client.GetAsync("api/Sales")
                .ConfigureAwait(false);
            var responseContent = await result.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<IEnumerable<SaleDto>>(responseContent);

            // Assert
            result.StatusCode.Should().Be(200);
            response.Should().ContainEquivalentOf(fakeSaleOne, options =>
                options.ExcludingMissingMembers());
            response.Should().ContainEquivalentOf(fakeSaleTwo, options =>
                options.ExcludingMissingMembers());
        }
    } 
}