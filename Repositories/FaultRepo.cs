using Report_a_Fault.Data;
using Report_a_Fault.Interface;
using Report_a_Fault.Models;
using Report_a_Fault.Repositories;

namespace Report_a_Fault.Repositories
{
    public class FaultRepo:Repository<Fault>,IFault
    {
        private readonly SqlDbContext _context;
        public FaultRepo(SqlDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
