using Photon.Realtime;

namespace PolyTics.Photon.Recipes
{
    using System;
    using System.Collections.Generic;
    using ExitGames.Client.Photon;
    using System.Linq;

    public static partial class ExtensionMethods
    {
        #region Expected Users

        public static bool AddExpectedUser(this Room room, string userId, bool force = false)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }
            if (room.ExpectedUsers != null && Array.IndexOf(room.ExpectedUsers, userId) != -1)
            {
                return false;
            }
            Hashtable hash;
            Hashtable expected = null;
            HashSet<string> hashSet;
            if (room.ExpectedUsers != null)
            {
                hashSet = new HashSet<string>(room.ExpectedUsers);
            }
            else
            {
                hashSet = new HashSet<string>();
            }
            hashSet.Add(userId);
            if (room.MaxPlayers > 0 && hashSet.Count + room.PlayerCount > room.MaxPlayers)
            {
                if (room.PlayerCount > 1 && room.Players.Values.Any(player => !player.IsLocal && string.IsNullOrEmpty(player.UserId)))
                {
                    return false; // Publish UserId is disabled!
                }
                int alreadyTaken = 0;
                foreach (string user in hashSet)
                {
                    if (room.Players.Values.Any(player => user.Equals(player.UserId)))
                    {
                        alreadyTaken++;
                    }
                }
                if (alreadyTaken > 0)
                {
                    hash = new Hashtable(1);
                    if (room.ExpectedUsers != null)
                    {
                        expected = new Hashtable(1);
                    }
                }
                else if (force)
                {
                    hash = new Hashtable(2);
                    hash.Add(GamePropertyKey.MaxPlayers, room.MaxPlayers + 1);
                    if (room.ExpectedUsers != null)
                    {
                        expected = new Hashtable(2);
                    }
                    else
                    {
                        expected = new Hashtable(1);
                    }
                    expected.Add(GamePropertyKey.MaxPlayers, room.MaxPlayers);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                hash = new Hashtable(1);
                if (room.ExpectedUsers != null)
                {
                    expected = new Hashtable(1);
                }
            }
            hash.Add(GamePropertyKey.ExpectedUsers, hashSet.ToArray());
            if (room.ExpectedUsers != null)
            {
                expected.Add(GamePropertyKey.ExpectedUsers, room.ExpectedUsers);
            }
            return room.LoadBalancingClient.OpSetPropertiesOfRoom(hash, expected);
        }

        public static bool RemoveExpectedUser(this Room room, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }
            if (room.ExpectedUsers == null || room.ExpectedUsers.Length == 0)
            {
                return false;
            }
            HashSet<string> hashSet = new HashSet<string>(room.ExpectedUsers);
            if (hashSet.Remove(userId))
            {
                Hashtable hash = new Hashtable(1);
                hash.Add(GamePropertyKey.ExpectedUsers, hashSet.ToArray());
                Hashtable expected = new Hashtable(1);
                expected.Add(GamePropertyKey.ExpectedUsers, room.ExpectedUsers);
                return room.LoadBalancingClient.OpSetPropertiesOfRoom(hash, expected);
            }
            return false;
        }

        public static bool AddExpectedUsers(this Room room, string[] userIds, bool force = false)
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
            Hashtable hash;
            Hashtable expected = null;
            if (room.MaxPlayers > 0 && hashSet.Count + room.PlayerCount > room.MaxPlayers)
            {
                if (room.PlayerCount > 1 && room.Players.Values.Any(player => !player.IsLocal && string.IsNullOrEmpty(player.UserId)))
                {
                    return false; // Publish UserId is disabled!
                }
                int alreadyTaken = 0;
                foreach (string user in hashSet)
                {
                    if (room.Players.Values.Any(player => user.Equals(player.UserId)))
                    {
                        alreadyTaken++;
                    }
                }
                if (hashSet.Count + room.PlayerCount - alreadyTaken <= room.MaxPlayers)
                {
                    hash = new Hashtable(1);
                    if (room.ExpectedUsers != null)
                    {
                        expected = new Hashtable(1);
                    }
                }
                else if (force)
                {
                    hash = new Hashtable(2);
                    hash.Add(GamePropertyKey.MaxPlayers, hashSet.Count + room.PlayerCount - alreadyTaken);
                    if (room.ExpectedUsers != null)
                    {
                        expected = new Hashtable(2);
                    }
                    else
                    {
                        expected = new Hashtable(1);
                    }
                    expected.Add(GamePropertyKey.MaxPlayers, room.MaxPlayers);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                hash = new Hashtable(1);
                if (room.ExpectedUsers != null)
                {
                    expected = new Hashtable(1);
                }
            }
            hash.Add(GamePropertyKey.ExpectedUsers, hashSet.ToArray());
            if (room.ExpectedUsers != null)
            {
                expected.Add(GamePropertyKey.ExpectedUsers, room.ExpectedUsers);
            }
            return room.LoadBalancingClient.OpSetPropertiesOfRoom(hash, expected);
        }

        public static bool RemoveExpectedUsers(this Room room, string[] userIds)
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
            for (int i = 0; i < userIds.Length; i++)
            {
                string userId = userIds[i];
                if (string.IsNullOrEmpty(userId))
                {
                    return false;
                }
                if (Array.IndexOf(room.ExpectedUsers, userId) == -1)
                {
                    return false;
                }
            }
            HashSet<string> hashSet = new HashSet<string>(room.ExpectedUsers);
            Hashtable hash = new Hashtable(1);
            hash.Add(GamePropertyKey.ExpectedUsers, hashSet.ToArray());
            Hashtable expected = new Hashtable(1);
            expected.Add(GamePropertyKey.ExpectedUsers, room.ExpectedUsers);
            return room.LoadBalancingClient.OpSetPropertiesOfRoom(hash, expected);
        }

        #endregion

        #region Interest Groups

        internal static readonly byte[] emptyByteArray = new byte[0];

        internal static readonly byte[] allByteValues = Enumerable.Range(1, 255).SelectMany(BitConverter.GetBytes).ToArray(); 

        public static bool AddInterestGroup(this LoadBalancingClient client, byte group)
        {
            return client.InRoom && client.OpChangeGroups(null, new[] { group });
        }

        public static bool AddInterestGroups(this LoadBalancingClient client, byte[] groups)
        {
            return client.InRoom && client.OpChangeGroups(null, groups);
        }

        public static bool RemoveInterestGroup(this LoadBalancingClient client, byte group)
        {
            return client.InRoom && client.OpChangeGroups(new[] { group }, null);
        }

        public static bool RemoveInterestGroups(this LoadBalancingClient client, byte[] groups)
        {
            return client.InRoom && client.OpChangeGroups(groups, null);
        }

        public static bool AddAllExistingInterestGroups(this LoadBalancingClient client)
        {
            return client.InRoom && client.OpChangeGroups(null, emptyByteArray);
        }

        public static bool RemoveAllExistingInterestGroups(this LoadBalancingClient client)
        {
            return client.InRoom && client.OpChangeGroups(emptyByteArray, null);
        }

        public static bool AddAllPossibleInterestGroups(this LoadBalancingClient client)
        {
            return client.InRoom && client.OpChangeGroups(null, allByteValues);
        }

        public static bool RemoveAllPossibleInterestGroups(this LoadBalancingClient client)
        {
            return client.InRoom && client.OpChangeGroups(allByteValues, null);
        }

        #endregion
    }
}