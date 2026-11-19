namespace DemoMVC1.Models
{
    public class StudentListViewModel
    {
        public IReadOnlyList<StudentListItemViewModel> Students { get; set; } = [];
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}
