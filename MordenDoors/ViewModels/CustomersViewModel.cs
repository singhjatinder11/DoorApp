using MordenDoors.Models;
using MordenDoors.Models.Customers;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MordenDoors.ViewModels
{
    public class CustomersViewModel
    {
        public IEnumerable<CustomerPriceModel> CustmersPrice { get; set; }
        public IEnumerable<ProductModel> Products { get; set; }
        public IEnumerable<SelectListItem> Category { get; set; }

        public IEnumerable<SelectListItem> Customers { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
    }
}