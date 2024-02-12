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
using System.Web.Razor.Tokenizer;

namespace ImageStore.Controllers
{
    public class UserController : Controller
    {
        IUserBusiness _userBusiness = new UserBusiness();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Save(User_Details user)
        {
            try
            {
                user.Pass = Helpers.Encrypt(user.Pass);
                Response res = _userBusiness.CreateUpdate(user);
                string[] mes = res.Message.Split('*');

                TempData["message"] = mes[1];
                TempData["messagetype"] = mes[0];
            }
            catch { }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int userId)
        {
            try
            {
                Response res = _userBusiness.Get(userId);
                IEnumerable<User_Details> qUser = (IEnumerable<User_Details>)res.Object;
                if (!qUser.Any())
                {
                    TempData["message"] = "Not Found";
                    TempData["messagetype"] = "error";
                    return RedirectToAction("Index");
                }

                User_Details user = qUser.FirstOrDefault(); 
                user.Pass = Helpers.Decrypt(user.Pass);

                ViewData["ActionName"] = "Edit";
                return View(user);

            }
            catch { }

            return View();
        }

        public ActionResult Create()
        {
            User_Details newUser = new User_Details();
            ViewData["ActionName"] = "Add";
            return View("Edit", newUser);
        }

        [HttpPost]
        public ActionResult Deactivate(string ids)
        {
            Response res = new Response();
            try
            {
                res = _userBusiness.Deactivate(ids);
            }
            catch { }

            return Json(res.Message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Activate(string ids)
        {
            Response res = new Response();
            try
            {
                res = _userBusiness.Activate(ids);
            }
            catch { }

            return Json(res.Message, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUsers(ServerDataTable serverData)
        {
            int count = 0;
            List<User_Details> users = new List<User_Details>();
            try
            {
                Response res = _userBusiness.Get(0, serverData.sSearch);

                if (res.Flag)
                {
                    string usertype = Request.Params["usertype"];
                    var eList = ((IEnumerable<User_Details>)res.Object).OrderBy(x => x.Email);
                    count = eList.Count();

                    users = eList.Where(x=> usertype == "All"?true:(x.UserType == usertype)).Skip(serverData.iDisplayStart).Take(serverData.iDisplayLength).ToList();
                }
            }
            catch { }

            var result = new
            {
                iTotalRecords = count,
                iTotalDisplayRecords = count,
                aaData = users
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

       
    }
}