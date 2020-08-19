namespace Infrastructure.Persistence.Repositories
{
    using Application.Dtos.Customer;
    using Application.Interfaces.Customer;
    using Application.Wrappers;
    using Domain.Entities;
    using Infrastructure.Persistence.Contexts;
    using Microsoft.EntityFrameworkCore;
    using Sieve.Models;
    using Sieve.Services;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class CustomerRepository : ICustomerRepository
    {
        private BespokedBikesDbContext _context;
        private readonly SieveProcessor _sieveProcessor;

        public CustomerRepository(BespokedBikesDbContext context,
            SieveProcessor sieveProcessor)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
            _sieveProcessor = sieveProcessor ??
                throw new ArgumentNullException(nameof(sieveProcessor));
        }

        public PagedList<Customer> GetCustomers(CustomerParametersDto customerParameters)
        {
            if (customerParameters == null)
            {
                throw new ArgumentNullException(nameof(customerParameters));
            }

            var collection = _context.Customers as IQueryable<Customer>; // TODO: AsNoTracking() should increase performance, but will break the sort tests. need to investigate

            var sieveModel = new SieveModel
            {
                Sorts = customerParameters.SortOrder,
                Filters = customerParameters.Filters
            };

            collection = _sieveProcessor.Apply(sieveModel, collection);

            return PagedList<Customer>.Create(collection,
                customerParameters.PageNumber,
                customerParameters.PageSize);
        }

        public async Task<Customer> GetCustomerAsync(int customerId)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public Customer GetCustomer(int customerId)
        {
            return _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
        }

        public void AddCustomer(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(Customer));
            }

            _context.Customers.Add(customer);
        }

        public void DeleteCustomer(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(Customer));
            }

            _context.Customers.Remove(customer);
        }

        public void UpdateCustomer(Customer customer)
        {
            // no implementation for now
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}