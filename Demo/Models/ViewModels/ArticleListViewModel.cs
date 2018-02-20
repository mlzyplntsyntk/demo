using Demo.Models.Entities;
using Demo.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Demo.Models.ViewModels
{
    public class ArticleListViewModel
    {
        public IEnumerable<Article> Articles { get; set; }
        public ArticleListPagingViewModel PagingInfo { get; set; }

        public string ViewKey { get; set; } = "latest";
        public string ViewTitle { get; set; } = "Latest Articles";
        public string PageAction { get; set; } = "List";
        public string ArticleOrder { get; set; } = "CreationTime desc";
    }
}
