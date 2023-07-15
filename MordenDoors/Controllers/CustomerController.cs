using MordenDoors.Database;
using MordenDoors.Repository;
using MordenDoors.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MordenDoors.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        private readonly MordenDoorsEntities mordenDoorsEntities = new MordenDoorsEntities();
        private readonly CustomerRepository _repository = new CustomerRepository();

        public ActionResult Index()
        {
            CustomersViewModel viewModel = new CustomersViewModel();
            viewModel.CustmersPrice = _repository.CustomerPrice();
            var firstCategoryID = mordenDoorsEntities.Customers.FirstOrDefault().Id;
            var categories = mordenDoorsEntities.Customers.Select(x => new SelectListItem { Text = x.CompanyName, Value = x.Id.ToString(), Selected = x.Id == firstCategoryID ? true : false }).ToList();
            viewModel.Customers = categories;
            //var allCat = viewModel.Category.Select(x => new SelectListItem { Text = "All Categories", Value = "0" });
            viewModel.Category = _repository.Category().Category;
            TempData["firstCategoryID"] = firstCategoryID;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CustomerPrice(int? id)
        {
            var customerList = mordenDoorsEntities.Customers.ToList();

            CustomersViewModel viewModel = new CustomersViewModel
            {
                CustmersPrice = _repository.CustomerPrice(id),
                Customers = customerList.Select(x => new SelectListItem { Text = x.CompanyName, Value = x.Id.ToString() }),
                Products = _repository.Products(),
                CustomerId = id,
                CustomerName = customerList.Where(c => c.Id == id).FirstOrDefault().CompanyName,
                Category = _repository.Category().Category
            };
            return PartialView("_CustomerPrice", viewModel);
        }

        [HttpPost]
        public bool UpdatePrice(int customerId, int productId, int categoryId, decimal price)
        {
            return _repository.UpdatePrice(categoryId, productId, customerId, price) ? true : false;
        }

        public ActionResult CustomerList()
        {
            var result = _repository.Customers();
            return View(result);
        }
    }
}