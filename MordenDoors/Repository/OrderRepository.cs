using MordenDoors.Database;
using MordenDoors.Models;
using MordenDoors.Models.Customers;
using MordenDoors.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static MordenDoors.Models.OrderModel;

namespace MordenDoors.Repository
{
    public class OrderRepository
    {
        public Nullable<DateTime> d { get; set; }
        private readonly MordenDoorsEntities _context;
        public OrderRepository()
        {
            _context = new MordenDoorsEntities();
        }

        public int FetchLastAddedOrder()
        {
            return _context.Orders.OrderByDescending(o => o.Id).FirstOrDefault().Id;
        }
        public bool RemovelastOrders(string orderid)
        {
            int ordid = Convert.ToInt32(orderid);
            using (var context = new MordenDoorsEntities())
            {
                try
                {
                    // context.Database.ExecuteSqlCommand("delete  from orders where Id="+ ordid + "");
                    //var orditems = context.OrderItems.Where(s => s.OrderId == ordid).ToList();
                    //foreach (var itm in orditems)
                    //{
                    //    context.Database.ExecuteSqlCommand("delete  from OrderWorkStages where WorkStageId=" + itm.Id + "");
                    //}
                    context.Database.ExecuteSqlCommand("delete from Operations where OrderId=" + ordid);
                    context.Database.ExecuteSqlCommand("delete from OrderItems where OrderId=" + ordid);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }
        public int UpdateOrder(OrderModelNew model, string userEmail,int _orderid)
        {
            int orderId = 0; string userId = string.Empty;
            if (!string.IsNullOrEmpty(userEmail))
            {
                userId = _context.AspNetUsers.Where(u => u.Email.Equals(userEmail)).FirstOrDefault().Id;
            }

            List<OrderItems> orderItemList = new List<OrderItems>();
            //for storing order item in seperate table
            List<OrderItemList> orderItem = new List<OrderItemList>();

            using (var orderTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Inserting Orders
                    var ord = _context.Orders.Where(s => s.Id == _orderid).FirstOrDefault();

                    ord.CustomerId = model.UserItems[0].customerID;
                    // ord.TotalAmount = model.OrderItems.Sum(x => x.TotalPrice);
                    ord.Comments = model.UserItems[0].orderComment;
                    ord.StatusId = model.UserItems[0].orderStatus;
                    ord.DeliveryTime = model.UserItems[0].OrderDDate;
                    ord.PaymentComplete = false;
                    // ord.CretaedOn = DateTime.Now;
                    ord.UpdatedOn = DateTime.Now;
                    //ord.CreatedBy = userId;
                    ord.UpdatedBy = userId;
                    // ord.InvoiceId = "Invoice- " + Guid.NewGuid().ToString();
                    // ord.TrackingID = Guid.NewGuid().ToString();
                    ord.AddressLine1 = model.UserItems[0].Address1;
                    ord.AddressLine2 = model.UserItems[0].Address2;
                    ord.Country = model.UserItems[0].Country;
                    ord.City = model.UserItems[0].City;
                    ord.PinCode = model.UserItems[0].Pincode;
                    ord.PO = model.UserItems[0].PO;
                    ord.Job = model.UserItems[0].Job;
                    ord.customerprice = Convert.ToDecimal(model.UserItems[0].CustomePrice);
                    ord.PayableAmount = Convert.ToDecimal(model.UserItems[0].Payableamount);
                    ord.TotalAmount = Convert.ToDecimal(model.UserItems[0].Totalamount);
                    ord.GST = Convert.ToDecimal(model.UserItems[0].GST);
                    ord.PST = Convert.ToDecimal(model.UserItems[0].PST);

                    _context.SaveChanges();
                    orderId = _orderid;

                    List<OrderItemWorkStages> productWorkStages = new List<OrderItemWorkStages>();
                    // Inserting OrderItems
                    for (int i = 0; i < model.OrderItems.Count(); i++)
                    {
                        var mainProductItem = model.OrderItems[i].Items[0];
                        int productId = mainProductItem.ProductId;
                        var worksatagelist = _context.ProductWorkstages.Where(s => s.ProductId == productId).ToList();
                        productWorkStages.Add(new OrderItemWorkStages() { ProductId = productId, OrderItemWorkstages = worksatagelist });
                        List<OrderSubItems> orderSubItems = new List<OrderSubItems>();
                        if (model.OrderItems[i].Items.Count > 1)
                        {
                            for (int j = 1; j < model.OrderItems[i].Items.Count; j++)
                            {
                                var subProductItem = model.OrderItems[i].Items[j];
                                var orderSubItem = new OrderSubItems()
                                {
                                    ProductId = subProductItem.ProductId,
                                    Price = subProductItem.Price,
                                    Height = Convert.ToDecimal(mainProductItem.Height),
                                    Width = Convert.ToDecimal(mainProductItem.Width),
                                    Quantity = Convert.ToDecimal(mainProductItem.Quantity),
                                    TotalPrice = mainProductItem.SubTotal,
                                    TotalWeight = mainProductItem.TotalWeight,
                                    UnitType = mainProductItem.selectedUnit,
                                    Unit = mainProductItem.Unit
                                };
                                orderSubItems.Add(orderSubItem);
                            }
                        }

                        var orderItems = new OrderItems()
                        {
                            OrderId = orderId,
                            ProductId = productId,
                            Price = mainProductItem.Price,
                            Height = Convert.ToDecimal(mainProductItem.Height),
                            Width = Convert.ToDecimal(mainProductItem.Width),
                            Quantity = Convert.ToDecimal(mainProductItem.Quantity),
                            Comments = !string.IsNullOrEmpty(mainProductItem.ItemComment) ? mainProductItem.ItemComment : string.Empty,
                            CurrentStageId = worksatagelist.Count != 0 ? Convert.ToInt32(worksatagelist[0].RoleId) : 0,
                            ItemStatus = Convert.ToInt16(OrderItemStatus.InProgress),
                            TotalPrice = mainProductItem.SubTotal,
                            TotalWeight = mainProductItem.TotalWeight,
                            UnitType = mainProductItem.selectedUnit,
                            Unit = mainProductItem.Unit,
                            OrderSubItems = orderSubItems
                        };
                        orderItemList.Add(orderItems);
                    }
                    _context.OrderItems.AddRange(orderItemList);
                    _context.SaveChanges();

                    //Inserting in orderitemList table to seperate data
                    for (int i = 0; i < orderItemList.Count(); i++)
                    {
                        int orderItemId = orderItemList[i].Id;
                        var productId = orderItemList[i].ProductId;
                        var workStages = productWorkStages.Where(s => s.ProductId == productId).FirstOrDefault();
                        if (workStages != null && workStages.OrderItemWorkstages != null && workStages.OrderItemWorkstages.Count > 0)
                        {
                            foreach (ProductWorkstages workStage in workStages.OrderItemWorkstages)
                            {
                                //Save in operation table
                                var obj = new Operations()
                                {
                                    OrderId = orderId,
                                    WorkcentreId = Convert.ToInt32(workStage.RoleId),
                                    OrderItem = orderItemId,
                                    Sort = workStage.Sort
                                    //TotalQty = Convert.ToInt32(model.OrderItems[i].Quantity)
                                };
                                _context.Operations.Add(obj);
                            }
                        }
                    }
                    _context.SaveChanges();
                    orderTransaction.Commit();
                }
                catch (Exception ex)
                {
                    orderTransaction.Rollback();
                }
            }

            return orderId;
        }
        public bool updateOrders2(OrderModelNew model, string userEmail, int _Oid)
        {
            bool result = false; string userId = string.Empty;
            int ordid = Convert.ToInt32(_Oid);
            if (!string.IsNullOrEmpty(userEmail))
            {
                userId = _context.AspNetUsers.Where(u => u.Email.Equals(userEmail)).FirstOrDefault().Id;
            }
            List<OrderItems> orderItemList = new List<OrderItems>();

            using (var orderTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Inserting Orders
                    var ord = _context.Orders.Where(s => s.Id == ordid).FirstOrDefault();

                    ord.CustomerId = model.UserItems[0].customerID;
                    // ord.TotalAmount = model.OrderItems.Sum(x => x.TotalPrice);
                    ord.Comments = model.UserItems[0].orderComment;
                    ord.StatusId = model.UserItems[0].orderStatus;
                    ord.DeliveryTime = model.UserItems[0].OrderDDate;
                    ord.PaymentComplete = false;
                    // ord.CretaedOn = DateTime.Now;
                    ord.UpdatedOn = DateTime.Now;
                    //ord.CreatedBy = userId;
                    ord.UpdatedBy = userId;
                    // ord.InvoiceId = "Invoice- " + Guid.NewGuid().ToString();
                    // ord.TrackingID = Guid.NewGuid().ToString();
                    ord.AddressLine1 = model.UserItems[0].Address1;
                    ord.AddressLine2 = model.UserItems[0].Address2;
                    ord.Country = model.UserItems[0].Country;
                    ord.City = model.UserItems[0].City;
                    ord.PinCode = model.UserItems[0].Pincode;
                    ord.PO = model.UserItems[0].PO;
                    ord.Job = model.UserItems[0].Job;
                    ord.customerprice = Convert.ToDecimal(model.UserItems[0].CustomePrice);
                    ord.PayableAmount = Convert.ToDecimal(model.UserItems[0].Payableamount);
                    ord.TotalAmount = Convert.ToDecimal(model.UserItems[0].Totalamount);
                    ord.GST = Convert.ToDecimal(model.UserItems[0].GST);
                    ord.PST = Convert.ToDecimal(model.UserItems[0].PST);

                    _context.SaveChanges();

                    // Inserting OrderItems
                    for (int i = 0; i < model.OrderItems.Count(); i++)
                    {
                      var itme = model.OrderItems[i].Items[0];
                        {
                            int productId = itme.ProductId;
                            var worksatagelist = _context.ProductWorkstages.Where(s => s.ProductId == productId).ToList();

                            OrderItems orderItems = new OrderItems()
                            {
                                OrderId = ordid,
                                ProductId = itme.ProductId,
                                Price = itme.Price,
                                Height = Convert.ToDecimal(itme.Height),
                                Width = Convert.ToDecimal(itme.Width),
                                Quantity = Convert.ToDecimal(itme.Quantity),
                                Comments = !string.IsNullOrEmpty(itme.ItemComment) ? itme.ItemComment : string.Empty,
                                CurrentStageId = worksatagelist.Count != 0 ? Convert.ToInt32(worksatagelist[0].RoleId) : 0,
                                ItemStatus = Convert.ToInt16(OrderItemStatus.InProgress),
                                TotalPrice = itme.SubTotal
                            };
                            orderItemList.Add(orderItems);
                            string itemList = itme.ItemComment;
                            //string[] words = itemList.Split(',');
                        }
                                      
                      
                    }
                    _context.OrderItems.AddRange(orderItemList);
                    _context.SaveChanges();

                    // Inserting OrderWorkStages
                    for (int i = 0; i < orderItemList.Count(); i++)
                    {

                        int orderItemId = orderItemList[i].Id;
                        int productId = orderItemList[i].ProductId;
                        // var workStage = model.OrderItems[i].workstage;
                    }

                    result = true;
                    orderTransaction.Commit();
                }
                catch (Exception ex)
                {
                    orderTransaction.Rollback();
                }
            }

            return result;
        }

