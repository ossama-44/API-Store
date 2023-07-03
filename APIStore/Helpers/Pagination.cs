﻿namespace APIStore.Helpers
{
    public class Pagination<T> where T : class
    {
        public Pagination(int pageIndex, int pageSize, int pageTotalCount, IReadOnlyList<T> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            PageTotalCount = pageTotalCount;
            Data = data;
        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int PageTotalCount { get; set; }
        public IReadOnlyList<T> Data { get; set; }
    }
}
