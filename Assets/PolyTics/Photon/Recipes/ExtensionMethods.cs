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
                        break;
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
            if (expected != null)
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
            if (expected != null)
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

        public static bool ChangeExpectedUsers(this Room room, string[] toRemove, string[] toAdd, bool force = false)
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
            if (expected != null)
            {
                expected.Add(GamePropertyKey.ExpectedUsers, room.ExpectedUsers);
            }
            return room.LoadBalancingClient.OpSetPropertiesOfRoom(hash, expected);
        }

        public static bool ReplaceExpectedUser(this Room room, string toRemove, string toAdd)
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
            Hashtable hash = new Hashtable(1);
            Hashtable expected = new Hashtable(1);
            hash.Add(GamePropertyKey.ExpectedUsers, hashSet.ToArray());
            expected.Add(GamePropertyKey.ExpectedUsers, room.ExpectedUsers);
            return room.LoadBalancingClient.OpSetPropertiesOfRoom(hash, expected);
        }

        #endregion

        #region Interest Groups

        internal static readonly byte[] emptyByteArray = new byte[0];

        internal static readonly byte[] allByteValues = Enumerable.Range(1, 255).SelectMany(BitConverter.GetBytes).ToArray();

        public static bool AddInterestGroup(this LoadBalancingClient client, byte group)
        {
            return client.OpChangeGroups(null, new[] { group });
        }

        public static bool AddInterestGroups(this LoadBalancingClient client, byte[] groups)
        {
            return client.OpChangeGroups(null, groups);
        }

        public static bool RemoveInterestGroup(this LoadBalancingClient client, byte group)
        {
            return client.OpChangeGroups(new[] { group }, null);
        }

        public static bool RemoveInterestGroups(this LoadBalancingClient client, byte[] groups)
        {
            return client.OpChangeGroups(groups, null);
        }

        public static bool AddAllExistingInterestGroups(this LoadBalancingClient client)
        {
            return client.OpChangeGroups(null, emptyByteArray);
        }

        public static bool RemoveAllExistingInterestGroups(this LoadBalancingClient client)
        {
            return client.OpChangeGroups(emptyByteArray, null);
        }

        public static bool AddAllPossibleInterestGroups(this LoadBalancingClient client)
        {
            return client.OpChangeGroups(null, allByteValues);
        }

        public static bool RemoveAllPossibleInterestGroups(this LoadBalancingClient client)
        {
            return client.OpChangeGroups(allByteValues, null);
        }

        #endregion

        public static bool OpSetProperties(this LoadBalancingClient client, int actorNr, Hashtable properties, Hashtable expectedProperties = null, WebFlags webFlags = null, bool notify = true, 
            SendOptions sendOptions = default)
        {
            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            if (properties != null)
            {
                opParameters.Add(ParameterCode.Properties, properties);
            }
            if (actorNr > 0)
            {
                opParameters.Add(ParameterCode.ActorNr, actorNr);
            }
            if (notify)
            {
                opParameters.Add(ParameterCode.Broadcast, true);
            }
            if (expectedProperties != null)
            {
                opParameters.Add(ParameterCode.ExpectedValues, expectedProperties);
            }
            if (webFlags != null)
            {
                opParameters[ParameterCode.EventForward] = webFlags.WebhookFlags;
            }
            return client.LoadBalancingPeer.SendOperation(OperationCode.SetProperties, opParameters, sendOptions);
        }

        public static bool OpSetPropertiesOfRoom(this LoadBalancingClient client, RoomPropertiesRequest roomProperties)
        {
            if (roomProperties == null)
            {
                return false;
            }
            return client.OpSetProperties(0, roomProperties.ToHashtable(), 
                roomProperties.ExpectedProperties, roomProperties.WebFlags, roomProperties.SendPropertiesChangedEvent, roomProperties.SendOptions);
        }

        public static bool OpSetPropertiesOfActor(this LoadBalancingClient client, ActorPropertiesRequest actorProperties)
        {
            if (actorProperties == null)
            {
                return false;
            }
            if (actorProperties.TargetActorNumber == 0)
            {
                return false;
            }
            return client.OpSetProperties(actorProperties.TargetActorNumber, actorProperties.ToHashtable(), 
                actorProperties.ExpectedProperties, actorProperties.WebFlags, actorProperties.SendPropertiesChangedEvent, actorProperties.SendOptions);
        }
    }

    public abstract class SetPropertiesRequest
    {
        public WebFlags WebFlags;
        public bool SendPropertiesChangedEvent = true;
        public SendOptions SendOptions = SendOptions.SendReliable;
        protected Hashtable expectedProperties;
        public Hashtable ExpectedProperties
        {
            get { return this.GetExpectedProperties(); }
            set { this.expectedProperties = this.SetExpectedProperties(value); }
        }
        public Hashtable CustomProperties;

        public void SetCustomProperty(object propKey, object propValue)
        {
            if (this.CustomProperties == null)
            {
                this.CustomProperties = new Hashtable();
            }
            if (propKey == null)
            {
                return;
            }
            this.CustomProperties[propKey] = propValue;
        }

        public void SetExpectedProperty(object propKey, object propValue)
        {
            if (this.ExpectedProperties == null)
            {
                this.ExpectedProperties = new Hashtable();
            }
            if (propKey == null)
            {
                return;
            }
            this.ExpectedProperties[propKey] = propValue;
        }

        public Hashtable ToHashtable()
        {
            Hashtable hash = this.CustomProperties;
            if (hash == null)
            {
                hash = new Hashtable();
            }
            this.AddToHashtable(hash);
            return hash;
        }

        protected virtual void AddToHashtable(Hashtable hash)
        {

        }

        protected virtual Hashtable SetExpectedProperties(Hashtable hash)
        {
            return hash;
        }

        protected virtual Hashtable GetExpectedProperties()
        {
            return this.expectedProperties;
        }
    }

    public class RoomPropertiesRequest : SetPropertiesRequest
    {
        public bool? IsOpen;
        public bool? IsVisible;
        //public bool? CleanUpCacheOnLeave;
        public int? PlayerTtl;
        public int? EmptyRoomTtl;
        public int? MaxPlayers;
        int? currentMasterClientId;
        private int? masterClientId; // only w/ CAS

        private bool expectedUsersSet;
        private string[] expectedUsers;
        public string[] ExpectedUsers
        {
            get { return this.expectedUsers; }
            set
            {
                expectedUsersSet = true;
                this.expectedUsers = value;
            }
        }
        private bool propsListedInLobbySet;
        private string[] propsListedInLobby;
        public string[] PropsListedInLobby
        {
            get
            {
                return this.propsListedInLobby;
            }
            set
            {
                propsListedInLobbySet = true;
                propsListedInLobby = value;
            }
        }

        protected override Hashtable SetExpectedProperties(Hashtable hash)
        {
            object temp;
            if (this.expectedProperties != null &&
                this.expectedProperties.TryGetValue(GamePropertyKey.MasterClientId, out temp))
            {
                currentMasterClientId = (int)temp;
            }
            this.expectedProperties = hash;
            if (!this.expectedProperties.ContainsKey(GamePropertyKey.MasterClientId) && currentMasterClientId.HasValue)
            {
                this.expectedProperties[GamePropertyKey.MasterClientId] = currentMasterClientId.Value;
            }
            return this.expectedProperties;
        }

        protected override Hashtable GetExpectedProperties()
        {
            if (this.masterClientId.HasValue)
            {
                if (this.currentMasterClientId.HasValue)
                {
                    this.SetExpectedProperty(GamePropertyKey.MasterClientId, this.currentMasterClientId.Value);
                }
            }
            return this.expectedProperties;
        }

        public bool SetMasterClient(int newMasterClient, int currentMasterClient)
        {
            if (newMasterClient != currentMasterClient)
            {
                return false;
            }
            this.masterClientId = newMasterClient;
            this.SetExpectedProperty(GamePropertyKey.MasterClientId, currentMasterClient);
            return true;
        }

        protected override void AddToHashtable(Hashtable hash)
        {
            if (hash == null)
            {
                return;
            }
            if (IsOpen.HasValue)
            {
                hash[GamePropertyKey.IsOpen] = IsOpen.Value;
            }
            if (IsVisible.HasValue)
            {
                hash[GamePropertyKey.IsVisible] = IsVisible.Value;
            }
            //if (CleanUpCacheOnLeave.HasValue)
            //{
            //    hash[GamePropertyKey.CleanupCacheOnLeave] = CleanUpCacheOnLeave.Value;
            //}
            if (PlayerTtl.HasValue)
            {
                hash[GamePropertyKey.PlayerTtl] = PlayerTtl.Value;
            }
            if (EmptyRoomTtl.HasValue)
            {
                hash[GamePropertyKey.EmptyRoomTtl] = EmptyRoomTtl.Value;
            }
            if (MaxPlayers.HasValue)
            {
                hash[GamePropertyKey.MaxPlayers] = MaxPlayers.Value;
            }
            if (masterClientId.HasValue)
            {
                hash[GamePropertyKey.MasterClientId] = masterClientId.Value;
            }
            if (expectedUsersSet)
            {
                hash[GamePropertyKey.ExpectedUsers] = ExpectedUsers;
            }
            if (propsListedInLobbySet)
            {
                hash[GamePropertyKey.PropsListedInLobby] = PropsListedInLobby;
            }
        }
    }

    public class ActorPropertiesRequest : SetPropertiesRequest
    {
        public int TargetActorNumber;

        private string nickname;
        private bool nicknameSet;
        public string Nickname
        {
            get
            {
                return this.nickname;
            }
            set
            {
                this.nickname = value;
                this.nicknameSet = true;
            }
        } 

        protected override  void AddToHashtable(Hashtable hash)
        {
            if (hash == null)
            {
                return;
            }
            if (this.nicknameSet)
            {
                hash[ActorProperties.PlayerName] = this.nickname;
            }
        }
    }
}