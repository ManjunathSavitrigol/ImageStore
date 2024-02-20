using Image;
using ImageStore.Business;
using ImageStore.Business.Interfaces;
using ImageStore.Data.EdmxModel;
using ImageStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageStore.Controllers
{
    public class SettingsController : Controller
    {
        ISettingsBusiness _settings = new SettingsBusiness();

        public ActionResult Index()
        {
            List<Settings> settings = new List<Settings>();
            try
            {
                Response res = _settings.Get();
                if (res.Flag)
                {
                    settings = (List<Settings>)res.Object;
                }

            }
            catch { }
            return View(settings);
        }

        public ActionResult Get(ServerDataTable server) 
        {
            int count = 0;
            List<Settings> settings = new List<Settings>(); 
            try
            {
                Response res = _settings.Get();
                if (res.Flag)
                {
                    settings = (List<Settings>)res.Object;
                }

            }
            catch { }
            var result = new
            {
                iTotalRecords = count,
                iTotalDisplayRecords = count,
                aaData = settings
            };
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Save(HttpPostedFileBase file, List<Settings> settings)
        {
            foreach(var item in settings)
            {
                item.File = Request.Files["key_" + item.Id];
            }

            Response res = new Response();
            if (settings != null)
            {              
                try
                {
                    res = _settings.Save(settings);
                }
                catch (Exception ex) { }
            }
           

            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}