using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image
{
    public class SessionHelper
    {
        public string UserType { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public DateTime LastLogin { get; set; }
        public string Token { get; set; }
        public string Navigation { get; set; }
        public string Redirect { get; set; }
        public string ProfileImage { get; set; }
        public string SessionStart { get; set; }    
        public string IsDemo { get; set; }
    }
}
