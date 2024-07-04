namespace EBMS.Infrastructure.Helpers
{
    public class PagedList<T>
    {
        private PagedList(List<T> items, int page, int pageSize, int totalItems, decimal totalPages)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }

        public List<T> Items { get; set; }

        public int Page {  get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public decimal TotalPages { get; set; }

        public bool HasNextPage => Page * PageSize < TotalItems;

        public bool HasPreviousPage => Page > 1;

        public static PagedList<T> Pagination(ICollection<T> query, int page, int pageSize)
        {
            var totalItems = query.Count();
            var totalPages = Math.Ceiling((decimal)totalItems/pageSize);

            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new(items, page, pageSize, totalItems, totalPages);
        }
    }
}
