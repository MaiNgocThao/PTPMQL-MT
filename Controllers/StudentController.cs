using DemoMVC1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMVC1.Data;

namespace DemoMVC1.Controllers
{
    public class StudentController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // READ
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students.ToListAsync();

            return View(students);
        }

        // CREATE - Hiển thị form
        [HttpGet]
        public IActionResult Create()
        {
            
            return View();
        }
        

        // CREATE - Thêm dữ liệu mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {   
            var studentExists = await _context.Students
        .AnyAsync(s => s.StudentCode == student.StudentCode);

    if (studentExists)
    {
        ModelState.AddModelError(
            "StudentCode",
            "Mã sinh viên này đã tồn tại!"
        );
    }

            if (ModelState.IsValid)
            {
                _context.Students.Add(student);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(student);
        }

        // UPDATE - Hiển thị form Edit

        [HttpGet]
        public async Task<IActionResult> Edit(string? studentCode)
        {
            if (studentCode == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(studentCode);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // UPDATE - Lưu dữ liệu Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string studentCode, Student student)
        {
            if (studentCode != student.StudentCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Students.Update(student);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(student);
        }

        // DELETE - Hiển thị xác nhận

        [HttpGet]
        public async Task<IActionResult> Delete(string? studentCode)
        {
            if (studentCode == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(studentCode);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // DELETE - Thực hiện xóa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string studentCode)
        {
            var student = await _context.Students.FindAsync(studentCode);

            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}





















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