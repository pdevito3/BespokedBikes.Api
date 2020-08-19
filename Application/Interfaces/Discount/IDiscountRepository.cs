namespace Application.Interfaces.Discount
{
    using Application.Dtos.Discount;
    using Application.Wrappers;
    using System.Threading.Tasks;
    using Domain.Entities;

    public interface IDiscountRepository
    {
        PagedList <Discount> GetDiscounts(DiscountParametersDto DiscountParameters);
        Task<Discount> GetDiscountAsync(int DiscountId);
        Discount GetDiscount(int DiscountId);
        void AddDiscount(Discount discount);
        void DeleteDiscount(Discount discount);
        void UpdateDiscount(Discount discount);
        bool Save();
    }
}