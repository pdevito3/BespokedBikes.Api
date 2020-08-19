
namespace BespokedBikes.Api.Tests.IntegrationTests.Product
{
    using Application.Dtos.Product;
    using FluentAssertions;
    using BespokedBikes.Api.Tests.Fakes.Product;
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
    public class GetProductIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    { 
        private readonly CustomWebApplicationFactory _factory;

        public GetProductIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        
        [Fact]
        public async Task GetProducts_ReturnsSuccessCodeAndResourceWithAccurateFields()
        {
            var fakeProductOne = new FakeProduct { }.Generate();
            var fakeProductTwo = new FakeProduct { }.Generate();

            var appFactory = _factory;
            using (var scope = appFactory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BespokedBikesDbContext>();
                context.Database.EnsureCreated();

                //context.Products.RemoveRange(context.Products);
                context.Products.AddRange(fakeProductOne, fakeProductTwo);
                context.SaveChanges();
            }

            var client = appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var result = await client.GetAsync("api/Products")
                .ConfigureAwait(false);
            var responseContent = await result.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(responseContent);

            // Assert
            result.StatusCode.Should().Be(200);
            response.Should().ContainEquivalentOf(fakeProductOne, options =>
                options.ExcludingMissingMembers());
            response.Should().ContainEquivalentOf(fakeProductTwo, options =>
                options.ExcludingMissingMembers());
        }
    } 
}