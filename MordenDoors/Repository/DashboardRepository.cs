using MordenDoors.Database;
using MordenDoors.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MordenDoors.Repository
{
    public class DashboardRepository
    {
        readonly MordenDoorsEntities _context;
        public DashboardRepository()
        {
            _context = new MordenDoorsEntities();
        }

        public DashboardView Dashboard()
        {
            var order = _context.Orders.ToList();
            var quoteOrder = order.Where(x => x.StatusId == 1).Count();
            var confirmOrder = order.Where(x => x.StatusId == 7).Count();
            var inProgressOrder = order.Where(x => x.StatusId == 2).Count();
            var completedOrder = order.Where(x => x.StatusId == 5).Count();

            DashboardView dashboard = new DashboardView();
            dashboard.TotalOrder = confirmOrder + inProgressOrder + completedOrder;
            //ProductSold is Completed Orders
            dashboard.ProductSold = completedOrder;
            dashboard.PendingOrder = confirmOrder + inProgressOrder;
            dashboard.TotalCustomer = _context.Customers.Where(s => s.Active == true).Count();

            var userRoles = _context.AspNetRoles.Select(s => s.Name).ToList();
            userRoles = userRoles.Where(s => s == "Employee").ToList();
            var usersWithRoles = (from user in _context.AspNetUsers
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.FirstName + " " + user.LastName,
                                      Email = user.Email,
                                      Status = user.status,
                                      LastUpdateTime = user.LastUpdateTime,
                                      RoleName = (from userRole in user.AspNetRoles
                                                  join role in _context.AspNetRoles on userRole.Id
                                                  equals role.Id
                                                  select role.Name).FirstOrDefault(),
                                      Skills = (from userskills in _context.EmployeeSkills
                                                join skill in _context.WorkStages on userskills.SkillId equals skill.Id
                                                where (userskills.UserId == user.Id)
                                                select skill.StageName).ToList()
                                  }).Where(x => x.Status == true && userRoles.Contains(x.RoleName)).OrderByDescending(x => x.LastUpdateTime).ToList().Select(p => new UserInRoleViewModel()
                                  {
                                      UserId = p.UserId,
                                      Username = p.Username,
                                      Email = p.Email,
                                      Skills = string.Join(",", p.Skills),
                                      Role = p.RoleName,
                                      Status = p.Status,
                                      LastUpdateTime = p.LastUpdateTime
                                  });
            dashboard.TotalEmployee = usersWithRoles.Count();
            var activeCategory = _context.Categories.Where(x => x.IsActive == true).Select(z => z.CategoryId).ToList();
            dashboard.TotalProducts = _context.Products.Where(s => s.IsActive == true && activeCategory.Contains(s.CategoryId)).Count();
            return dashboard;
        }
    }
}