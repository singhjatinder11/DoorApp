using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MordenDoors.ViewModels
{
    public class OperationsViewModel
    {
        public int Id { get; set; }
        public string empID { get; set; }
        public string fullname { get; set; }
        public int workstageId { get; set; }
        public string workstage { get; set; }
        public DateTime? finishTime { get; set; }
        public bool completeStatus { get; set; }
        public int orderId { get; set; }
        public int? qtyDone { get; set; }
        public DateTime? releaseDate { get; set; }
        public DateTime? startTime { get; set; }
        public decimal itemQty { get; set; }
        public int? OrderItem { get; set; }
        public string productName { get; set; }
        public byte? Sort { get; set; }
        public string Location { get; set; }
    }
}