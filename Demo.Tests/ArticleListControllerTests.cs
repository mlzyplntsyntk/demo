using Demo.Controllers;
using Demo.Models;
using Demo.Models.Entities;
using System.Linq;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;
using Demo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Tests
{
    public class ArticleListControllerTests : IClassFixture<Setup>
    {
        [Theory]
        [InlineData(2, 1)]
        [InlineData(4, 2)]
        [InlineData(12, 3)]
        public void CanListArticlesWithPaging(int PageSize, int PageNumber)
        {
            var controller = new DataController(Setup.TestMockData.Object, null);
            ArticleListViewModel result = controller.ArticleList(PageNumber, PageSize, "latest", true);

            Assert.NotNull(result);

            if (result.PagingInfo.TotalItems < PageSize * PageNumber)
            {
                Assert.False(result.Articles.Count() == PageSize);
            }
            else
            {
                Assert.True(result.Articles.Count() == PageSize);
            }

            Assert.True(result.PagingInfo.TotalPages == Math.Ceiling((decimal)result.PagingInfo.TotalItems / PageSize));
        }


    }
}
