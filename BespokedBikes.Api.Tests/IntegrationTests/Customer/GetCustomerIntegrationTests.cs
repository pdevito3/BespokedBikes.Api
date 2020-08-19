
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
    using Infrastructure.Persistence.Contexts;
    using Microsoft.Extensions.DependencyInjection;

    [Collection("Sequential")]
    public class GetCustomerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    { 
        private readonly CustomWebApplicationFactory _factory;

        public GetCustomerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        
        [Fact]
        public async Task GetCustomers_ReturnsSuccessCodeAndResourceWithAccurateFields()
        {
            var fakeCustomerOne = new FakeCustomer { }.Generate();
            var fakeCustomerTwo = new FakeCustomer { }.Generate();

            var appFactory = _factory;
            using (var scope = appFactory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BespokedBikesDbContext>();
                context.Database.EnsureCreated();

                //context.Customers.RemoveRange(context.Customers);
                context.Customers.AddRange(fakeCustomerOne, fakeCustomerTwo);
                context.SaveChanges();
            }

            var client = appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var result = await client.GetAsync("api/Customers")
                .ConfigureAwait(false);
            var responseContent = await result.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<IEnumerable<CustomerDto>>(responseContent);

            // Assert
            result.StatusCode.Should().Be(200);
            response.Should().ContainEquivalentOf(fakeCustomerOne, options =>
                options.ExcludingMissingMembers());
            response.Should().ContainEquivalentOf(fakeCustomerTwo, options =>
                options.ExcludingMissingMembers());
        }
    } 
}