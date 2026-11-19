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
        private async Task<string> GenerateStudentCode(int facultyId, int enrollmentYear)
        {
            var faculty = await _context.Faculties
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FacultyId == facultyId);

            if (faculty == null)
            {
                throw new InvalidOperationException("Khoa được chọn không tồn tại.");
            }

            var majorCode = GetMajorCode(faculty.FacultyName);
            if (majorCode == null)
            {
                throw new InvalidOperationException(
                    $"Chưa khai báo mã ngành cho khoa \"{faculty.FacultyName}\".");
            }

            var yy = (enrollmentYear % 100).ToString("D2");
            var prefix = yy + "21" + majorCode;

            for (var attempt = 0; attempt < 10000; attempt++)
            {
                var studentCodes = await _context.Students
                    .AsNoTracking()
                    .Where(s => s.StudentCode.Length == 10 &&
                                s.StudentCode.StartsWith(prefix))
                    .Select(s => s.StudentCode)
                    .ToListAsync();

                var maxNumber = 0;
                foreach (var code in studentCodes)
                {
                    if (int.TryParse(code.Substring(6, 4), out var number))
                    {
                        maxNumber = Math.Max(maxNumber, number);
                    }
                }

                var nextNumber = maxNumber + 1;
                if (nextNumber > 9999)
                {
                    throw new InvalidOperationException(
                        $"Đã hết số thứ tự sinh viên cho mã {prefix}");
                }

                var candidate = prefix + nextNumber.ToString("D4");
                var exists = await _context.Students
                    .AsNoTracking()
                    .AnyAsync(s => s.StudentCode == candidate);

                if (!exists)
                {
                    return candidate;
                }
            }

            throw new InvalidOperationException(
                "Không thể sinh mã sinh viên duy nhất. Vui lòng thử lại.");
        }

        private static string? GetMajorCode(string facultyName)
        {
            var name = facultyName.ToLowerInvariant();

            if (name.Contains("công nghệ thông tin")) return "05";
            if (name.Contains("dầu khí")) return "02";
            if (name.Contains("cơ") && name.Contains("điện")) return "03";
            if (name.Contains("xây dựng")) return "04";
            if (name.Contains("mỏ")) return "01";
            if (name.Contains("môi trường")) return "06";
            if (name.Contains("kinh tế") || name.Contains("quản trị kinh doanh")) return "07";
            if (name.Contains("địa chất")) return "08";
            if (name.Contains("trắc địa")) return "09";
            if (name.Contains("khoa học cơ bản")) return "10";
            if (name.Contains("lý luận chính trị")) return "11";

            return null;
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

        private bool IsAjaxRequest()
        {
            return Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        private IActionResult InvalidStudentForm(Student student)
        {
            if (IsAjaxRequest())
            {
                var result = PartialView("_StudentFormModal", student);
                result.StatusCode = StatusCodes.Status422UnprocessableEntity;
                return result;
            }

            return View(student);
        }

        private IActionResult StudentNotFound(string message)
        {
            return IsAjaxRequest()
                ? NotFound(new { message })
                : NotFound();
        }

        // READ - Danh sách sinh viên
        public async Task<IActionResult> Index(string? searchTerm, int page = 1, int pageSize = 10)
        {
            int[] allowedPageSizes = [5, 10, 20, 50];
            if (!allowedPageSizes.Contains(pageSize))
            {
                pageSize = 10;
            }

            searchTerm = searchTerm?.Trim();

            var studentsQuery = _context.Students.AsNoTracking();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                studentsQuery = studentsQuery.Where(s =>
                    s.StudentCode.Contains(searchTerm) ||
                    s.FullName.Contains(searchTerm) ||
                    (s.Address != null && s.Address.Contains(searchTerm)) ||
                    s.Email.Contains(searchTerm) ||
                    _context.Faculties.Any(f =>
                        f.FacultyId == s.FacultyId &&
                        f.FacultyName.Contains(searchTerm)));
            }

            int totalCount = await studentsQuery.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            page = totalPages == 0 ? 1 : Math.Clamp(page, 1, totalPages);

            var students = await studentsQuery
                .OrderBy(s => s.StudentCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new StudentListItemViewModel
                {
                    StudentCode = s.StudentCode,
                    FullName = s.FullName,
                    Address = s.Address,
                    Email = s.Email,
                    FacultyName = _context.Faculties
                        .Where(f => f.FacultyId == s.FacultyId)
                        .Select(f => f.FacultyName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var model = new StudentListViewModel
            {
                SearchTerm = searchTerm,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Students = students
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_StudentList", model);
            }

            return View(model);
        }

        // CREATE - Hiển thị form thêm sinh viên
        [HttpGet]
        public IActionResult Create()
        {
            LoadFaculties();

            if (IsAjaxRequest())
            {
                return PartialView("_StudentFormModal", new Student
                {
                    EnrollmentYear = DateTime.Now.Year
                });
            }

            return View(new Student
            {
                EnrollmentYear = DateTime.Now.Year
            });
        }

        // CREATE - Lưu sinh viên mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            // Mã sinh viên do hệ thống tự tạo
            ModelState.Remove(nameof(Student.StudentCode));

            // Kiểm tra khoa có tồn tại không
            bool facultyExists = student.FacultyId.HasValue &&
                await _context.Faculties.AnyAsync(f => f.FacultyId == student.FacultyId);

            if (!facultyExists)
            {
                ModelState.AddModelError(
                    nameof(Student.FacultyId),
                    "Vui lòng chọn khoa hợp lệ."
                );
            }

            if (!student.EnrollmentYear.HasValue)
            {
                ModelState.AddModelError(
                    nameof(Student.EnrollmentYear),
                    "Vui lòng nhập năm nhập học.");
            }
            else if (student.EnrollmentYear < 2000 || student.EnrollmentYear > 2099)
            {
                ModelState.AddModelError(
                    nameof(Student.EnrollmentYear),
                    "Năm nhập học phải từ 2000 đến 2099.");
            }

            if (ModelState.IsValid)
            {
                for (var attempt = 0; attempt < 5; attempt++)
                {
                    student.StudentCode = await GenerateStudentCode(
                        student.FacultyId!.Value,
                        student.EnrollmentYear!.Value);

                    await using var transaction =
                        await _context.Database.BeginTransactionAsync();

                    try
                    {
                        _context.Students.Add(student);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        break;
                    }
                    catch (DbUpdateException)
                    {
                        await transaction.RollbackAsync();
                        _context.Entry(student).State = EntityState.Detached;

                        if (attempt == 4)
                        {
                            throw;
                        }
                    }
                }

                if (IsAjaxRequest())
                {
                    return Ok(new { message = "Đã thêm sinh viên thành công." });
                }

                return RedirectToAction(nameof(Index));
            }

            LoadFaculties(student.FacultyId);

            return InvalidStudentForm(student);
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
                return StudentNotFound("Không tìm thấy sinh viên.");
            }

            if (IsAjaxRequest())
            {
                return PartialView("_StudentDetailsModal", student);
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
                return StudentNotFound("Không tìm thấy sinh viên.");
            }

            LoadFaculties(student.FacultyId);

            if (IsAjaxRequest())
            {
                return PartialView("_StudentFormModal", student);
            }

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
                return StudentNotFound("Mã sinh viên không khớp.");
            }

            // Kiểm tra sinh viên có tồn tại không
            var existingStudent = await _context.Students
                .FindAsync(studentCode);

            if (existingStudent == null)
            {
                return StudentNotFound("Không tìm thấy sinh viên.");
            }

            // Không cho phép thay đổi mã sinh viên
            student.StudentCode = existingStudent.StudentCode;

            // Kiểm tra khoa được chọn
            bool facultyExists = student.FacultyId.HasValue &&
                await _context.Faculties.AnyAsync(f => f.FacultyId == student.FacultyId);

            if (!facultyExists)
            {
                ModelState.AddModelError(
                    nameof(Student.FacultyId),
                    "Vui lòng chọn khoa hợp lệ."
                );
            }

            if (student.EnrollmentYear.HasValue &&
                (student.EnrollmentYear < 2000 || student.EnrollmentYear > 2099))
            {
                ModelState.AddModelError(
                    nameof(Student.EnrollmentYear),
                    "Năm nhập học phải từ 2000 đến 2099.");
            }

            // Không cần kiểm tra lại mã sinh viên từ form
            ModelState.Remove(nameof(Student.StudentCode));

            if (ModelState.IsValid)
            {
                existingStudent.FullName = student.FullName;
                existingStudent.Email = student.Email;
                existingStudent.Address = student.Address;
                existingStudent.FacultyId = student.FacultyId!.Value;
                if (existingStudent.EnrollmentYear == null)
                {
                    existingStudent.EnrollmentYear = student.EnrollmentYear;
                }

                await _context.SaveChangesAsync();

                if (IsAjaxRequest())
                {
                    return Ok(new { message = "Đã cập nhật sinh viên thành công." });
                }

                return RedirectToAction(nameof(Index));
            }

            LoadFaculties(student.FacultyId);

            return InvalidStudentForm(student);
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
                return StudentNotFound("Không tìm thấy sinh viên.");
            }

            if (IsAjaxRequest())
            {
                return PartialView("_StudentDeleteModal", student);
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
                return StudentNotFound("Không tìm thấy sinh viên.");
            }

            _context.Students.Remove(student);

            await _context.SaveChangesAsync();

            if (IsAjaxRequest())
            {
                return Ok(new { message = "Đã xóa sinh viên thành công." });
            }

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