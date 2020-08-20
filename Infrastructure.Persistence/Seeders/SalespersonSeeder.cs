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
                for (var eachCustomer = 1; eachCustomer <= 50; eachCustomer++)
                {
                    context.Salespersons.Add(new AutoFaker<Salesperson>()
                        .RuleFor(fake => fake.FirstName, fake => fake.Name.FirstName())
                        .RuleFor(fake => fake.LastName, fake => fake.Name.LastName())
                        .RuleFor(fake => fake.PhoneNumber, fake => fake.Phone.PhoneNumber("###-###-####"))
                        .RuleFor(fake => fake.StartDate, fake => fake.Date.PastOffset())
                        .RuleFor(fake => fake.Address1, fake => fake.Address.StreetAddress())
                        .RuleFor(fake => fake.Address2, fake => fake.Address.SecondaryAddress())
                        .RuleFor(fake => fake.City, fake => fake.Address.City())
                        .RuleFor(fake => fake.PostalCode, fake => fake.Address.ZipCode())
                        .RuleFor(fake => fake.TerminationDate, fake => fake.Date.PastOffset())
                        .RuleFor(fake => fake.StartDate, fake => fake.Date.PastOffset())
                        .RuleFor(fake => fake.Manager, fake => fake.Name.FullName())
                        );
                }

                context.SaveChanges();
            }
        }
    }
}