using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageStore.Controllers
{

    [RoutePrefix("Error")]
    public class CustomErrorsController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Error500()
        {
            return View();
        }
    }
}