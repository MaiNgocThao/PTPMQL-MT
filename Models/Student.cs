using System.ComponentModel.DataAnnotations;

namespace DemoMVC1.Models
{
    public class NewBaseType
    {
        public Faculty? Faculty { get; set; }
    }

    public interface IStudent
    {
        string StudentCode { get; set; }
        string FullName { get; set; }
        string? Address { get; set; }
        string Email { get; set; }
        int? FacultyId { get; set; }
        int? EnrollmentYear { get; set; }
    }

    public class Student : NewBaseType, IStudent
    {
        [Key]
        [Required(ErrorMessage = "Mã sinh viên không được để trống")]
        [StringLength(10, MinimumLength = 3,
            ErrorMessage = "Mã sinh viên phải từ 3 đến 10 ký tự")]
        public string StudentCode { get; set; } = string.Empty;



        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(50,
            ErrorMessage = "Họ tên không được vượt quá 50 ký tự")]
        public string FullName { get; set; } = string.Empty;


        [StringLength(100,
            ErrorMessage = "Địa chỉ không được vượt quá 100 ký tự")]
        public string? Address { get; set; }


        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        public int? FacultyId { get; set; }
        [Range(2000, 2099, ErrorMessage = "Năm nhập học phải từ 2000 đến 2099")]
        public int? EnrollmentYear { get; set; }
        public new Faculty? Faculty { get; set; }
    }
}