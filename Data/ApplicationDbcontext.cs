using Microsoft.EntityFrameworkCore;
using DemoMVC1.Models;

namespace DemoMVC1.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Student> Students { get; set; } = default!;
        public DbSet<StudentScore> StudentScores { get; set; } = default!;

        private DbSet<Product> product = default!;

        public DbSet<Product> GetProduct()
        {
            return product;
        }

        public void SetProduct(DbSet<Product> value)
        {
            product = value;
        }

        public DbSet<Faculty> Faculties { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Thêm dữ liệu mẫu cho bảng Faculty
            modelBuilder.Entity<Faculty>().HasData(
            new Faculty { FacultyId =1, FacultyName = "Khoa Công nghệ thông tin" },
            new Faculty { FacultyId =2, FacultyName = "Khoa Dầu khí" },
            new Faculty { FacultyId =3, FacultyName = "Khoa Cơ - Điện" },
            new Faculty { FacultyId =4, FacultyName = "Khoa Xây dựng" },
            new Faculty { FacultyId =5, FacultyName = "Khoa Mỏ" },
            new Faculty { FacultyId =6, FacultyName = "Khoa Môi trường" },
            new Faculty { FacultyId =7, FacultyName = "Khoa Kinh tế - Quản trị kinh doanh" },
            new Faculty { FacultyId =8, FacultyName = "Khoa Khoa học và ký thuật Địa chất" },
            new Faculty { FacultyId =9, FacultyName = "Khoa Trắc địa - Bản đồ và Quản lý đất đai" },
            new Faculty { FacultyId =10, FacultyName = "Khoa Khoa học Cơ bản" },
            new Faculty { FacultyId =11, FacultyName = "Khoa Lý luận chính trị" },
            new Faculty { FacultyId =12, FacultyName = "Khoa Dầu khí" }
        
        );
        }
    }
}