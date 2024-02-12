using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Image
{
    public class ImageUpload
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Category { get; set; }
        public int Resolution { get; set; }
        public float ImageSize { get; set; }
        public int UploaderId { get; set; }
        public string Tags { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
    }
}
