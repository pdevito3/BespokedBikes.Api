
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
    using Microsoft.AspNetCore.JsonPatch;
    using System.Linq;
    using AutoMapper;
    using Bogus;
    using Application.Mappings;
    using System.Text;

    [Collection("Sequential")]
    public class UpdateDiscountIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    { 
        private readonly CustomWebApplicationFactory _factory;

        public UpdateDiscountIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        
        [Fact]
        public async Task PatchDiscount204AndFieldsWereSuccessfullyUpdated()
        {
            //Arrange
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DiscountProfile>();
            }).CreateMapper();

            var lookupVal = 999999; // don't know the id at this scope, so need to have another value to lookup
            var fakeDiscountOne = new FakeDiscount { }.Generate();
            
            var expectedFinalObject = mapper.Map<DiscountDto>(fakeDiscountOne);
            expectedFinalObject.ProductId = lookupVal;

            var appFactory = _factory;
            using (var scope = appFactory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BespokedBikesDbContext> ();
                context.Database.EnsureCreated();

                context.Discounts.RemoveRange(context.Discounts);
                context.Discounts.AddRange(fakeDiscountOne);
                context.SaveChanges();
            }

            var client = appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var patchDoc = new JsonPatchDocument<DiscountForUpdateDto>();
            patchDoc.Replace(d => d.ProductId, lookupVal);
            var serializedDiscountToUpdate = JsonConvert.SerializeObject(patchDoc);

            // Act
            // get the value i want to update. assumes I can use sieve for this field. if this is not an option, just use something else
            var getResult = await client.GetAsync($"api/Discounts/?filters=ProductId=={fakeDiscountOne.ProductId}")
                .ConfigureAwait(false);
            var getResponseContent = await getResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var getResponse = JsonConvert.DeserializeObject<IEnumerable<DiscountDto>>(getResponseContent);
            var id = getResponse.FirstOrDefault().DiscountId;

            // patch it
            var method = new HttpMethod("PATCH");
            var patchRequest = new HttpRequestMessage(method, $"api/Discounts/{id}")
            {
                Content = new StringContent(serializedDiscountToUpdate,
                    Encoding.Unicode, "application/json")
            };
            var patchResult = await client.SendAsync(patchRequest)
                .ConfigureAwait(false);

            // get it again to confirm updates
            var checkResult = await client.GetAsync($"api/Discounts/{id}")
                .ConfigureAwait(false);
            var checkResponseContent = await checkResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var checkResponse = JsonConvert.DeserializeObject<DiscountDto>(checkResponseContent);

            // Assert
            patchResult.StatusCode.Should().Be(204);
            checkResponse.Should().BeEquivalentTo(expectedFinalObject, options =>
                options.ExcludingMissingMembers());
        }
        
        [Fact]
        public async Task PutDiscountReturnsBodyAndFieldsWereSuccessfullyUpdated()
        {
            //Arrange
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DiscountProfile>();
            }).CreateMapper();

            var fakeDiscountOne = new FakeDiscount { }.Generate();
            var expectedFinalObject = mapper.Map<DiscountDto>(fakeDiscountOne);
            expectedFinalObject.ProductId = 999999;

            var appFactory = _factory;
            using (var scope = appFactory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BespokedBikesDbContext> ();
                context.Database.EnsureCreated();

                context.Discounts.RemoveRange(context.Discounts);
                context.Discounts.AddRange(fakeDiscountOne);
                context.SaveChanges();
            }

            var client = appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var serializedDiscountToUpdate = JsonConvert.SerializeObject(expectedFinalObject);

            // Act
            // get the value i want to update. assumes I can use sieve for this field. if this is not an option, just use something else
            var getResult = await client.GetAsync($"api/Discounts/?filters=ProductId=={fakeDiscountOne.ProductId}")
                .ConfigureAwait(false);
            var getResponseContent = await getResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var getResponse = JsonConvert.DeserializeObject<IEnumerable<DiscountDto>>(getResponseContent);
            var id = getResponse.FirstOrDefault().DiscountId;

            // put it
            var patchResult = await client.PutAsJsonAsync($"api/Discounts/{id}", expectedFinalObject)
                .ConfigureAwait(false);

            // get it again to confirm updates
            var checkResult = await client.GetAsync($"api/Discounts/{id}")
                .ConfigureAwait(false);
            var checkResponseContent = await checkResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var checkResponse = JsonConvert.DeserializeObject<DiscountDto>(checkResponseContent);

            // Assert
            patchResult.StatusCode.Should().Be(204);
            checkResponse.Should().BeEquivalentTo(expectedFinalObject, options =>
                options.ExcludingMissingMembers());
        }
    } 
}