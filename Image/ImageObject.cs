using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image
{
    public class ImageObject
    {
        public int Id { get; set; }
        public string Name { get; set; }    
        public string Category { get; set; }
        public decimal Size { get; set; }
        public string Resolution { get; set; }
        public string Uploader { get; set; }
        public int UploaderId { get; set; }
        public string ImagePath { get; set; }
        public string ProfilePath { get; set; }
        public DateTime UploadedDate { get; set; }  
        public int? RejectedBy { get; set; }
        public int? ApprovedBy { get; set; }
        public bool IsRejected { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string Reason { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? ApprovedDate { get; set; }
       



    }
}
