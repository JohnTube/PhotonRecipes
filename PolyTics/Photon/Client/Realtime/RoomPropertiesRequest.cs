using Photon.Realtime;

namespace PolyTics.Photon.Client.Realtime
{
    using ExitGames.Client.Photon;

    public class RoomPropertiesRequest : SetPropertiesRequest
    {
        public bool? IsOpen;
        public bool? IsVisible;
        //public bool? CleanUpCacheOnLeave;
        public int? PlayerTtl;
        public int? EmptyRoomTtl;
        public int? MaxPlayers;
        
        private int? currentMasterClientId;
        private int? masterClientId; // only w/ CAS

        private bool expectedUsersSet;
        private string[] expectedUsers;

        public string[] ExpectedUsers
        {
            get => this.expectedUsers;
            set
            {
                this.expectedUsersSet = true;
                this.expectedUsers = value;
            }
        }

        private bool propsListedInLobbySet;
        private string[] propsListedInLobby;

        public string[] PropsListedInLobby
        {
            get => this.propsListedInLobby;
            set
            {
                this.propsListedInLobbySet = true;
                this.propsListedInLobby = value;
            }
        }

        protected override Hashtable SetExpectedProperties(Hashtable hash)
        {
            if (this.expectedProperties != null &&
                this.expectedProperties.TryGetValue(GamePropertyKey.MasterClientId, out object temp))
            {
                this.currentMasterClientId = (int)temp;
            }

            this.expectedProperties = hash;
            if (!this.expectedProperties.ContainsKey(GamePropertyKey.MasterClientId) && this.currentMasterClientId.HasValue)
            {
                this.expectedProperties[GamePropertyKey.MasterClientId] = this.currentMasterClientId.Value;
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

            if (this.IsOpen.HasValue)
            {
                hash[GamePropertyKey.IsOpen] = this.IsOpen.Value;
            }

            if (this.IsVisible.HasValue)
            {
                hash[GamePropertyKey.IsVisible] = this.IsVisible.Value;
            }

            //if (CleanUpCacheOnLeave.HasValue)
            //{
            //    hash[GamePropertyKey.CleanupCacheOnLeave] = CleanUpCacheOnLeave.Value;
            //}
            if (this.PlayerTtl.HasValue)
            {
                hash[GamePropertyKey.PlayerTtl] = this.PlayerTtl.Value;
            }

            if (this.EmptyRoomTtl.HasValue)
            {
                hash[GamePropertyKey.EmptyRoomTtl] = this.EmptyRoomTtl.Value;
            }

            if (this.MaxPlayers.HasValue)
            {
                hash[GamePropertyKey.MaxPlayers] = this.MaxPlayers.Value;
            }

            if (this.masterClientId.HasValue)
            {
                hash[GamePropertyKey.MasterClientId] = this.masterClientId.Value;
            }

            if (this.expectedUsersSet)
            {
                hash[GamePropertyKey.ExpectedUsers] = this.ExpectedUsers;
            }

            if (this.propsListedInLobbySet)
            {
                hash[GamePropertyKey.PropsListedInLobby] = this.PropsListedInLobby;
            }
        }

        public override string ToString()
        {
            return this.ToHashtable().PrintParams(typeof(GamePropertyKey)); // todo: optimize
        }
    }
}