namespace Infrastructure.Persistence.Seeders
{

    using AutoBogus;
    using Bogus;
    using Domain.Entities;
    using Infrastructure.Persistence.Contexts;
    using System;
    using System.Linq;

    public static class DiscountSeeder
    {
        public static void SeedSampleDiscountData(BespokedBikesDbContext context)
        {
            if (!context.Discounts.Any())
            {
                var products = context.Products.ToList();

                for (var eachCustomer = 1; eachCustomer <= 10; eachCustomer++)
                {
                    var productIndex = new Faker().Random.Number(1, products.Count());
                    var productKey = products[productIndex].ProductId;

                    context.Discounts.Add(new Faker<Discount>()
                        .RuleFor(fake => fake.ProductId, fake => productKey)
                        .RuleFor(fake => fake.BeginDate, fake => fake.Date.PastOffset())
                        .RuleFor(fake => fake.EndDate, fake => fake.Date.FutureOffset())
                        .RuleFor(fake => fake.DiscountPercentage, fake => fake.Random.Decimal((decimal)0, (decimal)0.3))
                        );
                }

                context.SaveChanges();
            }
        }
    }
}