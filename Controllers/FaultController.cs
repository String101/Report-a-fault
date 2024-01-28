
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Report_a_Fault.Interface;
using Report_a_Fault.Models;

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
        [Authorize(Roles = $"{SD.Role_Super_Admin},{SD.Role_Intern}")]
        public IActionResult GetAllFault()
        {
            var username = _usermanager.GetUserName(User);

            var user = _unitOfWork.User.Get(x => x.UserName == username);
            var faults = _unitOfWork.Fault.GetAll(u=>u.Computer.ComputerComponentStatus==SD.StatusFaulty || u.Computer.ComputerComponentStatus==SD.StatusFixInProgress,includeProperties: "Computer").OrderBy(u=>u.Computer.CreatedDate);

            foreach(var f in faults)
            {
                f.Computer.Lab=_unitOfWork.Lab.Get(o=>o.Id==f.Computer.LabId);
                f.Computer.Lab.Campus = _unitOfWork.Campus.Get(u => u.CampusId == f.Computer.Lab.CampusId);
            }

            var list = faults.Where(u => u.Computer.Lab.CampusId == user.CampusId);

            return View(list);
        }
        [HttpGet]
        [Authorize(Roles = SD.Role_Student_Assistant)]
        public IActionResult ReportFault(string id)
        {
            var computerFromDb = _unitOfWork.Computer.Get(u=>u.Id==id, includeProperties:"Lab");
            Fault fault = new();
            fault.computerId= computerFromDb.Id;
            fault.Computer = computerFromDb;
            return View(fault);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Student_Assistant)]
        public IActionResult ReportFault(Fault fault)
        {
            fault.Id= Guid.NewGuid().ToString();
              _unitOfWork.Fault.Add(fault);
          var computer=  _unitOfWork.Computer.Get(u => u.Id == fault.computerId,includeProperties: "Lab,Computer_Comps");
            computer.ComputerComponentStatus = SD.StatusFaulty;
            _unitOfWork.Computer.Update(computer);
            _unitOfWork.Save();
            return RedirectToAction(nameof(GetAllFault));
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Intern}")]
        public IActionResult UpdateComputerStatus(string id)
        {
            var computer = _unitOfWork.Computer.Get(u => u.Id == id, includeProperties: "Lab,Computer_Comps");
            return View(computer);
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Intern}")]
        public IActionResult UpdateComputerStatus(Computer computer)
        {

            var computerFromDb = _unitOfWork.Computer.Get(u => u.Id == computer.Id, includeProperties: "Lab,Computer_Comps");
            computerFromDb.ComputerComponentStatus = computer.ComputerComponentStatus;

            _unitOfWork.Computer.Update(computerFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(GetAllFault));
        }
    }
}
