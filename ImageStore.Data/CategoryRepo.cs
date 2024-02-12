using ImageStore.Data.EdmxModel;
using ImageStore.Data.Infrastructure;
using ImageStore.Data.Infrastructure.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStore.Data
{
    public class CategoryRepo : BaseRepository<Categories>
    {
        public CategoryRepo(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
