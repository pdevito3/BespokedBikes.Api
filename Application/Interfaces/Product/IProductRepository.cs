namespace Application.Interfaces.Product
{
    using Application.Dtos.Product;
    using Application.Wrappers;
    using System.Threading.Tasks;
    using Domain.Entities;

    public interface IProductRepository
    {
        PagedList <Product> GetProducts(ProductParametersDto ProductParameters);
        Task<Product> GetProductAsync(int ProductId);
        Product GetProduct(int ProductId);
        void AddProduct(Product product);
        void DeleteProduct(Product product);
        void UpdateProduct(Product product);
        bool Save();
    }
}