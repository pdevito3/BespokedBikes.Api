
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

    [Collection("Sequential")]
    public class CreateDiscountIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    { 
        private readonly CustomWebApplicationFactory _factory;

        public CreateDiscountIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        
        [Fact]
        public async Task PostDiscountReturnsSuccessCodeAndResourceWithAccurateFields()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var fakeDiscount = new FakeDiscountDto().Generate();

            // Act
            var httpResponse = await client.PostAsJsonAsync("api/Discounts", fakeDiscount)
                .ConfigureAwait(false);

            // Assert
            httpResponse.EnsureSuccessStatusCode();

            var resultDto = JsonConvert.DeserializeObject<DiscountDto>(await httpResponse.Content.ReadAsStringAsync()
                .ConfigureAwait(false));

            httpResponse.StatusCode.Should().Be(201);
            resultDto.ProductId.Should().Be(fakeDiscount.ProductId);
            resultDto.BeginDate.Should().Be(fakeDiscount.BeginDate);
            resultDto.EndDate.Should().Be(fakeDiscount.EndDate);
            resultDto.DiscountPercentage.Should().Be(fakeDiscount.DiscountPercentage);
        }
    } 
}