using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EonaCat.UFW.Manager.Models;
using System.Threading;

namespace EonaCat.UFW.Manager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CultureInfo UsCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = UsCulture;
            CultureInfo.DefaultThreadCurrentUICulture = UsCulture;

            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
