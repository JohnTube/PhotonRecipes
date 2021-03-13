using Photon.Realtime;

namespace PolyTics.Photon.Client.Realtime
{
    using System.Collections.Generic;
    using ExitGames.Client.Photon;
    using System.Linq;

    public static partial class ExtensionMethods
    {
        public static bool AddExpectedUser(this Room room, string userId, bool force = false, WebFlags webFlags = null, bool broadcast = true)
        {
            return !string.IsNullOrEmpty(userId) && room.AddExpectedUsers(new[] { userId }, force, webFlags, broadcast);
        }

        public static bool RemoveExpectedUser(this Room room, string userId, WebFlags webFlags = null, bool broadcast = true)
        {
            return !string.IsNullOrEmpty(userId) && room.RemoveExpectedUsers(new[] { userId }, webFlags, broadcast);
        }

        public static bool AddExpectedUsers(this Room room, string[] userIds, bool force = false, WebFlags webFlags = null, bool broadcast = true)
        {
            if (userIds == null || userIds.Length == 0)
            {
                return false;
            }

            HashSet<string> hashSet;
            if (room.ExpectedUsers != null)
            {
                hashSet = new HashSet<string>(room.ExpectedUsers);
            }
            else
            {
                hashSet = new HashSet<string>();
            }

            for (int i = 0; i < userIds.Length; i++)
            {
                string userId = userIds[i];
                if (string.IsNullOrEmpty(userId))
                {
                    return false;
                }

                if (!hashSet.Add(userId))
                {
                    return false;
                }
            }

            int maxPlayers = room.MaxPlayers;
            if (room.MaxPlayers > 0 && hashSet.Count + room.PlayerCount > room.MaxPlayers)
            {
                if (!room.PublishUserId)
                {
                    return false;
                }

                int alreadyTaken = 0;
                foreach (string user in hashSet)
                {
                    if (room.Players.Values.Any(player => user.Equals(player.UserId)))
                    {
                        alreadyTaken++;
                    }
                }

                if (hashSet.Count + room.PlayerCount - alreadyTaken > room.MaxPlayers)
                {
                    if (force)
                    {
                        maxPlayers = hashSet.Count + room.PlayerCount - alreadyTaken;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return room.SetExpectedUsers(hashSet, maxPlayers, webFlags, broadcast);
        }

        public static bool RemoveExpectedUsers(this Room room, string[] userIds, WebFlags webFlags = null, bool broadcast = true)
        {
            if (userIds == null || userIds.Length == 0)
            {
                return false;
            }

            if (room.ExpectedUsers == null || room.ExpectedUsers.Length == 0)
            {
                return false;
            }

            if (userIds.Length > room.ExpectedUsers.Length)
            {
                return false;
            }

            HashSet<string> hashSet = new HashSet<string>(room.ExpectedUsers);
            for (int i = 0; i < userIds.Length; i++)
            {
                string userId = userIds[i];
                if (string.IsNullOrEmpty(userId))
                {
                    return false;
                }

                if (!hashSet.Remove(userId))
                {
                    return false;
                }
            }

            return room.SetExpectedUsers(hashSet, webFlags:webFlags, broadcast:broadcast);
        }

        public static bool ChangeExpectedUsers(this Room room, string[] toRemove, string[] toAdd, bool force = false, WebFlags webFlags = null, bool broadcast = true)
        {
            HashSet<string> hashSet = null;
            if (toRemove != null && toRemove.Length > 0)
            {
                if (room.ExpectedUsers == null)
                {
                    return false;
                }

                if (toRemove.Length > room.ExpectedUsers.Length)
                {
                    return false;
                }

                hashSet = new HashSet<string>(room.ExpectedUsers);
                for (int i = 0; i < toRemove.Length; i++)
                {
                    string userId = toRemove[i];
                    if (string.IsNullOrEmpty(userId))
                    {
                        return false;
                    }

                    if (!hashSet.Remove(userId))
                    {
                        return false;
                    }
                }
            }
            else if (toAdd != null && toAdd.Length > 0)
            {
                if (room.ExpectedUsers == null)
                {
                    hashSet = new HashSet<string>();
                }
                else
                {
                    hashSet = new HashSet<string>(room.ExpectedUsers);
                }
            }
            else
            {
                return false;
            }

            for (int i = 0; i < toAdd.Length; i++)
            {
                string userId = toAdd[i];
                if (string.IsNullOrEmpty(userId))
                {
                    return false;
                }

                if (!hashSet.Add(userId))
                {
                    return false;
                }
            }

            int maxPlayers = room.MaxPlayers;
            if (room.MaxPlayers > 0 && hashSet.Count + room.PlayerCount > room.MaxPlayers)
            {
                if (!room.PublishUserId)
                {
                    return false;
                }

                int alreadyTaken = 0;
                foreach (string user in hashSet)
                {
                    if (room.Players.Values.Any(player => user.Equals(player.UserId)))
                    {
                        alreadyTaken++;
                    }
                }

                if (hashSet.Count + room.PlayerCount - alreadyTaken > room.MaxPlayers)
                {
                    if (force)
                    {
                        maxPlayers = hashSet.Count + room.PlayerCount - alreadyTaken;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return room.SetExpectedUsers(hashSet, maxPlayers, webFlags, broadcast);
        }

        public static bool ReplaceExpectedUser(this Room room, string toRemove, string toAdd, WebFlags webFlags = null, bool broadcast = true)
        {
            if (string.IsNullOrEmpty(toRemove) || string.IsNullOrEmpty(toAdd) || toRemove.Equals(toAdd))
            {
                return false;
            }

            if (room.ExpectedUsers == null || !room.ExpectedUsers.Contains(toRemove))
            {
                return false;
            }

            HashSet<string> hashSet = new HashSet<string>(room.ExpectedUsers);
            if (!hashSet.Remove(toRemove))
            {
                return false;
            }

            if (!hashSet.Add(toAdd))
            {
                return false;
            }

            return room.SetExpectedUsers(hashSet, webFlags:webFlags, broadcast:broadcast);
        }

        public static bool SetExpectedUsers(this Room room, HashSet<string> newExpectedUsers, int? newMaxPlayers = null, WebFlags webFlags = null, bool broadcast = true)
        {
            Hashtable hash = new Hashtable();
            Hashtable expected = new Hashtable();
            if (newMaxPlayers.HasValue)
            {
                hash.Add(GamePropertyKey.MaxPlayers, newMaxPlayers);
                expected.Add(GamePropertyKey.MaxPlayers, room.MaxPlayers);
            }

            if (newExpectedUsers == null)
            {
                hash.Add(GamePropertyKey.ExpectedUsers, null);
            }
            else
            {
                hash.Add(GamePropertyKey.ExpectedUsers, newExpectedUsers.ToArray());
            }

            if (room.ExpectedUsers != null)
            {
                expected.Add(GamePropertyKey.ExpectedUsers, room.ExpectedUsers);
            }

            ParameterDictionary opParameters = new ParameterDictionary(3);
            opParameters.Add(ParameterCode.Properties, hash);
            opParameters.Add(ParameterCode.ExpectedValues, expected);
            if (broadcast)
            {
                opParameters.Add(ParameterCode.Broadcast, true);
            }
            if (webFlags != null)
            {
                opParameters.Add(ParameterCode.EventForward, webFlags.WebhookFlags);
            }
            return room.LoadBalancingClient.LoadBalancingPeer.SendOperation(OperationCode.SetProperties, opParameters,
                SendOptions.SendReliable);
        }
    }
}