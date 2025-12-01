using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Messenger
{
    public static class ChatHistory
    {
        private static readonly string HistoryFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Messenger", "chat_history.json");

        static ChatHistory()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(HistoryFilePath)!);
        }

        public static async Task<List<Message>> LoadAsync()
        {
            if (!File.Exists(HistoryFilePath))
                return new List<Message>();

            try
            {
                var json = await File.ReadAllTextAsync(HistoryFilePath);
                var messages = JsonSerializer.Deserialize<List<Message>>(json);
                return messages ?? new List<Message>();
            }
            catch
            {
                return new List<Message>();
            }
        }

        public static async Task SaveAsync(List<Message> messages)
        {
            try
            {
                var json = JsonSerializer.Serialize(messages, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(HistoryFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения истории: {ex.Message}");
            }
        }
    }
}