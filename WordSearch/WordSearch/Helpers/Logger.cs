using Microsoft.AppCenter.Analytics;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            try
            {
                Log.Error(Defines.TAG, text);
                LogToDbAsync("ERROR: " + text);
                System.Diagnostics.Debug.WriteLine("ERROR: " + text);
                // Log to MS AppCenter
                Analytics.TrackEvent("Error", new Dictionary<string, string> { { "Exception", text } });
            }
            catch (Exception ex)
            {
                string error = $"Logger.Error exception, {ex.Message}";
                Log.Error(Defines.TAG, error);
                System.Diagnostics.Debug.WriteLine(error);
            }
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

        internal bool LoadAllRecords(out List<DebugLog> results)
        {
            bool bOK = true;
            results = new List<DebugLog>();
            try
            {
                using (var db = new DbContextLogs())
                {
                    results = db.Logs.OrderByDescending(item => item.CreatedDate).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"LoadAllRecords exception, {ex.Message}");
                bOK = false;
            }
            return bOK;
        }

        internal async Task<bool> DeleteAllRecords()
        {
            bool bOK = true;
            try
            {
                using (var db = new DbContextLogs())
                {
                    foreach (var data in db.Logs)
                        db.Logs.Remove(data);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"DeleteAllRecords exception, {ex.Message}");
                bOK = false;
            }
            return bOK;
        }
    }
}
