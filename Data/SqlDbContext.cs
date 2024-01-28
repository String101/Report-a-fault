
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
        
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Campus>().HasData(
                
                new Campus
                {
                    CampusId= Guid.NewGuid().ToString(),
                    CampusName= "Welkom"
                },
                 new Campus
                 {
                     CampusId = Guid.NewGuid().ToString(),
                     CampusName = "Bloemfontein"
                 },
                  new Campus
                  {
                      CampusId = Guid.NewGuid().ToString(),
                      CampusName = "Kimbarly"
                  }

                );
        }
        

    }
}
