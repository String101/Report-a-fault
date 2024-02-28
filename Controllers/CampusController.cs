
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
            campus.CampusName = campus.CampusName.ToUpper();
            campus.DateOpended = DateTime.Now; 
            _unitOfWork.Campus.Add(campus);
            _unitOfWork.Save();
            return RedirectToAction("Index");
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
            campus.CampusName = campus.CampusName.ToUpper();
            _unitOfWork.Campus.Update(campus);
            _unitOfWork.Save();
           return RedirectToAction("Index");
        }
        public IActionResult Details(string id)
        {
            var campus = _unitOfWork.Campus.Get(c => c.CampusId == id);

            return View(campus);
        }
        [HttpGet]
        public IActionResult Delete(string id)
        {
            var campus = _unitOfWork.Campus.Get(c => c.CampusId == id);

            return View(campus);
        }
        [HttpPost]
        public IActionResult Delete(Campus campus)
        {
            var CampusFromDb = _unitOfWork.Campus.Get(u=>u.CampusId==campus.CampusId);
            _unitOfWork.Campus.Remove(CampusFromDb);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

    }
}
