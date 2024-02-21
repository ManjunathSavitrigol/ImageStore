using ImageStore.Data.EdmxModel;
using ImageStore.Data.Infrastructure;
using ImageStore.Data.Infrastructure.Contract;

namespace ImageStore.Data
{
    public class LikesRepo : BaseRepository<Likes>
    {
        public LikesRepo(IUnitOfWork unitOfWork) : base(unitOfWork) {  }
    }
}
