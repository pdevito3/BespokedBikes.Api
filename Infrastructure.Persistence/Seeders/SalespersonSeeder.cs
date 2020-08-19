namespace Infrastructure.Persistence.Seeders
{

    using AutoBogus;
    using Domain.Entities;
    using Infrastructure.Persistence.Contexts;
    using System.Linq;

    public static class SalespersonSeeder
    {
        public static void SeedSampleSalespersonData(BespokedBikesDbContext context)
        {
            if (!context.Salespersons.Any())
            {
                context.Salespersons.Add(new AutoFaker<Salesperson>());
                context.Salespersons.Add(new AutoFaker<Salesperson>());
                context.Salespersons.Add(new AutoFaker<Salesperson>());

                context.SaveChanges();
            }
        }
    }
}