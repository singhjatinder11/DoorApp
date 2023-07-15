using Microsoft.Ajax.Utilities;
using MordenDoors.Database;
using MordenDoors.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace MordenDoors.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        dynamic showMessageString = string.Empty;
        private readonly MordenDoorsEntities db = new MordenDoorsEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllCategories()
        {
            try
            {
                var data = db.Categories.Where(cat => cat.IsActive == true).Select(cat =>new CategortyViewModel
                {
                    ID = cat.CategoryId,
                    CategoryName = cat.CategoryName,
                    CategoryDescription = cat.CategoryDescription,
                    Sort = cat.Sort,
                    IsMain = cat.IsMain,
                    IsActive = cat.IsActive,
                    isUsed = db.Products.Where(x => x.IsActive == true).Any(s => s.CategoryId == cat.CategoryId)

                }).ToList();
                return View(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult InactiveaCategories(int?id)
        {
            try
            {
                using(MordenDoorsEntities context = new MordenDoorsEntities())
                {
                    var data = context.Categories.Where(s => s.CategoryId == id).FirstOrDefault();
                    data.IsActive = false;
                    context.SaveChanges();
                }              
               
                return RedirectToAction("GetAllCategories");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult CreateCategory()
        {
            return PartialView("CreateCategory", new CategortyViewModel());
        }

        
        public ActionResult CategoryDetail(int id)
        {
            try
            {
                CategortyViewModel model = db.Categories.Where(x => x.CategoryId == id)
                    .Select(x => new CategortyViewModel
                    {
                        ID = x.CategoryId,
                        CategoryDescription = x.CategoryDescription,
                        CategoryName = x.CategoryName,
                        IsActive = x.IsActive,
                        IsMain = x.IsMain,
                        Sort = x.Sort
                    }).FirstOrDefault();
                return PartialView("_CategoryDetail", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult EditCategory(int id)
        {
            try
            {
                CategortyViewModel model = db.Categories.Where(x => x.CategoryId == id)
                    .Select(x => new CategortyViewModel
                    {
                        ID = x.CategoryId,
                        CategoryDescription = x.CategoryDescription,
                        CategoryName = x.CategoryName,
                        IsActive = x.IsActive,
                        IsMain = x.IsMain,
                        Sort = x.Sort
                    }).FirstOrDefault();
                return PartialView("CreateCategory", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult DeleteCategory(int id)
        {
            try
            {
                var result = db.Categories.Where(c => c.CategoryId == id).FirstOrDefault();
                if (result!=null && db.Products.All(c=>c.CategoryId != id)) 
                {
                    db.Categories.Remove(result);
                    db.SaveChanges();
                }
                else if(result!=null && db.Products.Where(c=> c.CategoryId == id).All(c=> c.IsActive == false))
                {
                    db.Categories.Where(c => c.CategoryId == id).ForEach(x => x.IsActive = false);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("GetAllCategories", "Category");
        }

        [HttpPost]
        public ActionResult CreateEditCategory(CategortyViewModel cat)
        {
            if (ModelState.IsValid)
            {
                using (MordenDoorsEntities db = new MordenDoorsEntities())
                {
                    Categories model = new Categories
                    {
                        CategoryId = cat.ID,
                        CategoryName = cat.CategoryName,
                        CategoryDescription = cat.CategoryDescription,
                        Sort = cat.Sort,
                        IsMain = cat.IsMain,
                        IsActive = cat.IsActive
                    };
                    if (cat.ID > 0)
                    {
                        db.Entry(model).State = EntityState.Modified;
                        var update = model;
                        TempData["UpdateMessage"] = update;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Categories.Add(model);
                        var insert = model;
                        TempData["SuccessMessage"] = insert;
                        db.SaveChanges();
                    }
                    if (cat.IsMain)
                    {
                        var CategoryList = db.Categories.Where(x => x.CategoryId!=cat.ID).ToList();
                        CategoryList.ForEach(a => a.IsMain = false);
                        db.SaveChanges();
                    }
                }

            }

            return RedirectToAction("GetAllCategories");
        }
    }
}