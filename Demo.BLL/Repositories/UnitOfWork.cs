using Demo.BLL.Interfaces;
using Demo.DAL.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MvcAppG02DbContext _dbContext;

        public IEmployeeRepository EmployeeRepository { get; set; }
        public IDepartmentRepository DepartmentRepository { get; set; }

        public UnitOfWork(MvcAppG02DbContext dbContext) // Ask CLR To Create Object From Dbcontext
        {
            EmployeeRepository = new EmployeeRepository(dbContext);
            DepartmentRepository = new DepartmentRepository(dbContext);
            _dbContext = dbContext;
        }

        public async Task<int> Complete()
        => await _dbContext.SaveChangesAsync();

        public void Dispose() // Called By CLR
       => _dbContext.Dispose();
    }
}
