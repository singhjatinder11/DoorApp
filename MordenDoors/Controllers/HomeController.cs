using MordenDoors.Database;
using System.Web.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using MordenDoors.Repository;
using System.Text.RegularExpressions;

namespace MordenDoors.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        DashboardRepository _repository;
        public HomeController()
        {
            _repository = new DashboardRepository();
        }

        public ActionResult Index()
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Home", "User");
            }

            var dataBarPoints = getbardata();
            ViewBag.DataPoints = JsonConvert.SerializeObject(dataBarPoints.Select(s => s.y));
            ViewBag.DataLabel = JsonConvert.SerializeObject(dataBarPoints.Select(s => s.legendText));

            var dataChartPoints = getchartdata();
            ViewBag.DataChart = JsonConvert.SerializeObject(dataChartPoints.Select(s => s.y));
            ViewBag.DataChartLabel = JsonConvert.SerializeObject(dataChartPoints.Select(s => s.legendText));
            var result = _repository.Dashboard();
            return View(result);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

       public List<Models.ChartModel> getbardata()
        {
            List<Models.ChartModel> objlist = new List<Models.ChartModel>();
            using (MordenDoorsEntities context = new MordenDoorsEntities())
            {
                objlist = context.Database.SqlQuery<Models.ChartModel>(
             "exec sp_WorkStageChart").ToList<Models.ChartModel>();
               
            }
            return objlist;
        }
        public List<Models.ChartModel> getchartdata()
        {
            List<Models.ChartModel> objlist = new List<Models.ChartModel>();
            using (MordenDoorsEntities context = new MordenDoorsEntities())
            {
                objlist = context.Database.SqlQuery<Models.ChartModel>(
             "exec chartOrderStatus").ToList<Models.ChartModel>();  
            }
            return objlist;
        }
    }


}