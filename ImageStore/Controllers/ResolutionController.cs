using Image;
using ImageStore.Business.Interfaces;
using ImageStore.Business;
using ImageStore.Data.EdmxModel;
using ImageStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageStore.FilterAttributes;

namespace ImageStore.Controllers
{
    [CheckAdmin]
    [CheckDemo]
    public class ResolutionController : Controller
    {
        IResolutionBusiness _resolutionBusiness = new ResolutionBusiness();

        [CheckDemo(Disable = true)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Save(int Id, string Resolution)
        {
            try
            {
                ResolutionMaster resolution = new ResolutionMaster()
                {
                    Id = Id,
                    Resolution = Resolution
                }; 
                var a = Request.Params["Resolution"].ToString();
                
                Response res = _resolutionBusiness.AddUpdate(resolution);
                string[] mes = res.Message.Split('*');

                TempData["message"] = mes[1];
                TempData["messagetype"] = mes[0];
            }
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("Save Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int resolutionId)
        {
            try
            {
                Response res = _resolutionBusiness.Get(resolutionId);
                IEnumerable<ResolutionMaster> qresolution = (IEnumerable<ResolutionMaster>)res.Object;
                if (!qresolution.Any())
                {
                    TempData["message"] = "Not Found";
                    TempData["messagetype"] = "error";
                    return RedirectToAction("Index");
                }

                ResolutionMaster resolution = qresolution.FirstOrDefault();

                string[] parameters = resolution.Resolution.Split('*');
                ViewBag.param1 = parameters[0];
                ViewBag.param2 = parameters[1]; 

                ViewData["ActionName"] = "Edit";
                return View(resolution);

            }
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("Edit Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }

            return View();
        }

        public ActionResult Add()
        {
            ResolutionMaster resolution = new ResolutionMaster();
            ViewData["ActionName"] = "Add";
            return View("Edit", resolution);
        }

        [HttpPost]
        public ActionResult Deactivate(string ids)
        {
            Response res = new Response();
            try
            {
                res = _resolutionBusiness.Deactivate(ids);
            }
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("Deactivate Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }

            return Json(res.Message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Activate(string ids)
        {
            Response res = new Response();
            try
            {
                res = _resolutionBusiness.Activate(ids);
            }
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("Activate Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }

            return Json(res.Message, JsonRequestBehavior.AllowGet);
        }

        [CheckDemo(Disable = true)]
        public ActionResult GetResolutions(ServerDataTable serverData)
        {
            int count = 0;
            List<ResolutionMaster> resolutions = new List<ResolutionMaster>();
            try
            {
                Response res = _resolutionBusiness.Get(0, serverData.sSearch);
                if (res.Flag)
                {
                    var eList = ((IEnumerable<ResolutionMaster>)res.Object).OrderBy(x => x.Resolution);
                    count = eList.Count();

                    resolutions = eList.Skip(serverData.iDisplayStart).Take(serverData.iDisplayLength).ToList();
                }


            }
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("GetResolutions Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }

            var result = new
            {
                iTotalRecords = count,
                iTotalDisplayRecords = count,
                aaData = resolutions
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}