namespace PharmacyInventoryAPI.DTOs
{
    public class PaginationDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class PagedResult<T>
    {
        public List<T> Data { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();
    }
}