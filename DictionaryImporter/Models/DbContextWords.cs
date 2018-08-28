using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryImporter.Models
{
    class DbContextWords : DbContext
    {
        string DBPath { get; set; }
        public DbSet<Word> Words { get; set; }

        public DbContextWords()
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            DBPath = Path.Combine(path, "words.db3");
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

