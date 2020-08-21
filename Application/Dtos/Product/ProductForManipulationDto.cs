namespace Application.Dtos.Product
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public abstract class ProductForManipulationDto 
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Style { get; set; }
        public int PurchasePrice { get; set; }
        public int SalePrice { get; set; }
        public int QuantityOnHand { get; set; }
        public decimal? CommissionPercentage { get; set; }

        // add-on property marker - Do Not Delete This Comment
    }
}