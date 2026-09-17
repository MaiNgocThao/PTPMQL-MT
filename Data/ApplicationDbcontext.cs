using Microsoft.EntityFrameworkCore;
using DemoMVC1.Models;

namespace DemoMVC1.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Student> Students { get; set; } = default!;
        public DbSet<StudentScore> StudentScores { get; set; } = default!;
    }
}