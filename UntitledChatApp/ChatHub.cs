using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Common.Caching;

namespace UntitledChatApp
{
    class ChatRoom
    {
        public Guid Id { get; set; }
        public string Label { get; set; }
        public List<ChatUser> Users { get; private set; }

        public ChatRoom()
        {
            Users = new List<ChatUser>();
        }
    }

    class ChatRoomCache : LRUCache<string, ChatRoom>
    {
        public ChatRoomCache() : base(5000) { }
    }

    class ChatConnection : PersistentConnection
    {

    }

    class ConnectionCache
    {
        readonly LRUCache<string, ChatUser> connectionIdUserLookup;

        public ConnectionCache()
        {
            connectionIdUserLookup = new LRUCache<string, ChatUser>(20000);
        }

        public ChatUser FindUser(string connectionId)
        {
            var user = connectionIdUserLookup.Get(connectionId);
            return user;
        }

        public void AddUser(string connectionId, ChatUser user)
        {
            connectionIdUserLookup.AddOrUpdate(connectionId, user);
        }
    }

    public class ChatHub : Hub
    {
        static ConnectionCache connectionCache = new ConnectionCache();
        static ChatRoomCache roomCache = new ChatRoomCache();

        const string ROOM = "xxxx";

        /// <summary>
        /// Here is where we would get their location, from their GPS
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnected()
        {
            await JoinUserToRoom();
            await base.OnConnected();
        }

        /// <summary>
        /// same, get their location here, and assign a group
        /// </summary>
        /// <returns></returns>
        public override async Task OnReconnected()
        {
            await JoinUserToRoom();
            await base.OnReconnected();
        }

        async Task JoinUserToRoom()
        {
            // get existing or create new room
            var room = roomCache.Get(ROOM);
            if (room == null)
            {
                room = new ChatRoom
                {
                    Id = Guid.NewGuid(),
                    Label = "chat room",
                };
                roomCache.AddOrUpdate(ROOM, room);
            }

            // get existing or create new user
            var key = GetUserLookupKey();
            var user = connectionCache.FindUser(key);
            if (user == null)
            {
                user = new ChatUser
                {
                    Id = Guid.NewGuid(),
                    Name = null,
                    ConnectionId = Context.ConnectionId,
                    CurrentRoom = ROOM, // you would replace this with a geo location lookup based on IP ADDRESS
                };
                connectionCache.AddUser(key, user);
            }

            room.Users.Add(user);
            await Groups.Add(Context.ConnectionId, user.CurrentRoom);
        }

        public void SetUserName(string name)
        {
            var key = GetUserLookupKey();
            var user = connectionCache.FindUser(key);
            if (user != null)
            {
                user.Name = name;
            }
        }

        public void Send(string message)
        {
            var key = GetUserLookupKey();
            var user = connectionCache.FindUser(key);
            if (user != null && !string.IsNullOrWhiteSpace(user.Name))
            {
                var roomId = user.CurrentRoom;
                var room = roomCache.Get(roomId);
                foreach (var other in room.Users)
                {
                    this.Clients.Client(other.ConnectionId)
                        .broadcastMessage(user.Name, message);
                }
            }
        }

        string GetUserLookupKey()
        {
            //return GetIpAddress();
            return Context.ConnectionId;
        }





        //protected string GetIpAddress()
        //{
        //    string ipAddress;
        //    object tempObject;

        //    Context.Request.Environment.TryGetValue("server.RemoteIpAddress", out tempObject);

        //    if (tempObject != null)
        //    {
        //        ipAddress = (string)tempObject;
        //    }
        //    else
        //    {
        //        ipAddress = "";
        //    }

        //    return ipAddress;
        //}

        //protected string GetIpAddress()
        //{
        //    var env = Get<IDictionary<string, object>>(Context.Request.Items, "owin.environment");
        //    if (env == null)
        //    {
        //        return null;
        //    }
        //    var ipAddress = Get<string>(env, "server.RemoteIpAddress");
        //    return ipAddress;
        //}

        //private static T Get<T>(IDictionary<string, object> env, string key)
        //{
        //    object value;
        //    return env.TryGetValue(key, out value) ? (T)value : default(T);
        //}
    }
}