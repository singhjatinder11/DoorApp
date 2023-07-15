using MordenDoors.Database;
using MordenDoors.Models;
using MordenDoors.Models.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MordenDoors.Repository
{
    public class AdminRepository
    {
        MordenDoorsEntities _context;
        public AdminRepository()
        {
            _context = new MordenDoorsEntities();
        }

        public RegisterViewModel EmployeeDetail(string email)
        {
            var stateList = _context.State.Where(x => x.countryId == 37).ToList();
            var user = _context.AspNetUsers.Where(x => x.Email == email).FirstOrDefault();
            var userSkillIds = new List<int>();
            if (user != null)
                userSkillIds = _context.EmployeeSkills.Where(s => s.UserId == user.Id).Select(s => s.SkillId).ToList();

            var result = _context.AspNetUsers.Where(x => x.Email == email).Select(p => new RegisterViewModel
            {
                EmoployeeNumber = p.EmoployeeNumber,
                Email = p.Email,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Gender = p.Gender,
                DateOfBirth = p.DateOfBirth,
                AddrLine1 = p.AddrLine1,
                AddrLine2 = p.AddrLine2,
                Country = p.Country,
                //State = p.StateId,
                stateName = _context.State.Where(s => s.Id == p.StateId).FirstOrDefault().stateName,
                City = p.City,
                SSN = p.SSN,
                Postalcode = p.Postalcode,
                PrintOnCheckName = p.PrintOnCheckName
            }).FirstOrDefault();
            result.StateList = stateList.Select(i => new SelectListItem { Text = i.stateName, Value = i.Id.ToString(), Selected = user.StateId.HasValue ? true : false });
            result.UserSkills = _context.WorkStages.Select(i => new SelectListItem { Text = i.StageName, Value = i.Id.ToString(), Selected = userSkillIds.Contains(i.Id) }).ToList();
            return result;
        }

        public int UpdateUser(RegisterViewModel model)
        {
            var result = _context.AspNetUsers.Where(x => x.Email == model.Email).FirstOrDefault();
            result.FirstName = model.FirstName;
            result.LastName = model.LastName;
            result.AddrLine1 = model.AddrLine1;
            result.AddrLine2 = model.AddrLine2;
            result.City = model.City;
            result.Country = model.Country;
            result.StateId = model.State;
            result.DateOfBirth = model.DateOfBirth;
            result.EmoployeeNumber = model.EmoployeeNumber;
            result.Gender = model.Gender;
            result.LastUpdateTime = DateTime.Now;
            result.Postalcode = model.Postalcode;
            result.PrintOnCheckName = model.PrintOnCheckName;
            result.SSN = model.SSN;
            result.status = true;
            result.EmoployeeNumber = model.EmoployeeNumber;
            if (model.Password != null)
                result.PasswordHash = model.Password;
            var save = _context.SaveChanges();
            if (save > 0)
                return save;
            else
                return 0;
        }

        public int RemoveUser(string email)
        {
            var result = _context.AspNetUsers.Where(x => x.Email == email).FirstOrDefault();
            result.status = false;
            var save = _context.SaveChanges();
            if (save > 0)
                return save;
            else
                return 0;
        }

        public IEnumerable<CustmerHomePage> WorkshopStatus(string status)
        {
            var employeeAssignedWorkList = (from opr in _context.Operations
                                            join order in _context.Orders on opr.OrderId equals order.Id
                                            join customer in _context.Customers on order.CustomerId equals customer.Id
                                            let workStage = _context.WorkStages.Where(x=>x.StageName.ToLower() == status.ToLower()).FirstOrDefault()
                                            join ordrItems in _context.OrderItems on order.Id equals ordrItems.OrderId
                                            let OrderQuantity = _context.OrderItems.Where(q => q.OrderId == order.Id).Sum(x => x.Quantity)
                                            where opr.WorkcentreId == workStage.Id && opr.IsComplete == false
                                            select new CustmerHomePage
                                            {
                                                OrderId = opr.OrderId,
                                                CustomerName = customer.CompanyName,
                                                ProductID = ordrItems.ProductId,
                                                ProductName = _context.Products.Where(x => x.ProductId == ordrItems.ProductId).FirstOrDefault().ProductName,
                                                WorkStage = workStage.StageName,
                                                Height = ordrItems.Height,
                                                Width = ordrItems.Width,
                                                OrderQuantity = ordrItems.Quantity,
                                                DeliveryTime = order.DeliveryTime,
                                                OperationsId = opr.Id
                                            }).ToList();
            return employeeAssignedWorkList;
        }
    }
}
