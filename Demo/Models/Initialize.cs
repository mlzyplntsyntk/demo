using Demo.Models.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Demo.Models
{
    public static class Initialize
    {
        static AppDbContext context;

        /**
         * we will call the database generation scripts respectively
         * by using Faker Library, we will be supplying a sample data
         * for our application.
         * 
         * Note that this methods will drop the existing database and
         * re-create them 
         */
        public static void Begin(IApplicationBuilder builder)
        {
            Program.Initializing = true;
            context = builder.ApplicationServices.GetRequiredService<AppDbContext>();
            ReBuild();
            AddFakeUsers();
            AddFakeArticles();
            GenerateArticleActions();
            PopulateArticleActionCounts();
            Program.Initializing = false;
        }

        /**
         * Delete and Create The Database for Demonstration purposes
         * In a real world example we should use Migrations to manipulate DB
         */
        private static void ReBuild()
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        /**
         * Adding Users of 100, by choosing the 2 of them as managers
         */
        private static void AddFakeUsers()
        {
            for (int i=0; i<100; i++)
            {
                // here we assign the 1st and 25th users as managers
                UserType userType = (i == 0 || i == 25) ? UserType.MANAGER : UserType.USER;
                User tempUser = GetFakeUser(userType);
                context.Add(tempUser);
            }

            context.SaveChanges();
        }
        /**
         * Creates a fake user instance and returns to the caller
         */
        private static User GetFakeUser(UserType userType)
        {
            string firstName = Faker.NameFaker.FirstName();
            string lastName = Faker.NameFaker.LastName();
            string email = string.Format("{0}.{1}@sunexpress.com",
                firstName.ToLower(),
                lastName.ToLower());

            return new User
            {
                FirstName = firstName,
                LastName = lastName,
                UserType = userType,
                Birthday = Faker.DateTimeFaker.BirthDay(20, 60),
                Email = email,
                Password = Faker.StringFaker.AlphaNumeric(8)
            };
        }
        /**
         * Here we get the managers from database and invoke AddArticle method
         */
        private static void AddFakeArticles()
        {
            var managers = (from p in context.Users
                            where p.UserType.Equals(UserType.MANAGER)
                            select new User
                            {
                                Id = p.Id
                            });
            
            foreach (User author in managers)
            {
                AddFakeArticlesForManager(author);
            }

            context.SaveChanges();
        }
        /**
         * Generating Likes for Users
         */
        private static void GenerateArticleActions()
        {
            Random numberGenerator = new Random();
            var users = (from p in context.Users select new User { Id=p.Id }).ToList();
            var articles = (from a in context.Articles select new Article { Id=a.Id });

            foreach (Article item in articles)
            {
                int totalLikes = numberGenerator.Next(0, 25);
                for (var i = 0; i < totalLikes; i++)
                {
                    context.Add(new ArticleAction
                    {
                        ArticleActionType = ArticleActionType.LIKE,
                        User = users.ElementAt(i),
                        Article = item
                    });
                    context.Add(new ArticleAction
                    {
                        ArticleActionType = ArticleActionType.READ,
                        User = users.ElementAt(i),
                        Article = item
                    });
                }

                int totalReads = numberGenerator.Next(totalLikes, 50);
                for (var i = totalLikes; i < totalReads; i++) {
                    context.Add(new ArticleAction
                    {
                        ArticleActionType = ArticleActionType.READ,
                        User = users.ElementAt(i),
                        Article = item
                    });
                }
            }

            context.SaveChanges();
        }
        /**
         * Generate Random Number of Articles for a single manager
         */
        private static void AddFakeArticlesForManager(User user)
        {
            Random numberGenerator = new Random();
            int totalArticles = numberGenerator.Next(30, 60);
            DateTime minDate = DateTime.Now.AddYears(-1);

            for (int i = 0; i < totalArticles; i++)
            {
                int totalLinesOfContent = numberGenerator.Next(20, 50);
                context.Add(new Article
                {
                    User = user,
                    Content = GetArticleContent(),
                    Title = Faker.TextFaker.Sentence(),
                    Photo = String.Format("{0}-{1}.jpg", (i + 1).ToString(), user.Id.ToString()),
                    CreationTime = Faker.DateTimeFaker.DateTime(minDate, DateTime.Now)
                });
            }
        }
        /**
         * Creates an article with random sentences and paragraphs
         */
        private static string GetArticleContent()
        {
            Random numberGenerator = new Random();
            int totalNumberOfParagraphs = numberGenerator.Next(3, 6);
            List<string> content = new List<string>();

            for (int i=0; i<totalNumberOfParagraphs; i++)
            {
                int totalSentencesInParagraph = numberGenerator.Next(3, 20);
                content.Add(Faker.TextFaker.Sentences(totalSentencesInParagraph));
            }

            return String.Join("<p>&nbsp;</p>", content);
        }
        /**
         * Populates like and read data for each article
         */
        private static void PopulateArticleActionCounts()
        {
            context.Database.ExecuteSqlCommand(@"update Articles set TotalLikes = 
                (select count(*) from ArticleActions where ArticleId=Articles.Id and ArticleActionType=@likes)
                update Articles set TotalReads = 
                (select count(*) from ArticleActions where ArticleId=Articles.Id and ArticleActionType=@reads)",
                new SqlParameter("likes", ArticleActionType.LIKE),
                new SqlParameter("reads", ArticleActionType.READ)
            );
        }

    }
}
