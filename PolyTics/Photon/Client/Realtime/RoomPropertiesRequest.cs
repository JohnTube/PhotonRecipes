using Photon.Realtime;

namespace PolyTics.Photon.Client.Realtime
{
    public class RoomPropertiesRequest : SetPropertiesRequest
    {
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
        public int? MaxPlayers
        {
            get
            {
                if (this.TryGetProperty(GamePropertyKey.MaxPlayers, out int temp))
                {
                    return temp;
                }
                return null;
            }
            set => this.SetProperty(GamePropertyKey.MaxPlayers, value);
        }
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
        public string[] ExpectedUsers
        {
            set => this.SetProperty(GamePropertyKey.ExpectedUsers, value);
        }
        public string[] MatchmakingPropertiesKeys
        {
            set => this.SetProperty(GamePropertyKey.PropsListedInLobby, value);
        }
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
        public bool TryGetExpectedUsers(out string[] expectedUsers)
        {
            if (this.TryGetProperty(GamePropertyKey.ExpectedUsers, out expectedUsers))
            {
                return true;
            }
            expectedUsers = null;
            return false;
        }
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
            return string.Format("RoomPropertiesRequest PropertiesSent:{0} ExpectedPropertiesSent:{1}", this.properties.PrintParams(typeof(GamePropertyKey)), this.expectedProperties.PrintParams(typeof(GamePropertyKey)));
        }
    }
}