using DemoMVC1.Data;
using DemoMVC1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC1.Controllers
{
    public class StudentController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // Hàm tự động sinh mã sinh viên
        private async Task<string> GenerateStudentCode(int FacultyId)
        {
            var majorCodes = new Dictionary<int, string>
    {
    { 1, "05" }, // Khoa Công nghệ thông tin
    { 2, "02" }, // Khoa Dầu khí
    { 3, "03" }, // Khoa Cơ - Điện
    { 4, "04" }, // Khoa Xây dựng
    { 5, "01" }, // Khoa Mỏ
    { 6, "06" }, // Khoa Môi trường
    { 7, "07" }, // Khoa Kinh tế - Quản trị kinh doanh
    { 8, "08" }, // Khoa Khoa học và Kỹ thuật Địa chất
    { 9, "09" }, // Khoa Trắc địa - Bản đồ và Quản lý đất đai
    { 10, "10" }, // Khoa Khoa học Cơ bản
    { 11, "11" }, // Khoa Lý luận chính trị
    { 12, "12" }  // Khoa Dầu khí

    };

    if (!majorCodes.TryGetValue(FacultyId, out string? majorCode))
    {
        throw new InvalidOperationException(
            "Chưa khai báo mã ngành cho khoa được chọn.");
    }

    string prefix = "2821" + majorCode;


            var studentCodes = await _context.Students
                .Where(s => s.StudentCode.StartsWith(prefix))
                .Select(s => s.StudentCode)
                .ToListAsync();

            int maxNumber = 0;

            foreach (var code in studentCodes)
            {
                if (code.Length != 10)
                    continue;

                string numberPart = code.Substring(6, 4);

                if (int.TryParse(numberPart, out int number))
                {
                    if (number > maxNumber)
                    {
                        maxNumber = number;
                    }
                }
            }

            int nextNumber = maxNumber + 1;

            if (nextNumber > 9999)
            {
                throw new InvalidOperationException(
                    $"Đã hết số thứ tự sinh viên cho mã {prefix}."

                );
            }

            return prefix + nextNumber.ToString("D4");
        }

        // Hàm nạp danh sách khoa vào dropdown
        private void LoadFaculties(int? selectedFacultyId = null)
        {
            ViewData["FacultyId"] = new SelectList(
                _context.Faculties,
                "FacultyId",
                "FacultyName",
                selectedFacultyId
            );
        }

        // READ - Danh sách sinh viên
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .Include(s => s.Faculty)
                .ToListAsync();

            return View(students);
        }

        // CREATE - Hiển thị form thêm sinh viên
        [HttpGet]
        public IActionResult Create()
        {
            LoadFaculties();

            return View();
        }

        // CREATE - Lưu sinh viên mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            // Mã sinh viên do hệ thống tự tạo
            ModelState.Remove(nameof(Student.StudentCode));

            // Kiểm tra khoa có tồn tại không
            bool facultyExists = await _context.Faculties
                .AnyAsync(f => f.FacultyId == student.FacultyId);

            if (!facultyExists)
            {
                ModelState.AddModelError(
                    nameof(Student.FacultyId),
                    "Vui lòng chọn khoa hợp lệ."
                );
            }

            if (ModelState.IsValid)
            {
                student.StudentCode = await GenerateStudentCode(student.FacultyId);

                _context.Students.Add(student);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            LoadFaculties(student.FacultyId);

            return View(student);
        }

        // DETAILS - Xem chi tiết sinh viên
        public async Task<IActionResult> Details(string? studentCode)
        {
            if (studentCode == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Faculty)
                .FirstOrDefaultAsync(s => s.StudentCode == studentCode);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // EDIT - Hiển thị form sửa sinh viên
        [HttpGet]
        public async Task<IActionResult> Edit(string? studentCode)
        {
            if (studentCode == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FindAsync(studentCode);

            if (student == null)
            {
                return NotFound();
            }

            LoadFaculties(student.FacultyId);

            return View(student);
        }

        // EDIT - Lưu thông tin đã sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string studentCode, Student student)
        {
            if (studentCode != student.StudentCode)
            {
                return NotFound();
            }

            // Kiểm tra sinh viên có tồn tại không
            var existingStudent = await _context.Students
                .FindAsync(studentCode);

            if (existingStudent == null)
            {
                return NotFound();
            }

            // Không cho phép thay đổi mã sinh viên
            student.StudentCode = existingStudent.StudentCode;

            // Kiểm tra khoa được chọn
            bool facultyExists = await _context.Faculties
                .AnyAsync(f => f.FacultyId == student.FacultyId);

            if (!facultyExists)
            {
                ModelState.AddModelError(
                    nameof(Student.FacultyId),
                    "Vui lòng chọn khoa hợp lệ."
                );
            }

            // Không cần kiểm tra lại mã sinh viên từ form
            ModelState.Remove(nameof(Student.StudentCode));

            if (ModelState.IsValid)
            {
                existingStudent.FullName = student.FullName;
                existingStudent.Email = student.Email;
                existingStudent.Address = student.Address;
                existingStudent.FacultyId = student.FacultyId;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            LoadFaculties(student.FacultyId);

            return View(student);
        }

        // DELETE - Hiển thị trang xác nhận xóa
        [HttpGet]
        public async Task<IActionResult> Delete(string? studentCode)
        {
            if (studentCode == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Faculty)
                .FirstOrDefaultAsync(s => s.StudentCode == studentCode);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // DELETE - Thực hiện xóa sinh viên
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string studentCode)
        {
            var student = await _context.Students
                .FindAsync(studentCode);

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