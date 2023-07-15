using MordenDoors.Database;
using MordenDoors.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace MordenDoors.Controllers
{
    [Authorize(Roles = "Admin")]
    public class WorkStageController : Controller
    {
        private readonly MordenDoorsEntities _db = new MordenDoorsEntities();

        // GET: WorkStage
        public ActionResult Index()
        {
            var viewModel = _db.WorkStages.Select(i => new WorkStageViewModel
            {
                Id = i.Id,
                StageName = i.StageName,
                StageDescription = i.Description
            }).ToList();
            return View(viewModel);
        }

        public ActionResult CreateWorkStage()
        {
            return PartialView("_AddWorkStage");
        }
        [HttpPost]
        public ActionResult CreateWorkStage(string stageName, string StageDescription)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(stageName))
            {
                WorkStages entity = new WorkStages
                {
                    StageName = stageName,
                    Description= StageDescription
                };
                _db.WorkStages.Add(entity);
                _db.SaveChanges();
                result = entity.Id > 0 ? true : false;
            }
            if (result)
                TempData["result"] = result;
            return RedirectToAction("Index", "WorkStage");
        }

        public ActionResult EditWorkStage(int id)
        {
            var item = _db.WorkStages.Where(x => x.Id == id).Select(y => new WorkStageViewModel
            {
                Id = y.Id,
                StageName = y.StageName,
               StageDescription = y.Description
            }).FirstOrDefault();
            return PartialView("_AddWorkStage", item);
        }

        [HttpPost]
        public ActionResult EditWorkStage(WorkStageViewModel workStage)
        {
            bool result = false;
            if (workStage != null)
            {
                var item = _db.WorkStages.Where(x => x.Id == workStage.Id).FirstOrDefault();
                if (item != null)
                {
                    item.StageName = workStage.StageName;
                    item.Description = workStage.StageDescription;
                    _db.SaveChanges();
                    result = true;
                }
            }
            if (result)
                TempData["update"] = result;
            return RedirectToAction("Index", "WorkStage");
        }

        public JsonResult DeleteWorkStage(int stageId)
        {
            bool result = false;
            if (stageId > 0)
            {
                var item = _db.WorkStages.Where(x => x.Id == stageId).FirstOrDefault();
                if (item != null)
                {
                    _db.WorkStages.Remove(item);
                    _db.SaveChanges();
                    result = true;
                }
            }
            return Json(new { status = result }, JsonRequestBehavior.AllowGet);
        }
    }
}