using MordenDoors.Database;
using MordenDoors.Models;
using MordenDoors.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MordenDoors.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly MordenDoorsEntities db = new MordenDoorsEntities();
        // GET: Products
       
        public ActionResult GetAllProducts(int? id)
        {
            try
            {
                List<SelectListItem> categoriesListItems = new List<SelectListItem>() { new SelectListItem { Text = "All Categories", Value = null } };

                if (id.HasValue && id.Value != 0)
                {
                    IEnumerable<SelectListItem> selectListItems = new List<SelectListItem>();
                    selectListItems = db.WorkStages.Select(x => new SelectListItem { Text = x.StageName, Value = x.Id.ToString()}).ToList();
                    ProductViewModel model = new ProductViewModel();
                    model.UserRoles = selectListItems;
                    var categories = db.Categories.Where(z => z.IsActive == true).OrderBy(s => s.ShortingNo).Select(x => new SelectListItem { Text = x.CategoryName, Value = x.CategoryId.ToString(), Selected = id.HasValue ? true : false }).ToList();
                    categoriesListItems.AddRange(categories);
                    model.Categories = categoriesListItems;
                    model.UnitsOfMeasures = db.UnitsOfMeasure.Select(x => new SelectListItem { Text = x.UnitName, Value = x.UnitID.ToString() }).ToList();
                    model.Products = db.Products.Where(x => x.IsActive == true && x.CategoryId == id).Select(x => new ProductModel
                    {
                        ProductId = x.ProductId,
                        CategoryId = x.CategoryId,
                        ProductName = x.ProductName,
                        ProductDescription = x.ProductDescription,
                        UnitsOfMeasureID = x.UnitsOfMeasureID,
                        Rate = x.Rate,
                        IsActive = x.IsActive,
                        QbId = x.QbId,
                        SKU = x.SKU,
                        productImage = x.productImage,
                        Weight = x.Weight,
                        MinimumSquareFeet = x.MinimumSquareFeet,
                        Category = new CategortyViewModel
                        {
                        CategoryName = x.Categories.CategoryName
                        },
                        UnitMeasure = new UnitViewModel
                        {
                        UnitName = x.UnitsOfMeasure.UnitName
                        }
                    }).ToList();
                    model.CategoryId = id.Value;
                    return View(model);
                }
                else
                {
                    IEnumerable<SelectListItem> selectListItems = new List<SelectListItem>();
                    selectListItems = db.WorkStages.Select(x => new SelectListItem { Text = x.StageName, Value = x.Id.ToString(), Selected = false }).ToList();
                    ProductViewModel model = new ProductViewModel();
                    model.UserRoles = selectListItems;
                    var categories = db.Categories.Where(z => z.IsActive == true).OrderBy(s => s.ShortingNo).Select(x => new SelectListItem { Text = x.CategoryName, Value = x.CategoryId.ToString(), Selected = id.HasValue ? true : false }).ToList();
                    categoriesListItems.AddRange(categories);
                    model.Categories = categoriesListItems;
                    model.UnitsOfMeasures = db.UnitsOfMeasure.Select(x => new SelectListItem { Text = x.UnitName, Value = x.UnitID.ToString() }).ToList();
                    var activeCategory = db.Categories.Where(x => x.IsActive == true).Select(z => z.CategoryId).ToList();
                    model.Products = db.Products.Where(x => x.IsActive == true && activeCategory.Contains(x.CategoryId)).Select(x => new ProductModel
                    {
                        ProductId = x.ProductId,
                        CategoryId = x.CategoryId,
                        ProductName = x.ProductName,
                        ProductDescription = x.ProductDescription,
                        UnitsOfMeasureID = x.UnitsOfMeasureID,
                        Rate = x.Rate,
                        IsActive = x.IsActive,
                        QbId = x.QbId,
                        SKU = x.SKU,
                        productImage = x.productImage,
                        Weight = x.Weight,
                        MinimumSquareFeet = x.MinimumSquareFeet,
                        Category = new CategortyViewModel
                        {
                            CategoryName = x.Categories.CategoryName
                        },
                        UnitMeasure = new UnitViewModel
                        {
                            UnitName = x.UnitsOfMeasure.UnitName
                        }
                    }).ToList();
                    model.CategoryId = null;
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult DeleteProduct(int? id, int? catid)
        {
            try
            {
                using (MordenDoorsEntities context = new MordenDoorsEntities())
                {
                    var data = context.Products.Where(s => s.ProductId == id).FirstOrDefault();
                    data.IsActive = false;
                    context.SaveChanges();
                }

                return RedirectToAction("GetAllProducts", catid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ResyncProduct(int? id, int? catid)
        {
            try
            {
                using (MordenDoorsEntities context = new MordenDoorsEntities())
                {
                    var data = context.Products.Where(s => s.ProductId == id).FirstOrDefault();
                    data.Resync = true;
                    context.SaveChanges();
                }

                return RedirectToAction("GetAllProducts", catid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult CreateProduct()
        {
            try
            {
                IEnumerable<SelectListItem> selectListItems = new List<SelectListItem>();
                selectListItems = db.WorkStages.Select(x => new SelectListItem { Text = x.StageName, Value = x.Id.ToString() }).ToList();

                ProductModel model = new ProductModel
                {
                    UnitsOfMeasures = db.UnitsOfMeasure.Select(x => new SelectListItem { Text = x.UnitDescription, Value = x.UnitID.ToString() }).ToList(),
                    Categories = db.Categories.Where(z => z.IsActive == true).OrderBy(s => s.ShortingNo).Select(x => new SelectListItem { Text = x.CategoryName, Value = x.CategoryId.ToString() }).ToList(),
                    UserRoles = selectListItems
                };
                ViewBag.PartialViewName = "~/Views/Product/_CreateProduct.cshtml";
                ViewBag.PartialViewModal = model;
                return PartialView("_CreateProduct", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult CreateProduct(ProductModel pro, HttpPostedFileBase file)
        {
            var allowedExtension = new[]
                    {
                        ".jpg", ".JPG", ".png", ".PNG", "jpeg"
                    };
            if (file != null)
            {
                pro.productImage = file.ToString();
                var fileName = Path.GetFileName(file.FileName);
                var ext = Path.GetExtension(file.FileName);

                if (allowedExtension.Contains(ext))
                {
                    string imagePath = "/" + ConfigurationManager.AppSettings["ImageFolder"] + "/";
                    string name = Path.GetFileNameWithoutExtension(fileName);
                    string myfile = name + "_" + pro.ProductId + ext;
                    var path = Path.Combine(Server.MapPath("~/Images"), myfile);
                    imagePath = imagePath + myfile;
                    pro.productImage = imagePath;
                    file.SaveAs(path);
                }
                else
                {
                    ViewBag.ImageError = "Please Choose Only One Image";
                }

            }


            if (ModelState.IsValid)
            {
                try
                {
                    Products product = new Products()
                    {
                        CategoryId = pro.CategoryId,
                        ProductName = pro.ProductName,
                        ProductDescription = pro.ProductDescription,
                        UnitsOfMeasureID = pro.UnitsOfMeasureID,
                        Rate = pro.Rate,
                        IsActive = pro.IsActive,
                        QbId = pro.QbId,
                        SKU = pro.SKU,
                        productImage = pro.productImage,
                        Weight = pro.Weight,
                        MinimumSquareFeet = pro.MinimumSquareFeet
                    };
                    db.Products.Add(product);
                    db.SaveChanges();
                    var roleIds = db.WorkStages.Where(o => pro.UserRole.Contains(o.Id.ToString())).Select(x => x.Id).ToList();
                     int[] _worlstages = pro.UserRole.Split(',').Select(int.Parse).ToArray();
                    int _sortOrder = 0;
                        foreach(var i in _worlstages)
                      {
                        ProductWorkstages productWorkstage = new ProductWorkstages
                        {
                            ProductId = product.ProductId,
                            RoleId = Convert.ToString(i),
                            Sort = Convert.ToByte(_sortOrder)
                        };
                        db.ProductWorkstages.Add(productWorkstage);
                        db.SaveChanges();
                        _sortOrder++;
                     }
                    TempData["SuccessMessage"] = "Success";
                }
                catch (Exception ex)
                {
                    TempData["FailureMessage"] = "Failed";
                }

            }

            return RedirectToAction("GetAllProducts");
        }
        public ActionResult EditProduct(int id)
         {
            EditProductModel model = new EditProductModel();
            try
            {
                var product = db.Products.Where(x => x.ProductId == id).FirstOrDefault();
                if (product != null)
                {
                    model = new EditProductModel
                    {
                        EditProductId = product.ProductId,
                        EditCategoryId = product.CategoryId,
                        EditProductName = product.ProductName,
                        EditProductDescription = product.ProductDescription,
                        EditUnitsOfMeasureID = product.UnitsOfMeasureID,
                        EditRate = product.Rate,
                        EditIsActive = product.IsActive,
                        EditQbId = product.QbId,
                        EditSKU = product.SKU,
                        Weight = product.Weight,
                        MinimumSquareFeet = product.MinimumSquareFeet,
                        EditproductImages = product.productImage ?? "~/Images/ImageNotAvailable.png"
                    };
                    model.EditCategories = db.Categories.Where(z => z.IsActive == true).OrderBy(s => s.ShortingNo).Select(x => new SelectListItem { Text = x.CategoryName, Value = x.CategoryId.ToString(), Selected = x.CategoryId == product.CategoryId }).ToList();
                    model.EditUnitsOfMeasures = db.UnitsOfMeasure.Select(x => new SelectListItem { Text = x.UnitDescription, Value = x.UnitID.ToString(), Selected = x.UnitID == product.UnitsOfMeasureID }).ToList();
                    List<string> userId = new List<string>();
                    var userList = db.ProductWorkstages.Where(x => x.ProductId == product.ProductId).ToList();
                    foreach( var i in userList)
                    {
                        userId.Add(i.RoleId);
                    }
                    var ListItems = (from w in db.WorkStages
                                     join pw in db.ProductWorkstages.Where(x => x.ProductId == product.ProductId) on w.Id.ToString() equals pw.RoleId into pstages
                                     from x in pstages.DefaultIfEmpty()
                                     select new
                                     {
                                         StageName = w.StageName,
                                         Id = w.Id.ToString(),
                                         IsSelected = x.Sort != null,
                                         Sort = x.Sort
                                     }).OrderBy(s => s.Sort == null).ThenBy(s => s.Sort).ToList();            
                    model.EditUserRoles = ListItems.Select(x => new SelectListItem { Text = x.StageName, Value = x.Id.ToString(), Selected = x.IsSelected });
                }
            }
            catch (Exception ex) {
            }

            return PartialView("_EditProduct", model);
        }

        [HttpPost]
        public ActionResult EditProduct(EditProductModel model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var pic = System.Web.HttpContext.Current.Request.Files["file"];

                    if (pic.FileName != "")
                    {
                        var ImageExtension = new[]
                        {
                             ".jpg", ".JPG", ".png", ".PNG", ".jpeg"
                        };

                        model.EditproductImages = file.ToString();
                        var fileName = Path.GetFileName(file.FileName);
                        var extentionName = Path.GetExtension(file.FileName);
                        if (ImageExtension.Contains(extentionName))
                        {
                            string imagePath = "/" + ConfigurationManager.AppSettings["ImageFolder"] + "/";
                            string name = Path.GetFileNameWithoutExtension(fileName);
                            string myFile = name + "_" + model.EditproductImages + extentionName;
                            var path = Path.Combine(Server.MapPath("~/Images/"), myFile);
                            imagePath = imagePath + myFile;
                            model.EditproductImages = imagePath;
                            file.SaveAs(path);
                            var product = db.Products.Where(x => x.ProductId == model.EditProductId).FirstOrDefault();
                            product.CategoryId = model.EditCategoryId;
                            product.ProductName = model.EditProductName;
                            product.ProductDescription = model.EditProductDescription;
                            product.UnitsOfMeasureID = model.EditUnitsOfMeasureID;
                            product.IsActive = model.EditIsActive;
                            product.Rate = model.EditRate;
                            product.SKU = model.EditSKU;
                            product.productImage = model.EditproductImages;
                            product.Resync = true;
                            db.SaveChanges();
                        }
                        else
                        {
                            ViewBag.EditImageError = "Please choose only one Image";
                        }

                    }
                    else
                    {
                        var product = db.Products.Where(x => x.ProductId == model.EditProductId).FirstOrDefault();
                        product.CategoryId = model.EditCategoryId;
                        product.ProductName = model.EditProductName;
                        product.ProductDescription = model.EditProductDescription;
                        product.UnitsOfMeasureID = model.EditUnitsOfMeasureID;
                        product.IsActive = model.EditIsActive;
                        product.Rate = model.EditRate;
                        product.MinimumSquareFeet = model.MinimumSquareFeet;
                        product.SKU = model.EditSKU;
                        db.SaveChanges();
                    }
                    var productWorkstages = db.ProductWorkstages.Where(x => x.ProductId == model.EditProductId).ToList();

                    if (productWorkstages != null)
                    {
                        db.ProductWorkstages.RemoveRange(productWorkstages);
                        db.SaveChanges();
                        if (model != null && model.EditUserRole!=null)
                        {
                            int[] _worlstages = model.EditUserRole.Split(',').Select(int.Parse).ToArray();
                            int _sortOrder = 0;
                            foreach (var u in _worlstages)
                            {
                                ProductWorkstages productWorkstage = new ProductWorkstages
                                {
                                    ProductId = model.EditProductId,
                                    RoleId = u.ToString(),
                                    Sort = Convert.ToByte(_sortOrder)
                                };
                                db.ProductWorkstages.Add(productWorkstage);
                                db.SaveChanges();
                                _sortOrder++;
                            }
                        }
                    }
                    TempData["UpdateMessage"] = "Success";
                }
                catch (Exception ex)
                {
                    TempData["FailureMessage"] = "Failed";
                }
            }
            return RedirectToAction("GetAllProducts");
        }


        public JsonResult ProductDetail(int id)
        {
            ProductViewModel model = new ProductViewModel();
            try
            {
                var ProductDetail = db.Products.Where(x => x.ProductId == id).FirstOrDefault();
                if (ProductDetail != null)
                {
                    model.ProductId = ProductDetail.ProductId;
                    model.CategoryId = ProductDetail.CategoryId;
                    model.ProductName = ProductDetail.ProductName;
                    model.ProductDescription = ProductDetail.ProductDescription;
                    //model.UnitsOfMeasureID = ProductDetail.UnitsOfMeasureID;
                    model.Rate = ProductDetail.Rate;
                    model.SKU = ProductDetail.SKU;
                    model.ProductImages = ProductDetail.productImage;
                    var categoryName = db.Categories.Where(x => x.CategoryId == ProductDetail.CategoryId).FirstOrDefault();
                    model.CategoryName = categoryName.CategoryName.ToString();
                    var unit = db.UnitsOfMeasure.Where(x => x.UnitID == ProductDetail.UnitsOfMeasureID).FirstOrDefault();
                    model.UnitName = unit.UnitName;
                    model.Product.Weight = ProductDetail.Weight;
                    model.Product.MinimumSquareFeet = ProductDetail.MinimumSquareFeet;
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }
    }
}


