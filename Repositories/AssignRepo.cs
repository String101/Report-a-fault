using Report_a_Fault.Data;
using Report_a_Fault.Interface;
using Report_a_Fault.Models;

namespace Report_a_Fault.Repositories
{
    public class AssignRepo : Repository<AssignJob>, IAssign
    {
        public AssignRepo(SqlDbContext context) : base(context)
        {
        }
    }
}
