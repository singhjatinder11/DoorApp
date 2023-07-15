using MordenDoors.Repository;
using MordenDoors.ViewModels;
using System.Web.Mvc;

namespace MordenDoors.Controllers
{
    [AllowAnonymous]
    public class OrderTrackController : Controller
    {
        // GET: OrderTrack
        private readonly TrackingRepository _trackRepository = new TrackingRepository();
        public ActionResult Tracking()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Tracking(OrderTrackViewModel model)
        {
            if (ModelState.IsValid)
            {
                OrderTrackViewModel orderDetail = _trackRepository.TrackOrder(model.TrackingID);
                return View(orderDetail);
            }
            else
                return View();
        }
    }
}