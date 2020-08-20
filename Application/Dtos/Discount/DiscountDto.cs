namespace Application.Dtos.Discount
{
    using Application.Dtos.Product;
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public  class DiscountDto 
    {
        public int DiscountId { get; set; }
        public int ProductId { get; set; }
        public DateTimeOffset? BeginDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public decimal? DiscountPercentage { get; set; }

        [JsonProperty("product")]
        public ProductDto ProductDto { get; set; }

        // add-on property marker - Do Not Delete This Comment
    }
}