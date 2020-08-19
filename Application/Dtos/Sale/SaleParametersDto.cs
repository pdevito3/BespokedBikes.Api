namespace Application.Dtos.Sale
{
    public class SaleParametersDto : SalePaginationParameters
    {
        public string Filters { get; set; }
        public string SortOrder { get; set; }
    }
}