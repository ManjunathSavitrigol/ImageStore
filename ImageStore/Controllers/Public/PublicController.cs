using Image;
using ImageStore.Business.Interfaces;
using ImageStore.Business;
using ImageStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageStore.Data.EdmxModel;

namespace ImageStore.Controllers.Public
{
    public class PublicController : Controller
    {
        IImageBusiness _image = new ImageBusiness();
        ISettingsBusiness _settings = new SettingsBusiness();
        ILikesBusiness _likes = new LikesBusiness();   
        IUserBusiness _user = new UserBusiness();   


        public ActionResult Index()
        {

            //get carousel image and logo
            Response res = _settings.Get();
            if (res.Flag)
            {
                var settings = (List<Settings>)res.Object;
                ViewBag.Logo = settings.Where(x => x.Key == "FILE_logo").FirstOrDefault().Value.Trim('~');
                ViewBag.CarouselImage = settings.Where(x => x.Key == "FILE_CarouselImage").FirstOrDefault().Value.Trim('~');

            }


            return View();
        }

        public ActionResult GetImages(string search, int DisplayStart, int category_id = 0, int resolution_id = 0, string sorty_by = "")
        {
            List<ImageObject> imagelist = new List<ImageObject>();
            try
            {
                Response res = _image.Get(search, category_id, resolution_id, 0);
                if (res.Flag)
                {

                    var images = (IEnumerable<ImageObject>)res.Object;
                    int count = images.Count();

                    switch (sorty_by)
                    {
                        case "N":
                            images = images.OrderBy(x => x.Name);
                            break;
                        case "DU":
                            images = images.OrderByDescending(x => x.UploadedDate);
                            break;
                        default:
                            images = images.OrderByDescending(x => x.UploadedDate);
                            break;
                    }

                    if (count > DisplayStart)
                    {
                        imagelist = images.Skip(DisplayStart).Take(10).ToList();
                    }
                }
            }
            catch { }
            return PartialView(imagelist);
        }

        [HttpPost]
        public JsonResult Like(int id)
        {
            Response res = new Response();
            if (Session["SessionStart"]?.ToString() == null)
            {
                res.Flag = false;
            }
            else
            {
                int userId = Convert.ToInt32(Session["UserId"].ToString());
                res = _likes.LikeDislike(id, userId);
            }
                
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Download(int id)
        {
            Response res = _image.Download(id);
            if (res.Flag)
            {
                return File(res.Object.ToString(), "image/jpeg", res.Object1+".jpg");
            }

            return RedirectToAction("Index");
        }

        public ActionResult UserProfile(int id)
        {
            User_Details user = new User_Details();
            try
            {
                if (id != 0)
                {
                    Response res = _user.Get(id);
                    if (res.Flag)
                    {
                        user = ((IEnumerable<User_Details>)res.Object).FirstOrDefault();
                    }
                }
            }
            catch { }
            return PartialView(user);
            
        }

    }
}