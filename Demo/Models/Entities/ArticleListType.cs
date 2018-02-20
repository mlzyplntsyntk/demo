using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Models.Entities
{
    public class ArticleListType
    {
    }

    class ArticleListOption
    {
        string Title { get; set; } = "Latest Articles";
        string OrderBy { get; set; } = "CreationDate";
        string Action { get; set; } = "List";
    }
}
