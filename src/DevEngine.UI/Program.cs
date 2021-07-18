using DevEngine.Core.Project;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevEngine.UI
{
    public class Program
    {
        public static IDevProject Project { get; private set; } = null!;

        public static string Folder { get; private set; } = null!;

        public static void Main(string[] args)
        {
            if (args.Length != 1)
                throw new Exception("Expected project path as argument");

            Project = new FakeTypes.Project.DevProject(System.IO.Path.GetFileName(args[0]), new RealTypes.RealTypesProviderService());
            Folder = args[0];

            if (!System.IO.File.Exists(System.IO.Path.Combine(args[0], "project.json")))
                Project.Save(Folder);
            else
                Project.Load(Folder);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
