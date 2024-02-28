using Microsoft.AspNetCore.Mvc;
using Report_a_Fault.Interface;
using Report_a_Fault.Models;

namespace Report_a_Fault.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var departments = _unitOfWork.Department.GetAll();

            return View(departments);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(Department department)
        {
            department.DepartmentId = Guid.NewGuid().ToString();
            department.DepartmentName = department.DepartmentName.ToUpper();
            _unitOfWork.Department.Add(department);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(string id)
        {
            var campus = _unitOfWork.Department.Get(c => c.DepartmentId == id);

            return View(campus);
        }
        [HttpPost]
        public IActionResult Delete(Department department)
        {
            var DepartmentFromDb = _unitOfWork.Department.Get(u => u.DepartmentId == department.DepartmentId);
            _unitOfWork.Department.Remove(DepartmentFromDb);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Update(string id)
        {
            var department = _unitOfWork.Department.Get(c => c.DepartmentId == id);

            return View(department);
        }
        [HttpPost]
        public IActionResult Update(Department department)
        {
            department.DepartmentName = department.DepartmentName.ToUpper();
            _unitOfWork.Department.Update(department);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
