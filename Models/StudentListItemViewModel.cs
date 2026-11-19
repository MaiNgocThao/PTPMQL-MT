namespace DemoMVC1.Models
{
    public class StudentListItemViewModel
    {
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FacultyName { get; set; }
    }
}
