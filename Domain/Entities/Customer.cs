namespace Domain.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Sieve.Attributes;

    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Sieve(CanFilter = true, CanSort = false)]
        public int CustomerId { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public string FirstName { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public string LastName { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public string Address1 { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public string Address2 { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public string City { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public string State { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public string PostalCode { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public string PhoneNumber { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public DateTimeOffset? StartDate { get; set; }

        // add-on property marker - Do Not Delete This Comment
    }
}