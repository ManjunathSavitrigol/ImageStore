using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ImageStore.Data.EdmxModel;

namespace ImageStore.Data.EdmxModel
{
    public partial class User_Details
    {
        public HttpPostedFileBase ProfileImage { get; set; }
    }
}
