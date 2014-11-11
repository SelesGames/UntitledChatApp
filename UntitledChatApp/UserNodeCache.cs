using Common.Caching;
using System;
using UntitledChatApp.Core.Graph;

namespace UntitledChatApp
{
    class UserNodeCache
    {
        const int CACHE_SIZE = 1000000;

        readonly LRUCache<string, UserNode> connectionIdUserLookup;
        readonly LRUCache<Guid, UserNode> userIdUserLookup;

        public UserNodeCache()
        {
            connectionIdUserLookup = new LRUCache<string, UserNode>(CACHE_SIZE);
            userIdUserLookup = new LRUCache<Guid, UserNode>(CACHE_SIZE);
        }

        public UserNode FindByConnectionId(string connectionId)
        {
            var user = connectionIdUserLookup.Get(connectionId);
            return user;
        }

        public void MapConnectionId(string connectionId, UserNode user)
        {
            connectionIdUserLookup.AddOrUpdate(connectionId, user);
        }

        public UserNode FindByUserId(Guid userId)
        {
            var user = userIdUserLookup.Get(userId);
            return user;
        }

        public void MapUserId(Guid userId, UserNode user)
        {
            userIdUserLookup.AddOrUpdate(userId, user);
        }
    }
}