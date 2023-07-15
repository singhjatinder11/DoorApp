using MordenDoors.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MordenDoors.ViewModels
{
    public class OrderPaymentViewModel
    {
        public List<OrderPaymentModel> OrderPayment { get; set; }

        public OrderPaymentModel AddPayment { get; set; }
        public int OrderId { get; set; }

        [Display(Name = "Total Amount")]
        public decimal OrderTotalAmount { get; set; }
        public enum PaymentModeOption
        {
            Cash,
            PayPal,
            [Display(Name = "Credit Card")]
            CreditCard,
            [Display(Name = "Debit Card")]
            DebitCard,
            NetBanKing
        }
    }
}