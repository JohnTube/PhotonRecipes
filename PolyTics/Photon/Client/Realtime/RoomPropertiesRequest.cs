JohnTubeusing Photon.Realtime;

namespace PolyTics.Photon.Client.Realtime
{
    /// <summary>
    /// A class that holds parameters of an outgoing SetProperties request that tries to set room properties.
    /// </summary>
    public class RoomPropertiesRequest : SetPropertiesRequest
    {
        /// <summary>
        /// Optional Well Known Room Property to set: IsOpen.
        /// </summary>
        public bool? IsOpen
        {
            get
            {
                if (this.TryGetProperty(GamePropertyKey.IsOpen, out bool temp))
                {
                    return temp;
                }
                return null;
            }
            set => this.SetProperty(GamePropertyKey.IsOpen, value);
        }
        /// <summary>
        /// Optional Well Known Room Property to set: IsVisible.
        /// </summary>
        public bool? IsVisible
        {
            get
            {
                if (this.TryGetProperty(GamePropertyKey.IsVisible, out bool temp))
                {
                    return temp;
                }
                return null;
            }
            set => this.SetProperty(GamePropertyKey.IsVisible, value);
        }
        /// <summary>
        /// Optional Well Known Room Property to set: PlayerTTL. In Milliseconds, negative or int.MaxValue means infinite.
        /// </summary>
        public int? PlayerTtl
        {
            get
            {
                if (this.TryGetProperty(GamePropertyKey.PlayerTtl, out int temp))
                {
                    return temp;
                }
                return null;
            }
            set => this.SetProperty(GamePropertyKey.PlayerTtl, value);
        }
        /// <summary>
        /// Optional Well Known Room Property to set: EmptyRoomTTL. In Milliseconds, max on public cloud is 300000.
        /// </summary>
        public int? EmptyRoomTtl
        {
            get
            {
                if (this.TryGetProperty(GamePropertyKey.EmptyRoomTtl, out int temp))
                {
                    return temp;
                }
                return null;
            }
            set => this.SetProperty(GamePropertyKey.EmptyRoomTtl, value);
        }
        /// <summary>
        /// Optional Well Known Room Property to set: MaxPlayers. 0 (default) means infinite.
        /// </summary>
        public byte? MaxPlayers
        {
            get
            {
                if (this.TryGetProperty(GamePropertyKey.MaxPlayers, out byte temp))
                {
                    return temp;
                }
                return null;
            }
            set => this.SetProperty(GamePropertyKey.MaxPlayers, value);
        }
        /// <summary>
        /// Optional Well Known Room Property to set: Master Client Actor Number. If you want to switch you need to use CAS and know the current value.
        /// </summary>
        public int? NewMasterClientActorNumber
        {
            get
            {
                if (this.TryGetProperty(GamePropertyKey.MasterClientId, out int temp))
                {
                    return temp;
                }
                return null;
            }
        }
        /// <summary>
        /// Optional Well Known Room Property to set: Expected Users' UserIDs strings. Used for private slot reservation in matchmaking.
        /// </summary>
        public string[] ExpectedUsers
        {
            set => this.SetProperty(GamePropertyKey.ExpectedUsers, value);
        }
        /// <summary>
        /// Optional Well Known Room Property to set: Properties Keys visible to the lobby or listed in the lobby. The properties that could be used as filters in random matchmaking.
        /// </summary>
        public string[] MatchmakingPropertiesKeys
        {
            set => this.SetProperty(GamePropertyKey.PropsListedInLobby, value);
        }
        /// <summary>
        /// Sets the new master client actor number and current one as this requires CAS.
        /// </summary>
        /// <param name="newMasterClient">New value for the master client's actor number.</param>
        /// <param name="currentMasterClient">Current value for the master client's actor number.</param>
        /// <returns></returns>
        public bool SetMasterClient(int newMasterClient, int currentMasterClient)
        {
            if (newMasterClient != currentMasterClient)
            {
                return false;
            }
            this.SetProperty(GamePropertyKey.MasterClientId, newMasterClient);
            this.SetExpectedProperty(GamePropertyKey.MasterClientId, currentMasterClient);
            return true;
        }
        /// <summary>
        /// Try to get the value of the expected users' UserIDs string array if exists in this request.
        /// </summary>
        /// <param name="expectedUsers">Expected Users array to get.</param>
        /// <returns>If this request contains expected users.</returns>
        public bool TryGetExpectedUsers(out string[] expectedUsers)
        {
            if (this.TryGetProperty(GamePropertyKey.ExpectedUsers, out expectedUsers))
            {
                return true;
            }
            expectedUsers = null;
            return false;
        }
        /// <summary>
        /// Try to get the value of properties keys visible in the lobby if exists in this request.
        /// </summary>
        /// <param name="matchmakingPropertiesKeys">Matchmaking properties keys to get.</param>
        /// <returns>If this request contains matchmaking properties keys.</returns>
        public bool TryGetMatchmakingPropertiesKeys(out string[] matchmakingPropertiesKeys)
        {
            if (this.TryGetProperty(GamePropertyKey.ExpectedUsers, out matchmakingPropertiesKeys))
            {
                return true;
            }
            matchmakingPropertiesKeys = null;
            return false;
        }
        public override string ToString()
        {
            return
                $"RoomPropertiesRequest Properties:{this.properties.PrintParams(typeof(GamePropertyKey))} ExpectedProperties:{this.expectedProperties.PrintParams(typeof(GamePropertyKey))}";
        }
    }
}