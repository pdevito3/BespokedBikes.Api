
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
    using Infrastructure.Persistence.Contexts;
    using Microsoft.Extensions.DependencyInjection;    
    using Microsoft.AspNetCore.JsonPatch;
    using System.Linq;
    using AutoMapper;
    using Bogus;
    using Application.Mappings;
    using System.Text;

    [Collection("Sequential")]
    public class UpdateSalespersonIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    { 
        private readonly CustomWebApplicationFactory _factory;

        public UpdateSalespersonIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        
        [Fact]
        public async Task PatchSalesperson204AndFieldsWereSuccessfullyUpdated()
        {
            //Arrange
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SalespersonProfile>();
            }).CreateMapper();

            var lookupVal = "Easily Identified Value For Test"; // don't know the id at this scope, so need to have another value to lookup
            var fakeSalespersonOne = new FakeSalesperson { }.Generate();
            
            var expectedFinalObject = mapper.Map<SalespersonDto>(fakeSalespersonOne);
            expectedFinalObject.FirstName = lookupVal;

            var appFactory = _factory;
            using (var scope = appFactory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BespokedBikesDbContext> ();
                context.Database.EnsureCreated();

                context.Salespersons.RemoveRange(context.Salespersons);
                context.Salespersons.AddRange(fakeSalespersonOne);
                context.SaveChanges();
            }

            var client = appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var patchDoc = new JsonPatchDocument<SalespersonForUpdateDto>();
            patchDoc.Replace(s => s.FirstName, lookupVal);
            var serializedSalespersonToUpdate = JsonConvert.SerializeObject(patchDoc);

            // Act
            // get the value i want to update. assumes I can use sieve for this field. if this is not an option, just use something else
            var getResult = await client.GetAsync($"api/Salespersons/?filters=FirstName=={fakeSalespersonOne.FirstName}")
                .ConfigureAwait(false);
            var getResponseContent = await getResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var getResponse = JsonConvert.DeserializeObject<IEnumerable<SalespersonDto>>(getResponseContent);
            var id = getResponse.FirstOrDefault().SalespersonId;

            // patch it
            var method = new HttpMethod("PATCH");
            var patchRequest = new HttpRequestMessage(method, $"api/Salespersons/{id}")
            {
                Content = new StringContent(serializedSalespersonToUpdate,
                    Encoding.Unicode, "application/json")
            };
            var patchResult = await client.SendAsync(patchRequest)
                .ConfigureAwait(false);

            // get it again to confirm updates
            var checkResult = await client.GetAsync($"api/Salespersons/{id}")
                .ConfigureAwait(false);
            var checkResponseContent = await checkResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var checkResponse = JsonConvert.DeserializeObject<SalespersonDto>(checkResponseContent);

            // Assert
            patchResult.StatusCode.Should().Be(204);
            checkResponse.Should().BeEquivalentTo(expectedFinalObject, options =>
                options.ExcludingMissingMembers());
        }
        
        [Fact]
        public async Task PutSalespersonReturnsBodyAndFieldsWereSuccessfullyUpdated()
        {
            //Arrange
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SalespersonProfile>();
            }).CreateMapper();

            var fakeSalespersonOne = new FakeSalesperson { }.Generate();
            var expectedFinalObject = mapper.Map<SalespersonDto>(fakeSalespersonOne);
            expectedFinalObject.FirstName = "Easily Identified Value For Test";

            var appFactory = _factory;
            using (var scope = appFactory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BespokedBikesDbContext> ();
                context.Database.EnsureCreated();

                context.Salespersons.RemoveRange(context.Salespersons);
                context.Salespersons.AddRange(fakeSalespersonOne);
                context.SaveChanges();
            }

            var client = appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var serializedSalespersonToUpdate = JsonConvert.SerializeObject(expectedFinalObject);

            // Act
            // get the value i want to update. assumes I can use sieve for this field. if this is not an option, just use something else
            var getResult = await client.GetAsync($"api/Salespersons/?filters=FirstName=={fakeSalespersonOne.FirstName}")
                .ConfigureAwait(false);
            var getResponseContent = await getResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var getResponse = JsonConvert.DeserializeObject<IEnumerable<SalespersonDto>>(getResponseContent);
            var id = getResponse.FirstOrDefault().SalespersonId;

            // put it
            var patchResult = await client.PutAsJsonAsync($"api/Salespersons/{id}", expectedFinalObject)
                .ConfigureAwait(false);

            // get it again to confirm updates
            var checkResult = await client.GetAsync($"api/Salespersons/{id}")
                .ConfigureAwait(false);
            var checkResponseContent = await checkResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var checkResponse = JsonConvert.DeserializeObject<SalespersonDto>(checkResponseContent);

            // Assert
            patchResult.StatusCode.Should().Be(204);
            checkResponse.Should().BeEquivalentTo(expectedFinalObject, options =>
                options.ExcludingMissingMembers());
        }
    } 
}