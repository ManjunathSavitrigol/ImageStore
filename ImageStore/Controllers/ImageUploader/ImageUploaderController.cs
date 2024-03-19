using Image;
using ImageStore.Business;
using ImageStore.Business.Interfaces;
using ImageStore.Data.EdmxModel;
using ImageStore.Domain;
using ImageStore.FilterAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Web;
using System.Web.Mvc;

namespace ImageStore.Controllers.ImageUploader
{

   
    public class ImageUploaderController : Controller
    {
        ICategoryBusiness _category = new CategoryBusiness();
        IResolutionBusiness _resolution = new ResolutionBusiness();
        IImageBusiness _image = new ImageBusiness();
        IUserBusiness _user = new UserBusiness();
        ISettingsBusiness _settings = new SettingsBusiness();

        [CheckIU]
        public ActionResult Index()
        {
            ViewBag.PersonalView = "true";
            ViewBag.layout = "~/Views/Shared/_Layout2.cshtml";
            return View();
        }

        [HttpPost]
        [CheckIU]
        public ActionResult Upload(ImageUpload image)
        {
            var context = HttpContext;
            Response res = new Response();
            try
            {
                image.UploaderId = Convert.ToInt32(HttpContext.Session["UserId"].ToString());
                var tags = image.Tags?.Split(',');
                string final5Tags = "";

                int count = 0;
                if (tags != null)
                {
                    foreach (var tag in tags)
                    {
                        if (count > 5)
                            break;

                        final5Tags += tag + "#";
                    }
                }

                image.Tags = final5Tags.Trim('#');
                res = _image.Save(image);                
            }
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("Upload Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Dropdownlist()
        {
            List<SelectListItem> categories = new List<SelectListItem>();
            List<SelectListItem> resolutions = new List<SelectListItem>();
            try
            {
                Response res = _category.Get();
                if (res.Flag)
                {
                    categories = ((IEnumerable<Categories>)res.Object).Where(x => x.Flag == true).OrderBy(x => x.Name).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
                }
                res = _resolution.Get();
                if (res.Flag)
                {
                    resolutions = ((IEnumerable<ResolutionMaster>)res.Object).Where(x => x.Flag == true).OrderBy(x => x.Resolution).Select(x => new SelectListItem { Text = x.Resolution, Value = x.Id.ToString() }).ToList();

                }
            }
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("Dropdownlist Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }
            return Json(new { categories = categories, resolutions = resolutions }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetImages(string search, int category_id, int resolution_id, string sorty_by, int displaystart, int uploaderid = 0)
        {
            List<ImageObject> imagelist = new List<ImageObject>();
            string type = "";
            try
            {
               if(uploaderid == 0)
                {
                    uploaderid = Convert.ToInt32(Session["UserId"].ToString());
                }
                
                Response res = _image.Get(search, category_id, resolution_id, 0, uploaderid);
                if (res.Flag)
                {
                    if (!string.IsNullOrEmpty(Request.Params["type"]))
                    {
                        type = Request.Params["type"];
                    }

                    var images = (IEnumerable<ImageObject>)res.Object;
                    if (type == "p")
                    {
                        images = images.Where(x => x.IsVerified != true && x.IsRejected != true);
                    }
                    else if (type == "r")
                    {
                        images = images.Where(x => x.IsRejected == true);
                    }
                    else
                    {
                        images = images.Where(x => x.IsVerified == true);
                    }


                    switch (sorty_by)
                    {
                        case "N":
                            imagelist = images.OrderBy(x => x.Name).Skip(displaystart).Take(10).ToList();
                            break;
                        case "DU":
                            imagelist = images.OrderByDescending(x => x.UploadedDate).Skip(displaystart).Take(10).ToList();
                            break;
                        default:
                            imagelist = images.Skip(displaystart).Take(10).ToList();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("GetImages Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }
            //return PartialView(imagelist);
            return Json(imagelist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PublicUploaderProfile(int userId)
        {
            User_Details user = new User_Details();
            try
            {
                //get the user
                int userid = Convert.ToInt32(Session["UserId"]?.ToString() ?? "0");
                if (userid == userId)
                {
                    return RedirectToAction("Index");
                }

                Response res = _user.Get(userId);
                if (res.Flag)
                {
                    user = ((IEnumerable<User_Details>)res.Object).FirstOrDefault();

                    //update user views
                    user.TotalViews++;
                    _user.CreateUpdate(user);

                    ViewBag.ProfileImage = user.Profile.Trim('~');
                    ViewBag.FullName = user.Full_Name;
                    ViewBag.UserId = userId;
                    ViewBag.layout = "~/Views/Shared/_LayoutPublic.cshtml";
                }

                res = _settings.Get();
                if (res.Flag)
                {
                    var settings = (List<Settings>)res.Object;
                    ViewBag.Logo = settings.Where(x => x.Key == "FILE_logo").FirstOrDefault().Value.Trim('~');

                }
            }
            catch (Exception ex) { Helpers.WriteErrorLog("PublicUploaderProfile Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace); }
            return View("Index");
        }

        public ActionResult GetVLD(int UserId)
        {
            VLD vld= new VLD();
            try
            {
                UserId = UserId == 0 ? Convert.ToInt32(Session["UserId"]?.ToString() ?? "0") : UserId;
                Response res = _user.GetWithLikes(UserId);
                if(res.Flag)
                {
                    vld = (VLD)res.Object;
                    res.Flag = true;
                }
            }
            catch (Exception ex) { Helpers.WriteErrorLog("GetVLD Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace); }
            return Json(vld, JsonRequestBehavior.AllowGet);
        }
    }
}