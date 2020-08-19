namespace Domain.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Sieve.Attributes;

    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Sieve(CanFilter = true, CanSort = false)]
        public int ProductId { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public string Name { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public string Manufacturer { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public string Style { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public int PurchasePrice { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public int SalePrice { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public int QuantityOnHand { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public int ComissionPercentage { get; set; }

        // add-on property marker - Do Not Delete This Comment
    }
}