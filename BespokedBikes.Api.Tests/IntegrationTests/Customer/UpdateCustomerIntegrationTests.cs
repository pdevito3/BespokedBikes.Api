
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
    using Microsoft.AspNetCore.JsonPatch;
    using System.Linq;
    using AutoMapper;
    using Bogus;
    using Application.Mappings;
    using System.Text;

    [Collection("Sequential")]
    public class UpdateCustomerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    { 
        private readonly CustomWebApplicationFactory _factory;

        public UpdateCustomerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        
        [Fact]
        public async Task PatchCustomer204AndFieldsWereSuccessfullyUpdated()
        {
            //Arrange
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CustomerProfile>();
            }).CreateMapper();

            var lookupVal = "Easily Identified Value For Test"; // don't know the id at this scope, so need to have another value to lookup
            var fakeCustomerOne = new FakeCustomer { }.Generate();
            
            var expectedFinalObject = mapper.Map<CustomerDto>(fakeCustomerOne);
            expectedFinalObject.FirstName = lookupVal;

            var appFactory = _factory;
            using (var scope = appFactory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BespokedBikesDbContext> ();
                context.Database.EnsureCreated();

                context.Customers.RemoveRange(context.Customers);
                context.Customers.AddRange(fakeCustomerOne);
                context.SaveChanges();
            }

            var client = appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var patchDoc = new JsonPatchDocument<CustomerForUpdateDto>();
            patchDoc.Replace(c => c.FirstName, lookupVal);
            var serializedCustomerToUpdate = JsonConvert.SerializeObject(patchDoc);

            // Act
            // get the value i want to update. assumes I can use sieve for this field. if this is not an option, just use something else
            var getResult = await client.GetAsync($"api/Customers/?filters=FirstName=={fakeCustomerOne.FirstName}")
                .ConfigureAwait(false);
            var getResponseContent = await getResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var getResponse = JsonConvert.DeserializeObject<IEnumerable<CustomerDto>>(getResponseContent);
            var id = getResponse.FirstOrDefault().CustomerId;

            // patch it
            var method = new HttpMethod("PATCH");
            var patchRequest = new HttpRequestMessage(method, $"api/Customers/{id}")
            {
                Content = new StringContent(serializedCustomerToUpdate,
                    Encoding.Unicode, "application/json")
            };
            var patchResult = await client.SendAsync(patchRequest)
                .ConfigureAwait(false);

            // get it again to confirm updates
            var checkResult = await client.GetAsync($"api/Customers/{id}")
                .ConfigureAwait(false);
            var checkResponseContent = await checkResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var checkResponse = JsonConvert.DeserializeObject<CustomerDto>(checkResponseContent);

            // Assert
            patchResult.StatusCode.Should().Be(204);
            checkResponse.Should().BeEquivalentTo(expectedFinalObject, options =>
                options.ExcludingMissingMembers());
        }
        
        [Fact]
        public async Task PutCustomerReturnsBodyAndFieldsWereSuccessfullyUpdated()
        {
            //Arrange
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CustomerProfile>();
            }).CreateMapper();

            var fakeCustomerOne = new FakeCustomer { }.Generate();
            var expectedFinalObject = mapper.Map<CustomerDto>(fakeCustomerOne);
            expectedFinalObject.FirstName = "Easily Identified Value For Test";

            var appFactory = _factory;
            using (var scope = appFactory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BespokedBikesDbContext> ();
                context.Database.EnsureCreated();

                context.Customers.RemoveRange(context.Customers);
                context.Customers.AddRange(fakeCustomerOne);
                context.SaveChanges();
            }

            var client = appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var serializedCustomerToUpdate = JsonConvert.SerializeObject(expectedFinalObject);

            // Act
            // get the value i want to update. assumes I can use sieve for this field. if this is not an option, just use something else
            var getResult = await client.GetAsync($"api/Customers/?filters=FirstName=={fakeCustomerOne.FirstName}")
                .ConfigureAwait(false);
            var getResponseContent = await getResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var getResponse = JsonConvert.DeserializeObject<IEnumerable<CustomerDto>>(getResponseContent);
            var id = getResponse.FirstOrDefault().CustomerId;

            // put it
            var patchResult = await client.PutAsJsonAsync($"api/Customers/{id}", expectedFinalObject)
                .ConfigureAwait(false);

            // get it again to confirm updates
            var checkResult = await client.GetAsync($"api/Customers/{id}")
                .ConfigureAwait(false);
            var checkResponseContent = await checkResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var checkResponse = JsonConvert.DeserializeObject<CustomerDto>(checkResponseContent);

            // Assert
            patchResult.StatusCode.Should().Be(204);
            checkResponse.Should().BeEquivalentTo(expectedFinalObject, options =>
                options.ExcludingMissingMembers());
        }
    } 
}