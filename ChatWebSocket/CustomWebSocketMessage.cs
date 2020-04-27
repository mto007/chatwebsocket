using System;
using System.Net.WebSockets;

namespace ChatWebSocket
{
    public class CustomWebSocketMessage
    {
        public string Text { get; set; }
        public DateTime MessagDateTime { get; set; }
        public string Username { get; set; }

        public bool IsInitialMessage { get; set; }

        public int Status { get; set; }

        public Guid? Id { get; set; }

        
    }
}
