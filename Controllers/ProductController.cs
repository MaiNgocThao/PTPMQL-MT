using DemoMVC1.Models;
using Microsoft.AspNetCore.Mvc;

namespace DemoMVC1.Controllers
{
    public class ProductController : Controller
    {
        // GET: hiển thị form trống, chờ người dùng nhập
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Title = "Nhập thông tin sản phẩm";
            return View(new Product()); // object rỗng để View bind an toàn
        }

        // POST: nhận dữ liệu người dùng gõ vào form
        [HttpPost]
        public IActionResult Index(Product product)
        {
            ViewBag.Title = "Nhập thông tin sản phẩm";

            if (!ModelState.IsValid)
            {
                // dữ liệu không hợp lệ -> trả lại form kèm lỗi + giữ nguyên dữ liệu đã gõ
                return View(product);
            }

            // Hợp lệ -> đẩy qua TempData để "sống sót" qua Redirect
            TempData["MaSanPham"] = product.MaSanPham;
            TempData["TenSanPham"] = product.TenSanPham;
            TempData["Gia"] = product.Gia.ToString();
            TempData["ThongBao"] = "Thêm sản phẩm thành công!";

            return RedirectToAction("KetQua"); // PRG pattern: tránh resubmit khi F5
        }

        // GET: trang hiển thị kết quả sau redirect
        [HttpGet]
        public IActionResult KetQua()
        {
            // Nếu người dùng gõ thẳng URL /Product/KetQua (không qua form)
            // thì TempData sẽ rỗng -> nên kiểm tra và điều hướng lại
            if (TempData["MaSanPham"] == null)
            {
                return RedirectToAction("Index");
            }

            ViewData["ThoiGianXuLy"] = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
            return View();
        }
    }
}