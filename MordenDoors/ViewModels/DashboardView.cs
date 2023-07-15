using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MordenDoors.ViewModels
{
    public class DashboardView
    {
        public int TotalOrder { get; set; }
        public int ProductSold { get; set; }
        public int PendingOrder { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCustomer { get; set; }
        public int TotalEmployee { get; set; }
        public string getchartdata { get; set; }
    }
}