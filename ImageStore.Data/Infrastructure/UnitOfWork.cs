using ImageStore.Data.EdmxModel;
using ImageStore.Data.Infrastructure.Contract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStore.Data.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ImageStoreEntities _dbContext;

        public UnitOfWork()
        {
            _dbContext = new ImageStoreEntities();
        }

        public DbContext Db
        {
            get { return _dbContext; }
        }

        public void Dispose()
        {
        }
    }
}
