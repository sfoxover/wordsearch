using Serilog;
using System;
using WordSearch.Models;

namespace WordSearch.Helpers
{
    public class Logger
    {
        private static readonly Lazy<Logger> lazy = new Lazy<Logger>(() => new Logger());
        public static Logger Instance { get { return lazy.Value; } }

        private Logger()
        {
            EnsureCreated();
        }

        public void Debug(string text)
        {
            Log.Debug(Defines.TAG, text);
#if DEBUG
            LogToDbAsync(text);
#endif
            System.Diagnostics.Debug.WriteLine(text);
        }

        public void EnsureCreated()
        {
            try
            {
                using (var db = new DbContextLogs())
                {
                    db.Database.EnsureCreated();
                }
            }
            catch (Exception ex)
            {
                Log.Error(Defines.TAG, "EnsureCreated exception, " + ex.Message);
            }
        }

        public void Error(string text)
        {
            Log.Error(Defines.TAG, text);
            LogToDbAsync("ERROR: " + text);
            System.Diagnostics.Debug.WriteLine("ERROR: " + text);
        }

        // log message to database
        public async void LogToDbAsync(string text)
        {
            try
            {
                using (var db = new DbContextLogs())
                {
                    var log = new DebugLog();
                    log.Log = text;
                    await db.Logs.AddAsync(log);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Error(Defines.TAG, "LogToDbAsync exception, " + ex.Message);
            }
        }
    }
}
