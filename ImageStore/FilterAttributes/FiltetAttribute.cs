using ImageStore.Business;
using ImageStore.Business.Interfaces;
using ImageStore.Data;
using ImageStore.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ImageStore.FilterAttributes
{
    
    public class ExceptionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if(filterContext.Exception != null && !filterContext.ExceptionHandled)
            {
                string controller = filterContext.Controller.ToString();
                var innerException = filterContext.Exception.InnerException;
                var message = filterContext.Exception.Message;
                var stackStrace = filterContext.Exception.StackTrace;

                string path = System.Web.HttpContext.Current.Server.MapPath("~\\Logfiles");
                string fileName = path + @"\error" + DateTime.Now.ToString("ddMMyyyy") + ".txt";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var objStreamWriter = new StreamWriter(fileName, true);
                objStreamWriter.WriteLine("Error Occured At Controller : " + controller + "|" + message + "|" + innerException + "|" + stackStrace + "  Date:" + DateTime.Now);
                objStreamWriter.Dispose();

                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                    {"Controller", "Login" },
                    {"Action", "Index" }
                    });
            }

            base.OnActionExecuted(filterContext);
        }
               
    }

    public class CheckAdmin : ActionFilterAttribute
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private readonly ILoginBusiness loginBusiness = new LoginBusiness();      

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext context = HttpContext.Current;

            if (context.Session["SessionStart"] == null || context.Session["Token"] == null || !loginBusiness.CheckToken(context.Session["Token"].ToString()) || context.Session["UserType"].ToString() != "A") 
            {
                filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary {
                                { "Controller", "Login" },
                                { "Action", "Index" }
                            });

            }
           
        }
    }

    public class CheckIU : ActionFilterAttribute
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private readonly ILoginBusiness loginBusiness = new LoginBusiness();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext context = HttpContext.Current;

            if (context.Session["SessionStart"] == null || context.Session["Token"] == null || !loginBusiness.CheckToken(context.Session["Token"].ToString()) || context.Session["UserType"].ToString() != "IU")
            {
                filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary {
                                { "Controller", "Login" },
                                { "Action", "Index" }
                            });

            }

        }
    }

    public class CheckIA : ActionFilterAttribute
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private readonly ILoginBusiness loginBusiness = new LoginBusiness();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext context = HttpContext.Current;

            if (context.Session["SessionStart"] == null || context.Session["Token"] == null || !loginBusiness.CheckToken(context.Session["Token"].ToString()) || context.Session["UserType"].ToString() != "IA")
            {
                filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary {
                                { "Controller", "Login" },
                                { "Action", "Index" }
                            });

            }

        }
    }
}