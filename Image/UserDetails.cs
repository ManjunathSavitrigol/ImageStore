using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStore.Domain
{
    public class UserDetails
    {
        public int Id { get; set; } 
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }    
        public string ConfirmPassword { get; set; } 
        public string Token { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
