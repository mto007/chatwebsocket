
using System.Collections.Generic;
using System.Linq;


namespace ChatWebSocket
{
    public interface ICustomWebSocketFactory
    {
        void Add(CustomWebSocket uws);

        bool AddUser(string users);

        void AddMessage(CustomWebSocketMessage msg);

        void DeleteMessage(CustomWebSocketMessage msg);

        void ModifyMessage(CustomWebSocketMessage msg);
        void Remove(string username);
        List<CustomWebSocket> All();

        List<string> GetUserNames();
        List<CustomWebSocket> Others(CustomWebSocket client);
        CustomWebSocket Client(string username);

        bool IsClientUnique(string id);

        List<CustomWebSocketMessage> GetMessages ();
    }

    public class CustomWebSocketFactory : ICustomWebSocketFactory
    {
        List<CustomWebSocket> List;
        List<CustomWebSocketMessage> Messages;
        List<string> UserNames;

        public CustomWebSocketFactory()
        {
            List = new List<CustomWebSocket>();
            Messages = new List<CustomWebSocketMessage>();
            UserNames = new List<string>();
        }

        public void Add(CustomWebSocket uws)
        {
            List.Add(uws);
        }
        public void AddMessage(CustomWebSocketMessage msg)
        {
            Messages.Add(msg);
        }
        public bool AddUser(string user)
        {
            if (UserNames.IndexOf(user) < 0)
            {
                UserNames.Add(user);
                return true;
            }
            return false;
        }

        public void ModifyMessage(CustomWebSocketMessage msg)
        {
            var foundIndex = Messages.FindIndex(ind => ind.Id == msg.Id);
            if (foundIndex > -1)
            {
                msg.Text = msg.Text + "( edited )";
                msg.Status = 1;
                Messages[foundIndex] = msg;
            }

        }

        public void DeleteMessage(CustomWebSocketMessage msg)
        {
            var foundIndex = Messages.FindIndex(ind => ind.Id == msg.Id);
            if (foundIndex > -1)
            {
                Messages[foundIndex].Text = "This message was deleted.";
                Messages[foundIndex].Status = 2;
            }
        }

        public void Remove(string username)
        {
            List.Remove(Client(username));
        }

        public bool IsClientUnique(string id)
        {
            return List.FindIndex(u => u.Id == id) <= -1;
        }

        public List<CustomWebSocket> All()
        {
            return List;
        }

        public List<string> GetUserNames()
        {
            return UserNames;
        }

        public List<CustomWebSocketMessage> GetMessages()
        {
            return Messages;
        }

        public List<CustomWebSocket> Others(CustomWebSocket client)
        {
            return List.Where(c => c.Id != client.Id).ToList();
        }

        public CustomWebSocket Client(string id)
        {
            return List.First(c => c.Id == id);
        }
    }
}
