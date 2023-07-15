using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MordenDoors.ViewModels
{
    public class InvoiceModel
    {
        public int OrderId { get; set; }
        public string Tax { get; set; }
        public string ZohoCustomerId { get; set; }
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public string Comments { get; set; }
        public int? CurrentStageId { get; set; }
        public string AssigedTo { get; set; }
        public int ItemStatus { get; set; }
        public decimal? TotalPrice { get; set; }
        public string ProductName { get; set; }
        public string PrdouctDescription { get; set; }
        public string PrdouctZohoId { get; set; }
        public string PO { get; set; }
    }
}