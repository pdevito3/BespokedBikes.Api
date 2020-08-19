
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

    [Collection("Sequential")]
    public class CreateSalespersonIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    { 
        private readonly CustomWebApplicationFactory _factory;

        public CreateSalespersonIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        
        [Fact]
        public async Task PostSalespersonReturnsSuccessCodeAndResourceWithAccurateFields()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var fakeSalesperson = new FakeSalespersonDto().Generate();

            // Act
            var httpResponse = await client.PostAsJsonAsync("api/Salespersons", fakeSalesperson)
                .ConfigureAwait(false);

            // Assert
            httpResponse.EnsureSuccessStatusCode();

            var resultDto = JsonConvert.DeserializeObject<SalespersonDto>(await httpResponse.Content.ReadAsStringAsync()
                .ConfigureAwait(false));

            httpResponse.StatusCode.Should().Be(201);
            resultDto.FirstName.Should().Be(fakeSalesperson.FirstName);
            resultDto.LastName.Should().Be(fakeSalesperson.LastName);
            resultDto.Address1.Should().Be(fakeSalesperson.Address1);
            resultDto.Address2.Should().Be(fakeSalesperson.Address2);
            resultDto.City.Should().Be(fakeSalesperson.City);
            resultDto.State.Should().Be(fakeSalesperson.State);
            resultDto.PostalCode.Should().Be(fakeSalesperson.PostalCode);
            resultDto.PhoneNumber.Should().Be(fakeSalesperson.PhoneNumber);
            resultDto.StartDate.Should().Be(fakeSalesperson.StartDate);
            resultDto.TerminationDate.Should().Be(fakeSalesperson.TerminationDate);
            resultDto.Manager.Should().Be(fakeSalesperson.Manager);
        }
    } 
}