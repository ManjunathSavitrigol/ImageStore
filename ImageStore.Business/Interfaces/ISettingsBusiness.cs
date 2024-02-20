using Image;
using ImageStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStore.Business.Interfaces
{
    public interface ISettingsBusiness
    {
        Response Save(List<Settings> settings);
        Response Get();
    }
}
