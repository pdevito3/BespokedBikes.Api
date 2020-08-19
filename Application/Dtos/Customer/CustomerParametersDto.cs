namespace Application.Dtos.Customer
{
    public class CustomerParametersDto : CustomerPaginationParameters
    {
        public string Filters { get; set; }
        public string SortOrder { get; set; }
    }
}