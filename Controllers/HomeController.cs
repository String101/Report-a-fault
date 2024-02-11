using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Report_a_Fault.Interface;
using Report_a_Fault.Models;
using Report_a_Fault.ViewModel;
using System.Diagnostics;

namespace Report_a_Fault.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _usermanager;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork, UserManager<ApplicationUser> usermanager)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _usermanager = usermanager;
        }

        public IActionResult Index()
        {
            var username = _usermanager.GetUserName(User);

            var user = _unitOfWork.User.Get(x => x.UserName == username);
            var faults = _unitOfWork.Fault.GetAll();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
