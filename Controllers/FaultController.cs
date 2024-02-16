
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Report_a_Fault.Interface;
using Report_a_Fault.Models;
using Report_a_Fault.ViewModel;

namespace Report_a_Fault.Controllers
{

    public class FaultController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _usermanager;

        public FaultController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> usermanager)
        {
            _unitOfWork = unitOfWork;
            _usermanager = usermanager;
        }
        [Authorize(Roles = $"{SD.Role_Super_Admin},{SD.Role_Intern},{SD.Role_Admin},{SD.Role_Student_Assistant}")]
        public IActionResult GetAllFault()
        {
            var username = _usermanager.GetUserName(User);

            var user = _unitOfWork.User.Get(x => x.UserName == username);
            var faults = _unitOfWork.Fault.GetAll(includeProperties: "Computer").OrderBy(u => u.Computer.CreatedDate);

            foreach (var f in faults)
            {
                f.Computer.Lab = _unitOfWork.Lab.Get(o => o.Id == f.Computer.LabId);
                f.Computer.Lab.Building = _unitOfWork.Building.Get(u => u.BuildingId == f.Computer.Lab.BuildingId);
            }

            var list = faults.Where(u => u.Computer.Lab.Building.CampusId == user.CampusId);

            return View(list);
        }
        [HttpGet]
        [Authorize(Roles =SD.Role_Student_Assistant)]
        public IActionResult ReportFault(string id)
        {
            var username = _usermanager.GetUserName(User);
            var computerFromDb = _unitOfWork.Computer.Get(u => u.Id == id, includeProperties: "Lab");
            Fault fault = new();
            fault.computerId = computerFromDb.Id;
            fault.Computer = computerFromDb;
            fault.StudentEmail = username!;

            bool faultExist = _unitOfWork.Fault.Any(u => u.computerId == fault.computerId);

            if (!faultExist)
            {
                return View(fault);
            }
            else
            {
                TempData["error"] = "Fault already reported.";
                return RedirectToAction("Index", "Building");
            }

        }
        [HttpPost]
        [Authorize(Roles =SD.Role_Student_Assistant)]
        [ValidateAntiForgeryToken]
        public IActionResult ReportFault(Fault fault)
        {
            if (fault != null)
            {
                fault.Id = Guid.NewGuid().ToString();
                _unitOfWork.Fault.Add(fault);
                var computer = _unitOfWork.Computer.Get(u => u.Id == fault.computerId, includeProperties: "Lab,Computer_Comps");
                computer.ComputerComponentStatus = SD.StatusFaulty;
                _unitOfWork.Computer.Update(computer);
                _unitOfWork.Save();
                return RedirectToAction("Index", "Building");
            }
            else
            {
                TempData["error"] = "Something went wrong";
                return RedirectToAction("Index", "Building");
            }
           
        }

        [HttpGet]
        [Authorize(Roles = SD.Role_Intern)]
        public IActionResult UpdateComputerStatus(string id)
        {
            var computer = _unitOfWork.Computer.Get(u => u.Id == id, includeProperties: "Lab,Computer_Comps");
            return View(computer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles =SD.Role_Intern)]
        public IActionResult UpdateComputerStatus(Computer computer)
        {
            var username = _usermanager.GetUserName(User);

            var computerFromDb = _unitOfWork.Computer.Get(u => u.Id == computer.Id, includeProperties: "Lab,Computer_Comps");
            computerFromDb.ComputerComponentStatus = computer.ComputerComponentStatus;
            var computerFault = _unitOfWork.Fault.Get(u => u.computerId == computerFromDb.Id);
            computerFault.InternEmail = username;
            computerFault.DateUpdate = DateTime.Now;
            _unitOfWork.Computer.Update(computerFromDb);
            var p = _unitOfWork.Assign.Get(u => u.FaultId == computerFault.Id);
            if (computerFromDb.ComputerComponentStatus == SD.StatusHealty)
            {
                _unitOfWork.Assign.Remove(p);
               
                _unitOfWork.Fault.Remove(computerFault);
               
            }


            _unitOfWork.Save();
            return RedirectToAction(nameof(GetAllFault));
        }

        [HttpGet]
        
        public IActionResult Details(string id)
        {
            var computer = _unitOfWork.Fault.Get(u => u.Id == id, includeProperties: "Computer");
            computer.Computer.Lab = _unitOfWork.Lab.Get(u => u.Id == computer.Computer.LabId);
            computer.Computer.Lab.Building = _unitOfWork.Building.Get(u => u.BuildingId == computer.Computer.Lab.BuildingId);
            computer.Computer.Lab.Building.Campus= _unitOfWork.Campus.Get(u=>u.CampusId==computer.Computer.Lab.Building.CampusId);
            computer.Computer.Computer_Comps = _unitOfWork.Computer_Comp.GetAll(u => u.ComputerId == computer.computerId);
            return View(computer);
        }
        [HttpGet]
        [Authorize(Roles = $"{SD.Role_Super_Admin},{SD.Role_Admin}")]
        public IActionResult AssignJobToIntern(string id)
        {
            var username = _usermanager.GetUserName(User);
            var currentUser = _unitOfWork.User.Get(u => u.Email == username);
            AssignJob job = new()
            {
                FaultId = id,
                Fault = _unitOfWork.Fault.Get(u => u.Id == id, includeProperties: "Computer"),
            };
            
            job.Fault.Computer = _unitOfWork.Computer.Get(o => o.Id == job.Fault.computerId, includeProperties: "Lab,Computer_Comps");
            job.Fault.Computer.Lab=_unitOfWork.Lab.Get(u=>u.Id==job.Fault.Computer.LabId, includeProperties: "Building");
            job.Fault.Computer.Lab.Building.Campus = _unitOfWork.Campus.Get(i => i.CampusId == currentUser.CampusId);
            AssignVM vM = new()
            {
              FaultAssign=job,
                EmployeeList = _unitOfWork.User.GetAll(u => u.Role == SD.Role_Intern && u.CampusId == currentUser.CampusId).Select(u => new SelectListItem
                {
                    Text = u.Name + "\t" + u.Usersurname,
                    Value = u.Id,
                })
            };

         
            return View(vM);
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Super_Admin},{SD.Role_Admin}")]
        [ValidateAntiForgeryToken]
        public IActionResult AssignJobToIntern(AssignVM vM)
        {
          
            if(ModelState.IsValid)
            {
                vM.FaultAssign.Id = Guid.NewGuid().ToString();
                _unitOfWork.Assign.Add(vM.FaultAssign);
                _unitOfWork.Save();
                return RedirectToAction("Index", "Building");
            }
            
            return View(vM);
        }
        [HttpGet]
        public IActionResult Jobs()
        {
            var username = _usermanager.GetUserName(User);
            var currentUser = _unitOfWork.User.Get(u => u.Email == username);
            if(currentUser.Role==SD.Role_Intern)
            {
                var jobs = _unitOfWork.Assign.GetAll(u=>u.InternId==currentUser.Id,includeProperties: "Fault,Intern");
                foreach (var job in jobs)
                {
                    job.Fault.Computer = _unitOfWork.Computer.Get(u => u.Id == job.Fault.computerId);
                    job.Fault.Computer.Lab = _unitOfWork.Lab.Get(u => u.Id == job.Fault.Computer.LabId);
                    job.Fault.Computer.Lab.Building = _unitOfWork.Building.Get(o => o.BuildingId == job.Fault.Computer.Lab.BuildingId);
                }
                return View(jobs);
            }
            else
            {
                var jobs = _unitOfWork.Assign.GetAll(includeProperties: "Fault,Intern");
                foreach(var job in jobs)
                {
                    job.Fault.Computer = _unitOfWork.Computer.Get(u => u.Id == job.Fault.computerId);
                    job.Fault.Computer.Lab = _unitOfWork.Lab.Get(u => u.Id == job.Fault.Computer.LabId);
                    job.Fault.Computer.Lab.Building = _unitOfWork.Building.Get(o => o.BuildingId == job.Fault.Computer.Lab.BuildingId);
                }
                var newJobs = jobs.Where(u => u.Fault.Computer.Lab.Building.CampusId == currentUser.CampusId);
                return View(newJobs);
            }
        
         

        }
    }
}
