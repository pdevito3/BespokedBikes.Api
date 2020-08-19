namespace Domain.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Sieve.Attributes;

    public class Sale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Sieve(CanFilter = true, CanSort = false)]
        public int SaleId { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public int ProductId { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public int SalespersonId { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public DateTimeOffset? SaleDate { get; set; }

        // add-on property marker - Do Not Delete This Comment
        [ForeignKey("ProductId")]
        public Product Product { get; set; } = new Product { };

        [ForeignKey("SalespersonId")]
        public Salesperson Salesperson { get; set; } = new Salesperson { };
    }
}