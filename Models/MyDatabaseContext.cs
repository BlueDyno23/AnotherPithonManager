using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AnotherPithonManager.Models
{
    public class MyDatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("MyDb");
        }
    }
}
