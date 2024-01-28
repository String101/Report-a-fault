
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
        public IActionResult Index(string id,string computerNumber)
        {
            var username = _usermanager.GetUserName(User);

            var user = _unitOfWork.User.Get(x => x.UserName == username);
            var computerComp = _unitOfWork.Computer_Comp.GetAll(u=>u.Computer.Lab.CampusId==user.CampusId&&u.Computer.LabId==id &&u.Computer.ComputerNumber==computerNumber,includeProperties: "Computer");

            foreach (var com in computerComp)
            {
                com.Computer.Lab = _unitOfWork.Lab.Get(u => u.Id == com.Computer.LabId);
                com.Computer.Lab.Campus = _unitOfWork.Campus.Get(o => o.CampusId == com.Computer.Lab.CampusId);
            }
            return View(computerComp);
        }
      
        [HttpGet]
        
        public IActionResult Create()
        {
            var username = _usermanager.GetUserName(User);

            var user = _unitOfWork.User.Get(x => x.UserName == username, includeProperties: "Campus");
            CompVM compVM = new()
            {
                ComputerList = _unitOfWork.Computer.GetAll(u=>u.Lab.CampusId==user.CampusId,includeProperties:"Lab").OrderBy(u=>u.LabId).Select(u => new SelectListItem
                {
                    
                    Text = user.Campus.CampusName +"\\"+u.Lab.LabNumber+"\\"+u.ComputerNumber.ToString(),
                    Value = u.Id.ToString(),
                })
            };

            return View(compVM);
        }
        [HttpPost]
       
        public IActionResult Create(CompVM compVM)
        {
            bool computerComponentExists = _unitOfWork.Computer_Comp.Any(u => u.Name == compVM.ComputerComp.Name && u.ComputerId == compVM.ComputerComp.ComputerId);

            if(ModelState.IsValid&& !computerComponentExists)
            {
                _unitOfWork.Computer_Comp.Add(compVM.ComputerComp);
                _unitOfWork.Save();
                return RedirectToAction("Index", "Lab");
            }


            return RedirectToAction(nameof(Index));
        }
    }
}
