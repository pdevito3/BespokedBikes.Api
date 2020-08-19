namespace Application.Dtos.Salesperson
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public abstract class SalespersonForManipulationDto 
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? TerminationDate { get; set; }
        public int Manager { get; set; }

        // add-on property marker - Do Not Delete This Comment
    }
}