        public int GetOrderStatusIdbyName(string status)
        {
            var orderStatus = _context.OrderStatus.Where(x => x.orderStatus1 == status).FirstOrDefault();
            
            if (orderStatus != null)
            {
                return orderStatus.Id;
            }
            return 0;
        }

        public OrderViewModel orderListAccordingToStatus(int? statusId=0)
        {
            var quoteStatusId = Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["QuoteStatusId"]);
            var confirmedOrderStatuId = Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["ConfirmedOrderStatuId"]);

            List<SelectListItem> statusItems = new List<SelectListItem>() { new SelectListItem() { Value = "0", Text = "All Status" } };
            statusItems.AddRange(_context.OrderStatus.Where(a=>a.Status==true).OrderBy(o=>o.sortKey).Select(z => new SelectListItem { Text = z.orderStatus1, Value = z.Id.ToString(), Selected = z.Id==statusId }).ToList());
            statusId = statusId == null ? 0 : statusId;
            try
            {
                {
                    if (statusId != 0)
                    {
                        OrderViewModel viewModel = new OrderViewModel()
                        {
                            Orders = _context.Orders.Where(o => o.StatusId == statusId).Select(x => new OrdersModel
                            {
                                Id = x.Id,
                                CustomerId = x.CustomerId,
                                TotalAmount = x.TotalAmount,
                                StatusId = x.StatusId,
                                DeliveryTime = x.DeliveryTime,
                                CustomerName = _context.Customers.Where(o => o.Id == x.CustomerId).Select(s => s.CompanyName).FirstOrDefault(),
                                CurrentStatus = x.StatusId,
                                PendingAmount = _context.OrderPayments.Where(s => s.OrderId == x.Id).OrderByDescending(s => s.ID).FirstOrDefault().PendingAmount,
                                CretaedOn = x.CretaedOn,
                                ItemQuantity = _context.OrderItems.Where(s => s.OrderId == x.Id).Sum(s => s.Quantity)
                            }).OrderByDescending(s => s.Id).ToList(),
                            OrdersStatus = statusItems
                        };
                        return viewModel;
                    }
                    else
                    {
                        OrderViewModel viewModel = new OrderViewModel()
                        {
                            Orders = _context.Orders.Select(x => new OrdersModel
                            {
                                Id = x.Id,
                                CustomerId = x.CustomerId,
                                TotalAmount = x.TotalAmount,
                                StatusId = x.StatusId,
                                DeliveryTime = x.DeliveryTime,
                                CustomerName = _context.Customers.Where(o => o.Id == x.CustomerId).Select(s => s.CompanyName).FirstOrDefault(),
                                CurrentStatus = x.StatusId,
                                PendingAmount = _context.OrderPayments.Where(s => s.OrderId == x.Id).OrderByDescending(s => s.ID).FirstOrDefault().PendingAmount,
                                CretaedOn = x.CretaedOn,
                                ItemQuantity = _context.OrderItems.Where(s => s.OrderId == x.Id).Sum(s => s.Quantity)
                            }).OrderByDescending(s => s.Id).ToList(),
                            OrdersStatus = statusItems
                        };
                        return viewModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CreateOrders(OrderModel model, string userEmail)
        {
            bool result = false; string userId = string.Empty;
            if (!string.IsNullOrEmpty(userEmail))
            {
                userId = _context.AspNetUsers.Where(u => u.Email.Equals(userEmail)).FirstOrDefault().Id;
            }

            List<OrderItems> orderItemList = new List<OrderItems>();
            //for storing order item in seperate table
            List<OrderItemList> orderItem = new List<OrderItemList>();

            using (var orderTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Inserting Orders
                    Orders Addorder = new Orders()
                    {
                        CustomerId = model.UserItems[0].customerID,
                        // TotalAmount = model.OrderItems.Sum(x => x.TotalPrice),
                        Comments = model.UserItems[0].orderComment,
                        StatusId = model.UserItems[0].orderStatus,
                        DeliveryTime = model.UserItems[0].OrderDDate,
                        PaymentComplete = false,
                        CretaedOn = DateTime.Now,
                        UpdatedOn = DateTime.Now,
                        CreatedBy = userId,
                        UpdatedBy = userId,
                        InvoiceId = "Invoice- " + Guid.NewGuid().ToString(),
                        TrackingID = Guid.NewGuid().ToString(),
                        AddressLine1 = model.UserItems[0].Address1,
                        AddressLine2 = model.UserItems[0].Address2,
                        Country = model.UserItems[0].Country,
                        City = model.UserItems[0].City,
                        PinCode = model.UserItems[0].Pincode,
                        Job = model.UserItems[0].Job,
                        PO = model.UserItems[0].PO,
                        customerprice = Convert.ToDecimal(model.UserItems[0].CustomePrice),
                        PayableAmount = Convert.ToDecimal(model.UserItems[0].Payableamount),
                        TotalAmount = Convert.ToDecimal(model.UserItems[0].Totalamount),
                        GST = model.UserItems[0].GST,
                        PST = model.UserItems[0].PST
                    };
                    _context.Orders.Add(Addorder);
                    _context.SaveChanges();

                       // List<OrderItemWorkStages> productWorkStages = new List<OrderItemWorkStages>();
                    // Inserting OrderItems
                    for (int i = 0; i < model.OrderItems.Count(); i++)
                    {
                        int productId = model.OrderItems[i].ProductId;
                        var worksatagelist = _context.ProductWorkstages.Where(s => s.ProductId == productId).ToList();
                       // productWorkStages.Add(new OrderItemWorkStages() { ProductId = productId, OrderItemWorkstages = worksatagelist });
                        int orderId = Addorder.Id;
                        // _context.Orders.OrderByDescending(x => x.Id).FirstOrDefault().Id;
                        if(worksatagelist.Count()>0)
                        {
                            foreach(var items in worksatagelist)
                            {
                                Operations obj = new Operations()
                                {
                                    OrderId = orderId,
                                    WorkcentreId = items.Id,
                                    TotalQty = Convert.ToInt32(model.OrderItems[i].Quantity)
                                };
                               _context.Operations.Add(obj);
                            


                            }
                             
                        }
                        //Orders order = _context.Orders.Where(o => o.Id == orderId).FirstOrDefault();

                        OrderItems orderItems = new OrderItems()
                        {
                            OrderId = orderId,
                            ProductId = model.OrderItems[i].ProductId,
                            Price = model.OrderItems[i].Price,
                            Height = Convert.ToDecimal(model.OrderItems[i].Height),
                            Width = Convert.ToDecimal(model.OrderItems[i].Width),
                            Quantity = Convert.ToDecimal(model.OrderItems[i].Quantity),
                            Comments = !string.IsNullOrEmpty(model.OrderItems[i].ItemComment) ? model.OrderItems[i].ItemComment : string.Empty,
                            CurrentStageId = worksatagelist.Count != 0 ? Convert.ToInt32(worksatagelist[0].RoleId) : 0,
                            ItemStatus = Convert.ToInt16(OrderItemStatus.InProgress),
                            TotalPrice = model.OrderItems[i].TotalPrice,
                            TotalWeight = model.OrderItems[i].TotalWeight,
                            UnitType=model.OrderItems[i].UnitType,
                            Unit=model.OrderItems[i].Unit,
                        };
                        orderItemList.Add(orderItems);
                    }
                    _context.OrderItems.AddRange(orderItemList);
                    _context.SaveChanges();

                    ////Inserting in orderitemList table to seperate data
                    //for (int i = 0; i < orderItemList.Count(); i++)
                    //{
                    //    int orderItemId = orderItemList[i].Id;
                    //    string orderDescription = orderItemList[i].Comments;
                    //    string trimmedData = orderDescription.TrimEnd(',');
                    //    string[] data = trimmedData.Split(',');
                    //    foreach (var sub in data)
                    //    {
                    //        OrderItemList ilm = new OrderItemList
                    //        {
                    //            OrderItemId = orderItemId,
                    //            Category = sub.Split('(')[0],
                    //            Product = sub.TrimEnd(')').Split('(')[1]
                    //        };
                    //        orderItem.Add(ilm);                            
                    //    }
                    //    _context.OrderItemList.AddRange(orderItem);
                    //    _context.SaveChanges();

                    //    var productId = orderItemList[i].ProductId;
                    //    var orderId = orderItemList[i].OrderId;
                    ////    var workStages = productWorkStages.Where(s => s.ProductId == productId).FirstOrDefault();
                    //  //  if (workStages != null && workStages.OrderItemWorkstages != null && workStages.OrderItemWorkstages.Count > 0)
                    //    {
                    //      //  foreach (ProductWorkstages workStage in workStages.OrderItemWorkstages)
                    //     //   {
                    //            //Save in operation table
                    //       // }
                    //    }
                    //}

                    result = true;
                    orderTransaction.Commit();
                }
                catch (Exception ex)
                {
                    orderTransaction.Rollback();
                }
            }

            return result;
        }
        public int SaveOrder(OrderModelNew model, string userEmail)
        {
            int orderId = 0; string userId = string.Empty;
            if (!string.IsNullOrEmpty(userEmail))
            {
                userId = _context.AspNetUsers.Where(u => u.Email.Equals(userEmail)).FirstOrDefault().Id;
            }

            List<OrderItems> orderItemList = new List<OrderItems>();
            //for storing order item in seperate table
            List<OrderItemList> orderItem = new List<OrderItemList>();

            using (var orderTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Inserting Orders
                    Orders Addorder = new Orders()
                    {
                        CustomerId = model.UserItems[0].customerID,
                        // TotalAmount = model.OrderItems.Sum(x => x.TotalPrice),
                        Comments = model.UserItems[0].orderComment,
                        StatusId = model.UserItems[0].orderStatus,
                        DeliveryTime = model.UserItems[0].OrderDDate,
                        PaymentComplete = false,
                        CretaedOn = DateTime.Now,
                        UpdatedOn = DateTime.Now,
                        CreatedBy = userId,
                        UpdatedBy = userId,
                        InvoiceId = "Invoice- " + Guid.NewGuid().ToString(),
                        TrackingID = Guid.NewGuid().ToString(),
                        AddressLine1 = model.UserItems[0].Address1,
                        AddressLine2 = model.UserItems[0].Address2,
                        Country = model.UserItems[0].Country,
                        City = model.UserItems[0].City,
                        PinCode = model.UserItems[0].Pincode,
                        Job = model.UserItems[0].Job,
                        PO = model.UserItems[0].PO,
                        customerprice = Convert.ToDecimal(model.UserItems[0].CustomePrice),
                        PayableAmount = Convert.ToDecimal(model.UserItems[0].Payableamount),
                        TotalAmount = Convert.ToDecimal(model.UserItems[0].Totalamount),
                        GST = model.UserItems[0].GST,
                        PST = model.UserItems[0].PST
                    };
                    _context.Orders.Add(Addorder);
                    _context.SaveChanges(); 
                    orderId = Addorder.Id;

                    List<OrderItemWorkStages> productWorkStages = new List<OrderItemWorkStages>();
                    // Inserting OrderItems
                    for (int i = 0; i < model.OrderItems.Count(); i++)
                    {
                        var mainProductItem = model.OrderItems[i].Items[0];
                        int productId = mainProductItem.ProductId;
                        var worksatagelist = _context.ProductWorkstages.Where(s => s.ProductId == productId).ToList();
                        productWorkStages.Add(new OrderItemWorkStages() { ProductId = productId, OrderItemWorkstages = worksatagelist });
                        List<OrderSubItems> orderSubItems = null;
                        if (model.OrderItems[i].Items.Count > 1)
                        {
                            orderSubItems = new List<OrderSubItems>();
                            for (int j = 1; j < model.OrderItems[i].Items.Count; j++)
                            {
                                var subProductItem = model.OrderItems[i].Items[j];
                                var orderSubItem = new OrderSubItems()
                                {
                                    ProductId = subProductItem.ProductId,
                                    Price = subProductItem.Price,
                                    Height = Convert.ToDecimal(mainProductItem.Height),
                                    Width = Convert.ToDecimal(mainProductItem.Width),
                                    Quantity = Convert.ToDecimal(mainProductItem.Quantity),
                                    TotalPrice = mainProductItem.SubTotal,
                                    TotalWeight = mainProductItem.TotalWeight,
                                    UnitType = mainProductItem.selectedUnit,
                                    Unit = mainProductItem.Unit
                                };
                                orderSubItems.Add(orderSubItem);
                            }
                        }

                        var orderItems = new OrderItems()
                        {
                            OrderId = orderId,
                            ProductId = productId,
                            Price = mainProductItem.Price,
                            Height = Convert.ToDecimal(mainProductItem.Height),
                            Width = Convert.ToDecimal(mainProductItem.Width),
                            Quantity = Convert.ToDecimal(mainProductItem.Quantity),
                            Comments = !string.IsNullOrEmpty(mainProductItem.ItemComment) ? mainProductItem.ItemComment : string.Empty,
                            CurrentStageId = worksatagelist.Count != 0 ? Convert.ToInt32(worksatagelist[0].RoleId) : 0,
                            ItemStatus = Convert.ToInt16(OrderItemStatus.InProgress),
                            TotalPrice = mainProductItem.SubTotal,
                            TotalWeight = mainProductItem.TotalWeight,
                            UnitType = mainProductItem.selectedUnit,
                            Unit = mainProductItem.Unit,
                            OrderSubItems = orderSubItems
                        };
                        orderItemList.Add(orderItems);
                    }
                    _context.OrderItems.AddRange(orderItemList);
                    _context.SaveChanges();

                    //Inserting in orderitemList table to seperate data
                    for (int i = 0; i < orderItemList.Count(); i++)
                    {
                        int orderItemId = orderItemList[i].Id;
                        var productId = orderItemList[i].ProductId;
                        var workStages = productWorkStages.Where(s => s.ProductId == productId).FirstOrDefault();
                        if (workStages != null && workStages.OrderItemWorkstages != null && workStages.OrderItemWorkstages.Count > 0)
                        {
                            foreach (ProductWorkstages workStage in workStages.OrderItemWorkstages)
                            {
                                //Save in operation table
                                var obj = new Operations()
                                {
                                    OrderId = orderId,
                                    WorkcentreId = Convert.ToInt32(workStage.RoleId),
                                    OrderItem = orderItemId,
                                    Sort = workStage.Sort
                                    //TotalQty = Convert.ToInt32(model.OrderItems[i].Quantity)
                                };
                                _context.Operations.Add(obj);
                            }
                        }
                    }
                    _context.SaveChanges();
                    orderTransaction.Commit();
                }
                catch (Exception ex)
                {
                    orderTransaction.Rollback();
                }
            }

            return orderId;
        }

        public OrderViewModel OrderRelatedDetail(int customerId)
        {
            OrderViewModel model = new OrderViewModel
            {
                CustomerList = _context.Customers.Select(x => new SelectListItem { Text = x.CompanyName, Value = x.Id.ToString() }).ToList(),
                UnitList = _context.UnitsOfMeasure.Select(x => new SelectListItem { Text = x.UnitDescription, Value = x.UnitID.ToString() }).ToList(),
                workStages = _context.WorkStages.ToList(),
                Prodcutlist = (from p in _context.Products
                               join unt in _context.UnitsOfMeasure on p.UnitsOfMeasureID equals unt.UnitID
                               join cat in _context.Categories on p.CategoryId equals cat.CategoryId
                               join cp in _context.CustomerPrice on new { a = p.ProductId, b = customerId } equals new { a = cp.ProductId, b = cp.CustomerId } into cprice
                               from Price in cprice.DefaultIfEmpty()
                               where cat.IsActive == true && p.IsActive == true
                               select (new productlist
                               {
                                   ProductId = p.ProductId,
                                   CategoryId = p.CategoryId,
                                   Rate = Price == null ? p.Rate : p.Rate - ((Price.Price * p.Rate) / 100),
                                   ProductName = p.ProductName,
                                   Units = unt.UnitName,
                                   UnitId = p.UnitsOfMeasureID
                               })).ToList()

            };
            var priceList = _context.CustomerPrice.ToList();
            var categoryList = _context.Categories.Where(s => s.IsActive == true).ToList();
            var list = categoryList.Where(s => s.IsMain == false).OrderBy(s => s.ShortingNo).ToList();
            var ismain = categoryList.Where(s => s.IsMain == true).FirstOrDefault();
            list.Insert(0, ismain);
            model.CategoryList = list.Select(x => new SelectListItem { Text = x.CategoryName, Value = x.CategoryId.ToString() }).ToList();
            return model;
        }

        public OrderPaymentViewModel OrderPayments(int orderId)
        {
            OrderPaymentViewModel model = new OrderPaymentViewModel();
            var result = _context.OrderPayments.Where(x => x.OrderId == orderId).OrderBy(x => x.ID).Select(x => new OrderPaymentModel
            {
                Comments = x.Comments,
                OrderId = x.OrderId,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                Payment = x.Payment,
                PaymentMode = x.PaymentMode,
                PendingAmount = x.PendingAmount,
                UpdatedBy = _context.AspNetUsers.Where(y => y.Email.Equals(x.UpdatedBy)).FirstOrDefault().FirstName,
                UpdatedOn = x.UpdatedOn
            }).ToList();
            model.OrderPayment = result;
            model.OrderId = orderId;
            model.OrderTotalAmount = _context.Orders.Where(z => z.Id == orderId).FirstOrDefault().TotalAmount;
            return model;
        }

        public OrderModel EditOrders(int id)
        {
            OrderModel model = new OrderModel();

            model.UserItems = _context.Orders.Where(o => o.Id == id).Select(order => new UserItemModel
            {
                customerID = order.CustomerId,
                orderStatus = order.StatusId,
                OrderDDate = order.DeliveryTime,
                orderComment = order.Comments,
                Address1 = order.AddressLine1,
                Address2 = order.AddressLine2,
                Country = order.Country,
                City = order.City,
                Pincode = order.PinCode
            }).ToList();

            model.OrderItems = _context.sp_SelectAllCustomers(id).Select(sp => new OrderItemModel
            {
                SelectedCategoryText = sp.CategoryName,
                SelectedCategoryValue = sp.CategoryId,
                SelectedProductText = sp.ProductName,
                SelectedProductValue = sp.ProductId,
                Quantity = Convert.ToString(sp.Quantity),
                Height = Convert.ToString(sp.Height),
                Width = Convert.ToString(sp.Width),
                Price = sp.Price,
                CustomPrice = sp.Price,
                TotalPrice = Convert.ToDecimal(sp.TotalPrice),
                ItemComment = sp.ItemStatus.ToString()
            }).ToList();

            return model;
        }

        public OrderPaymentViewModel OrderPaymentDetail(int orderId)
        {
            OrderPaymentViewModel model = new OrderPaymentViewModel();
            var totalAmount = _context.Orders.Where(z => z.Id == orderId).FirstOrDefault().TotalAmount;
            var result = _context.OrderPayments.Where(x => x.OrderId == orderId).OrderByDescending(x => x.ID).Select(x => new OrderPaymentModel
            {
                OrderId = orderId,
                OrderTotalAmount = totalAmount,
                PaymentMode = x.PaymentMode,
                PendingAmount = x.PendingAmount
            }).FirstOrDefault();
            if (result == null)
            {
                OrderPaymentModel pendingAmount = new OrderPaymentModel();
                pendingAmount.PendingAmount = totalAmount;
                pendingAmount.OrderId = orderId;
                pendingAmount.OrderTotalAmount = totalAmount;
                model.AddPayment = pendingAmount;
            }
            else
                model.AddPayment = result;
            return model;
        }

        public bool AddPayment(OrderPaymentViewModel model)
        {
            OrderPayments addPayments = new OrderPayments();
            addPayments.OrderId = model.AddPayment.OrderId;
            addPayments.Payment = model.AddPayment.Payment;
            addPayments.PaymentMode = model.AddPayment.PaymentMode;
            addPayments.PendingAmount = model.AddPayment.PendingAmount - model.AddPayment.Payment;
            addPayments.UpdatedBy = model.AddPayment.UpdatedBy;
            addPayments.CreatedBy = model.AddPayment.CreatedBy;
            addPayments.UpdatedOn = DateTime.Now;
            addPayments.CreatedOn = DateTime.Now;
            addPayments.Comments = model.AddPayment.Comments;
            _context.OrderPayments.Add(addPayments);
            var save = _context.SaveChanges();
            if (save > 0)
                return true;
            else
                return false;
        }
        public int AddOperation(int orderno, int stageID, int totalQuantity, int itemId)
        {
            Operations operation = new Operations();
            operation.OrderId = orderno;
            operation.WorkcentreId = stageID;
            operation.TotalQty = totalQuantity;
            operation.OrderItem = itemId;
            operation.ReleasedDate = DateTime.Now;
            var oprations = _context.Operations.Where(x => x.OrderItem == itemId);
            var max_value = oprations.Where(x => x.Sort != null).Select(x => x.Sort).Max();
            if (max_value.HasValue)
                operation.Sort = Convert.ToByte(max_value.Value + 1);
            else
                operation.Sort = 0;// Convert.ToByte(0);
            _context.Operations.Add(operation);
            if (oprations.Where(x => x.IsComplete == false).Count() == 0) {
                var orderItem = _context.OrderItems.Where(x => x.Id == itemId).FirstOrDefault();
                if (orderItem != null) {
                    orderItem.CurrentStageId = stageID;
                }
            }
            var order = _context.Orders.Where(x=>x.Id== orderno).FirstOrDefault();
            if (order != null)
            {
                order.StatusId = 2;
            }
            var save = _context.SaveChanges();
            if (save > 0)
                return _context.Operations.OrderByDescending(x => x.Id).FirstOrDefault().Id;
            else
                return 0;
        }
        public bool UpdateOperation(int operationID, int orderno, int stageID, string empId, bool isComplete, int qytDone, string location )
        {
            Operations operation = new Operations();
            using (var orderTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var op1 = _context.Operations.Where(o => o.OrderId == orderno);
                    var op = op1.Where(o => o.Id == operationID && o.WorkcentreId == stageID).FirstOrDefault();
                    if (op != null)
                    {
                        var orderItem = _context.OrderItems.Where(x => x.Id == op.OrderItem).FirstOrDefault();
                        if (isComplete)
                        {
                            op.IsComplete = true;
                            op.QtyDone = qytDone;
                            op.FinishTime = DateTime.Now;
                            op.Location = location;
                            if (orderItem != null)
                            {
                                var nextSatge = op1.Where(x => x.OrderItem==op.OrderItem && x.Sort == op.Sort + 1).FirstOrDefault();
                                if (nextSatge != null)
                                {
                                    orderItem.AssigedTo = nextSatge.EmployeeId;
                                    orderItem.CurrentStageId = nextSatge.WorkcentreId;
                                }
                                else if (op1.ToList().All(x => x.IsComplete == true))
                                {
                                    var order = _context.Orders.Where(x => x.Id == orderItem.OrderId).FirstOrDefault();
                                    if (order != null)
                                    {
                                        order.StatusId = 5;
                                    }
                                }
                            }
                        }
                        else
                        {
                            op.StartTime = DateTime.Now;
                            op.EmployeeId = empId;
                            op.IsComplete = false;
                            if (orderItem != null)
                            {
                                orderItem.AssigedTo = empId;
                                orderItem.CurrentStageId = stageID;
                            }
                        }
                        var save = _context.SaveChanges();
                        orderTransaction.Commit();
                        if (save > 0)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                    
                }
                catch (Exception ex)
                {
                    orderTransaction.Rollback();
                }
            }
            return false;
        }
        public List<OperationsViewModel> OperationsByProductId(int itemId)
        {
            var operations = (from o in _context.Operations
                              join oi in _context.OrderItems on o.OrderItem equals oi.Id
                              join p in _context.Products on oi.ProductId equals p.ProductId
                              join ws in _context.WorkStages on o.WorkcentreId equals ws.Id
                              let isEmpID = _context.AspNetUsers.Where(x => x.Id == o.EmployeeId).FirstOrDefault()
                              let orderItem= _context.OrderItems.Where(i=>i.Id== o.OrderItem).FirstOrDefault()
                              where o.OrderItem == itemId
                              select new OperationsViewModel
                              {
                                  Id = o.Id,
                                  OrderItem = o.OrderItem,
                                  productName = p.ProductName,
                                  empID = isEmpID != null ? isEmpID.Id : o.EmployeeId,
                                  fullname = isEmpID != null ? isEmpID.FirstName + "" + isEmpID.LastName : "",
                                  workstageId = ws.Id,
                                  workstage = ws.StageName,
                                  finishTime = o.FinishTime,
                                  completeStatus = o.IsComplete,
                                  orderId = o.OrderId,
                                  qtyDone = o.QtyDone,
                                  releaseDate = o.ReleasedDate,
                                  itemQty = orderItem.Quantity,
                                  startTime = o.StartTime,
                                  Sort=o.Sort,
                                  Location=o.Location
                              }).OrderBy(s=>s.Sort).ToList();
            return operations;
        }

        public IEnumerable<Taxes> GetTaxes()
        {
            return _context.Taxes.ToList();
        }
    }
}