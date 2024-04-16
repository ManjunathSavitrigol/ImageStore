using Image;
using ImageStore.Business;
using ImageStore.Business.Interfaces;
using ImageStore.Data.EdmxModel;
using ImageStore.Domain;
using ImageStore.FilterAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;

namespace ImageStore.Controllers
{
    [CheckAdmin]
    [CheckDemo]
    public class CategoryController : Controller
    {
        ICategoryBusiness _categoryBusiness = new CategoryBusiness();

        [CheckDemo(Disable=true)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Save(Categories category)
        {
            try
            {
                Response res = _categoryBusiness.AddUpdate(category);
                string[] mes = res.Message.Split('*');

                TempData["message"] = mes[1];
                TempData["messagetype"] = mes[0];
            }
            catch (Exception ex) { Helpers.WriteErrorLog("Save Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace); }

            return RedirectToAction("Index"); 
        }

        public ActionResult Edit(int categoryId)
        {
            try
            {
                Response res = _categoryBusiness.Get(categoryId);
                IEnumerable<Categories> qCategory = (IEnumerable<Categories>)res.Object;
                if(!qCategory.Any())
                {
                    TempData["message"] = "Not Found";
                    TempData["messagetype"] = "error";
                    return RedirectToAction("Index");
                }

                Categories categoty = qCategory.FirstOrDefault();
                ViewData["ActionName"] = "Edit";
                return View(categoty);
                
            }
            catch (Exception ex) { Helpers.WriteErrorLog("Edit Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace); }

            return View();
        }

        public ActionResult Add()
        {
            Categories category = new Categories();
            ViewData["ActionName"] = "Add";
            return View("Edit", category);
        }

        [HttpPost]
        public ActionResult Deactivate(string ids)
        {
            Response res = new Response();
            try
            {
                res = _categoryBusiness.Deactivate(ids);
            }
            catch (Exception ex) { Helpers.WriteErrorLog("Deactivate Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace); }

            return Json(res.Message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Activate(string ids)
        {
            Response res = new Response();
            try
            {
                res = _categoryBusiness.Activate(ids);
            }
            catch (Exception ex) { Helpers.WriteErrorLog("Activate Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace); }

            return Json(res.Message, JsonRequestBehavior.AllowGet);
        }

        [CheckDemo(Disable = true)]
        public ActionResult GetCategories(ServerDataTable serverData)
        {
            int count = 0;
            List<Categories> categories = new List<Categories>();   
            try
            {
                Response res = _categoryBusiness.Get(0, serverData.sSearch);
                if (res.Flag)
                {
                    var eList = ((IEnumerable<Categories>)res.Object).OrderBy(x => x.Name);
                    count = eList.Count();

                    categories = eList.Skip(serverData.iDisplayStart).Take(serverData.iDisplayLength).ToList();
                }
                

            }
            catch (Exception ex) { Helpers.WriteErrorLog("GetCategories Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace); }

            var result = new
            {
                iTotalRecords = count,
                iTotalDisplayRecords = count,
                aaData = categories
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }

}