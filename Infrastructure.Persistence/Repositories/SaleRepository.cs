namespace Infrastructure.Persistence.Repositories
{
    using Application.Dtos.Sale;
    using Application.Interfaces.Sale;
    using Application.Wrappers;
    using Domain.Entities;
    using Infrastructure.Persistence.Contexts;
    using Microsoft.EntityFrameworkCore;
    using Sieve.Models;
    using Sieve.Services;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class SaleRepository : ISaleRepository
    {
        private BespokedBikesDbContext _context;
        private readonly SieveProcessor _sieveProcessor;

        public SaleRepository(BespokedBikesDbContext context,
            SieveProcessor sieveProcessor)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
            _sieveProcessor = sieveProcessor ??
                throw new ArgumentNullException(nameof(sieveProcessor));
        }

        public PagedList<Sale> GetSales(SaleParametersDto saleParameters)
        {
            if (saleParameters == null)
            {
                throw new ArgumentNullException(nameof(saleParameters));
            }

            var collection = _context.Sales
                .Include(s => s.Product)
                .Include(s => s.Customer)
                .Include(s => s.Salesperson) as IQueryable<Sale>; // TODO: AsNoTracking() should increase performance, but will break the sort tests. need to investigate

            var sieveModel = new SieveModel
            {
                Sorts = saleParameters.SortOrder,
                Filters = saleParameters.Filters
            };

            collection = _sieveProcessor.Apply(sieveModel, collection);

            return PagedList<Sale>.Create(collection,
                saleParameters.PageNumber,
                saleParameters.PageSize);
        }

        public async Task<Sale> GetSaleAsync(int saleId)
        {
            return await _context.Sales.FirstOrDefaultAsync(s => s.SaleId == saleId);
        }

        public Sale GetSale(int saleId)
        {
            return _context.Sales.FirstOrDefault(s => s.SaleId == saleId);
        }

        public void AddSale(Sale sale)
        {
            if (sale == null)
            {
                throw new ArgumentNullException(nameof(Sale));
            }

            _context.Sales.Add(sale);
        }

        public void DeleteSale(Sale sale)
        {
            if (sale == null)
            {
                throw new ArgumentNullException(nameof(Sale));
            }

            _context.Sales.Remove(sale);
        }

        public void UpdateSale(Sale sale)
        {
            // no implementation for now
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}