using ImageStore.Business.Interfaces;
using ImageStore.Data.EdmxModel;
using ImageStore.Data.Infrastructure;
using ImageStore.Data;
using ImageStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStore.Business
{
    public class ResolutionBusiness:IResolutionBusiness
    {
        //db instance
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private readonly ResolutionRepo _resolution;

        public ResolutionBusiness()
        {
            _resolution = new ResolutionRepo(_unitOfWork);
        }

        public Response AddUpdate(ResolutionMaster resolution)
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                //check if a category of the same name exists
                int duplicateCount = _resolution.GetAll().Where(x => x.Resolution == resolution.Resolution && x.Id != resolution.Id).Count();
                if (duplicateCount > 0)
                {
                    res.Message = "warning*A category of the same name already exists";
                }
                else
                {
                    if (resolution.Id != 0)
                    {
                        ResolutionMaster existingResolution = _resolution.SingleOrDefault(x => x.Id == resolution.Id);
                        if (existingResolution != null)
                        {
                            existingResolution.Resolution = resolution.Resolution;
                            _resolution.Update(existingResolution);

                            res.Message = "success*Category saved successfully";
                            res.Flag = true;
                        }
                        else
                        {
                            res.Message = "error*Not Found!";
                        }
                    }
                    else
                    {
                        ResolutionMaster newResolution = new ResolutionMaster();
                        newResolution.Resolution = resolution.Resolution;
                        newResolution.Flag = true;
                        _resolution.Insert(newResolution);

                        res.Message = "success*Category saved successfully";
                        res.Flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("AddUpdate Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
                res.Message = "error*Something went wrong!";
            }

            return res;
        }

        public Response Get(int id = 0, string search = "")
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                var count = _resolution.GetAll().Count();
                res.Object = _resolution.GetAll().Where(x =>
                (id == 0 ? true : (x.Id == id))
                && (string.IsNullOrEmpty(search) ? true : x.Resolution.StartsWith(search)));

                res.Flag = true;
            }
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("Get Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }
            return res;
        }

        public Response Deactivate(string ids)
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                string[] splitIds = ids.Split(',');
                foreach (string id in splitIds)
                {
                    //try parse then continue
                    if (!int.TryParse(id, out int actId))
                        continue;

                    //get category and remove
                    ResolutionMaster resolution = _resolution.SingleOrDefault(x => x.Id == actId);
                    if (resolution == null)
                        continue;
                    else
                    {
                        resolution.Flag = false;
                        _resolution.Update(resolution);
                    }

                }

                res.Message = "success*Resolutions Updated Successfully!";
                res.Flag = true;
            }
            catch (Exception ex)
            {
                Helpers.WriteErrorLog("Deactivate Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace);
            }
            return res;
        }

        public Response Activate(string ids)
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                string[] splitIds = ids.Split(',');
                foreach (string id in splitIds)
                {
                    //try parse then continue
                    if (!int.TryParse(id, out int actId))
                        continue;

                    //get category and remove
                    ResolutionMaster resolution = _resolution.SingleOrDefault(x => x.Id == actId);
                    if (resolution == null)
                        continue;
                    else
                    {
                        resolution.Flag = true;
                        _resolution.Update(resolution);
                    }
                }

                res.Message = "success*Resolutions Updated Successfully!";
                res.Flag = true;
            }
            catch (Exception ex) { Helpers.WriteErrorLog("Activate Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace); }

            return res;
        }
    }
}
