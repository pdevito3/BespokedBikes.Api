namespace Application.Dtos.Discount
{
    public class DiscountParametersDto : DiscountPaginationParameters
    {
        public string Filters { get; set; }
        public string SortOrder { get; set; }
    }
}