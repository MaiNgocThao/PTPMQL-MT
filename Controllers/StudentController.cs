// namespace DemoMVC1.Controllers
// {
//     using DemoMVC1.Models;
//     using Microsoft.AspNetCore.Mvc;

//     public class StudentController : Controller
//     {
//         /* public IActionResult Index()
//         {
//             ViewData["Title"] = "Form nhập thông tin sinh viên";
//             return View();
//         }

//         // POST: /Student (submit từ form)
//         [HttpPost]
//         public IActionResult Index(string hoten, string quequan, string truonghoc)
//         {
//             ViewData["Title"] = "Kết quả sau khi xử lý";
//             ViewData["Hoten"] = hoten;
//             ViewData["Quequan"] = quequan;
//             ViewData["Truonghoc"] = truonghoc;
//             return View();
//         } */
//     public IActionResult Index()
//         {
//             ViewData["Title"] = "Form nhập thông tin sinh viên";
//             return View(new Student()); // truyền object rỗng để tránh NullReferenceException
//         }

//         // POST: /Student
//         [HttpPost]
//         public IActionResult Index(Student student)
//         {
//             ViewData["Title"] = "Kết quả sau khi xử lý";

//             if (!ModelState.IsValid)
//             {
//                 // dữ liệu không hợp lệ -> trả lại form kèm lỗi + dữ liệu đã nhập
//                 return View(student);
//             }

//             return View(student); // hợp lệ -> hiển thị lại dữ liệu vừa nhận
    
    
    
//     }

    
// }
// }
using DemoMVC1.Models;
using Microsoft.AspNetCore.Mvc;

namespace DemoMVC1.Controllers
{
    public class StudentController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new Student());
        }

        [HttpPost]
        public IActionResult Index(Student student)
        {
            return View(student);
        }
    }
}