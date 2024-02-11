using Report_a_Fault.Data;
using Report_a_Fault.Interface;
using Report_a_Fault.Models;

namespace Report_a_Fault.Repositories
{
    public class BuildingRepo: Repository<Building>, IBuilding
    {
        public BuildingRepo(SqlDbContext context) : base(context)
        {
        }
    }
}
