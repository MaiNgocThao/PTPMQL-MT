using System.ComponentModel.DataAnnotations;

namespace DemoMVC1.Models
{
    public class Product
    {
        [Required(ErrorMessage = "Vui lòng nhập mã sản phẩm")]
        public string? MaSanPham { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        public string? TenSanPham { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải >= 0")]
        public double Gia { get; set; }
    }
}