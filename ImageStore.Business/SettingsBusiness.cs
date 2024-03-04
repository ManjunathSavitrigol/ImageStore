using Image;
using ImageStore.Business.Interfaces;
using ImageStore.Data;
using ImageStore.Data.EdmxModel;
using ImageStore.Data.Infrastructure;
using ImageStore.Domain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStore.Business
{
    public class SettingsBusiness : ISettingsBusiness
    {
        //db instance
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private readonly SettingsRepo _settings;

        public SettingsBusiness()
        {
            _settings = new SettingsRepo(_unitOfWork);
        }

        public Response Save(List<Settings> settings)
        {
            Response res = new Response();
            try
            {
                IList<Configurations> configurations = new List<Configurations>();
                
                foreach(var item in settings) 
                {
                    //get the item 
                    Configurations setting = _settings.GetAll().FirstOrDefault(x => x.Id == item.Id);
                    if(setting != null) 
                    {
                        if (setting.KeyName.StartsWith("FILE_"))
                        {
                            //setting.Value = Helpers.SaveFile(item.File, "SettingsFile", setting.KeyName+setting.Id+DateTime.Now.ToString("ddMMyyyyhhmmssfff"));

                            System.Drawing.Image settingimage = new Bitmap(item.File.InputStream);
                            setting.Value = Helpers.CompressAndSaveImage(settingimage, 100, "SettingsFile", setting.KeyName + setting.Id + DateTime.Now.ToString("ddMMyyyyhhmmssfff"), 1200);

                        }
                        else
                        {
                            setting.Value = item.Value;
                        }

                        configurations.Add(setting);
                    }                    
                }

                _settings.UpdateAll(configurations);
                res.Message = "success*Saved successfully";
                res.Flag = true;
            }
            catch (Exception ex) { Helpers.WriteErrorLog("Save Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace); }

            return res;
        }

        public Response Get()
        {
            Response res = new Response();
            try
            {
                List<Settings> settings = new List<Settings>();
                IEnumerable<Configurations> configurations = _settings.GetAll().ToList();
                
                foreach(var item in configurations)
                {
                    settings.Add(new Settings
                    {
                        Id = item.Id,
                        Value = item.Value,
                        Key = item.KeyName,
                        Description = item.Details
                    });
                }

                res.Object = settings;
                res.Flag = true;
            }
            catch (Exception ex) { Helpers.WriteErrorLog("Get Error | " + ex.Message + " | " + ex.InnerException + " | " + ex.StackTrace); }
            return res;
        }
    }
}
 