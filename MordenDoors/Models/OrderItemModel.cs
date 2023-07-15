using System;
using System.Collections.Generic;

namespace MordenDoors.Models
{
    public class OrderItemModel
    {
        public int RowId { get; set; }
        public int ProductId { get; set; }
        public string SelectedCategoryText { get; set; }
        public int SelectedCategoryValue { get; set; }
        public string SelectedProductText { get; set; }
        public int SelectedProductValue { get; set; }
        public string Quantity { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public decimal Price { get; set; }
        public decimal CustomPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string ItemComment { get; set; }
        public int[] workstage { get; set; }

        public decimal TotalWeight { get; set; }

        public string UnitType { get; set; }

        public string Unit { get; set; }
    }
    public class OrderItemModelNew
    {
        public int RowId { get; set; }
        public int ProductId { get; set; }
        public string Quantity { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal MainHeight { get; set; }
        public decimal MainWidth { get; set; }
        public decimal Price { get; set; }
        public decimal CustomPrice { get; set; }
        public decimal SubTotal { get; set; }
        public string ItemComment { get; set; }
        public decimal TotalWeight { get; set; }
        public string selectedUnit { get; set; }
        public string Unit { get; set; }
        public bool IsSubItem { get; set; }
    }
    public class OrderItemModelList
    {
        public int RowId { get; set; }
        public List<OrderItemModelNew> Items { get; set; }
    }
    public class UserItemModel
    {
        public int customerID { get; set; }
        
        public DateTime OrderDDate { get; set; }
        public string orderComment { get; set; }
        public int orderStatus { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string PO { get; set; }
        public string Job { get; set; }
        public string CustomePrice { get; set; }
        public decimal Payableamount { get; set; }
        public decimal Totalamount { get; set; }
        public decimal GST { get; set; }
        public decimal PST { get; set; }

    }
}