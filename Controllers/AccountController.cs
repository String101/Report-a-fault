
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

            LoginVM loginVM = new()
            {
                RedirectUrl = returnUrl,
            };
            return View(loginVM);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var user =_unitOfWork.User.Get(u=>u.Email==loginVM.Email);

                bool userEnabled = _unitOfWork.User.Any(u => u.Enabled == true);
                if (userEnabled)
                {
                    var result = await _signinManager.
                    PasswordSignInAsync(loginVM.Email, loginVM.Password, loginVM.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(loginVM.RedirectUrl))
                        {
                            if (User.IsInRole(SD.Role_Admin))
                            {
                                return LocalRedirect(loginVM.RedirectUrl);
                            }
                            else if (User.IsInRole(SD.Role_Intern))
                            {
                                return LocalRedirect(loginVM.RedirectUrl);
                            }
                            else if (User.IsInRole(SD.Role_Student_Assistant))
                            {
                                return LocalRedirect(loginVM.RedirectUrl);
                            }
                        }
                        else
                        {
                            return NotFound();
                        }

                    }
                    else
                    {
                        TempData["error"] = "Invalid login credentials.";
                    }
                }
                else
                {
                    TempData["error"] = "Account has been blocked.";
                }

            }

            return View(loginVM);
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Register()
        {
            RegisterVM registerVM = new() 
            {
                CampusList = _unitOfWork.Campus.GetAll().Select(u => new SelectListItem
                {
                    Text = u.CampusName,
                    Value = u.CampusId,
                })

            };

            return View(registerVM);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
         
                    ApplicationUser user = new()
                    {
                        UserName = registerVM.Email,
                        Email = registerVM.Email,
                        Name = registerVM.Name,
                        Usersurname = registerVM.Surname,
                        EmailConfirmed = true,
                        NormalizedEmail = registerVM.Email,
                        Role =registerVM.Role,
                        CampusId = registerVM.CampusId,
                       

                    };
                    var resultForAdmin = await _usermanager.CreateAsync(user, registerVM.Password);
                    if (resultForAdmin.Succeeded)
                    {
                        if(!_roleManager.RoleExistsAsync(SD.Role_Super_Admin).GetAwaiter().GetResult())
                        {
                        _roleManager.CreateAsync(new IdentityRole(SD.Role_Super_Admin)).Wait();
                            _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait();
                            _roleManager.CreateAsync(new IdentityRole(SD.Role_Intern)).Wait();
                            _roleManager.CreateAsync(new IdentityRole(SD.Role_Student_Assistant)).Wait();
                        }
                            await _usermanager.AddToRoleAsync(user,registerVM.Role);
                        await _signinManager.SignInAsync(user, isPersistent: false);
                        if (string.IsNullOrEmpty(registerVM.RedirectUrl))
                        {
                            return RedirectToAction("Index", "Lab");
                        }
                        else
                        {
                            return LocalRedirect(registerVM.RedirectUrl);
                        }
                    }

            }

            return View();
        }

         [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
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

                 

                    return View("ForgetPasswordConfirmation");
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
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
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
                        return View("ResetPasswordConfirmation");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                return View("ResetPasswordConfirmation");
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
                })
            };
            return View(vm);
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Super_Admin}")]
        public async Task<IActionResult> AddUser(AddUserVM addUserVM)
        {
            if (ModelState.IsValid)
            {
               
                    ApplicationUser user = new()
                    {
                        UserName = addUserVM.Email,
                        Email = addUserVM.Email,
                        Name = addUserVM.Name,
                        Usersurname = addUserVM.Surname,
                        EmailConfirmed = true,
                        NormalizedEmail = addUserVM.Email,
                        Role = addUserVM.Role,
                        CampusId= addUserVM.CampusId,
                    };
               
             
                var result = await _usermanager.CreateAsync(user,addUserVM.Password );
                    if (result.Succeeded)
                    {
                    if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                    {
                        _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait();
                        _roleManager.CreateAsync(new IdentityRole(SD.Role_Intern)).Wait();
                        _roleManager.CreateAsync(new IdentityRole(SD.Role_Student_Assistant)).Wait();
                    }

                     await _usermanager.AddToRoleAsync(user, addUserVM.Role);

                       
                        
                            return RedirectToAction("Index", "Home");
                       
                       

                    }
                



            }

            return View();
        }
        [Authorize(Roles =$"{SD.Role_Admin},{SD.Role_Super_Admin}")]
        public IActionResult GetAllEmployees()
        {
            var employees = _unitOfWork.User.GetAll(includeProperties:"Campus");
            return View("GetAllEmployees",employees);
        }

        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> SetPassword(string id)
        {
            if (ModelState.IsValid)
            {
                var user = await _usermanager.FindByNameAsync(id);

                if (user != null && await _usermanager.IsEmailConfirmedAsync(user))
                {
                    var token = await _usermanager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = id, token = token }, Request.Scheme);

                    _logger.Log(LogLevel.Warning, passwordResetLink);

                    ResetPasswordViewM resetPasswordVM = new()
                    {
                        Token = token,
                        Email = id,
                        Password = $"/Cut@" + new Random().Next(100000) + "/",

                    };
                    _logger.Log(LogLevel.Warning, resetPasswordVM.Password);

                    return RedirectToAction("ResetNewPassword",resetPasswordVM) ;
                }

                return View("ForgetPasswordConfirmation");
            }
            return View(id);
        }

        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> ResetNewPassword(ResetPasswordViewM model)
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
                return View("ResetPasswordConfirmation");
            }

            return View(model);
        }

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
    }
}
