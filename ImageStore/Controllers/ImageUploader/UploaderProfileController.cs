using Image;
using ImageStore.Business;
using ImageStore.Business.Interfaces;
using ImageStore.Data.EdmxModel;
using ImageStore.Domain;
using ImageStore.FilterAttributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageStore.Controllers.ImageUploader
{

    [CheckIU]
    public class UploaderProfileController : Controller
    {
        IUserBusiness _userBusiness = new UserBusiness();
        public ActionResult Index()
        {
            User_Details user = new User_Details();
            try
            {
                //get the user
                int userId = Convert.ToInt32(Session["UserId"].ToString());
                Response res = _userBusiness.Get(userId);
                if (res.Flag)
                {
                    user = ((IEnumerable<User_Details>)res.Object).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("Index Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult UpdateUser(User_Details user)
        {
            try
            {
                Response res = _userBusiness.Get(user.Id);
                if (res.Flag)
                {
                    User_Details ext_user = ((IEnumerable<User_Details>)res.Object).FirstOrDefault();
                    if (ext_user != null)
                    {
                        ext_user.DOB = user.DOB;
                        ext_user.MobileNo = user.MobileNo;
                        ext_user.Bio = user.Bio;

                        //save photo to a file and save path in the db
                        if (user.ProfileImage != null)
                        {
                            //ext_user.Profile = Helpers.SaveFile(user.ProfileImage, "UserProfileImages", user.Full_Name + DateTime.Now.ToString("yyyyMMddHHmmss"));

                            System.Drawing.Image profileImage = new Bitmap(user.ProfileImage.InputStream);
                            ext_user.Profile = Helpers.CompressAndSaveImage(profileImage, 80, "UserProfileImages", user.Full_Name + DateTime.Now.ToString("yyyyMMddHHmmss"), 400);

                        }

                        res = _userBusiness.CreateUpdate(ext_user);
                        if (res.Flag)
                        {
                            Session["ProfileImage"] = ext_user.Profile;
                            var arr = (Session["Redirect"].ToString()).Split('*');
                            return RedirectToAction(arr[0], arr[1]);
                        }
                    }
                }
            }
            catch (Exception ex) { Helpers.WriteErrorLog("UpdateUser Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace); }
            return RedirectToAction("index");
        }

        [HttpPost]
        public ActionResult ChangePassword(string oldpass, string password)
        {
            Response res = new Response();
            try
            {
                int userid = Convert.ToInt32(Session["UserId"]?.ToString() ?? "0");
                res = _userBusiness.ChangePassword(userid, oldpass, password);

                string[] mes = res.Message.Split('*');
                TempData["messagetype"] = mes[0];
                TempData["message"] = mes[1];

            }
            catch (Exception ex) { Helpers.WriteErrorLog("ChangePassword Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace); }

            //return Json(res, JsonRequestBehavior.AllowGet);
            return RedirectToAction("Index");
        }        
    }
}