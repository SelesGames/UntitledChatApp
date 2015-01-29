using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Common.Caching;
using UntitledChatApp.Core;
using UntitledChatApp.Core.Graph;

namespace UntitledChatApp
{
    public class UserInfo
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class ChatHub : Hub
    {
        static readonly UserNodeCache userNodeCache = new UserNodeCache();
        static readonly RoomTree tree = RoomTree.Instance;




        #region OnConnected and OnReconnected overrides

        public override async Task OnConnected()
        {
            await base.OnConnected();
        }

        public override async Task OnReconnected()
        {
            await base.OnReconnected();
        }

        #endregion




        #region Find User - either by UserId or ConnectionId

        /// <summary>
        /// Finds UserNode by the current connection Id.  Throws an exception if no user matches
        /// the current context ConnectionId.
        /// </summary>
        UserNode FindUserByConnectionId()
        {
            var key = Context.ConnectionId;
            var user = userNodeCache.FindByConnectionId(key);
            if (user == null)
                throw new Exception("user not found");

            return user;
        }

        /// <summary>
        /// Finds UserNode by the current connection Id.  Throws an exception if no user matches
        /// the current context ConnectionId.
        /// </summary>
        UserNode FindUserByUserId(Guid userId)
        {
            var user = userNodeCache.FindByUserId(userId);
            if (user == null)
                throw new Exception("user not found");

            return user;
        }

        #endregion




        #region Set properties on the user (ConnectionId, Location, UserName)

        /// <summary>
        /// Given a userName, create a new UserNode, and map the current ConnectionId and
        /// UserId to the newly created UserNode.
        /// 
        /// Use this when no cookie has been recovered in the browser.
        /// </summary>
        public UserInfo CreateUser(string userName)
        {
            ValidateUserName(userName);

            var user = new UserNode();
            user.UserName = userName;
            var connectionId = Context.ConnectionId;
            user.ConnectionId = connectionId;
            userNodeCache.MapConnectionId(connectionId, user);
            userNodeCache.MapUserId(user.Id, user);

            return new UserInfo { id = user.Id.ToString("N"), name = user.UserName };
        }

        /// <summary>
        /// Validate the user's name.  Should also check for uniqueness in a given room.
        /// </summary>
        static void ValidateUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new Exception("invalid userName!");
        }

        /// <summary>
        /// Bind a recovered UserId to the current ConnectionId.  If no user was found, 
        /// create a new one with the same UserId and UserName, and map the UserId to the 
        /// user.  Lastly, map the current ConnectionId to the user.
        /// </summary>
        public void BindConnection(Guid userId, string userName)
        {
            var user = userNodeCache.FindByUserId(userId);
            if (user == null)
            {
                user = new UserNode { Id = userId, UserName = userName };
                userNodeCache.MapUserId(userId, user);
            }

            var connectionId = Context.ConnectionId;
            user.ConnectionId = connectionId;
            userNodeCache.MapConnectionId(connectionId, user);
        }

        public void Login(Guid userId, string userName, double latitude, double longitude)
        {
            var user = userNodeCache.FindByUserId(userId);
            if (user == null)
            {
                ValidateUserName(userName);
                user = new UserNode { Id = userId, UserName = userName };
                userNodeCache.MapUserId(userId, user);
            }

            // set the ConnectionId
            var connectionId = Context.ConnectionId;
            user.ConnectionId = connectionId;
            userNodeCache.MapConnectionId(connectionId, user);

            // set the Location
            DecimalCoordinates coords;
            coords.decimalLatitude = latitude;
            coords.decimalLongitude = longitude;
            user.MidPoint = coords.ToRadians().ToCartesian();
            tree.AddUser(user);
        }

        /// <summary>
        /// Set the user's location, post-Login.
        /// </summary>
        public void SetLocation(double latitude, double longitude)
        {
            DecimalCoordinates decimalCoordinates;
            decimalCoordinates.decimalLatitude = latitude;
            decimalCoordinates.decimalLongitude = longitude;

            var user = FindUserByConnectionId();
            user.MidPoint = decimalCoordinates.ToRadians().ToCartesian();
        }

        /// <summary>
        /// Set the user's UserName, post-Login.
        /// </summary>
        public void SetUserName(string name)
        {
            var user = FindUserByConnectionId();
            if (user != null)
            {
                user.UserName = name;
            }
        }

        #endregion




        #region Broadcast a message to the users in this user's room

        /// <summary>
        /// Broadcast a message to every user in this UserNode's room.  Only valid post-Login.
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Send(string message)
        {
            var user = FindUserByConnectionId();
            if (!string.IsNullOrWhiteSpace(user.UserName))
            {
                var userName = user.UserName;
                var room = user.Parent as RoomNode;
                var roomUsers = room.Children.OfType<UserNode>();
                foreach (var node in roomUsers)
                {
                    this.Clients.Client(node.ConnectionId)
                        .broadcastMessage(userName, message);
                }
            }
        }

        #endregion




        //async Task JoinUserToRoom()
        //{
        //    // get existing or create new room
        //    var room = roomCache.Get(ROOM);
        //    if (room == null)
        //    {
        //        room = new ChatRoom
        //        {
        //            Id = Guid.NewGuid(),
        //            Label = "chat room",
        //        };
        //        roomCache.AddOrUpdate(ROOM, room);
        //    }

        //    // get existing or create new user
        //    var key = GetUserLookupKey();
        //    var user = userNodeCache.FindUser(key);
        //    if (user == null)
        //    {
        //        user = new ChatUser
        //        {
        //            Id = Guid.NewGuid(),
        //            Name = null,
        //            ConnectionId = Context.ConnectionId,
        //            CurrentRoom = ROOM, // you would replace this with a geo location lookup based on IP ADDRESS
        //        };
        //        userNodeCache.AddUser(key, user);
        //    }

        //    room.Users.Add(user);
        //    await Groups.Add(Context.ConnectionId, user.CurrentRoom);
        //}



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