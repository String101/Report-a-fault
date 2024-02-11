using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Report_a_Fault.Interface;
using Report_a_Fault.Models;
using Report_a_Fault.ViewModel;

namespace Report_a_Fault.Controllers
{
    public class BuildingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _usermanager;

        public BuildingController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> usermanager)
        {
            _unitOfWork = unitOfWork;
            _usermanager = usermanager;
        }
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Super_Admin},{SD.Role_Intern},{SD.Role_Student_Assistant}")]
        public IActionResult Index()
        {
            var username = _usermanager.GetUserName(User);
            var currentUser =_unitOfWork.User.Get(u=>u.Email==username);
            var buildings = _unitOfWork.Building.GetAll(includeProperties:"Campus").Where(u=>u.CampusId==currentUser.CampusId);
            return View(buildings);
        }
        [HttpGet]
        [Authorize(Roles =SD.Role_Intern)]
        public IActionResult Create()
        {
            var username = _usermanager.GetUserName(User);
            var currentUser = _unitOfWork.User.Get(u => u.Email == username);
            Building building = new()
            {
                CampusId = currentUser.CampusId,
            };
            return View(building);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Intern)]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Building building)
        {
            ModelState.Remove("BuildingId");
            if (ModelState.IsValid)
            {
                building.BuildingId = Guid.NewGuid().ToString();
                _unitOfWork.Building.Add(building);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return View(building);
            }
      
        }
        [HttpGet]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Super_Admin},{SD.Role_Intern},{SD.Role_Student_Assistant}")]
        public IActionResult Details(string id)
        {
            var building = _unitOfWork.Building.Get(u => u.BuildingId == id,includeProperties:"Campus") ;
            BuildingLabNumbersVM vM = new()
            {
                Building = building,
                NumberOfLabs = _unitOfWork.Lab.GetAll(u => u.BuildingId == id).Count(),
            };
            return View(vM);
        }
        [HttpGet]
        [Authorize(Roles =SD.Role_Intern)]
        public IActionResult Update(string id)
        {
            var building = _unitOfWork.Building.Get(u => u.BuildingId == id, includeProperties: "Campus");
          
            return View(building);
        }
        [HttpPost]
        [Authorize(Roles =SD.Role_Intern)]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Building building)
        {
            if(ModelState.IsValid)
            {
                _unitOfWork.Building.Update(building);
                _unitOfWork.Save();
            }


            return RedirectToAction(nameof(Index));
        }
    }
}
