using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using System.Threading;

namespace ChatWebSocket
{
    public interface ICustomWebSocketMessageHandler
    {
        Task SendInitialMessages(CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
        Task HandleMessage(string message, byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
        Task BroadcastToOthers(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
        Task BroadcastToAll(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
    }

    public class CustomWebsocketMessageHandler : ICustomWebSocketMessageHandler
    {
        public async Task SendInitialMessages(CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory)
        {
            WebSocket webSocket = userWebSocket.WebSocket;
            var msg = new CustomWebSocketMessage
            {
                MessagDateTime = DateTime.Now,
                Text = $"T{userWebSocket.Id} joined the chat",
                Username = userWebSocket.Id,
                Id = Guid.NewGuid(),
                IsInitialMessage = true,
                Status = 0
            };
            wsFactory.AddMessage(msg);
            var websocketData = new CustomWebSocketData
            {
                Messages = wsFactory.GetMessages(),
                UserNames = wsFactory.GetUserNames()
            };

            var jsonStr = JsonSerializer.Serialize<CustomWebSocketData>(websocketData);
            byte[] bytes = Encoding.ASCII.GetBytes(jsonStr);
            await BroadcastToAll(bytes, userWebSocket, wsFactory);
        }

        public async Task HandleMessage(string msg, byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory)
        {
            
            try
            {
                var message = JsonSerializer.Deserialize<CustomWebSocketMainMessage>(msg);
                if (message.WSType == WebSocketMessageType.Text)
                {
                    if (message.Type == "U")
                    {
                         if (wsFactory.AddUser(message.UserName))
                        {
                            var userMsg = new CustomWebSocketMessage
                            {
                                MessagDateTime = DateTime.Now,
                                Text = $"{message.UserName} joined the chat",
                                Username = message.UserName,
                                Id = Guid.NewGuid(),
                                IsInitialMessage = true,
                                Status = 0
                            };
                            wsFactory.AddMessage(userMsg);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (message.Message.Status == 2)
                        {
                            // delete message
                            wsFactory.DeleteMessage(message.Message);
                        }
                        else if (message.Message.Status == 1)
                        {
                            wsFactory.ModifyMessage(message.Message);
                        }
                        else
                        {
                            var newMessage = new CustomWebSocketMessage
                            {
                                Id = Guid.NewGuid(),
                                MessagDateTime = DateTime.Now,
                                Text = message.Message.Text,
                                Username = message.Message.Username,
                                IsInitialMessage = false,
                                Status = 0
                            };
                            wsFactory.AddMessage(newMessage);
                        }
                    }
                   
                    var websocketData = new CustomWebSocketData
                    {
                        Messages = wsFactory.GetMessages(),
                        UserNames = wsFactory.GetUserNames()
                    };

                    var jsonStr = JsonSerializer.Serialize<CustomWebSocketData>(websocketData);
                    byte[] bytes = Encoding.ASCII.GetBytes(jsonStr);
                    await BroadcastToAll(bytes, userWebSocket, wsFactory);
                }
            }
            catch (Exception e)
            {
                await userWebSocket.WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public async Task BroadcastToOthers(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory)
        {
            var others = wsFactory.Others(userWebSocket);
            foreach (var uws in others)
            {
                await uws.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public async Task BroadcastToAll(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory)
        {
            var all = wsFactory.All();
            foreach (var uws in all)
            {
                await uws.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}