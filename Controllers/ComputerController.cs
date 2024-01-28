
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Report_a_Fault.Interface;
using Report_a_Fault.Models;
using Report_a_Fault.ViewModel;

namespace Report_a_Fault.Controllers
{
    
    public class ComputerController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _usermanager;

        public ComputerController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> usermanager)
        {
            _unitOfWork = unitOfWork;
            _usermanager = usermanager;
        }
        public IActionResult Index(int id)
        {
            var username = _usermanager.GetUserName(User);

            var user = _unitOfWork.User.Get(x => x.UserName == username);
            var computers = _unitOfWork.Computer.GetAll(u=>u.Lab.CampusId==user.CampusId&&u.Lab.LabNumber==id,includeProperties: "Lab,Computer_Comps").OrderBy(u=>u.CreatedDate);
            foreach (var computer in computers)
            {
                computer.Lab.Campus = _unitOfWork.Campus.Get(u => u.CampusId == user.CampusId);
            }
            return View(computers);
        }
        [HttpGet]
        [Authorize(Roles = $"{SD.Role_Super_Admin},{SD.Role_Intern}")]
        public IActionResult Create()
        {

            var username = _usermanager.GetUserName(User);

            var user = _unitOfWork.User.Get(x => x.UserName == username);
            ComputerVM computerVM = new()
            {
                LabList = _unitOfWork.Lab.GetAll(u=>u.CampusId==user.CampusId).Select(u=> new SelectListItem
                {
                   Text=u.LabNumber.ToString(),
                   Value=u.Id,
                })
            };

            return View(computerVM);
        }
     
        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Super_Admin},{SD.Role_Intern}")]
        public IActionResult Create(ComputerVM computerVM)
        {
            computerVM.Computer.Id= Guid.NewGuid().ToString();
            bool computerNumberExists = _unitOfWork.Computer.Any(u => u.ComputerNumber == computerVM.Computer.ComputerNumber && u.LabId==computerVM.Computer.LabId);
            ModelState.Remove("Id");
            if(ModelState.IsValid && !computerNumberExists)
            {
                _unitOfWork.Computer.Add(computerVM.Computer);
                _unitOfWork.Save();
            }

            var lab = _unitOfWork.Lab.Get(u => u.Id == computerVM.Computer.LabId);
            int labId = lab.LabNumber;
            return RedirectToAction("Index", new { id = labId });
        }
        [HttpGet]

        public IActionResult Details(string id)
        {
            var computer = _unitOfWork.Computer.Get(u => u.Id == id, includeProperties: "Lab,Computer_Comps");
            return View(computer);
        }
        [HttpGet]
        public IActionResult Delete(string id)
        {
            var computer = _unitOfWork.Computer.Get(u => u.Id == id, includeProperties: "Lab");
            return View(computer);
        }
        [HttpPost]
        public IActionResult Delete(Computer computer)
        {
            var computerFromDb = _unitOfWork.Computer.Get(u => u.Id == computer.Id, includeProperties: "Lab");
            _unitOfWork.Computer.Remove(computerFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

    }
}
