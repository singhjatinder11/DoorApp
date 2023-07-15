using MordenDoors.Database;
using MordenDoors.Models;
using MordenDoors.Models.Customers;
using MordenDoors.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace MordenDoors.Repository
{
    public class CustomerRepository
    {
        private readonly MordenDoorsEntities _context;
        public CustomerRepository()
        {
            _context = new MordenDoorsEntities();
        }

        public string GetCustomerEmailByID(int _id)
        {
            string _email = _context.Customers.Where(x => x.Id == _id).FirstOrDefault().PrimaryEmailAddr;
            return _email;
        }

        public IEnumerable<CustomersModel> CustomersList(int id)
        {
            return _context.Customers.Where(x => x.Id == id).Select(p => new CustomersModel
            {
                CompanyName = p.CompanyName,
                Id = p.Id
            }).ToList();
        }


        public IEnumerable<ProductModel> Products()
        {
            var cat = _context.Categories.Where(x => x.IsActive == true).Select(y=>y.CategoryId).ToList();
            return _context.Products.Where(x => x.IsActive == true && cat .Contains(x.CategoryId)).Select(p => new ProductModel
            {
                ProductId = p.ProductId,
                CategoryId = p.CategoryId,
                ProductName = p.ProductName,
                Rate = p.Rate,
                Category = _context.Categories.Where(x => x.CategoryId == p.CategoryId).Select(c => new CategortyViewModel
                { ID = p.CategoryId, CategoryName = p.Categories.CategoryName.ToString() }).FirstOrDefault()
            }).ToList();
        }
        public IEnumerable<CustomerPriceModel> CustomerPrice(int? customerId)
        {
            var res = _context.CustomerPrice.Where(x => x.CustomerId == customerId).Select(p => new CustomerPriceModel
            {
                CategoryId = p.CategoryId,
                CustomerId = p.CustomerId,
                ID = p.ID,
                Price = p.Price,
                ProductId = p.ProductId
            }).ToList();
            return res;
        }

        public  CustomersViewModel Category()
        {
            CustomersViewModel customerPrice = new CustomersViewModel();
            customerPrice.Category = _context.Categories.Where(s=>s.IsActive==true).Select(x => new System.Web.Mvc.SelectListItem
            {
                Text = x.CategoryName,
                Value = x.CategoryId.ToString()
            });
            return customerPrice;
        }

        public IEnumerable<CustomerPriceModel> CustomerPrice()
        {
            return _context.CustomerPrice.Select(p => new CustomerPriceModel
            {
                CategoryId = p.CategoryId,
                CustomerId = p.CustomerId,
                ID = p.ID,
                Price = p.Price,
                ProductId = p.ProductId
            }).ToList();
        }

        public bool UpdatePrice(int catId, int proId, int custId, decimal price)
        {
            var result = _context.CustomerPrice.Where(x => x.CustomerId == custId && x.CategoryId == catId && x.ProductId == proId).FirstOrDefault();
            if (result != null)
            {
                if (result.Price == price)
                    return true;
                var cust = (from c in _context.CustomerPrice
                            where c.ProductId == proId && c.CustomerId == custId && c.CategoryId == catId
                            select c).First();
                cust.Price = price;
                var save = _context.SaveChanges();
                if (save > 0)
                    return true;
                else
                    return false;
            }
            else
            {
                CustomerPrice customerPrice = new CustomerPrice();
                customerPrice.CategoryId = catId;
                customerPrice.ProductId = proId;
                customerPrice.CustomerId = custId;
                customerPrice.Price = price;
                _context.CustomerPrice.Add(customerPrice);
                var save = _context.SaveChanges();
                if (save > 0)
                    return true;
                else
                    return false;
            }
        }

        public IEnumerable<CustomersModel> Customers()
        {

            return _context.Customers.Select(p => new CustomersModel
            {
                CompanyName = p.CompanyName,
                PrimaryEmailAddr = p.PrimaryEmailAddr,
                PrimaryPhone = p.PrimaryPhone,
                Balance = p.Balance
            }).ToList();
        }
    }
}