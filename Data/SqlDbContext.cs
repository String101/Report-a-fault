using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Report_a_Fault.Models;

namespace Report_a_Fault.Data
{
    public class SqlDbContext : IdentityDbContext<ApplicationUser>
    {
        public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)

        {

        }
        public DbSet<Labs> Labs { get; set; }
        public DbSet<Computer> Computer { get; set; }
        public DbSet<Computer_comp> Computer_comp { get; set; }
        public DbSet<Fault> Fault { get; set; }
        public DbSet<Campus> Campuses { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Department>().HasData(

                new Department
                {
                    DepartmentId = Guid.NewGuid().ToString(),
                    DepartmentName = "Department of Built Environment"
                },
                 new Department
                 {
                     DepartmentId = Guid.NewGuid().ToString(),
                     DepartmentName = "Department of Civil Engineering"
                 }, new Department
                 {
                     DepartmentId = Guid.NewGuid().ToString(),
                     DepartmentName = "Department of Electrical, Electronic and Computer Engineering"
                 },
                 new Department
                 {
                     DepartmentId = Guid.NewGuid().ToString(),
                     DepartmentName = "Department of Mechanical and Mechatronic Engineering"
                 }, new Department
                 {
                     DepartmentId = Guid.NewGuid().ToString(),
                     DepartmentName = "Department of Mathematical and Physical Sciences"
                 },
                 new Department
                 {
                     DepartmentId = Guid.NewGuid().ToString(),
                     DepartmentName = "Department of Information Technology"
                 }

                );
        }
        public DbSet<Report_a_Fault.Models.Building> Building { get; set; } = default!;


    }
}