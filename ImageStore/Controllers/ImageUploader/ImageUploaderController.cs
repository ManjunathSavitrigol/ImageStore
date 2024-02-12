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
    public class ImageUploaderController : Controller
    {
        ICategoryBusiness _category = new CategoryBusiness();
        IResolutionBusiness _resolution = new ResolutionBusiness();
        IImageBusiness _image = new ImageBusiness();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(ImageUpload image)
        {
            image.UploaderId = Convert.ToInt32(HttpContext.Session["UserId"].ToString());
            var tags = image.Tags.Split(',');
            string final5Tags = "";

            int count = 0;
            foreach (var tag in tags)
            {
                if (count > 5)
                    break;

                final5Tags += tag + "#";
            }

            image.Tags = final5Tags.Trim('#');
            Response res = _image.Save(image);
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
            catch { }
            return Json(new { categories = categories, resolutions = resolutions }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetImages(string search, int category_id, int resolution_id, string sorty_by)
        {
            List<ImageObject> imagelist = new List<ImageObject>();
            string type = "";
            try
            {
                int uploaderid = Convert.ToInt32(Session["UserId"].ToString());
                Response res = _image.Get(search, category_id, resolution_id, 0, uploaderid);
                if (res.Flag)
                {
                    if (!string.IsNullOrEmpty(Request.Params["type"]))
                    {
                        type = Request.Params["type"];
                    }

                    var images = (IEnumerable<ImageObject>)res.Object;
                    if (type == "pending")
                    {
                        images = images.Where(x => x.IsVerified == true);
                    }

                    
                    switch (sorty_by)
                    {
                        case "N":
                            imagelist = images.OrderBy(x => x.Name).ToList();
                            break;
                        case "DU":
                            imagelist = images.OrderByDescending(x => x.UploadedDate).ToList();
                            break;
                        default:
                            imagelist = images.ToList();
                            break;
                    }
                }
            }
            catch { }
            return PartialView(imagelist);
        }

       
    }
}