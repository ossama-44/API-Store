namespace Core.Specifications
{
    public class ProductSpecParams
    {
        public int? BrandId { get; set; }
        public int? TypeId { get; set; }
        public string Sort { get; set; }
        private const int MAXPAGESIZE = 50;
        private int _pageSize = 6;
        
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MAXPAGESIZE) ? MAXPAGESIZE : value;
        }
 
        public int PageIndex { get; set; } = 1;

        private string? _search;

        public string? Search
        {
            get { return _search; }
            set { _search = value.ToLower(); }
        }

    }
}
