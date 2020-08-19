namespace Application.Dtos.Sale
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public  class SaleDto 
    {
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public int SalespersonId { get; set; }
        public DateTimeOffset? SaleDate { get; set; }

        // add-on property marker - Do Not Delete This Comment
    }
}