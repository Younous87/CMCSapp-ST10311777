using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCSapp_ST10311777.Models
{
    public class AppDbContext : DbContext
    {
        //public DbSet<ClaimTable> Claims { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(
        //        "Server=tcp:cloudev-sql-server.database.windows.net,1433;Initial Catalog=CLOUD-db;Persist Security Info=False;User ID=admin-youyou;Password=C'esttropcool87;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30"
        //    );
        //}

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    // Configure primary key for ClaimTable
        //    modelBuilder.Entity<ClaimTable>()
        //        .HasKey(c => c.claimID);  // Specify the primary key explicitly (claimID)
        //}
    }
    
}
