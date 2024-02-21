using ImageStore.Data.Infrastructure.Contract;
using ImageStore.Data.Infrastructure;
using ImageStore.Domain;
using ImageStore.Data;
using System.Linq;
using ImageStore.Data.EdmxModel;
using ImageStore.Business.Interfaces;

namespace ImageStore.Business
{
    public class LikesBusiness:ILikesBusiness
    {
        private readonly IUnitOfWork _unitofwork = new UnitOfWork();
        private readonly LikesRepo _likesRepo;

        public LikesBusiness()
        {
            _likesRepo = new LikesRepo(_unitofwork);
        }

        public Response LikeDislike(int imageId, int userId)
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                //get the liked row
                Likes like = _likesRepo.GetAll(x => x.UserId == userId && x.ImageId == imageId).FirstOrDefault();
                if (like == null)
                {
                    like = new Likes()
                    {
                        ImageId = imageId,
                        IsLiked = true,
                        UserId = userId
                    };

                    _likesRepo.Insert(like);
                    res.Flag = true;
                    res.Object = 1;
                }
                else
                {
                    if (like.IsLiked)
                    {
                        like.IsLiked = false;
                        res.Object = 0;
                    }
                    else
                    {
                        like.IsLiked = true;
                        res.Object = 1;
                    }                      

                    _likesRepo.Update(like); 
                    res.Flag = true;
                    
                }

            }
            catch { }
            return res;
        }

        public Response Get(int imageId = 0, int userId = 0)
        {
            Response res = new Response();
            try
            {
                res.Object = _likesRepo.GetAll(x => imageId == 0? true: x.ImageId == imageId
                                                     && userId == 0? true:  x.UserId == userId);
                res.Flag = true;
            }
            catch { }
            return res; 
        }
    }
}
