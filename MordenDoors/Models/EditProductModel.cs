using MordenDoors.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MordenDoors.Models
{
    public class EditProductModel
    {
        public int EditProductId { get; set; }
        public int EditCategoryId { get; set; }
        public string EditProductName { get; set; }
        public string EditProductDescription { get; set; }
        public int? EditUnitsOfMeasureID { get; set; }
        public decimal EditRate { get; set; }
        public bool EditIsActive { get; set; }
        public string EditQbId { get; set; }
        public string EditSKU { get; set; }
        public string EditproductImages { get; set; }
        public System.Nullable<decimal> Weight { get; set; }
        public System.Nullable<decimal> MinimumSquareFeet { get; set; }
        public CategortyViewModel EditCategory { get; set; }
        public UnitViewModel EditUnitMeasure { get; set; }
        public IEnumerable<SelectListItem> EditUnitsOfMeasures { get; set; }
        public IEnumerable<SelectListItem> EditCategories { get; set; }
        public string EditUserRole { get; set; }
        public IEnumerable<SelectListItem> EditUserRoles { get; set; }

       // public IEnumerable<SelectWorkstageListitem> EditUserRoles { get; set; }
    }
}