using Microsoft.AspNet.Identity;
using MordenDoors.Repository;
using MordenDoors.ViewModels;
using System.Web.Mvc;

namespace MordenDoors.Controllers
{
    [Authorize(Roles = "Employee")]
    public class UserController : Controller
    {
        UserRepository _userRepository;

        public UserController()
        {
            _userRepository = new UserRepository();
        }

        public ActionResult Home()
        {
            string empId = User.Identity.GetUserId().ToString();
            var result = _userRepository.GetEmployeeWork(empId);
            ViewBag.EmployeeId = empId;
            return View(result);
        }

        public ActionResult GetWork()
        {

            string empId = User.Identity.GetUserId().ToString();
            var result = _userRepository.GetEmployeeWork(empId);
            //string UserId = UserDetail.Users(User.Identity.Name.ToString()).Id;
            //var result = _userRepository.GetWork(UserId);
            return View(result);
        }

        [HttpPost]
        public JsonResult UpdateWorkStatus(int operationsId, int qytDone, int orderId, int workStageId, bool isCompleted,string Location)
        {
            string userId = User.Identity.GetUserId();
            var result = _userRepository.UpdateWorkStatus(operationsId,orderId, workStageId, userId, isCompleted, qytDone,Location);
            TempData["result"] = result;
            return Json(new { status = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CompleteWork(int operationsId, int qytDone, int orderId, int workStageId, bool isCompleted,string Location)
        {
            string userId = User.Identity.GetUserId();
            var result = _userRepository.UpdateWorkStatus(operationsId, orderId, workStageId, userId, isCompleted, qytDone,Location);
            TempData["complete"] = result;
            return Json(new { status = result }, JsonRequestBehavior.AllowGet);
        }
    }
}