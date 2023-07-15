using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MordenDoors.Models.Customers
{
    public class CustmerHomePage
    {
        public int OrderId { get; set; }
        public int? OrderItemId { get; set; }
        public string CustomerName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public System.DateTime DeliveryTime { get; set; }

        public int ProductID { get; set; }
        public string ProductName { get; set; }

        public Nullable<decimal> Height { get; set; }

        public Nullable<decimal> Width { get; set; }

        public decimal OrderQuantity { get; set; }
        public int WorkStageId { get; set; }
        public string WorkStage { get; set; }
        public int Sort { get; set; }
        public int OperationsId { get; set; }
        public int QuantityDone { get; set; }
        public bool Operation { get; set; }
        public bool CanGet { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public Nullable<System.DateTime> StartTime { get; set; }
        public string Location { get; set; }
        public string EmployeeId { get; set; }
    }
}