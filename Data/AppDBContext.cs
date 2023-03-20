using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CachingWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CachingWebApi.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }

        public DbSet<Driver> Drivers {get; set; }
    }
}