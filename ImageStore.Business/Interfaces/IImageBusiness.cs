using Image;
using ImageStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ImageStore.Business.Interfaces
{
    public interface IImageBusiness
    {
        Response Save(ImageUpload image);
        Response Get(string search, int category_id, int resolution_id, int id = 0, int uploaderid = 0);
        Response ApproveReject(int id, string status, string reason, int approverid);
    }
}
