using Photon.Realtime;

namespace PolyTics.Photon.Client.Realtime
{
    using ExitGames.Client.Photon;

    public class ActorPropertiesRequest : SetPropertiesRequest
    {
        public int TargetActorNumber;

        private string nickname;
        private bool nicknameSet;

        public string Nickname
        {
            get { return this.nickname; }
            set
            {
                this.nickname = value;
                this.nicknameSet = true;
            }
        }

        protected override void AddToHashtable(Hashtable hash)
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

        public override string ToString()
        {
            return this.ToHashtable().PrintParams(typeof(ActorProperties)); // todo: optimize
        }
    }
}