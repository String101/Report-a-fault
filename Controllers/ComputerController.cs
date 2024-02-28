
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
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Super_Admin},{SD.Role_Intern},{SD.Role_Student_Assistant}")]
        public IActionResult Index(string id,string search,int? pageNumber)
        {
            pageNumber = pageNumber ?? 1;
            int pageSize = 10;
            var computers = _unitOfWork.Computer.GetAll(u =>u.Lab.Id == id, includeProperties: "Lab,Computer_Comps").OrderBy(u => u.CreatedDate);
            foreach (var computer in computers)
            {
                computer.Lab = _unitOfWork.Lab.Get(u => u.Id == computer.LabId);
            }
            if(search != null)
            {
                SearchComputerVM vM = new()
                {
                    Computers = PaginatedList<Computer>.Create(computers.AsQueryable().Where(u => u.ComputerNumber == search.Trim()).OrderBy(u => u.ComputerNumber), pageNumber ?? 1, pageSize),
                    LabIdentityNumber = id,
                };
                return View(vM);
            }
            else
            {
                SearchComputerVM vM = new()
                {
                    Computers = PaginatedList<Computer>.Create(computers.AsQueryable().OrderBy(i=>i.ComputerNumber), pageNumber ?? 1, pageSize),
                    LabIdentityNumber = id,
                };
                return View(vM);
            }
            

        }
        [HttpGet]
        [Authorize(Roles = SD.Role_Intern)]
        public IActionResult Create(string id)
        {

           
            ComputerVM computerVM = new()
            {
                BuildingList = _unitOfWork.Lab.GetAll(u => u.Id == id).OrderBy(u => u.LabNumber).Select(u => new SelectListItem
                {
                    Text = u.LabNumber.ToString(),
                    Value = u.Id,
                })
            };

            return View(computerVM);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Intern)]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ComputerVM computerVM)
        {
            computerVM.Computer.Id = Guid.NewGuid().ToString();
            bool computerNumberExists = _unitOfWork.Computer.Any(u => u.ComputerNumber == computerVM.Computer.ComputerNumber && u.LabId == computerVM.Computer.LabId);
            ModelState.Remove("Id");
            if (ModelState.IsValid && !computerNumberExists)
            {
                _unitOfWork.Computer.Add(computerVM.Computer);
                _unitOfWork.Save();
            }

            var lab = _unitOfWork.Lab.Get(u => u.Id == computerVM.Computer.LabId);
            string labId = lab.Id;
            return RedirectToAction("Index", new { id = labId });
        }
        [HttpGet]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Super_Admin},{SD.Role_Intern},{SD.Role_Student_Assistant}")]
        public IActionResult Details(string id)
        {
            var computer = _unitOfWork.Computer.Get(u => u.Id == id, includeProperties: "Lab,Computer_Comps");
            computer.Lab.Building = _unitOfWork.Building.Get(u => u.BuildingId == computer.Lab.BuildingId);
            return View(computer);
        }
        [HttpGet]
        [Authorize(Roles =SD.Role_Intern)]
        public IActionResult Delete(string id)
        {
            var computer = _unitOfWork.Computer.Get(u => u.Id == id, includeProperties: "Lab,Computer_Comps");
            computer.Lab.Building=_unitOfWork.Building.Get(u=>u.BuildingId == computer.Lab.BuildingId);
            computer.Lab.Building.Campus = _unitOfWork.Campus.Get(u => u.CampusId == computer.Lab.Building.CampusId);
            return View(computer);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Intern)]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Computer computer)
        {
            var computerFromDb = _unitOfWork.Computer.Get(u => u.Id == computer.Id, includeProperties: "Lab,Computer_Comps");
            _unitOfWork.Computer.Remove(computerFromDb);
            _unitOfWork.Save();
            var labId = computerFromDb.LabId;
            return RedirectToAction("Index", new { id = labId });
        }
        [HttpGet]
        public IActionResult AddRange(string id)
        {
            AddComputerRangeVM computerVM = new()
            {
                BuildingList = _unitOfWork.Lab.GetAll(u => u.Id == id).OrderBy(u => u.LabNumber).Select(u => new SelectListItem
                {
                    Text = u.LabNumber.ToString(),
                    Value = u.Id,
                })
            };
            return View(computerVM);
        }
        [HttpPost]
        public IActionResult AddRange(string labId, int numberOfComputer)
        {
            for (int i = 1; i <= numberOfComputer; i++)
            {
                if (i < 10)
                {
                    Computer computer = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        LabId = labId,
                        ComputerNumber = "0"+i.ToString(),
                    };
                    bool computerExist = _unitOfWork.Computer.Any(u => u.ComputerNumber == computer.ComputerNumber && u.LabId == computer.LabId);
                    if (!computerExist)
                    {
                        _unitOfWork.Computer.Add(computer);
                    }
                }
                else
                {
                    Computer computer = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        LabId = labId,
                        ComputerNumber = i.ToString(),
                    };
                    bool computerExist = _unitOfWork.Computer.Any(u=>u.ComputerNumber==computer.ComputerNumber&&u.LabId==computer.LabId);
                    if (!computerExist)
                    {
                        _unitOfWork.Computer.Add(computer);
                    }
                  
                }
             
                
            }
            _unitOfWork.Save();
            return RedirectToAction("Index", new { id = labId });
            
        }
    }
}
