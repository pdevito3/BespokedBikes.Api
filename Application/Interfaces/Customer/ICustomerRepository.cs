namespace Application.Interfaces.Customer
{
    using Application.Dtos.Customer;
    using Application.Wrappers;
    using System.Threading.Tasks;
    using Domain.Entities;

    public interface ICustomerRepository
    {
        PagedList <Customer> GetCustomers(CustomerParametersDto CustomerParameters);
        Task<Customer> GetCustomerAsync(int CustomerId);
        Customer GetCustomer(int CustomerId);
        void AddCustomer(Customer customer);
        void DeleteCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        bool Save();
    }
}