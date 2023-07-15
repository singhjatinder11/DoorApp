using MordenDoors.Database;
using MordenDoors.Models.Customers;
using MordenDoors.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MordenDoors.Repository
{
    public class UserRepository
    {
        MordenDoorsEntities _context;
        public UserRepository()
        {
            _context = new MordenDoorsEntities();
        }

        public IEnumerable<CustmerHomePage> GetEmployeeWork(string id)
        {
            return _context.uspGetEmployeeWork(id).OrderByDescending(x => x.Id).Select(w => new CustmerHomePage
            {
                OrderId = w.OrderId,
                CustomerName = w.CompanyName,
                ProductID = Convert.ToInt32(w.ProductId),
                ProductName = w.ProductName,
                WorkStageId= w.WorkStageId,
                WorkStage = w.StageName,
                Height = w.Height,
                Width = w.Width,
                OrderQuantity = Convert.ToInt32(w.Quantity),
                StartTime = w.StartTime,
                OperationsId = w.Id,
                Location=string.Empty,
                CanGet = Convert.ToBoolean(w.CanGet),
                Sort= Convert.ToByte( w.Sort),
                EmployeeId=w.EmployeeId
                
            }).ToList();
        }

        public bool UpdateWorkStatus(int operationID, int orderNo, int stageID, string empId, bool isComplete, int qytDone, string location)
        {
            Operations operation = new Operations();
            using (var orderTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var op1 = _context.Operations.Where(o => o.OrderId == orderNo);
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
                                var nextSatge = op1.Where(x => x.OrderItem == op.OrderItem && x.Sort == op.Sort + 1).FirstOrDefault();
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
                            if (op.EmployeeId == empId || string.IsNullOrEmpty(op.EmployeeId))
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

        public bool CompleteWork(int OperationsId, int quantityDone)
        {
            var workOperations = _context.Operations.Where(x => x.Id == OperationsId).FirstOrDefault();
            if (workOperations != null)
            {
                workOperations.QtyDone = quantityDone;
                workOperations.FinishTime = System.DateTime.Now;
                workOperations.IsComplete = true;
                int save = _context.SaveChanges();
                if (save > 0)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public List<Products> GetProducts()
        {
            return (from prod in _context.Products
                    join cat in _context.Categories on prod.CategoryId equals cat.CategoryId
                    where cat.IsMain == true
                    select prod).ToList();
        }
        public List<Products> GetProductsForUpdate()
        {
            return (from prod in _context.Products
                    join cat in _context.Categories on prod.CategoryId equals cat.CategoryId
                    where cat.IsMain == true && prod.QbId != null && prod.Resync == true
                    select prod).ToList();
        }
        public bool InsertProductExternalId(int id, string extrnalId)
        {
            var product = _context.Products.Where(x => x.ProductId == id).FirstOrDefault();
            product.QbId = extrnalId;
            _context.SaveChanges();
            return true;

        }
        public bool InsertOderExternalId(int id, string extrnalId)
        {
            var order = _context.Orders.Where(x => x.Id == id).FirstOrDefault();
            order.ExtrnalId = extrnalId;
            _context.SaveChanges();
            return true;

        }
        public bool ProductResyncDone(int id, string extrnalId)
        {
            var product = _context.Products.Where(x => x.ProductId == id && x.QbId == extrnalId).FirstOrDefault();
            product.Resync = false;
            _context.SaveChanges();
            return true;

        }
        public List<InvoiceModel> GetInvoice()
        {
            return (from ord in _context.Orders
                    join cust in _context.Customers on ord.CustomerId equals cust.Id
                    join orditem in _context.OrderItems on ord.Id equals orditem.OrderId
                    join pro in _context.Products on orditem.ProductId equals pro.ProductId
                    where ord.ExtrnalId == null && pro.QbId != null && cust.ZohoContactId != null && ord.StatusId==5
                    select new InvoiceModel
                    {
                        OrderId = ord.Id,
                        Tax = cust.DefaultTaxCodeRef,
                        ZohoCustomerId = cust.ZohoContactId,
                        OrderItemId = orditem.Id,
                        ProductId = pro.ProductId,
                        Price = orditem.Price,
                        Quantity = orditem.Quantity,
                        Comments = orditem.Comments,
                        ProductName = pro.ProductName,
                        PrdouctZohoId = pro.QbId,
                        PrdouctDescription = pro.ProductDescription,
                        PO = ord.PO
                    }).OrderBy(s => s.OrderId).ToList();
        }
    }
}