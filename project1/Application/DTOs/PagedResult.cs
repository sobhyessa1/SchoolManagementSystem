using System.Collections.Generic;

namespace project1.Application.DTOs
{
    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long Total { get; set; }
        public IEnumerable<T> Items { get; set; } = System.Array.Empty<T>();
    }
}
