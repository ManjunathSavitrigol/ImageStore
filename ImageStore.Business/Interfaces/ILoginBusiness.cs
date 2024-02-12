using ImageStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStore.Business.Interfaces
{
    public interface ILoginBusiness
    {
        Response CheckUser(string username, string password);
        Response RegisterUser(UserDetails userDetails);
        bool CheckToken(string token);
    }
}
