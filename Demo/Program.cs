using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Demo
{
    public class Program
    {
        public const string tokenKey = "My Login Token Key Is Here";
        // i needed below variable to generate demo data
        // it's used by the Model/Initialize Object
        public static bool Initializing { get; set; }

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://*:5000")
                .UseDefaultServiceProvider(options => options.ValidateScopes = false)
                .Build();

    }
}
