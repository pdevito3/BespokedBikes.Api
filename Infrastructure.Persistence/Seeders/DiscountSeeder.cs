namespace Infrastructure.Persistence.Seeders
{

    using AutoBogus;
    using Domain.Entities;
    using Infrastructure.Persistence.Contexts;
    using System.Linq;

    public static class DiscountSeeder
    {
        public static void SeedSampleDiscountData(BespokedBikesDbContext context)
        {
            if (!context.Discounts.Any())
            {
                context.Discounts.Add(new AutoFaker<Discount>());
                context.Discounts.Add(new AutoFaker<Discount>());
                context.Discounts.Add(new AutoFaker<Discount>());

                context.SaveChanges();
            }
        }
    }
}