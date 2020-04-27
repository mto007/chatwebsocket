using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatWebSocket
{
    public class CustomWebSocketData
    {
        public List<string> UserNames { get; set; }

        public List<CustomWebSocketMessage> Messages { get; set; }
    }
}
