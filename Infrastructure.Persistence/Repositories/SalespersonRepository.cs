namespace Infrastructure.Persistence.Repositories
{
    using Application.Dtos.Salesperson;
    using Application.Interfaces.Salesperson;
    using Application.Wrappers;
    using Domain.Entities;
    using Infrastructure.Persistence.Contexts;
    using Microsoft.EntityFrameworkCore;
    using Sieve.Models;
    using Sieve.Services;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class SalespersonRepository : ISalespersonRepository
    {
        private BespokedBikesDbContext _context;
        private readonly SieveProcessor _sieveProcessor;

        public SalespersonRepository(BespokedBikesDbContext context,
            SieveProcessor sieveProcessor)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
            _sieveProcessor = sieveProcessor ??
                throw new ArgumentNullException(nameof(sieveProcessor));
        }

        public PagedList<Salesperson> GetSalespersons(SalespersonParametersDto salespersonParameters)
        {
            if (salespersonParameters == null)
            {
                throw new ArgumentNullException(nameof(salespersonParameters));
            }

            var collection = _context.Salespersons as IQueryable<Salesperson>; // TODO: AsNoTracking() should increase performance, but will break the sort tests. need to investigate

            var sieveModel = new SieveModel
            {
                Sorts = salespersonParameters.SortOrder,
                Filters = salespersonParameters.Filters
            };

            collection = _sieveProcessor.Apply(sieveModel, collection);

            return PagedList<Salesperson>.Create(collection,
                salespersonParameters.PageNumber,
                salespersonParameters.PageSize);
        }

        public async Task<Salesperson> GetSalespersonAsync(int salespersonId)
        {
            return await _context.Salespersons.FirstOrDefaultAsync(s => s.SalespersonId == salespersonId);
        }

        public Salesperson GetSalesperson(int salespersonId)
        {
            return _context.Salespersons.FirstOrDefault(s => s.SalespersonId == salespersonId);
        }

        public void AddSalesperson(Salesperson salesperson)
        {
            if (salesperson == null)
            {
                throw new ArgumentNullException(nameof(Salesperson));
            }

            _context.Salespersons.Add(salesperson);
        }

        public void DeleteSalesperson(Salesperson salesperson)
        {
            if (salesperson == null)
            {
                throw new ArgumentNullException(nameof(Salesperson));
            }

            _context.Salespersons.Remove(salesperson);
        }

        public void UpdateSalesperson(Salesperson salesperson)
        {
            // no implementation for now
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}