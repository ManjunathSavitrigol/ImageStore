using Image;
using ImageStore.Business.Interfaces;
using ImageStore.Data;
using ImageStore.Data.EdmxModel;
using ImageStore.Data.Infrastructure;
using ImageStore.Data.Infrastructure.Contract;
using ImageStore.Domain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
        private readonly SettingsRepo _settingsRepo;

        public ImageBusiness()
        {
            _imageRepo = new ImagesRepo(_unitofwork);
            _categoryRepo = new CategoryRepo(_unitofwork);
            _resolutionRepo = new ResolutionRepo(_unitofwork);
            _userRepo = new UserDetailsRepo(_unitofwork);
            _settingsRepo = new SettingsRepo(_unitofwork);
        }
        public Response Save(ImageUpload image)
        {
            Response res = new Response();
            try
            {
                if (image.Id == 0)
                {
                    //check if the size is greater than the specified size
                    Configurations sizeconfiguration = _settingsRepo.GetAll(x => x.KeyName == "UploadSize").FirstOrDefault();
                    if (sizeconfiguration != null)
                    {
                        if (Convert.ToInt32(sizeconfiguration.Value) > image.ImageSize)
                        {
                            res.Message = "warning*Please select a file grater than equal to " + sizeconfiguration.Value + "MB";
                            goto ret;
                        }
                    }

                    //check if the name is already used by this user
                    int count = _imageRepo.GetAll(x => x.UploaderId == image.UploaderId && x.Name == image.Name).Count();
                    if (count > 0)
                    {
                        res.Message = "warning*This image name is already used";
                        goto ret;
                    }

                    //save file to a folder
                    //old full size
                    //string filePath = Helpers.SaveFile(image.ImageFile, "Images/" + image.UploaderId, image.Name);
                    //if (string.IsNullOrEmpty(filePath))
                    //{
                    //    res.Message = "error*Failed to save file! Please try again later.";
                    //    goto ret;
                    //}

                    //Save different sizes of the same image
                    //compressing original image to 80
                    System.Drawing.Image bitimage = new Bitmap(image.ImageFile.InputStream);
                    var a = bitimage.HorizontalResolution;
                    var b = bitimage.VerticalResolution;
                    var c = bitimage.PixelFormat;
                    var d = bitimage.RawFormat;
                    var e = bitimage.PropertyItems;

                    //get if watermark is enabled
                    string isWatermarkEnabled = _settingsRepo.GetAll(x => x.KeyName == "EnableSizeWaterMark").FirstOrDefault()?.Value ?? "false";

                    //image to be downloaded
                    string filePath = Helpers.CompressAndSaveImage(bitimage, 80, "Images/" + image.UploaderId + "/" + image.Name, image.Name, Convert.ToInt32(bitimage.Width/2), isWatermarkEnabled);

                    //width 1200 quality 70
                    Helpers.CompressAndSaveImage(bitimage, 70, "Images/" + image.UploaderId + "/" + image.Name, image.Name + "_1200", 1200, isWatermarkEnabled);

                    ////width 800 quality 60
                    Helpers.CompressAndSaveImage(bitimage, 70, "Images/" + image.UploaderId + "/" + image.Name, image.Name + "_800", 800, isWatermarkEnabled);

                    ////width 600 quality 50
                    Helpers.CompressAndSaveImage(bitimage, 70, "Images/" + image.UploaderId + "/" + image.Name, image.Name + "_600", 600, isWatermarkEnabled);

                    ////width 400 quality 45
                    Helpers.CompressAndSaveImage(bitimage, 70, "Images/" + image.UploaderId + "/" + image.Name, image.Name + "_400", 400, isWatermarkEnabled);

                    ////width 300 quality 40
                    Helpers.CompressAndSaveImage(bitimage, 70, "Images/" + image.UploaderId + "/" + image.Name, image.Name + "_300", 300, isWatermarkEnabled);

                    ////width 150 quality 35
                    Helpers.CompressAndSaveImage(bitimage, 70, "Images/" + image.UploaderId + "/" + image.Name, image.Name + "_150", 150, isWatermarkEnabled);


                    Images newImage = new Images();
                    newImage.Name = image.Name;
                    newImage.Size = Convert.ToDecimal(image.ImageSize);
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
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("Save Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }

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
                                                          UploaderId = u.Id,
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
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("Get Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }
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
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("ApproveReject Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }

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
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("Like Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }
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
                res.Object1 = image.Name;
            }
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("Download Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }

            return res;
        }

        //private void ImageCompressor()
        //{
        //    using (MagickImage image = new MagickImage(@"YourImage.jpg"))
        //    {
        //        image.Format = MagickFormat.Jpeg; // Set the format of the image.
        //        image.Resize(40, 40); // Fit the image into the requested width and height.
        //        image.Quality = 10; // Set the compression level.
        //        image.Write("YourFinalImage.jpg");
        //    }
        //}


    }
}

