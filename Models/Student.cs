using System.ComponentModel.DataAnnotations;

namespace DemoMVC1.Models
{
    public class Student
    {
        [Key]
        [Required(ErrorMessage = "Mã sinh viên không được để trống")]
        [StringLength(10, MinimumLength = 3,
            ErrorMessage = "Mã sinh viên phải từ 3 đến 10 ký tự")]
        public string StudentCode { get; set; } = default!;


        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(50,
            ErrorMessage = "Họ tên không được vượt quá 50 ký tự")]
        public string FullName { get; set; } = default!;


        [StringLength(100,
            ErrorMessage = "Địa chỉ không được vượt quá 100 ký tự")]
        public string? Address { get; set; }


        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = default!;
    }
}