
namespace BespokedBikes.Api.Tests.IntegrationTests.Discount
{
    using Application.Dtos.Discount;
    using FluentAssertions;
    using BespokedBikes.Api.Tests.Fakes.Discount;
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
    public class GetDiscountIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    { 
        private readonly CustomWebApplicationFactory _factory;

        public GetDiscountIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        
        [Fact]
        public async Task GetDiscounts_ReturnsSuccessCodeAndResourceWithAccurateFields()
        {
            var fakeDiscountOne = new FakeDiscount { }.Generate();
            var fakeDiscountTwo = new FakeDiscount { }.Generate();

            var appFactory = _factory;
            using (var scope = appFactory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BespokedBikesDbContext>();
                context.Database.EnsureCreated();

                //context.Discounts.RemoveRange(context.Discounts);
                context.Discounts.AddRange(fakeDiscountOne, fakeDiscountTwo);
                context.SaveChanges();
            }

            var client = appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var result = await client.GetAsync("api/Discounts")
                .ConfigureAwait(false);
            var responseContent = await result.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<IEnumerable<DiscountDto>>(responseContent);

            // Assert
            result.StatusCode.Should().Be(200);
            response.Should().ContainEquivalentOf(fakeDiscountOne, options =>
                options.ExcludingMissingMembers());
            response.Should().ContainEquivalentOf(fakeDiscountTwo, options =>
                options.ExcludingMissingMembers());
        }
    } 
}