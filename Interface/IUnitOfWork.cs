namespace Report_a_Fault.Interface
{
    public interface IUnitOfWork
    {
        ILab Lab { get;}
        IComputer Computer { get;}
        IComputer_Comp Computer_Comp { get;}
        IFault Fault { get;}
        IApplicationUser User { get;}
        ICampus Campus { get;}
        IDepartment Department { get;}
        IBuilding Building { get;}
    
        void Save();
    }
}
