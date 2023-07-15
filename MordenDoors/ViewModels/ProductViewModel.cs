using MordenDoors.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MordenDoors.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int? UnitsOfMeasureID { get; set; }
        public decimal? Rate { get; set; }
        public bool IsActive { get; set; }
        public string QbId { get; set; }
        public string SKU { get; set; }
        public CategortyViewModel category { get; set; }
        public UnitViewModel UnitMeasure { get; set; }
        //public IEnumerable<SelectListItem> UnitsOfMeasures { get; set; }
        //public IEnumerable<SelectListItem> Categories { get; set; }

        public ProductModel Product { get; set; }
        public IEnumerable<ProductModel> Products { get; set; }
        public IEnumerable<SelectListItem> UnitsOfMeasures { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public string[] UserRole { get; set; }
        public IEnumerable<SelectListItem> UserRoles { get; set; }

        public string ProductImages { get; set; }

        public string CategoryName { get; set; }

        public string UnitName { get; set; }

        public int? CategoryId { get; set; }
    }
    public class UnitViewModel
    {
        public int UnitID { get; set; }
        public string UnitName { get; set; }
        public string UnitDescription { get; set; }
        public bool IsActive { get; set; }
    }

}