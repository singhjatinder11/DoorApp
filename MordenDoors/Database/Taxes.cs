//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MordenDoors.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class Taxes
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ExternalId { get; set; }
        public Nullable<decimal> TaxPercentage { get; set; }
        public string TaxType { get; set; }
        public string Country { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    }
}
