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

namespace ImageStore.Controllers.ImageUploader
{
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
            catch { }          

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
                            ext_user.Profile = Helpers.SaveFile(user.ProfileImage, "UserProfileImages", user.Full_Name + DateTime.Now.ToString("yyyyMMddHHmmss"));

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
            catch { }
            return RedirectToAction("index");
        }      
    }
}