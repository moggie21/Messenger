using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Messenger
{
    public class Message
    {
        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("recipient")]
        public string? Recipient { get; set; }

        public Message()
        {
            Timestamp = DateTime.UtcNow;
        }

        public Message(string author, string text, string? recipient = null)
        {
            Author = author ?? throw new ArgumentNullException(nameof(author));
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Recipient = recipient;
            Timestamp = DateTime.UtcNow;
        }

        public override string ToString()
        {
            if (Recipient == null)
            {
                return $"[{Timestamp.ToLocalTime():HH:mm}] {Author}: {Text}";
            }
            else
            {
                return $"[{Timestamp.ToLocalTime():HH:mm}] {Author} → {Recipient}: {Text}";
            }
        }


    }
}
