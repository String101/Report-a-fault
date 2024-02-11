using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Report_a_Fault.Interface;
using Report_a_Fault.Models;

namespace Report_a_Fault.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly SqlDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public DbInitializer(SqlDbContext db, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,IUnitOfWork unitOfWork)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        public void Initialize()
        {
            try
            {

                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
                if (!_roleManager.RoleExistsAsync(SD.Role_Super_Admin).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Intern)).Wait();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Super_Admin)).Wait();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Student_Assistant)).Wait();

                    Campus campusW = new()
                    {
                        CampusId = Guid.NewGuid().ToString(),
                        CampusName = "WELKOM"
                    };
                    _unitOfWork.Campus.Add(campusW);

                    Campus campusB = new()
                    {
                        CampusId = Guid.NewGuid().ToString(),
                        CampusName = "BLOEMFONTEIN"
                    };
                    _unitOfWork.Campus.Add(campusB);
                    _unitOfWork.Save();

                    _userManager.CreateAsync(new ApplicationUser
                    {
                        Name="Test",
                        Usersurname="Test1",
                        UserName = "sbongakonkesihle31@gmail.com",
                        Email = "sbongakonkesihle31@gmail.com",
                        EmailConfirmed = true,
                        PhoneNumber = "0657290039",
                        NormalizedUserName = "SBONGAKONKESIHLE31@GMAIL.COM",
                        NormalizedEmail = "SBONGAKONKESIHLE31@GMAIL.COM",
                       CampusId = campusW.CampusId,
                       Role=SD.Role_Super_Admin
                    }, "Developer@Admin12343").GetAwaiter().GetResult();
                    _userManager.CreateAsync(new ApplicationUser
                    {
                        Name = "Test2",
                        Usersurname = "Test2",
                        UserName = "thusisihle01@gmail.com",
                        Email = "thusisihle01@gmail.com",
                        EmailConfirmed = true,
                        PhoneNumber = "0657290039",
                        NormalizedUserName = "THUSISIHLE01@GMAIL.COM",
                        NormalizedEmail = "THUSISIHLE01@GMAIL.COM",
                        CampusId = campusB.CampusId,
                        Role = SD.Role_Super_Admin
                    }, "Developer@Admin12343").GetAwaiter().GetResult();




                    ApplicationUser developer = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "sbongakonkesihle31@gmail.com");
                    _userManager.AddToRoleAsync(developer, SD.Role_Super_Admin).GetAwaiter().GetResult();

                    ApplicationUser developer2 = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "thusisihle01@gmail.com");
                    _userManager.AddToRoleAsync(developer2, SD.Role_Super_Admin).GetAwaiter().GetResult();
                }
            }

            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
