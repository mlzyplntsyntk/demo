using System;

namespace Demo.Models.ViewModels
{
    public class ArticleListPagingViewModel
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public bool HasNext => TotalItems > (CurrentPage+1) * ItemsPerPage;
        // it is better to keep the total page property even if it's not used
        // we may decide to implement the paging of lists with page numbers in the future
        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
    }
}
