using System.ComponentModel.DataAnnotations;

namespace DemoMVC1.Models
{
    public class Student
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string? Hoten { get; set; }

        [Required(ErrorMessage = "Quê quán không được để trống")]
        public string? Quequan { get; set; }

        [Required(ErrorMessage = "Trường học không được để trống")]
        public string?  Truonghoc { get; set; }
    }
}