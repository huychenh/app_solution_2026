namespace ShopOnline.Api.Helpers
{
    public class PagedResult<T>
    {
        // The paginated subset of records for the requested page
        public IEnumerable<T> Items { get; set; } = [];

        // Total number of items matching the criteria before pagination was applied
        public int TotalCount { get; set; }
    }
}
