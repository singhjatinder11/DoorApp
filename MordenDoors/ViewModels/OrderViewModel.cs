using MordenDoors.Database;
using MordenDoors.Models.Customers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MordenDoors.ViewModels
{
    public class OrderViewModel
    {
        public IEnumerable<SelectListItem> Products { get; set; }
        public IEnumerable<OrdersModel> Orders { get; set; }
        public IEnumerable<SelectListItem> OrdersStatus { get; set; }
        public IEnumerable<SelectListItem> OrdersWorkStages { get; set; }
        public IEnumerable<SelectListItem> SortStatus { get; set; }
        public IEnumerable<OrderViewModel> OrderDetail { get; set; }
        public IEnumerable<productlist> Prodcutlist { get; set; }

        public List<OrderItem> orderitems { get; set; }

        public  ArrayList Productname = new ArrayList();
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public IEnumerable<SelectListItem> CustomerList { get; set; }
        public IEnumerable<SelectListItem> UnitList { get; set; }
        public IEnumerable<WorkStages> workStages { get; set; }
        public ProductViewModel ProductModel { get; set; }
        public OrderDetail orderdetail { get; set; }
        public List<OrderDetail> orderdetaillist { get; set; }
        public OrdersTable orders { get; set; }
        public UnitViewModel UnitViewModel { get; set; }
        public CustomerPriceModel CustomerPrice { get; set; }
        public List<CustomerPriceModel> CustomerPriceList { get; set; }
        public string[] EditUserRole { get; set; }
        public IEnumerable<SelectListItem> EditUserRoles { get; set; }
        public DateTime? DueDate { get; set; }
        public string estmtDate { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal PendingAmount { get; set; }
        public string Balance { get; set; }
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public Nullable<decimal> Height { get; set; }
        public Nullable<decimal> Width { get; set; }
        public decimal Quantity { get; set; }
        public string Comments { get; set; }
        public Nullable<int> CurrentStageId { get; set; }
        public string AssigedTo { get; set; }
        public Nullable<int> ItemStatus { get; set; }
        public string OrderStatus { get; set; }
        public string ProductName { get; set; }
        public string  StageName { get; set; }
        public string TrackingID { get; set; }

    }

 

    public class productlist
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string ProductName { get; set; }       
        public string Units { get; set; }
        public int? UnitId { get; set; }
        public decimal Rate { get; set; }
        public decimal CustomPrice { get; set; }
        public decimal MinSqFt { get; set; }
       
    }

    public class OrdersModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Comments { get; set; }
        public int StatusId { get; set; }
        public bool PaymentComplete { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public System.DateTime CretaedOn { get; set; }

        public System.DateTime UpdatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string InvoiceId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public System.DateTime DeliveryTime { get; set; }
        public string CustomerName { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime? DueDate { get; set; }
        public int? CurrentStatus { get; set; }
        public decimal? PendingAmount { get; set; }

        public decimal? ItemQuantity { get; set; }

    }

    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string FullyQualifiedName { get; set; }
        public string DisplayName { get; set; }
        public string PrintOnCheckName { get; set; }
        public string Active { get; set; }
        public string PrimaryPhone { get; set; }
        public string AlternatePhone { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public string DefaultTaxCodeRef { get; set; }
        public string time { get; set; }
        public string CreateTime { get; set; }
        public string LastUpdatedTime { get; set; }
        public string PreferredDeliveryMethod { get; set; }
        public string CurrencyRef { get; set; }
        public string Balance { get; set; }
        public string BalanceWithJobs { get; set; }
        public string Taxable { get; set; }
        public string BillAddrId { get; set; }
        public string BillAddrLine1 { get; set; }
        public string BillAddrLine2 { get; set; }
        public string BillAddrCity { get; set; }
        public string BillAddrCountry { get; set; }
        public string BillAddrCountrySubDivisionCode { get; set; }
        public string BillAddrPostalCode { get; set; }
        public string ShipAddrId { get; set; }
        public string ShipAddrLine1 { get; set; }
        public string ShipAddrLine2 { get; set; }
        public string ShipAddrCity { get; set; }
        public string ShipAddrCountry { get; set; }
        public string ShipAddrCountrySubDivisionCode { get; set; }
        public string ShipAddrPostalCode { get; set; }
        public string PrimaryEmailAddr { get; set; }
    }

    public partial class OrderDetail
    {
        public int OrderDetailsID { get; set; }
        public int OrderID { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public decimal Quantity { get; set; }
        public string ProductIDs { get; set; }
        public string Description { get; set; }
        public int itemStatus { get; set; }
        public Nullable<int> QuantitySent { get; set; }
        public Nullable<int> InchMm { get; set; }
        public Nullable<int> UnitsOfMeasureID { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public Nullable<double> SubQuantity { get; set; }
        public string Customization { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string ItemHeader { get; set; }
        public string QbItemId { get; set; }
        public Nullable<double> HtInDec { get; set; }
        public Nullable<double> WdInDec { get; set; }
    }
    public partial class OrderDetail
    {
        public Nullable<decimal> Price { get; set; }
        public int cateid { get; set; }
        public string unitname { get; set; }
        public int? unitid { get; set; }
        public List<int> workstage { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
    }


    public partial class OrdersTable
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Comments { get; set; }
        public int StatusId { get; set; }
        public bool PaymentComplete { get; set; }
        public System.DateTime CretaedOn { get; set; }
        public System.DateTime UpdatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string InvoiceId { get; set; }
        public System.DateTime DeliveryTime { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }
        public string PO { get; set; }
        public string Job { get; set; }
        public string CustomePrice { get; set; }
        public decimal? Payableamount { get; set; }
        public decimal? GST { get; set; }
        public decimal? PST { get; set; }

        public string Balance { get; set; }
    }

    public class OrderTrackViewModel
    {
        public int Id { get; set; }
        //public int CustomerId { get; set; }
        public string OrderStatus { get; set; }
        public string CurrentItemStage { get; set; }

        [Required(ErrorMessage = "Please enter your Tracking ID")]
        public string TrackingID { get; set; }
        public System.DateTime DeliveryTime { get; set; }
        public string ProductName { get; set; }
        public string itemStatus { get; set; }
        public decimal TotalPrice { get; set; }
        
        public string DeliveryAddress { get; set; }


        public IEnumerable<OrderTrackViewModel> OrderItemList { get; set; }
    }
}