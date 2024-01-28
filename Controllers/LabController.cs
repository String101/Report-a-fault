
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
        public IActionResult Index()
        {
            var username = _usermanager.GetUserName(User);

            var user = _unitOfWork.User.Get(x => x.UserName == username);
            var labs=_unitOfWork.Lab.GetAll(c=>c.CampusId==user.CampusId,includeProperties:"Campus");
            return View(labs);
        }
        [HttpGet]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Super_Admin}")]
        public IActionResult create() 
        {
            var username = _usermanager.GetUserName(User);
            var user = _unitOfWork.User.Get(x => x.UserName == username);
            CompLabVM compLabVM = new()
            {
                CampusList = _unitOfWork.Campus.GetAll(c => c.CampusId == user.CampusId).Select(u => new SelectListItem
                {
                    Text = u.CampusName,
                    Value = u.CampusId,
                })
            };
            return View(compLabVM);
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Super_Admin}")]
        public IActionResult create(CompLabVM lab)
        {
            bool labExist = _unitOfWork.Lab.Any(u => u.LabNumber == lab.Labs.LabNumber && u.CampusId == lab.Labs.CampusId);
            if (!labExist)
            {
                lab.Labs.Id = Guid.NewGuid().ToString();
                _unitOfWork.Lab.Add(lab.Labs);
                _unitOfWork.Save();
            }
           

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Super_Admin}")]
        public IActionResult Edit(string id)
        {
            var lab=_unitOfWork.Lab.Get(u=>u.Id==id);
            return View(lab);
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Super_Admin}")]
        public IActionResult Edit(Labs lab)
        {
            _unitOfWork.Lab.Update(lab);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
            
        }
        [HttpGet]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Super_Admin}")]

        public IActionResult Delete(string id)
        {
            var lab = _unitOfWork.Lab.Get(u => u.Id == id);
            return View(lab);
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Super_Admin}")]
        public IActionResult Delete(Labs lab)
        {
            _unitOfWork.Lab.Remove(lab);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }
    }
}
