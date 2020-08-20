namespace Domain.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Sieve.Attributes;

    public class Discount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Sieve(CanFilter = true, CanSort = false)]
        public int DiscountId { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public int ProductId { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public DateTimeOffset? BeginDate { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public DateTimeOffset? EndDate { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public decimal? DiscountPercentage { get; set; }

        // add-on property marker - Do Not Delete This Comment
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}