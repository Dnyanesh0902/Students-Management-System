using Microsoft.EntityFrameworkCore;
using Student_Management_API.Models;

namespace Student_Management_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<User> Users { get; set; }
       
    }
}
