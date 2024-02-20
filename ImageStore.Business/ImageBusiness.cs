using Image;
using ImageStore.Business.Interfaces;
using ImageStore.Data;
using ImageStore.Data.EdmxModel;
using ImageStore.Data.Infrastructure;
using ImageStore.Data.Infrastructure.Contract;
using ImageStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ImageStore.Business
{
    public class ImageBusiness : IImageBusiness
    {
        private readonly IUnitOfWork _unitofwork = new UnitOfWork();
        private readonly ImagesRepo _imageRepo;
        private readonly CategoryRepo _categoryRepo;
        private readonly ResolutionRepo _resolutionRepo;
        private readonly UserDetailsRepo _userRepo;

        public ImageBusiness()
        {
            _imageRepo = new ImagesRepo(_unitofwork);
            _categoryRepo = new CategoryRepo(_unitofwork);
            _resolutionRepo = new ResolutionRepo(_unitofwork);
            _userRepo = new UserDetailsRepo(_unitofwork);
        }
        public Response Save(ImageUpload image)
        {
            Response res = new Response();
            try
            {
                if (image.Id == 0)
                {
                    //check if the name is already used by this user
                    int count = _imageRepo.GetAll(x => x.UploaderId == image.UploaderId && x.Name == image.Name).Count();
                    if (count > 0)
                    {
                        res.Message = "warning*This image name is already used";
                        goto ret;
                    }

                    //save file to a folder
                    string filePath = Helpers.SaveFile(image.ImageFile, "Images/" + image.UploaderId, image.Name);
                    if (string.IsNullOrEmpty(filePath))
                    {
                        res.Message = "error*Failed to save file! Please try again later.";
                        goto ret;
                    }

                    Images newImage = new Images();
                    newImage.Name = image.Name;
                    newImage.Size = Convert.ToDecimal(image.ImageSize); ;
                    newImage.CategoryId = image.Category;
                    newImage.ResolutionId = image.Resolution;
                    newImage.FilePath = filePath;
                    newImage.UploadDate = DateTime.Now;
                    newImage.UploaderId = image.UploaderId;
                    newImage.Tags = image.Tags;

                    _imageRepo.Insert(newImage);

                    res.Message = "success*Image uploaded successfully!";
                    res.Flag = true;

                }

            }
            catch { }

        ret:
            return res;
        }

        public Response Get(string search, int category_id, int resolution_id, int id = 0, int uploaderid = 0)
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                IEnumerable<Categories> categories = _categoryRepo.GetAll();
                IEnumerable<ResolutionMaster> resolutions = _resolutionRepo.GetAll();
                IEnumerable<Images> images = _imageRepo.GetAll();
                IEnumerable<User_Details> users = _userRepo.GetAll();

                IEnumerable<ImageObject> allimages = (from i in images
                                                      join c in categories on i.CategoryId equals c.Id
                                                      join r in resolutions on i.ResolutionId equals r.Id
                                                      join u in users on i.UploaderId equals u.Id
                                                      where ((uploaderid == 0 ? true : (i.UploaderId == uploaderid))
                                                             && (id == 0 ? true : (i.Id == id))
                                                             && (category_id == 0 ? true : (c.Id == category_id))
                                                             && (resolution_id == 0 ? true : (r.Id == resolution_id))
                                                             && ((string.IsNullOrEmpty(search) ? true : (i.Name.Contains(search)))
                                                                 || (string.IsNullOrEmpty(search) ? true : (i.Tags == null ? false : i.Tags.Split('#').Contains(search)))))
                                                      select new ImageObject
                                                      {
                                                          Id = i.Id,
                                                          Name = i.Name,
                                                          Category = c.Name,
                                                          Resolution = r.Resolution,
                                                          Uploader = u.Full_Name,
                                                          ImagePath = i.FilePath,
                                                          ProfilePath = u.Profile,
                                                          UploadedDate = i.UploadDate,
                                                          Size = i.Size,
                                                          IsVerified = i.IsVerified,
                                                          IsRejected = i.IsRejected,
                                                          ApprovedBy = i.VerifiedBy,
                                                          RejectedBy = i.RejectedBy,
                                                          ApprovedDate = i.ApprovedDate,
                                                          RejectedDate = i.RejectedDate
                                                      }
                         );

                res.Object = allimages;
                res.Flag = true;
            }
            catch { }
            return res;
        }

        public Response ApproveReject(int id, string status, string reason, int approverid)
        {
            Response res = new Response();
            try
            {
                //get the image
                Images exist_img = _imageRepo.GetAll(x => x.Id == id).FirstOrDefault();
                if (exist_img != null)
                {
                    //check if the image is already approved or rejected and return request is already handled
                    if (exist_img.IsRejected == true || exist_img.IsVerified == true)
                    {
                        res.Message = "warning*This request is already handled";
                        return res;
                    }


                    if (status == "approve")
                    {
                        exist_img.IsVerified = true;
                        exist_img.VerifiedBy = approverid;
                        exist_img.ApprovedDate = DateTime.Now;

                        _imageRepo.Update(exist_img);

                        res.Flag = true;
                        res.Message = "success*Image approved!";

                    }
                    else
                    {
                        exist_img.IsRejected = true;
                        exist_img.RejectedDate = DateTime.Now;
                        exist_img.RejectedBy = approverid;
                        exist_img.Reason = reason;

                        _imageRepo.Update(exist_img);

                        res.Flag = true;
                        res.Message = "success*Image Rejected!";

                    }
                }
                else
                {
                    res.Message = "error*Not Found!";
                }
            }
            catch { }

            return res;
        }

        public Response Like(int id)
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                //get image
                Images image = _imageRepo.SingleOrDefault(x => x.Id == id);
                if (image == null)
                {
                    res.Message = "Not found!";
                    return res;
                }

                image.Likes += 1;
                _imageRepo.Update(image);

                res.Message = "success*successfull";
                res.Flag = true;
            }
            catch { }
            return res;
        }

        public Response Download(int id)
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                //get image
                Images image = _imageRepo.SingleOrDefault(x => x.Id == id);
                if (image == null)
                {
                    res.Message = "Not found!";
                    return res;
                }

                image.Downloads += 1;
                _imageRepo.Update(image);

                res.Message = "success*successfull";
                res.Flag = true;
                res.Object = image.FilePath;
            }
            catch { }

            return res;
        }

    }
}

