using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace ChatWebSocket
{
    public class CustomWebSocketMainMessage
    {
        // U or M
        public string Type { get; set; }

        public WebSocketMessageType WSType { get; set; }

        public string UserName { get; set; }

        public CustomWebSocketMessage Message { get; set; }
    }
}
