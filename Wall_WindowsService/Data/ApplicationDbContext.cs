using System.Data.Entity;
using Wall_WindowsService.Models;

namespace Wall_WindowsService.Data
{
    public class ApplicationDbContext :DbContext
    {
        // Constructor to pass the connection string
        public ApplicationDbContext()
            : base("name=DefaultConnection")
        {
        }

        // DbSet for Evenement table
        public DbSet<Evenement> Evenements { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map the Evenement table
            modelBuilder.Entity<Evenement>().ToTable("Evenement");
            modelBuilder.Entity<Evenement>().HasKey(e => e.ID);
        }
    }
}
