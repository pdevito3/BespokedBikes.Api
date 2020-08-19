namespace Application.Dtos.Product
{
    public class ProductParametersDto : ProductPaginationParameters
    {
        public string Filters { get; set; }
        public string SortOrder { get; set; }
    }
}