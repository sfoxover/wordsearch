using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WordSearch.Helpers;
using Xamarin.Forms;

namespace WordSearch.Models
{
    class DbContextLogs : DbContext
    {
        string DBPath { get; set; }
        public DbSet<DebugLog> Logs { get; set; }

        public DbContextLogs()
        {
            DBPath = DependencyService.Get<IDependencyHelper>().GetLocalDatabaseFilePath("logs.db3");
            Debug.WriteLine("DbContextLogs path: " + DBPath);
        }

        public DbContextLogs(string dbPath)
        {
            DBPath = dbPath;
            Debug.WriteLine("DbContextLogs path: " + DBPath);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Filename={DBPath}");
        }
    }
}
