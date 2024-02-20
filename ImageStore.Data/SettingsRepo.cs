using ImageStore.Data.EdmxModel;
using ImageStore.Data.Infrastructure.Contract;
using ImageStore.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStore.Data
{
    public class SettingsRepo : BaseRepository<Configurations>
    {
        public SettingsRepo(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
