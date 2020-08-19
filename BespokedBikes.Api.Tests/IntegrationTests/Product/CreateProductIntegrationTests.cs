
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

    [Collection("Sequential")]
    public class CreateProductIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    { 
        private readonly CustomWebApplicationFactory _factory;

        public CreateProductIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        
        [Fact]
        public async Task PostProductReturnsSuccessCodeAndResourceWithAccurateFields()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var fakeProduct = new FakeProductDto().Generate();

            // Act
            var httpResponse = await client.PostAsJsonAsync("api/Products", fakeProduct)
                .ConfigureAwait(false);

            // Assert
            httpResponse.EnsureSuccessStatusCode();

            var resultDto = JsonConvert.DeserializeObject<ProductDto>(await httpResponse.Content.ReadAsStringAsync()
                .ConfigureAwait(false));

            httpResponse.StatusCode.Should().Be(201);
            resultDto.Name.Should().Be(fakeProduct.Name);
            resultDto.Manufacturer.Should().Be(fakeProduct.Manufacturer);
            resultDto.Style.Should().Be(fakeProduct.Style);
            resultDto.PurchasePrice.Should().Be(fakeProduct.PurchasePrice);
            resultDto.SalePrice.Should().Be(fakeProduct.SalePrice);
            resultDto.QuantityOnHand.Should().Be(fakeProduct.QuantityOnHand);
            resultDto.ComissionPercentage.Should().Be(fakeProduct.ComissionPercentage);
        }
    } 
}