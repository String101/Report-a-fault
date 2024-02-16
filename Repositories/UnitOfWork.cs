

using Report_a_Fault.Data;
using Report_a_Fault.Interface;

namespace Report_a_Fault.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly SqlDbContext _db;

        
        public ILab Lab { get; private set; }

        public IComputer Computer { get; private set; }

        public IComputer_Comp Computer_Comp { get; private set; }

        public IFault Fault { get; private set; }

        public IApplicationUser User { get; private set; }

        public ICampus Campus { get; private set; }

        public IDepartment Department { get; private set; }

        public IBuilding Building { get; private set; }

        public IAssign Assign { get; private set; }

        public UnitOfWork(SqlDbContext db)
        {
            _db = db;
            Lab = new LabRepo(_db);
            Computer = new ComputerRepo(_db);
            Computer_Comp= new Computer_CompRepo(_db);
            Fault = new FaultRepo(_db);
            User = new ApplicationUserRepo(_db);
            Campus = new CampusRepo(_db);
           Department = new DepartmentRepo(_db);
            Building = new BuildingRepo(_db);
            Assign = new AssignRepo(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
