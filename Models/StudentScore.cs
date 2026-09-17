using System.ComponentModel.DataAnnotations;
    namespace DemoMVC1.Models
{
    public class StudentScore
    {
        [Key]
        public int Id { get; set; }
        public string StudentCode { get; set; } = default!;
        public string Subject { get; set; } = default!;

        [Range(0, 10, ErrorMessage = "Điểm phải nằm trong khoảng từ 0 đến 10")]
        public double Score { get; set; }

    }
}