using DemoMVC1.Data;
using DemoMVC1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC1.Controllers
{
    public class FacultyController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        private bool IsAjaxRequest()
        {
            return Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        public async Task<IActionResult> Index(
            string? searchTerm,
            int page = 1,
            int pageSize = 10)
        {
            int[] allowedPageSizes = [5, 10, 20, 50];
            if (!allowedPageSizes.Contains(pageSize))
            {
                pageSize = 10;
            }

            searchTerm = searchTerm?.Trim();
            var query = _context.Faculties.AsNoTracking();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(f => f.FacultyName.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            page = totalPages == 0 ? 1 : Math.Clamp(page, 1, totalPages);

            var model = new FacultyListViewModel
            {
                SearchTerm = searchTerm,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Faculties = await query
                    .OrderBy(f => f.FacultyId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync()
            };

            if (IsAjaxRequest())
            {
                return PartialView("_FacultyList", model);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var faculty = new Faculty();

            if (IsAjaxRequest())
            {
                return PartialView("_FacultyFormModal", faculty);
            }

            return View(faculty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Faculty faculty)
        {
            faculty.FacultyName = faculty.FacultyName?.Trim() ?? string.Empty;

            var duplicate = await _context.Faculties
                .AsNoTracking()
                .AnyAsync(f => f.FacultyName.ToLower() == faculty.FacultyName.ToLower());

            if (duplicate)
            {
                ModelState.AddModelError(
                    nameof(Faculty.FacultyName),
                    "Tên khoa đã tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                if (IsAjaxRequest())
                {
                    var invalid = PartialView("_FacultyFormModal", faculty);
                    invalid.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return invalid;
                }

                return View(faculty);
            }

            _context.Faculties.Add(faculty);
            await _context.SaveChangesAsync();

            if (IsAjaxRequest())
            {
                return Ok(new { message = "Đã thêm khoa thành công." });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? facultyId)
        {
            if (facultyId == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculties.FindAsync(facultyId);
            if (faculty == null)
            {
                return NotFound(new { message = "Không tìm thấy khoa." });
            }

            if (IsAjaxRequest())
            {
                return PartialView("_FacultyFormModal", faculty);
            }

            return View(faculty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int facultyId, Faculty faculty)
        {
            if (facultyId != faculty.FacultyId)
            {
                return NotFound(new { message = "Mã khoa không khớp." });
            }

            faculty.FacultyName = faculty.FacultyName?.Trim() ?? string.Empty;

            var duplicate = await _context.Faculties
                .AsNoTracking()
                .AnyAsync(f =>
                    f.FacultyId != facultyId &&
                    f.FacultyName.ToLower() == faculty.FacultyName.ToLower());

            if (duplicate)
            {
                ModelState.AddModelError(
                    nameof(Faculty.FacultyName),
                    "Tên khoa đã tồn tại.");
            }

            var existing = await _context.Faculties.FindAsync(facultyId);
            if (existing == null)
            {
                return NotFound(new { message = "Không tìm thấy khoa." });
            }

            if (!ModelState.IsValid)
            {
                if (IsAjaxRequest())
                {
                    var invalid = PartialView("_FacultyFormModal", faculty);
                    invalid.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return invalid;
                }

                return View(faculty);
            }

            existing.FacultyName = faculty.FacultyName;
            await _context.SaveChangesAsync();

            if (IsAjaxRequest())
            {
                return Ok(new { message = "Đã cập nhật khoa thành công." });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? facultyId)
        {
            if (facultyId == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculties
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FacultyId == facultyId);

            if (faculty == null)
            {
                return NotFound(new { message = "Không tìm thấy khoa." });
            }

            if (IsAjaxRequest())
            {
                return PartialView("_FacultyDetailsModal", faculty);
            }

            return View(faculty);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? facultyId)
        {
            if (facultyId == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculties.FindAsync(facultyId);
            if (faculty == null)
            {
                return NotFound(new { message = "Không tìm thấy khoa." });
            }

            if (IsAjaxRequest())
            {
                return PartialView("_FacultyDeleteModal", faculty);
            }

            return View(faculty);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int facultyId)
        {
            var faculty = await _context.Faculties.FindAsync(facultyId);
            if (faculty == null)
            {
                return NotFound(new { message = "Không tìm thấy khoa." });
            }

            var studentCount = await _context.Students
                .CountAsync(s => s.FacultyId == facultyId);

            if (studentCount > 0)
            {
                var message =
                    $"Không thể xóa khoa này vì đang có {studentCount} sinh viên tham chiếu.";

                if (IsAjaxRequest())
                {
                    return Conflict(new { message });
                }

                TempData["FacultyDeleteError"] = message;
                return RedirectToAction(nameof(Index));
            }

            _context.Faculties.Remove(faculty);
            await _context.SaveChangesAsync();

            if (IsAjaxRequest())
            {
                return Ok(new { message = "Đã xóa khoa thành công." });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
