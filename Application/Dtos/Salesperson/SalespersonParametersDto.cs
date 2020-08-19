namespace Application.Dtos.Salesperson
{
    public class SalespersonParametersDto : SalespersonPaginationParameters
    {
        public string Filters { get; set; }
        public string SortOrder { get; set; }
    }
}