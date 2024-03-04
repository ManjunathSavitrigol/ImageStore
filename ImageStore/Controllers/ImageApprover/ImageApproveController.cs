using ImageStore.Business.Interfaces;
using ImageStore.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Image;
using ImageStore.Data.EdmxModel;
using ImageStore.Domain;
using ImageStore.FilterAttributes;

namespace ImageStore.Controllers.ImageApprover
{
    [CheckIA]
    public class ImageApproveController : Controller
    {
        IImageBusiness _image = new ImageBusiness();

        public ActionResult Index()
        {
            Session["PreviousPage"] = "All";
            Session["ActionName"] = "Index";
            Session["ControllerName"] = "ImageApprove";
            return View();
        }

        public ActionResult GetImages(ServerDataTable serverData)
        {
            DateTime fromdate = DateTime.Now.Date;
            DateTime todate = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            string type = "all";
            int userid = Convert.ToInt32(Session["UserId"].ToString());

            int count = 0;
            List<ImageObject> images = new List<ImageObject>();
            try
            {
                Response res = _image.Get(serverData.sSearch, 0, 0);
                if (res.Flag)
                {
                    if (!string.IsNullOrEmpty(Request.Params["fromdate"]))
                    {
                        fromdate = Convert.ToDateTime(Request.Params["fromdate"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["todate"]))
                    {
                        todate = Convert.ToDateTime(Request.Params["todate"]).AddHours(23).AddMinutes(59).AddSeconds(59);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["type"]))
                    {
                        type = Request.Params["type"];  
                    }

                    var eList = ((IEnumerable<ImageObject>)res.Object).OrderByDescending(x => x.UploadedDate).Where(x => x.UploadedDate > fromdate && x.UploadedDate < todate);
                    if(type == "approved") 
                    {
                       eList =  eList.Where(x => x.ApprovedBy == userid && x.IsVerified == true);
                    }
                    else if(type == "rejected")
                    {
                        eList = eList.Where(x => x.RejectedBy == userid && x.IsRejected == true);
                    }
                    else if(type == "pending")
                    {
                        eList = eList.Where(x => x.IsVerified == false && x.IsRejected == false);
                    }
                    count = eList.Count();

                    images = eList.Skip(serverData.iDisplayStart).Take(serverData.iDisplayLength).ToList();
                }


            }
            catch(Exception ex) 
            {
                Helpers.WriteErrorLog("GetImages Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }

            var result = new
            {
                iTotalRecords = count,
                iTotalDisplayRecords = count,
                aaData = images
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewImage(int id)
        {
            try
            {
                //just to keep alive Tempdata["PreviousPage"] to next view page;
                var previouspage = Session["PreviousPage"];
                var action = Session["ActionName"];
                var controller = Session["ControllerName"];

                Response res = _image.Get("", 0, 0, id);
                if (res.Flag)
                {
                    IEnumerable<ImageObject> eimage = (IEnumerable<ImageObject>)res.Object;
                    ImageObject image = eimage.FirstOrDefault();
                    return View(image);
                }
            }
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("ViewImage Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ApproveReject(int id, string status, string reason)
        {
            Response res = new Response();
            try
            {
                int approveid = Convert.ToInt32( Session["UserId"].ToString());
                res = _image.ApproveReject(id, status, reason, approveid);
            }
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("ApproveReject Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }
            return Json(res, JsonRequestBehavior.AllowGet);   
        }

        public ActionResult Byme()
        {
            Session["PreviousPage"] = "By me";
            Session["ActionName"] = "Byme";
            Session["ControllerName"] = "ImageApprove";

            return View();
        }

        public ActionResult Pending()
        {
            Session["PreviousPage"] = "Pending";
            Session["ActionName"] = "Pending";
            Session["ControllerName"] = "ImageApprove";
            return View();
        }
        

        //public ActionResult ApproveReject()
        //{

        //}
    }
}