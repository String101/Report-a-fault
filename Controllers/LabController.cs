
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Report_a_Fault.Interface;
using Report_a_Fault.Models;
using Report_a_Fault.ViewModel;

namespace Report_a_Fault.Controllers
{

    public class LabController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _usermanager;
        public LabController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> usermanager)
        {
            _unitOfWork = unitOfWork;
            _usermanager = usermanager;
        }
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Super_Admin},{SD.Role_Intern},{SD.Role_Student_Assistant}")]
        public IActionResult Index(string buildingId)
        {
            var labs = _unitOfWork.Lab.GetAll(c => c.BuildingId == buildingId, includeProperties: "Building").OrderBy(u => u.LabNumber);
            LabsBuildingVM labsBuildingVM = new()
            {
                Lab = labs,
                BuildindId = buildingId,
            };
            return View(labsBuildingVM);
        }
        [HttpGet]
        public IActionResult create(string buildingId)
        {
            var username = _usermanager.GetUserName(User);
            var user = _unitOfWork.User.Get(x => x.UserName == username);

            Labs labs = new()
            {
                BuildingId = buildingId,
            };
            return View(labs);
        }
        [HttpPost]
       
        public IActionResult create(Labs lab)
        {
            bool labExist = _unitOfWork.Lab.Any(u => u.LabNumber == lab.LabNumber && u.BuildingId == lab.BuildingId);
            if (!labExist)
            {
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                    lab.Id = Guid.NewGuid().ToString();
                    _unitOfWork.Lab.Add(lab);
                    _unitOfWork.Save();
                }
            }
            
         


            return RedirectToAction(nameof(Index), new { buildingId = lab.BuildingId });
        }
        //[HttpGet]
        //[Authorize(Roles = SD.Role_Intern)]
        //public IActionResult Edit(string id)
        //{
        //    var lab = _unitOfWork.Lab.Get(u => u.Id == id);
        //    return View(lab);
        //}
        //[HttpPost]
        //[Authorize(Roles = SD.Role_Intern)]
        //public IActionResult Edit(Labs lab)
        //{
        //    _unitOfWork.Lab.Update(lab);
        //    _unitOfWork.Save();
        //    return RedirectToAction(nameof(Index));

        //}
        [HttpGet]
        [Authorize(Roles = SD.Role_Intern)]

        public IActionResult Delete(string id)
        {
            var lab = _unitOfWork.Lab.Get(u => u.Id == id,includeProperties:"Building");

            return View(lab);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Intern)]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Labs lab)
        {
            _unitOfWork.Lab.Remove(lab);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index), new { buildingId = lab.BuildingId });

        }
        [HttpGet]


        public IActionResult Details(string id)
        {
            var lab = _unitOfWork.Lab.Get(u => u.Id == id, includeProperties: "Building");
            lab.Building.Campus = _unitOfWork.Campus.Get(u => u.CampusId == lab.Building.CampusId);
            LabNumberOfComputer lab1 = new()
            {
                Labs = lab,
                NumberOfComputer = _unitOfWork.Computer.GetAll(u=>u.LabId== id).Count(),

            };
            return View(lab1);
        }
    }
}
