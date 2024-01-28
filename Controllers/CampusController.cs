
using Microsoft.AspNetCore.Mvc;
using Report_a_Fault.Interface;
using Report_a_Fault.Models;

namespace Report_a_Fault.Controllers
{
    public class CampusController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CampusController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var campuses = _unitOfWork.Campus.GetAll();

            return View(campuses);
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(Campus campus)
        {
            campus.CampusId= Guid.NewGuid().ToString();
            _unitOfWork.Campus.Add(campus);
            _unitOfWork.Save();
            return View();
        }
        [HttpGet]
        public IActionResult Update(string id)
        {
            var campus = _unitOfWork.Campus.Get(c => c.CampusId == id);

            return View(campus);
        }
        [HttpPost]
        public IActionResult Update(Campus campus)
        {
         _unitOfWork.Campus.Update(campus);
            _unitOfWork.Save();
            return View(campus);
        }

    }
}
