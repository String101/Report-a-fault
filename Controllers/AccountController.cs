
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Report_a_Fault.Data;
using Report_a_Fault.Interface;
using Report_a_Fault.Models;
using Report_a_Fault.ViewModel;



namespace Report_a_Fault.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> usermanager, SignInManager<ApplicationUser> signinManager, ILogger<AccountController> logger,IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager)
        {
            _usermanager = usermanager;
            _signinManager = signinManager;
            _roleManager = roleManager;
            _logger = logger;
            _unitOfWork = unitOfWork;

        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

          
            return RedirectToAction("Index","Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            loginVM.RedirectUrl??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user =_unitOfWork.User.Get(u=>u.Email==loginVM.Email);


                var result = await _signinManager.
                    PasswordSignInAsync(loginVM.Email, loginVM.Password, false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(loginVM.RedirectUrl))
                    {
                        if (User.IsInRole(SD.Role_Admin))
                        {
                            return RedirectToAction("Index", "Building");
                        }
                        else if (User.IsInRole(SD.Role_Intern))
                        {
                            return RedirectToAction("Index", "Building");
                        }
                        else if (User.IsInRole(SD.Role_Student_Assistant))
                        {
                            return RedirectToAction("Index", "Building");
                        }
                        else if (User.IsInRole(SD.Role_Super_Admin))
                        {
                            return RedirectToAction("Index", "Building");
                        }
                    }
                    else
                    {
                        return NotFound();
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Invalid login credentials.");
                    TempData["error"] = "Invalid login credentials.";
                    TempData["error"] = "Intern is already assigned to this fault.";
                    return RedirectToAction("Index", "Home");
                }

            }

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        } 

         [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _usermanager.FindByNameAsync(model.Email);

                if (user != null && await _usermanager.IsEmailConfirmedAsync(user))
                {
                    var token = await _usermanager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);

                    _logger.Log(LogLevel.Warning, passwordResetLink);

                 

                    return RedirectToAction("Index","Home");
                }

                return View("ForgetPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password rest token");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ResetPasswordVM model = new()
                {
                    Token = token,
                    Email = email,
                };
                return View(model);
            }
          
          
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _usermanager.FindByNameAsync(model.Email);
                if (user is not null)
                {
                    var result = await _usermanager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Super_Admin}")]
        public IActionResult AddUser()
        {
            AddUserVM vm = new ()
            {
                CampusList = _unitOfWork.Campus.GetAll().Select(u => new SelectListItem
                {
                    Text = u.CampusName,
                    Value = u.CampusId,
                }),
                DepartmentList =_unitOfWork.Department.GetAll().Select(d => new SelectListItem
                {
                    Text = d.DepartmentName.ToUpper(),
                    Value = d.DepartmentId,
                })
                
            };
            return View(vm);
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Super_Admin}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(AddUserVM addUserVM)
        {
            if (ModelState.IsValid)
            {
                var password = "";
                    ApplicationUser user = new()
                    {
                        UserName = addUserVM.Email,
                        Email = addUserVM.Email,
                        Name = addUserVM.Name.ToUpper(),
                        Usersurname = addUserVM.Surname.ToUpper(),
                        EmailConfirmed = true,
                        NormalizedEmail = addUserVM.Email,
                        Role = addUserVM.Role,
                        CampusId= addUserVM.CampusId,
                        DepartmentId=addUserVM.DepartmentId,
                    };
                  if(user.Role==SD.Role_Intern)
                  {
                    password = "Cut@" + SD.Role_Intern + "01";
                  }
                  else if (user.Role == SD.Role_Student_Assistant)
                  {
                    password = "Cut*Assistant01";
                  }
                  else if (user.Role == SD.Role_Admin)
                  {
                    password = "Cut$" + SD.Role_Admin + "#02";
                  }
                
                var result = await _usermanager.CreateAsync(user,password);
                    if (result.Succeeded)
                    {
                    if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                    {
                        _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait();
                        _roleManager.CreateAsync(new IdentityRole(SD.Role_Intern)).Wait();
                        _roleManager.CreateAsync(new IdentityRole(SD.Role_Student_Assistant)).Wait();
                    }

                     await _usermanager.AddToRoleAsync(user, addUserVM.Role);

                     TempData["Success"] = "New user registered.";

                    return RedirectToAction("Index", "Building");
                       
                       

                    }
                



            }

            return View();
        }
        [Authorize(Roles =$"{SD.Role_Admin},{SD.Role_Super_Admin}")]
        public IActionResult GetAllEmployees()
        {
            var username = _usermanager.GetUserName(User);
            var user =_unitOfWork.User.Get(u=>u.Email==username);

            if(user.Role==SD.Role_Admin)
            {
                var employeess = _unitOfWork.User.GetAll(u => u.Role != SD.Role_Super_Admin&&u.Role!=SD.Role_Admin && u.CampusId==user.CampusId, includeProperties: "Campus,Department").OrderBy(i => i.Role);
                return View("GetAllEmployees", employeess);
            }
            var employees = _unitOfWork.User.GetAll(u => u.Role != SD.Role_Super_Admin, includeProperties: "Campus,Department").OrderBy(i => i.Role);
            return View("GetAllEmployees", employees);

        }
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Super_Admin}")]
        public bool BlockUser(string email)
        {
            var user =_unitOfWork.User.Get(u=>u.Email == email);
            user.Enabled = false;
            _unitOfWork.User.Update(user);
            _unitOfWork.Save();

            return user.Enabled;
        }
        public IActionResult AccessDenied()
        {

            return View();
        }
        [HttpGet]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Super_Admin}")]
        public IActionResult DeleteUser(string id)
        {
            var user = _unitOfWork.User.Get(u => u.Id == id,includeProperties:"Campus,Department");
            return View(user);
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Super_Admin}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(ApplicationUser user)
        {
            

            var userToDelete = _unitOfWork.User.Get(u => u.Email == user.Email);

            if (userToDelete != null)
            {
                var result = await _usermanager.DeleteAsync(userToDelete);

                if (result.Succeeded)
                {
                    // User deleted successfully
                    return RedirectToAction("Index", "Building");
                }
                else
                {
                    // Handle errors

                    return RedirectToAction("Index", "Building");
                }
            }
            else
            {
                // User not found
                return RedirectToAction("Index", "Building");
            }
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Super_Admin}")]
        public ActionResult UpdateEnabled(string id, bool updatedEnabled)
        {
            // Retrieve the item by id from your data source
            var item = _unitOfWork.User.Get(u=>u.Id==id);

            if (item != null)
            {
                // Update the boolean value
                item.Enabled = updatedEnabled;

                // Save changes to your data source
                _unitOfWork.Save();
            }

            // Return a JSON result or an empty result
            return Json(new { success = true });
        }

    }
}
