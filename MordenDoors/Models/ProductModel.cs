using MordenDoors.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MordenDoors.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int? UnitsOfMeasureID { get; set; }
        public System.Nullable<decimal> Weight { get; set; }
        public System.Nullable<decimal> MinimumSquareFeet { get; set; }
        public decimal Rate { get; set; }
        public bool IsActive { get; set; }
        public ProductModel()
        {
            IsActive = true;

        }
        public string QbId { get; set; }
        public string SKU { get; set; }
        public string productImage { get; set; }

        public CategortyViewModel Category { get; set; }
        public UnitViewModel UnitMeasure { get; set; }
        public IEnumerable<SelectListItem> UnitsOfMeasures { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public string UserRole { get; set; }
        public IEnumerable<SelectListItem> UserRoles { get; set; }
        public List<string> WorkStages { get; set; }
    }
}