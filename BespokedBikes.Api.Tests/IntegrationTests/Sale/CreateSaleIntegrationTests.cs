
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

    [Collection("Sequential")]
    public class CreateSaleIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    { 
        private readonly CustomWebApplicationFactory _factory;

        public CreateSaleIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        
        [Fact]
        public async Task PostSaleReturnsSuccessCodeAndResourceWithAccurateFields()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var fakeSale = new FakeSaleDto().Generate();

            // Act
            var httpResponse = await client.PostAsJsonAsync("api/Sales", fakeSale)
                .ConfigureAwait(false);

            // Assert
            httpResponse.EnsureSuccessStatusCode();

            var resultDto = JsonConvert.DeserializeObject<SaleDto>(await httpResponse.Content.ReadAsStringAsync()
                .ConfigureAwait(false));

            httpResponse.StatusCode.Should().Be(201);
            resultDto.ProductId.Should().Be(fakeSale.ProductId);
            resultDto.SalespersonId.Should().Be(fakeSale.SalespersonId);
            resultDto.SaleDate.Should().Be(fakeSale.SaleDate);
        }
    } 
}