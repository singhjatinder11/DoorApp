using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MordenDoors.ViewModels
{
    public class OrderItem
    {
        public int ProductId { get; set; }

        public int OrderitemID { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public decimal Productheight { get; set; }
        public decimal ProductWidth { get; set; }

        public decimal ProductQuantity { get; set; }

        public decimal ProductSubtotal { get; set; }

        public int? UnitID { get; set; }

        public string Unit { get; set; }
        public string selectedUnit { get; set; }
        public decimal MinSqFt { get; set; }
        public decimal TotalWeight { get; set; }

        public List<OrderSubItem> orderSubitems = new List<OrderSubItem>();

    }

    public class OrderSubItem
    {
        public int OrderitemID { get; set; }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string SubProductName { get; set; }
        public decimal SubPrice { get; set; }
        public decimal Subheight { get; set; }

        public decimal MinSqFt { get; set; }
        public decimal SubWidth { get; set; }

        public decimal SubQuantity { get; set; }

        public decimal SubTotalPrice { get; set; }

        public decimal SubTotalWeight { get; set; }

        public int SubUnit { get; set; }

        public string SubUnitType { get; set; }

    }
}

