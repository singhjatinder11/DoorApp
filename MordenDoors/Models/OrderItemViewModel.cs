using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MordenDoors.Models
{
    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public Nullable<decimal> Height { get; set; }
        public Nullable<decimal> Width { get; set; }
        public decimal Quantity { get; set; }
        public string Comments { get; set; }
        public Nullable<int> CurrentStageId { get; set; }
        public string AssigedTo { get; set; }
        public int ItemStatus { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
    }
}