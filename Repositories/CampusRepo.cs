

using Report_a_Fault.Data;
using Report_a_Fault.Interface;
using Report_a_Fault.Models;
using Report_a_Fault.Repositories;

namespace Report_a_Fault.Repositories
{
    public class CampusRepo : Repository<Campus>, ICampus
    {
        public CampusRepo(SqlDbContext context) : base(context)
        {
        }
    }
}
