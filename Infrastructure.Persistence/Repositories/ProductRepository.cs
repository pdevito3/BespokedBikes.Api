namespace Infrastructure.Persistence.Repositories
{
    using Application.Dtos.Product;
    using Application.Interfaces.Product;
    using Application.Wrappers;
    using Domain.Entities;
    using Infrastructure.Persistence.Contexts;
    using Microsoft.EntityFrameworkCore;
    using Sieve.Models;
    using Sieve.Services;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class ProductRepository : IProductRepository
    {
        private BespokedBikesDbContext _context;
        private readonly SieveProcessor _sieveProcessor;

        public ProductRepository(BespokedBikesDbContext context,
            SieveProcessor sieveProcessor)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
            _sieveProcessor = sieveProcessor ??
                throw new ArgumentNullException(nameof(sieveProcessor));
        }

        public PagedList<Product> GetProducts(ProductParametersDto productParameters)
        {
            if (productParameters == null)
            {
                throw new ArgumentNullException(nameof(productParameters));
            }

            var collection = _context.Products as IQueryable<Product>; // TODO: AsNoTracking() should increase performance, but will break the sort tests. need to investigate

            var sieveModel = new SieveModel
            {
                Sorts = productParameters.SortOrder,
                Filters = productParameters.Filters
            };

            collection = _sieveProcessor.Apply(sieveModel, collection);

            return PagedList<Product>.Create(collection,
                productParameters.PageNumber,
                productParameters.PageSize);
        }

        public async Task<Product> GetProductAsync(int productId)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public Product GetProduct(int productId)
        {
            return _context.Products.FirstOrDefault(p => p.ProductId == productId);
        }

        public void AddProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(Product));
            }

            _context.Products.Add(product);
        }

        public void DeleteProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(Product));
            }

            _context.Products.Remove(product);
        }

        public void UpdateProduct(Product product)
        {
            // no implementation for now
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}