
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Report_a_Fault.Interface;
using Report_a_Fault.Models;
using Report_a_Fault.ViewModel;

namespace Report_a_Fault.Controllers
{

    public class ComputerCompController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _usermanager;
        public ComputerCompController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> usermanager)
        {
            _unitOfWork = unitOfWork;
            _usermanager = usermanager;
        }
 
        [HttpGet]
        [Authorize(Roles =SD.Role_Intern)]

        public IActionResult Create(string labId, string computerNumber)
        {
            var username = _usermanager.GetUserName(User);

            var user = _unitOfWork.User.Get(x => x.UserName == username, includeProperties: "Campus");
            CompVM compVM = new()
            {
                ComputerList = _unitOfWork.Computer.GetAll(u => u.LabId == labId && u.ComputerNumber == computerNumber, includeProperties: "Lab").OrderBy(u => u.LabId).Select(u => new SelectListItem

                {

                    Text = u.ComputerNumber.ToString(),
                    Value = u.Id.ToString(),
                })
            };

            return View(compVM);
        }
        [HttpPost]
        [Authorize(Roles =SD.Role_Intern)]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CompVM compVM)
        {
            bool computerComponentExists = _unitOfWork.Computer_Comp.Any(u => u.Name == compVM.ComputerComp.Name && u.ComputerId == compVM.ComputerComp.ComputerId);

            if (ModelState.IsValid && !computerComponentExists)
            {
                _unitOfWork.Computer_Comp.Add(compVM.ComputerComp);
                _unitOfWork.Save();
                var computer = _unitOfWork.Computer.Get(u => u.Id == compVM.ComputerComp.ComputerId, includeProperties: "Lab");
                var labNumber = computer.Lab.Id;
                return RedirectToAction("Index", "Computer", new { id = labNumber });

            }
            else
            {
                var computer = _unitOfWork.Computer.Get(u => u.Id == compVM.ComputerComp.ComputerId, includeProperties: "Lab");
                var labNumber = computer.Lab.Id;
                return RedirectToAction("Index", "Computer", new { id = labNumber });
            }
           

            
        }
    }
}
