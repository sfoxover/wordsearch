using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WordSearch.Util;
using Xamarin.Forms;

namespace WordSearch.Models
{
    class DbContextWords : DbContext
    {
        string DBPath { get; set; }
        public DbSet<Word> Words { get; set; }

        public DbContextWords()
        {
            DBPath = DependencyService.Get<IDependencyHelper>().GetLocalDatabaseFilePath("words.db3");
            Debug.WriteLine("DbContextScores path: " + DBPath);
        }

        public DbContextWords(string dbPath)
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
