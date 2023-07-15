using MordenDoors.Database;
using MordenDoors.Models;
using MordenDoors.Repository;
using MordenDoors.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using MordenDoors.CustomFilter;
using MordenDoors.Models.Customers;
using System.IO;
using MordenDoors.Utility;
using System.Web.Configuration;
using Microsoft.Ajax.Utilities;

namespace MordenDoors.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        // GET: Order
        private readonly MordenDoorsEntities db = new MordenDoorsEntities();
        private readonly OrderRepository _orderRepository = new OrderRepository();
        private readonly CustomerRepository _customerRepository = new CustomerRepository();

        [ExceptionHandler]
        public ActionResult OrderStatus(string status)
        {
            int statusId = _orderRepository.GetOrderStatusIdbyName(status);
            var page = string.Empty;
            var quoteStatusId = Convert.ToInt32(WebConfigurationManager.AppSettings["QuoteStatusId"]);

            OrderViewModel orderViewModel = new OrderViewModel();

            if (statusId == 1)
            {
                page = "quotes";
                orderViewModel = _orderRepository.orderListAccordingToStatus(quoteStatusId);
            }
            else if (statusId == 2)
            {
                page = "workshop";
                orderViewModel = _orderRepository.orderListAccordingToStatus(2);
            }
            else
            {
                var viewModel = _orderRepository.orderListAccordingToStatus(statusId);
                orderViewModel.Orders = viewModel.Orders.Where(o => o.StatusId != quoteStatusId);
                orderViewModel.OrdersStatus = viewModel.OrdersStatus.Where(o => o.Value != quoteStatusId.ToString());
            }

            ViewBag.Page = page;
            return View("Index", orderViewModel);
        }
        [ExceptionHandler]
        public ActionResult Index(int? statusId)
        {
            var page = string.Empty;
            var quoteStatusId = Convert.ToInt32(WebConfigurationManager.AppSettings["QuoteStatusId"]);
            
            OrderViewModel orderViewModel = new OrderViewModel();

            var url = Request.Url.Segments[1].Replace("/","").ToLower();
            if (url == "quotes")
            {
                page = "quotes";
                orderViewModel = _orderRepository.orderListAccordingToStatus(quoteStatusId);
            }
            else if (url == "workshop") {
                page = "workshop";
                orderViewModel = _orderRepository.orderListAccordingToStatus(2);
            }
            else
            {
                var viewModel = _orderRepository.orderListAccordingToStatus(statusId);
                orderViewModel.Orders = viewModel.Orders.Where(o => o.StatusId != quoteStatusId);
                orderViewModel.OrdersStatus = viewModel.OrdersStatus.Where(o => o.Value != quoteStatusId.ToString());
            }

            ViewBag.Page = page;
            return View(orderViewModel);
        }

        public ActionResult CreateOrder()
        {
            try
            {
                var AllCategories = db.Categories.Where(c=>c.IsActive==true).OrderByDescending(x=>x.IsMain).ToList();
                //  var Balance = db.Customers.Where(c => c.Balance == c.Balance).ToList();
               
               

                //JOin query main function for products then append product name

                OrderViewModel model = new OrderViewModel
                {
                                   CategoryList = AllCategories.Select(x => new SelectListItem { Text = x.CategoryName,        Value = x.CategoryId.ToString() }).ToList(),
                                   Prodcutlist = (from p in db.Products
                                   join unt in db.UnitsOfMeasure on p.UnitsOfMeasureID equals unt.UnitID
                                   join cat in db.Categories on p.CategoryId equals cat.CategoryId
                                   where cat.IsActive == true
                                   select (new productlist { ProductId = p.ProductId, CategoryId = p.CategoryId, Rate = p.Rate, ProductName = p.ProductName, Units = unt.UnitName, UnitId = p.UnitsOfMeasureID })).ToList(),
                CustomerList = db.Customers.Select(x => new SelectListItem { Text = x.CompanyName, Value = x.Id.ToString() }).ToList(),
                };
                //OrderViewModel model = new OrderViewModel();
                //model=_orderRepository.OrderRelatedDetail();
                // return View(model);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult AddOrder()
        {
            try
            {
                var AllCategories = db.Categories.Where(c => c.IsActive == true).OrderBy(s => s.Sort).OrderByDescending(x => x.IsMain).ToList();
                OrderViewModel model = new OrderViewModel
                {
                    CategoryList = AllCategories.Select(x => new SelectListItem { Text = x.CategoryName, Value = x.CategoryId.ToString()}).ToList(),
                    Prodcutlist = (from p in db.Products where p.IsActive ==true
                                   join unt in db.UnitsOfMeasure on p.UnitsOfMeasureID equals unt.UnitID
                                   join cat in db.Categories on p.CategoryId equals cat.CategoryId
                                   where cat.IsActive == true && p.IsActive == true
                                   select (new productlist { ProductId = p.ProductId, CategoryId = p.CategoryId, Rate = p.Rate, ProductName = p.ProductName, Units = unt.UnitName, UnitId = p.UnitsOfMeasureID, MinSqFt = p.MinimumSquareFeet ?? 0 })).ToList(),
                    CustomerList = db.Customers.Select(x => new SelectListItem { Text = x.CompanyName, Value = x.Id.ToString() }).ToList(),
                };
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult GetBalance(int Id)
        {
        var Balance = from s in db.Customers
                        where s.Id == Id
                        select s;
            return Json(new SelectList(Balance.ToArray(), "Id", "Balance"), JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult GetCreditLimit(int Id)
        {
        var CreditLimit = from s in db.Customers
                          where s.Id == Id
                          select s;
            return Json(new SelectList(CreditLimit.ToArray(), "Id", "CreditLimit"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ProductPrice(int productId, int customerId)
        {
            var product = db.Products.Where(x => x.ProductId == productId).Select(x => new { Rate = x.Rate, UnitId = x.UnitsOfMeasure.UnitID, UnitName = x.UnitsOfMeasure.UnitName, IsDimension = x.UnitsOfMeasure.HasHeight }).FirstOrDefault();
            var customerPrice = db.CustomerPrice.Where(x => x.ProductId == productId && x.CustomerId == customerId).FirstOrDefault();//var currentWorkStages = db.ProductWorkstages.Where(o => o.ProductId == productId).Select(x=> x.RoleId).ToList();
            IEnumerable<string> userId = new List<string>();
            var userList = db.ProductWorkstages.Where(x => x.ProductId == productId).Select(a => a.RoleId).ToList();
            userId = userList;
            var listItems = db.WorkStages.Where(a => userId.Contains(a.Id.ToString())).Select(x => new SelectListItem { Text = x.StageName, Value = x.Id.ToString(), Selected = true }).ToList();
            var nListItems = db.WorkStages.Where(a => !userId.Contains(a.Id.ToString())).Select(x => new SelectListItem { Text = x.StageName, Value = x.Id.ToString() }).ToList();
            listItems.AddRange(nListItems);
            return Json(new { productPrice = product, customerPrice = customerPrice == null ? product.Rate : customerPrice.Price, productWorkstages = listItems }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProductList(int categoryId)
        {
            var productList = db.Products.Where(x => x.CategoryId == categoryId).Select(x => new SelectListItem { Text = x.ProductDescription, Value = x.ProductId.ToString(), }).ToList();
            return Json(new { products = productList }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Product(string categoryName)
        {
            var productList = db.Products.Where(x => x.Categories.CategoryName == categoryName).Select(x => new SelectListItem { Text = x.ProductDescription, Value = x.ProductId.ToString() }).ToList();
            return Json(new { products = productList }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult CreateOrders(OrderModel model)
        //{
        //    string userEmail = Session["UserEmail"] == null ? User.Identity.Name : Session["UserEmail"].ToString();
        //    bool result = _orderRepository.CreateOrders(model, userEmail);
        //    int OrderID = _orderRepository.FetchLastAddedOrder();
        //    if(model.UserItems[0].orderStatus == 1)
        //    {
        //      bool _status = QuotationEmail(OrderID, model.UserItems[0].customerID, model);
        //    }
        //    return Json(new { status = result, RequestID = OrderID }, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult SaveOrder(OrderModelNew model)
        {
            try
            {
                string userEmail = Session["UserEmail"] == null ? User.Identity.Name : Session["UserEmail"].ToString();
                int OrderID = _orderRepository.SaveOrder(model, userEmail);
                if (model.UserItems[0].orderStatus == 1)
                {
                    bool _status = QuotationEmail(OrderID, model.UserItems[0].customerID, model);
                }
                return Json(new { status = true, RequestID = OrderID }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, RequestID = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditOrder(int id)
        {
            OrderViewModel model = new OrderViewModel();
            var result = _orderRepository.EditOrders(id);
            if (result != null)
            {
                var customerId = result.UserItems[0].customerID;
                var prodcutlist = (from p in db.Products
                                   where p.IsActive == true
                                   join unt in db.UnitsOfMeasure on p.UnitsOfMeasureID equals unt.UnitID
                                   join cat in db.Categories on p.CategoryId equals cat.CategoryId
                                   let customerPrice = db.CustomerPrice.Where(c => c.CustomerId == customerId && c.ProductId == p.ProductId).FirstOrDefault()
                                   where cat.IsActive == true && p.IsActive == true
                                   select (new productlist { ProductId = p.ProductId, CategoryId = p.CategoryId, Rate = (customerPrice == null ? p.Rate : (p.Rate - ((customerPrice.Price * p.Rate) / 100))), ProductName = p.ProductName, Units = unt.UnitName, UnitId = p.UnitsOfMeasureID, MinSqFt = p.MinimumSquareFeet ?? 0 })).ToList();

                model = new OrderViewModel
                {
                   orderitems = (from OI in db.OrderItems
                                  join _p in db.Products on OI.ProductId equals _p.ProductId
                                  join _um in db.UnitsOfMeasure on _p.UnitsOfMeasureID equals _um.UnitID
                                  where OI.OrderId == id
                                  select (new OrderItem { ProductId = _p.ProductId, CategoryId = _p.CategoryId, ProductName = _p.ProductName, Unit = _um.UnitName, selectedUnit = OI.UnitType, MinSqFt = _p.MinimumSquareFeet ?? 0 , OrderitemID = OI.Id, Price = OI.Price, UnitID = _um.UnitID, Productheight = OI.Height ?? 0, ProductWidth = OI.Width ?? 0, ProductQuantity = OI.Quantity, ProductSubtotal = OI.TotalPrice ?? 00 }
                                  )).ToList(),                    
                    CustomerList = db.Customers.Select(x => new SelectListItem { Text = x.CompanyName, Value = x.Id.ToString() }).ToList(),
                    UnitList = db.UnitsOfMeasure.Select(x => new SelectListItem { Text = x.UnitDescription, Value = x.UnitID.ToString() }).ToList(),
                    workStages = db.WorkStages.ToList(),                    
                    Prodcutlist = prodcutlist,
                    OrderId = id,
                    orders = db.Orders.Where(s => s.Id == id).Select(x => new OrdersTable
                    {
                        Id = x.Id,
                        CustomerId = x.CustomerId,
                        TotalAmount = x.TotalAmount,
                        StatusId = x.StatusId,
                        DeliveryTime = x.DeliveryTime,
                        // CustomerName = db.Customers.Where(o => o.Id == x.CustomerId).Select(s => s.CompanyName).FirstOrDefault(),
                        //CurrentStatus = x.StatusId,
                        //PendingAmount = db.OrderPayments.Where(s => s.OrderId == x.Id).OrderByDescending(s => s.ID).FirstOrDefault().PendingAmount,
                        CretaedOn = x.CretaedOn,
                        AddressLine1 = x.AddressLine1,
                        AddressLine2 = x.AddressLine2,
                        City = x.City,
                        Country = x.Country,
                        PinCode = x.PinCode,
                        Comments = x.Comments,
                        PO = x.PO == null ? "" : x.PO,
                        Job = x.Job,
                        CustomePrice = x.customerprice.ToString(),
                        Payableamount = (x.customerprice == 0 ? x.TotalAmount : x.PayableAmount),
                        GST = x.GST,
                        PST = x.PST,
                    }).FirstOrDefault()
                    
                };
            }
            for(int i=0; i< model.orderitems.Count; i++)
            {
                var orderItem = model.orderitems[i];
                var _result = (from OI in db.OrderSubItems
                               join _p in db.Products on OI.ProductId equals _p.ProductId
                               join _um in db.UnitsOfMeasure on _p.UnitsOfMeasureID equals _um.UnitID
                               where OI.OrderItemId == orderItem.OrderitemID
                               select (new OrderSubItem { ProductId = _p.ProductId, CategoryId = _p.CategoryId, SubProductName = _p.ProductName, SubUnitType = OI.UnitType, MinSqFt = _p.MinimumSquareFeet ?? 0, OrderitemID = OI.Id, SubPrice = OI.Price ?? 00, SubUnit = _um.UnitID, Subheight = OI.Height ?? 0, SubWidth = OI.Width ?? 0, SubQuantity = OI.Quantity, SubTotalPrice = OI.TotalPrice ?? 00 })).ToList();
                model.orderitems[i].orderSubitems.AddRange(_result.ToList());
            }
            productlist productlist = new productlist();
            var list = db.Categories.Where(s => s.IsActive == true && s.IsMain == false).OrderBy(s => s.ShortingNo).ToList();
            var ismain = db.Categories.Where(s => s.IsActive == true && s.IsMain == true).FirstOrDefault();
            list.Insert(0, ismain);
            model.CategoryList = list.Select(x => new SelectListItem { Text = x.CategoryName, Value = x.CategoryId.ToString() }).ToList();
            model.estmtDate = model.orders.DeliveryTime.ToString("yyyy-MM-dd");
            model.orders.Payableamount = Math.Round(Convert.ToDecimal(model.orders.Payableamount), 2);
            //return Json(new { orderDetail = result.UserItems, orderItemDetail = result.OrderItems }, JsonRequestBehavior.AllowGet);
            return View(model);
        }

        public bool QuotationEmail(int _orderID, int _CustomerID, OrderModelNew _model)
        {
            var callbackUrl = Url.Action("ConfirmQuotation", "Order", new { _OrderID = _orderID }, protocol: Request.Url.Scheme);
            string _email = _customerRepository.GetCustomerEmailByID(_CustomerID);
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Views/MailTemplate/QuotationConfirmation.cshtml")))
            {
               body = reader.ReadToEnd();
            }
            body = body.Replace("##ConfirmationLink##", callbackUrl);
            body = body.Replace("##CustomerAddress##", _model.UserItems[0].Address1);
            body = body.Replace("##DeliveryAddress##", _model.UserItems[0].Address2);
            body = body.Replace("##EstimatedeliveryDate##", (_model.UserItems[0].OrderDDate).ToString("dd-MM-yyyy"));
            body = body.Replace("##Po##", _model.UserItems[0].PO);
            body = body.Replace("##Comments##", _model.UserItems[0].orderComment);
            body = body.Replace("##PST##", Convert.ToString(_model.UserItems[0].PST));
            body = body.Replace("##GST##", Convert.ToString(_model.UserItems[0].GST));
            body = body.Replace("##TotalAmount##", Convert.ToString(_model.UserItems[0].Totalamount));
            string rowitems = string.Empty;
            for (int i = 0; i < _model.OrderItems.Count(); i++)
            {
                var mainProductItem = _model.OrderItems[i].Items[0];
                string row = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/Views/MailTemplate/Rowtemplate.html")))
                {
                    row = reader.ReadToEnd();
                }
                row = row.Replace("##ProductName##", db.Products.Where(x => x.ProductId == mainProductItem.ProductId).FirstOrDefault().ProductName);
                row = row.Replace("##Quantity##", mainProductItem.Quantity);
                row = row.Replace("##Height##", Convert.ToString(mainProductItem.Height));
                row = row.Replace("##Width##", Convert.ToString(mainProductItem.Width));
                row = row.Replace("##Total##", Convert.ToString(mainProductItem.Price));
                if (_model.OrderItems[i].Items.Count > 1)
                {
                    string rowSubitems = string.Empty;
                    for (int j = 1; j < _model.OrderItems[i].Items.Count; j++)
                    {
                        var subProductItem = _model.OrderItems[i].Items[j];
                        string Subrow = string.Empty;
                        using (StreamReader reader = new StreamReader(Server.MapPath("~/Views/MailTemplate/SubRowtemplate.html")))
                        {
                            Subrow = reader.ReadToEnd();
                        }                       
                        Subrow = Subrow.Replace("##SubProductName##", db.Products.Where(x => x.ProductId == subProductItem.ProductId).FirstOrDefault().ProductName);
                        Subrow = Subrow.Replace("##Subunit##", subProductItem.Unit);
                        rowSubitems += Subrow;
                    }
                    row = row.Replace("##rowSubitem##", rowSubitems);
                }
                rowitems += row;
            }
            body = body.Replace("##rowitem##", rowitems);
            bool IsSendEmail = SendEmail.EmailSend(_email, "Confirm your Quotation", body, true);
            return IsSendEmail;
        }

        [AllowAnonymous]
        public ActionResult ConfirmQuotation(int _OrderID)
        {
            if (_OrderID == 0)
            {
                return View("Error");
            }
            try
            {
                var _data = db.Orders.Where(f => f.Id == _OrderID).FirstOrDefault();
                if (_data != null)
                {
                    _data.StatusId = Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["ConfirmedOrderStatuId"]);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
            return View();
        }
        public ActionResult ConfirmQuotationByAdmin(int id)
        {
            try
            {
                var _data = db.Orders.Where(f => f.Id == id).FirstOrDefault();
                if (_data!=null)
                {
                    _data.StatusId = Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["ConfirmedOrderStatuId"]);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                
            }
            return RedirectToAction("");
        }

        public JsonResult EditsaveOrders(OrderModelNew model, int _orderid)
        {
            if (_orderRepository.RemovelastOrders(Convert.ToString(_orderid)))
            {
                string userEmail = Session["UserEmail"] == null ? User.Identity.Name : Session["UserEmail"].ToString();
                int result = _orderRepository.UpdateOrder(model, userEmail, _orderid);
                if (model.UserItems[0].orderStatus == 1)
                {
                    bool _status = QuotationEmail(_orderid, model.UserItems[0].customerID, model);
                }
                return Json(new { status = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetorderItem(string orderid)
        {
            List<OrderDetail> orderItems = new List<OrderDetail>();
            var CategoryList = db.Categories.ToList();
            using (MordenDoorsEntities context = new MordenDoorsEntities())
            {
                int id = Convert.ToInt32(orderid);

                orderItems = context.OrderItems.Where(s => s.OrderId == id).Select(x => new OrderDetail
                {
                    OrderID = x.OrderId,
                    ProductId = x.ProductId,
                    ProductName = context.Products.Where(q => q.ProductId == x.ProductId).FirstOrDefault().ProductName,
                    Height = x.Height.ToString(),
                    Width = x.Width.ToString(),
                    
                    Quantity = x.Quantity,
                    Description = x.Comments,
                    // workstage = context.OrderWorkStages.Where(s => s.OrderItemId == x.Id).Select(s => s.WorkStageId).ToList(),
                    unitname = (from mm in context.UnitsOfMeasure join p in context.Products on mm.UnitID equals p.UnitsOfMeasureID select mm.UnitName).FirstOrDefault(),
                    unitid = (from mm in context.UnitsOfMeasure join p in context.Products on mm.UnitID equals p.UnitsOfMeasureID select mm.UnitID).FirstOrDefault(),
                    Rate = context.Products.Where(s => s.ProductId == x.ProductId).FirstOrDefault().Rate,
                    Price = context.CustomerPrice.Where(s => s.ProductId == x.ProductId).FirstOrDefault().Price,
                    TotalPrice = x.TotalPrice
                    // cateid = context.Products.Where(s => s.ProductId == x.ProductId).FirstOrDefault().CategoryId

                }).ToList();
            }
            return Json(orderItems, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult getcustomerprice(string customerId, string productId, string categoryId)
        {
            int cust = Convert.ToInt32(customerId);
            int prd = Convert.ToInt32(productId);
            int cat = Convert.ToInt32(categoryId);
            using (MordenDoorsEntities context = new MordenDoorsEntities())
            {
                var res = context.CustomerPrice.Where(s => s.CategoryId == cat && s.ProductId == prd && s.CustomerId == cust).FirstOrDefault();
                if (res == null)
                {
                    return Json("00", JsonRequestBehavior.AllowGet);
                }
                return Json(res.Price, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PaymentStatus(int id)
        {
            var result = _orderRepository.OrderPayments(id);
            return View(result);
        }

        [HttpPost]
        public ActionResult CreatePayment(int orderId)
        {
            var result = _orderRepository.OrderPaymentDetail(orderId);
            return PartialView("_AddPayment", result);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AddPayment(OrderPaymentViewModel model)
        {
            int orderId = model.AddPayment.OrderId;
            model.AddPayment.UpdatedBy = UserDetail.Users(User.Identity.Name.ToString()).Email;
            model.AddPayment.CreatedBy = UserDetail.Users(User.Identity.Name.ToString()).Email;
            bool result = _orderRepository.AddPayment(model);
            TempData["done"] = result;
            return Json(new { result = result, orderId = orderId }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeStatus(int changeStatusValueTo, int? orderId)
        {
            bool result = false;
            using (MordenDoorsEntities context = new MordenDoorsEntities())
            {
                var data = context.Orders.Where(s => s.Id == orderId).FirstOrDefault();
                if (data != null)
                {
                    data.StatusId = changeStatusValueTo;
                    context.SaveChanges();
                    result = true;
                }
            }
            return Json(result);
        }
        [HttpPost]
        public JsonResult GetCustomerdetail(int clientid)
        {
            using (MordenDoorsEntities context = new MordenDoorsEntities())
            {
                var nm = context.Customers.Where(s => s.Id == clientid).FirstOrDefault();
                var taxes = (string.IsNullOrEmpty(nm.DefaultTaxCodeRef) ? null : context.Taxes.Where(s => s.ExternalId == nm.DefaultTaxCodeRef).ToList());
                var tax = new
                {
                    Address1 = nm.ShipAddrLine1,
                    Address2 = nm.ShipAddrLine2,
                    Country = nm.ShipAddrCountry,
                    City = nm.ShipAddrCity,
                    PostalCode = nm.ShipAddrPostalCode,
                    BillAddrLine1 = nm.BillAddrLine1,
                    BillAddrLine2 = nm.BillAddrLine2,
                    BillAddrCountry = nm.BillAddrCountry,
                    BillAddrCity = nm.BillAddrCity,
                    BillAddrPostalCode = nm.BillAddrPostalCode,
                    CreditLimit = nm.CreditLimit,
                    Balance = nm.Balance,
                    taxlist = taxes
                };

                var AllCategories = db.Categories.Where(c => c.IsActive == true).OrderBy(s => s.Sort).OrderByDescending(x => x.IsMain).ToList();
                var categoryList = AllCategories.Select(x => new SelectListItem { Text = x.CategoryName, Value = x.CategoryId.ToString() }).ToList();
                var prodcutlist = (from p in db.Products
                                   where p.IsActive == true
                                   join unt in db.UnitsOfMeasure on p.UnitsOfMeasureID equals unt.UnitID
                                   join cat in db.Categories on p.CategoryId equals cat.CategoryId
                                   let customerPrice = db.CustomerPrice.Where(c => c.CustomerId == clientid && c.ProductId == p.ProductId).FirstOrDefault()
                                   where cat.IsActive == true && p.IsActive == true
                                   select (new productlist { ProductId = p.ProductId, CategoryId = p.CategoryId, Rate = (customerPrice == null ? p.Rate : (p.Rate - ((customerPrice.Price * p.Rate) / 100))), ProductName = p.ProductName, Units = unt.UnitName, UnitId = p.UnitsOfMeasureID, MinSqFt = p.MinimumSquareFeet ?? 0 })).ToList();

                return Json(new { customer = tax, categoryList = categoryList, prodcutlist = prodcutlist }, JsonRequestBehavior.AllowGet);
            }
        }
        //public JsonResult dropdownBind(int clientid)
        //{
        //    using (MordenDoorsEntities context = new MordenDoorsEntities())
        //    {


        //        var nm = context.Customers.Where(s => s.Id == clientid).FirstOrDefault();
        //        var customerPrice = db.CustomerPrice.Where(x => x.ProductId == nm.p && x.CustomerId == nm.Id).FirstOrDefault();


        //        return Json(Dropdown,JsonRequestBehavior.AllowGet)

        //}
        public JsonResult OrderDetail(int id)
        {
            List<OrderViewModel> model = new List<OrderViewModel>();
            try
            {
                var orderComment = db.Orders.Where(s => s.Id == id).FirstOrDefault().Comments;
                model = db.OrderItems.Where(x => x.OrderId == id).Select(OrderDetail => new OrderViewModel
                {
                    OrderId = OrderDetail.OrderId,
                    ProductName = db.Products.Where(x => x.ProductId == OrderDetail.ProductId).FirstOrDefault().ProductName,
                    Height = OrderDetail.Height,
                    Width = OrderDetail.Width,
                    Quantity = OrderDetail.Quantity,
                    Price = OrderDetail.Price,
                    Comments = orderComment == null ? "No Comment" : orderComment,
                    OrderStatus = db.OrderStatus.Where(x => x.Id == OrderDetail.ItemStatus).FirstOrDefault().orderStatus1,
                    StageName = db.WorkStages.Where(x => x.Id == OrderDetail.CurrentStageId).FirstOrDefault().StageName,
                    TrackingID = db.Orders.Where(x => x.Id == OrderDetail.OrderId).FirstOrDefault().TrackingID
                }).ToList();
              }
            catch (Exception ex)
            {

            }
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateOrderStatus(int id)
        {
            List<SelectListItem> workstageList = new List<SelectListItem>() { new SelectListItem() { Value = "0", Text = "Select Work Stage" } };
            workstageList.AddRange(db.WorkStages.Select(z => new SelectListItem { Text = z.StageName, Value = z.Id.ToString() }).ToList());
            List<SelectListItem> selectProduct = new List<SelectListItem>();
            OrderViewModel orderView = new OrderViewModel();
            var products = (from item in db.OrderItems
                            join prod in db.Products on item.ProductId equals prod.ProductId
                            where item.OrderId == id
                            select new SelectListItem { Text = prod.ProductName, Value = item.Id.ToString() }).ToList();
            selectProduct.AddRange(products);
            orderView.Products = selectProduct;
            orderView.OrdersWorkStages = workstageList;
            return PartialView("_UpdateOrderStatus", orderView);
        }
        public JsonResult GetOrderDataByOrderId(int orderId)
        {
            decimal quantity = 0;
            try
            {
                quantity = db.OrderItems.Where(x => x.OrderId == orderId).Sum(q => q.Quantity);
            }
            catch (Exception ex)
            {

            }
            return Json(new { quantity = quantity }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployee(int Id)
        {
            var model = (from u in db.AspNetUsers
                         join e in db.EmployeeSkills on u.Id equals e.UserId
                         where e.SkillId == Id && u.status == true
                         select (new UserInRoleViewModel
                         {
                             UserId = u.Id,
                             Username = u.FirstName
                         })).ToList();
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddOperation(int orderNo, int stageId, int totalQuantity, int itemId)
        {
            int result = 0;
            try
            {
                result = _orderRepository.AddOperation(orderNo, stageId, totalQuantity, itemId);
            }
            catch (Exception ex)
            {

            }
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult OperationList(int orderNo)
        {
            List<OperationsViewModel> model = new List<OperationsViewModel>();
            try
            {
                model = _orderRepository.OperationsByProductId(orderNo);
            }
            catch (Exception ex)
            {

            }
            return Json(new { operationList = model }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult OperationsByProductId(int itemId)
        {
            List<OperationsViewModel> model = new List<OperationsViewModel>();
            try
            {
                model = _orderRepository.OperationsByProductId(itemId);
            }
            catch (Exception ex)
            {

            }
            return Json(new { operationList = model }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateOperation(int operationID, int orderNo, int stageID, string empId, bool isComplete, int qytDone, string location)
        {
            bool result = false;
            try
            {
                result = _orderRepository.UpdateOperation(operationID, orderNo, stageID, empId, isComplete, qytDone, location);
            }
            catch (Exception ex)
            {

            }
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Taxes()
        {
            var result = _orderRepository.GetTaxes();
            return View(result);
        }

        public ActionResult GetCategoryProductDetail(int clientid)
        {
            try
            {
                OrderViewModel model = new OrderViewModel();
                model = _orderRepository.OrderRelatedDetail(clientid);
                return PartialView("_BindproductInOrder", model);
              //  return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult AssignEmpToOperation(string empId, int operationId)
        {
            bool result = false;
            using (MordenDoorsEntities context = new MordenDoorsEntities())
            {
                var data = context.Operations.Where(s => s.Id == operationId).FirstOrDefault();
                if (data != null)
                {
                    data.EmployeeId = empId;
                    context.SaveChanges();
                    result = true;
                }
            }
            return Json(result);
        }
        [HttpPost]
        public JsonResult UpdateOperationSorting(List<int> oprIds, int orderItemId)
        {
            bool result = false;
            using (MordenDoorsEntities context = new MordenDoorsEntities())
            {
                var data = context.Operations.Where(s => s.OrderItem== orderItemId && oprIds.Contains(s.Id)).ToList();
                data.ForEach(x => x.Sort = Convert.ToByte(oprIds.FindIndex(y => y == x.Id)));

                var orderItem = context.OrderItems.Where(x => x.Id == orderItemId).FirstOrDefault();
                if (orderItem != null)
                {
                    var op = data.Where(x => x.Sort == 0).FirstOrDefault();
                    orderItem.CurrentStageId = op.WorkcentreId;
                    orderItem.AssigedTo = op.EmployeeId;
                }
                context.SaveChanges();
            }
            return Json(result);
        }
    }
}


