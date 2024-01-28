

using Report_a_Fault.Data;
using Report_a_Fault.Interface;
using Report_a_Fault.Models;

namespace Report_a_Fault.Repositories
{
    public class ApplicationUserRepo : Repository<ApplicationUser>, IApplicationUser
    {
        private readonly SqlDbContext _context;
        public ApplicationUserRepo(SqlDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
