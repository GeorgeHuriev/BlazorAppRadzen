using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp1.Server.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        private readonly IMemoryCache cache = null;
        private readonly string ConnectionString = null;

        /// <summary>
        /// Для вызова из контроллера API.
        /// </summary>
        /// <param name="memoryCache"></param>
        public ApplicationDbContextFactory(IMemoryCache memoryCache)
        {
            cache = memoryCache;
        }

        public ApplicationDbContextFactory(string cs)
        {
            ConnectionString = cs;
        }

        public ApplicationDbContextFactory()
        {

        }

        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Если был получен доступ к кешу.
            if (cache != null)
            {
                var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
                cache.TryGetValue("Configuration", out IConfiguration _configuration);

                // Если в кеше не конфига, то обращаемся к конфигурационному файлу.
                if (_configuration == null)
                {
                    return DefaultContext();
                }
                // Если есть конфиг, берем из него строку подключения.
                else
                {
                    var cs = _configuration["ConnectionStrings:DefaultConnection"];
                    return ContextByConnectionString(cs);
                }
            }
            // Если есть строка подключения, то строим DbContext на ее основе.
            else if (!string.IsNullOrEmpty(ConnectionString))
            {
                return ContextByConnectionString(ConnectionString);
            }
            // В любом ином случае обращаемся к конфигурационному файлу.
            else
            {
                return DefaultContext();
            }
        }

        private ApplicationDbContext DefaultContext()
        {
            var basePath = Directory.GetCurrentDirectory();
            var configuration = new ConfigurationBuilder()
                                .SetBasePath(Environment.CurrentDirectory)
                                .AddJsonFile("appsettings.json", reloadOnChange: true, optional: false)
                                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var cs = configuration["ConnectionStrings:DefaultConnection"];
            builder.UseNpgsql(cs);
            return new ApplicationDbContext(builder.Options, null);
        }

        public ApplicationDbContext ContextByConnectionString(string cs)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseNpgsql(cs);
            return new ApplicationDbContext(builder.Options, null);
        }
    }
}