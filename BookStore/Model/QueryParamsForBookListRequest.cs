namespace BookStore.Model
{
    public class QueryParamsForBookListRequest
    {
        public FilterForBookListRequest? Filter { get; set; }

        public int? PageNumber { get; set; }

        public string? Sort { get; set; }
    }

    public class FilterForBookListRequest
    {
        public int? PriceMin { get; set; }

        public int? PriceMax { get; set; }

        public int? RateMin { get; set; }

        public int? RateMax { get; set; }
    }
}
