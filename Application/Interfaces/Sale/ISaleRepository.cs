namespace Application.Interfaces.Sale
{
    using Application.Dtos.Sale;
    using Application.Wrappers;
    using System.Threading.Tasks;
    using Domain.Entities;

    public interface ISaleRepository
    {
        PagedList <Sale> GetSales(SaleParametersDto SaleParameters);
        Task<Sale> GetSaleAsync(int SaleId);
        Sale GetSale(int SaleId);
        void AddSale(Sale sale);
        void DeleteSale(Sale sale);
        void UpdateSale(Sale sale);
        bool Save();
    }
}