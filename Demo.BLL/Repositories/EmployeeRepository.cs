using Demo.BLL.Interfaces;
using Demo.DAL.Contexts;
using Demo.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly MvcAppG02DbContext _dbContext;

        public EmployeeRepository(MvcAppG02DbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }
        public IQueryable<Employee> GetEmployeesByAddress(string address)
        => _dbContext.Employees.Where(E => E.Address == address);

        public IQueryable<Employee> SearchEmployeesByName(string Name)
        => _dbContext.Employees.Where(E => E.Name.ToLower().Contains(Name.ToLower()));
        
    }
}
