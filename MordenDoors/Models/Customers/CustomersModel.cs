using System.ComponentModel.DataAnnotations;

namespace MordenDoors.Models.Customers
{
    public class CustomersModel
    {
        public int Id { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Customer Name")]
        public string FullyQualifiedName { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string DisplayName { get; set; }
        public string PrintOnCheckName { get; set; }
        public bool Active { get; set; }
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
}