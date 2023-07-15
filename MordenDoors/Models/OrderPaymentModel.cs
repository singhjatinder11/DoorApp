using System.ComponentModel.DataAnnotations;

namespace MordenDoors.Models
{
    public class OrderPaymentModel
    {
        public int ID { get; set; }
        public int OrderId { get; set; }
        [Display(Name = "Total Amount (CAD)")]
        public decimal OrderTotalAmount { get; set; }

        [Display(Name = "Pending Amount (CAD)")]
        public decimal PendingAmount { get; set; }

        [Display(Name = "Current Payment (CAD)")]
        public decimal Payment { get; set; }

        [Display(Name = "Comment")]
        public string Comments { get; set; }

        [Display(Name = "Payment Mode")]
        public string PaymentMode { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public System.DateTime CreatedOn { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public System.DateTime UpdatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

    }
}