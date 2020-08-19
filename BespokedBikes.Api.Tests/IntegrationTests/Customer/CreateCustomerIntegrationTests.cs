
namespace BespokedBikes.Api.Tests.IntegrationTests.Customer
{
    using Application.Dtos.Customer;
    using FluentAssertions;
    using BespokedBikes.Api.Tests.Fakes.Customer;
    using Microsoft.AspNetCore.Mvc.Testing;
    using System.Threading.Tasks;
    using Xunit;
    using Newtonsoft.Json;
    using System.Net.Http;
    using WebApi;
    using System.Collections.Generic;

    [Collection("Sequential")]
    public class CreateCustomerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    { 
        private readonly CustomWebApplicationFactory _factory;

        public CreateCustomerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        
        [Fact]
        public async Task PostCustomerReturnsSuccessCodeAndResourceWithAccurateFields()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var fakeCustomer = new FakeCustomerDto().Generate();

            // Act
            var httpResponse = await client.PostAsJsonAsync("api/Customers", fakeCustomer)
                .ConfigureAwait(false);

            // Assert
            httpResponse.EnsureSuccessStatusCode();

            var resultDto = JsonConvert.DeserializeObject<CustomerDto>(await httpResponse.Content.ReadAsStringAsync()
                .ConfigureAwait(false));

            httpResponse.StatusCode.Should().Be(201);
            resultDto.FirstName.Should().Be(fakeCustomer.FirstName);
            resultDto.LastName.Should().Be(fakeCustomer.LastName);
            resultDto.Address1.Should().Be(fakeCustomer.Address1);
            resultDto.Address2.Should().Be(fakeCustomer.Address2);
            resultDto.City.Should().Be(fakeCustomer.City);
            resultDto.State.Should().Be(fakeCustomer.State);
            resultDto.PostalCode.Should().Be(fakeCustomer.PostalCode);
            resultDto.PhoneNumber.Should().Be(fakeCustomer.PhoneNumber);
            resultDto.StartDate.Should().Be(fakeCustomer.StartDate);
        }
    } 
}