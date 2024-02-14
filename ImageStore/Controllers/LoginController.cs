using Image;
using ImageStore.Business;
using ImageStore.Business.Interfaces;
using ImageStore.Data.EdmxModel;
using ImageStore.Domain;
using ImageStore.FilterAttributes;
using System;
using System.Collections.Generic;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace ImageStore.Controllers
{
    
    public class LoginController : Controller
    {

        ILoginBusiness loginBusiness = new LoginBusiness();
        IUserBusiness userBusiness = new UserBusiness();

        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Login(string login_email, string login_password)
        {
            try
            {
                Response check = loginBusiness.CheckUser(login_email, login_password);
                if(check.Flag == true) 
                {
                    User_Details user = (User_Details)check.Object1;
                    SetSession((SessionHelper)check.Object);

                    if(user.UserType == "IU" &&( user.DOB == null || user.Profile == null || user.MobileNo == null))
                    {
                        return RedirectToAction("GetStarted", user);
                    }
                    goto redirect;
                }
                else
                {
                    string[] mes = check.Message.Split('*');
                    TempData["message"] = mes[1];
                    TempData["messagetype"] = mes[0];
                    return RedirectToAction("Index");
                }
            }
            catch { }

            return RedirectToAction("Index");

            redirect:
            var arr = (Session["Redirect"].ToString()).Split('*');
            return RedirectToAction(arr[0], arr[1]);
        }

        [HttpPost]
        public ActionResult SignUp(UserDetails userdetails)
        {
            try
            {
                Response newLogin = loginBusiness.RegisterUser(userdetails);
                if(newLogin.Flag == true) 
                {
                    Response check = loginBusiness.CheckUser(userdetails.Email, userdetails.Password);
                    if (check.Flag == true)
                    {
                        SetSession((SessionHelper)check.Object);
                        User_Details user = (User_Details)check.Object1;
                        return RedirectToAction("GetStarted", user);
                    }
                }
                else
                {
                    string[] mes = newLogin.Message.Split('*');
                    TempData["message"] = mes[1];
                    TempData["messagetype"] = mes[0];
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex) { }
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }

        public ActionResult GetStarted(User_Details user)
        {
            if (user != null)
                return View(user);
            else
                Session.Abandon();
                return RedirectToAction("Index");
        }

        public ActionResult UpdateUser(User_Details user)
        {
            try
            {
                Response res = userBusiness.Get(user.Id);
                if(res.Flag)
                {
                    User_Details ext_user = ((IEnumerable<User_Details>)res.Object).FirstOrDefault();
                    if(ext_user != null)
                    {
                        ext_user.DOB = user.DOB;
                        ext_user.MobileNo = user.MobileNo;                        

                        //save photo to a file and save path in the db
                        ext_user.Profile = Helpers.SaveFile(user.ProfileImage, "UserProfileImages", user.Full_Name+DateTime.Now.ToString("yyyyMMddHHmmss"));

                        res = userBusiness.CreateUpdate(ext_user);
                        if(res.Flag)
                        {
                            Session["ProfileImage"] = ext_user.Profile;
                            var arr = (Session["Redirect"].ToString()).Split('*');
                            return RedirectToAction(arr[0], arr[1]);
                        }
                    }
                }
            }
            catch { }
            return RedirectToAction("index");
        }       

        private void SetSession(SessionHelper sessionHelper)
        {
            try
            {
                Session.Clear();
                foreach(var property in typeof(SessionHelper).GetProperties())
                {
                    Session[property.Name]  = property.GetValue(sessionHelper) ?? ""; 
                }
            }
            catch (Exception ex) { }
        }
       
    }
}