using Demo.Models;
using Demo.Models.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Demo.Infrastructure;
using System.Linq;

namespace Demo.Tests
{
    public class Setup : IDisposable
    {
        public static Mock<IDataRepository> TestMockData;
        public static Mock<ISessionRepository> TestMockSession;

        public static User UserTypeManager;
        public static User UserTypeNormal;
        public static User UserTypeAnonymous;

        public Setup()
        {
            UserTypeManager = new User {
                Id =1,
                FirstName ="User",
                LastName ="Manager",
                Email ="manager@test.com",
                Password ="1234",
                UserType = UserType.MANAGER
            };
            UserTypeNormal = new User {
                Id = 2,
                FirstName = "User",
                LastName = "Normal",
                Email = "normal@test.com",
                Password = "1234",
                UserType = UserType.USER
            };
            UserTypeAnonymous = new User { };

            TestMockSession = new Mock<ISessionRepository>();
            TestMockData = new Mock<IDataRepository>();

            TestMockData.Setup(m => m.Users).Returns((new User[] {
                UserTypeManager, UserTypeNormal
            }).AsQueryable<User>());

            TestMockData.Setup(m => m.Articles).Returns((new Article[] {
                new Article { Id=1, User = UserTypeManager, Title = "Test", Content="Test"},
                new Article { Id=2, User = UserTypeManager, Title = "Test 2", Content="Test"},
                new Article { Id=3, User = UserTypeManager, Title = "Test 3", Content="Test"},
                new Article { Id=4, User = UserTypeManager, Title = "Test 4", Content="Test"},
                new Article { Id=5, User = UserTypeManager, Title = "Test 5", Content="Test"}
            }).AsQueryable<Article>());
        }

        public static ISessionRepository GetUser(User u)
        {
            TestMockSession.Reset();
            TestMockSession.Setup(m=>m.User).Returns((u));
            return TestMockSession.Object;
        }

        public void Dispose()
        {
            
        }
    }
}
