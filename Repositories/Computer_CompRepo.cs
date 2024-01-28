using Report_a_Fault.Data;
using Report_a_Fault.Interface;
using Report_a_Fault.Models;
using Report_a_Fault.Repositories;

namespace Report_a_Fault.Repositories
{
    public class Computer_CompRepo:Repository<Computer_comp>,IComputer_Comp
    {
        private readonly SqlDbContext _context;
        public Computer_CompRepo(SqlDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
