using System.Collections.Generic;
using System.Linq;

namespace UntitledChatApp
{
    /// <summary>
    /// Maps a T key to a set of connection Ids
    /// </summary>
    /// <typeparam name="T">The type of key to be used</typeparam>
    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> connectionsLookup =
            new Dictionary<T, HashSet<string>>();

        public int Count
        {
            get
            {
                return connectionsLookup.Count;
            }
        }

        public void Add(T key, string connectionId)
        {
            lock (connectionsLookup)
            {
                HashSet<string> connections;
                if (!connectionsLookup.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    connectionsLookup.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (connectionsLookup.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (connectionsLookup)
            {
                HashSet<string> connections;
                if (!connectionsLookup.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        connectionsLookup.Remove(key);
                    }
                }
            }
        }
    }
}