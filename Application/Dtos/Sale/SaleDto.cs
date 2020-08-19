namespace Application.Dtos.Sale
{
    using Application.Dtos.Product;
    using Application.Dtos.Salesperson;
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public  class SaleDto 
    {
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public int SalespersonId { get; set; }
        public DateTimeOffset? SaleDate { get; set; }

        [JsonProperty("product")]
        public ProductDto ProductDto { get; set; } = new ProductDto { };

        [JsonProperty("salesperson")]
        public SalespersonDto SalespersonDto { get; set; } = new SalespersonDto { };
        // add-on property marker - Do Not Delete This Comment
    }
}