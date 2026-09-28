using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ITSupportCenter.Services
{
    public class ActionHistoryItem
    {
        public string ToolId { get; set; } = "";
        public string ToolTitle { get; set; } = "";
        public string Category { get; set; } = "";
        public DateTime ExecutedAt { get; set; } = DateTime.Now;
        public double DurationSeconds { get; set; }
        public string Status { get; set; } = "Success"; // Success, Warning, Error
        public string Summary { get; set; } = "";
    }

    public static class SessionHistoryTracker
    {
        private static readonly List<ActionHistoryItem> _items = new();
        private static readonly string _historyFilePath = Path.Combine(Path.GetTempPath(), $"ITSupportCenter_session_{Environment.MachineName}.json");

        public static void Record(string toolId, string toolTitle, string category, double durationSeconds, string status, string summary)
        {
            var item = new ActionHistoryItem
            {
                ToolId = toolId,
                ToolTitle = toolTitle,
                Category = category,
                ExecutedAt = DateTime.Now,
                DurationSeconds = durationSeconds,
                Status = status,
                Summary = summary
            };

            lock (_items)
            {
                _items.Add(item);
                SaveToFile();
            }
        }

        public static IReadOnlyList<ActionHistoryItem> GetSessionItems()
        {
            lock (_items)
            {
                return _items.ToArray();
            }
        }

        private static void SaveToFile()
        {
            Task.Run(() =>
            {
                try
                {
                    string json;
                    lock (_items)
                    {
                        json = JsonSerializer.Serialize(_items, new JsonSerializerOptions { WriteIndented = true });
                    }
                    File.WriteAllText(_historyFilePath, json);
                }
                catch { }
            });
        }
    }
}
