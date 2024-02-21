using ImageStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStore.Business.Interfaces
{
    public interface ILikesBusiness
    {
        Response LikeDislike(int imageId, int userId);
    }
}
