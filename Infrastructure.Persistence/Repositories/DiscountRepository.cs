namespace Infrastructure.Persistence.Repositories
{
    using Application.Dtos.Discount;
    using Application.Interfaces.Discount;
    using Application.Wrappers;
    using Domain.Entities;
    using Infrastructure.Persistence.Contexts;
    using Microsoft.EntityFrameworkCore;
    using Sieve.Models;
    using Sieve.Services;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class DiscountRepository : IDiscountRepository
    {
        private BespokedBikesDbContext _context;
        private readonly SieveProcessor _sieveProcessor;

        public DiscountRepository(BespokedBikesDbContext context,
            SieveProcessor sieveProcessor)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
            _sieveProcessor = sieveProcessor ??
                throw new ArgumentNullException(nameof(sieveProcessor));
        }

        public PagedList<Discount> GetDiscounts(DiscountParametersDto discountParameters)
        {
            if (discountParameters == null)
            {
                throw new ArgumentNullException(nameof(discountParameters));
            }

            var collection = _context.Discounts
                .Include(d => d.Product) as IQueryable<Discount>; // TODO: AsNoTracking() should increase performance, but will break the sort tests. need to investigate

            var sieveModel = new SieveModel
            {
                Sorts = discountParameters.SortOrder,
                Filters = discountParameters.Filters
            };

            collection = _sieveProcessor.Apply(sieveModel, collection);

            return PagedList<Discount>.Create(collection,
                discountParameters.PageNumber,
                discountParameters.PageSize);
        }

        public async Task<Discount> GetDiscountAsync(int discountId)
        {
            return await _context.Discounts.FirstOrDefaultAsync(d => d.DiscountId == discountId);
        }

        public Discount GetDiscount(int discountId)
        {
            return _context.Discounts.FirstOrDefault(d => d.DiscountId == discountId);
        }

        public void AddDiscount(Discount discount)
        {
            if (discount == null)
            {
                throw new ArgumentNullException(nameof(Discount));
            }

            _context.Discounts.Add(discount);
        }

        public void DeleteDiscount(Discount discount)
        {
            if (discount == null)
            {
                throw new ArgumentNullException(nameof(Discount));
            }

            _context.Discounts.Remove(discount);
        }

        public void UpdateDiscount(Discount discount)
        {
            // no implementation for now
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}