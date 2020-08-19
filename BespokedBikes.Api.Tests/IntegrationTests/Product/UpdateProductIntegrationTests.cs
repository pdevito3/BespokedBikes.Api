
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
    using Microsoft.AspNetCore.JsonPatch;
    using System.Linq;
    using AutoMapper;
    using Bogus;
    using Application.Mappings;
    using System.Text;

    [Collection("Sequential")]
    public class UpdateProductIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    { 
        private readonly CustomWebApplicationFactory _factory;

        public UpdateProductIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        
        [Fact]
        public async Task PatchProduct204AndFieldsWereSuccessfullyUpdated()
        {
            //Arrange
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductProfile>();
            }).CreateMapper();

            var lookupVal = "Easily Identified Value For Test"; // don't know the id at this scope, so need to have another value to lookup
            var fakeProductOne = new FakeProduct { }.Generate();
            
            var expectedFinalObject = mapper.Map<ProductDto>(fakeProductOne);
            expectedFinalObject.Name = lookupVal;

            var appFactory = _factory;
            using (var scope = appFactory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BespokedBikesDbContext> ();
                context.Database.EnsureCreated();

                context.Products.RemoveRange(context.Products);
                context.Products.AddRange(fakeProductOne);
                context.SaveChanges();
            }

            var client = appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var patchDoc = new JsonPatchDocument<ProductForUpdateDto>();
            patchDoc.Replace(p => p.Name, lookupVal);
            var serializedProductToUpdate = JsonConvert.SerializeObject(patchDoc);

            // Act
            // get the value i want to update. assumes I can use sieve for this field. if this is not an option, just use something else
            var getResult = await client.GetAsync($"api/Products/?filters=Name=={fakeProductOne.Name}")
                .ConfigureAwait(false);
            var getResponseContent = await getResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var getResponse = JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(getResponseContent);
            var id = getResponse.FirstOrDefault().ProductId;

            // patch it
            var method = new HttpMethod("PATCH");
            var patchRequest = new HttpRequestMessage(method, $"api/Products/{id}")
            {
                Content = new StringContent(serializedProductToUpdate,
                    Encoding.Unicode, "application/json")
            };
            var patchResult = await client.SendAsync(patchRequest)
                .ConfigureAwait(false);

            // get it again to confirm updates
            var checkResult = await client.GetAsync($"api/Products/{id}")
                .ConfigureAwait(false);
            var checkResponseContent = await checkResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var checkResponse = JsonConvert.DeserializeObject<ProductDto>(checkResponseContent);

            // Assert
            patchResult.StatusCode.Should().Be(204);
            checkResponse.Should().BeEquivalentTo(expectedFinalObject, options =>
                options.ExcludingMissingMembers());
        }
        
        [Fact]
        public async Task PutProductReturnsBodyAndFieldsWereSuccessfullyUpdated()
        {
            //Arrange
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductProfile>();
            }).CreateMapper();

            var fakeProductOne = new FakeProduct { }.Generate();
            var expectedFinalObject = mapper.Map<ProductDto>(fakeProductOne);
            expectedFinalObject.Name = "Easily Identified Value For Test";

            var appFactory = _factory;
            using (var scope = appFactory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BespokedBikesDbContext> ();
                context.Database.EnsureCreated();

                context.Products.RemoveRange(context.Products);
                context.Products.AddRange(fakeProductOne);
                context.SaveChanges();
            }

            var client = appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var serializedProductToUpdate = JsonConvert.SerializeObject(expectedFinalObject);

            // Act
            // get the value i want to update. assumes I can use sieve for this field. if this is not an option, just use something else
            var getResult = await client.GetAsync($"api/Products/?filters=Name=={fakeProductOne.Name}")
                .ConfigureAwait(false);
            var getResponseContent = await getResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var getResponse = JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(getResponseContent);
            var id = getResponse.FirstOrDefault().ProductId;

            // put it
            var patchResult = await client.PutAsJsonAsync($"api/Products/{id}", expectedFinalObject)
                .ConfigureAwait(false);

            // get it again to confirm updates
            var checkResult = await client.GetAsync($"api/Products/{id}")
                .ConfigureAwait(false);
            var checkResponseContent = await checkResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var checkResponse = JsonConvert.DeserializeObject<ProductDto>(checkResponseContent);

            // Assert
            patchResult.StatusCode.Should().Be(204);
            checkResponse.Should().BeEquivalentTo(expectedFinalObject, options =>
                options.ExcludingMissingMembers());
        }
    } 
}