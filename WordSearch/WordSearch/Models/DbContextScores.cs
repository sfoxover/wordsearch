using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WordSearch.Helpers;
using Xamarin.Forms;

namespace WordSearch.Models
{
    public class DbContextScores : DbContext
    {
        string DBPath { get; set; }
        public DbSet<Score> Scores { get; set; }

        public DbContextScores()
        {
            DBPath = DependencyService.Get<IDependencyHelper>().GetLocalDatabaseFilePath("scores.db3");
            Debug.WriteLine("DbContextScores path: " + DBPath);
        }

        public DbContextScores(string dbPath)
        {
            DBPath = dbPath;
            Debug.WriteLine("DbContextScores path: " + DBPath);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Filename={DBPath}");
        }
    }
}
