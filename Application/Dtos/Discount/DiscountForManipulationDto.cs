namespace Application.Dtos.Discount
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public abstract class DiscountForManipulationDto 
    {
        public int ProductId { get; set; }
        public DateTimeOffset? BeginDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public decimal? DiscountPercentage { get; set; }

        // add-on property marker - Do Not Delete This Comment
    }
}