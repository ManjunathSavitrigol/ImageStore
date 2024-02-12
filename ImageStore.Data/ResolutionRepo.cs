using ImageStore.Data.EdmxModel;
using ImageStore.Data.Infrastructure.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStore.Data.Infrastructure
{
    public class ResolutionRepo:BaseRepository<ResolutionMaster>
    {
        public ResolutionRepo(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
