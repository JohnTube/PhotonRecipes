using Photon.Realtime;

namespace PolyTics.Photon.Client.Realtime
{
    using System;

    public class ActorPropertiesRequest : SetPropertiesRequest
    {
        public int TargetActorNumber;

        public string Nickname
        {
            get
            {
                if (this.TryGetProperty(ActorProperties.PlayerName, out string temp))
                {
                    return temp;
                }
                return null;
            }
            set => this.SetProperty(ActorProperties.PlayerName, value);
        }

        public override string ToString()
        {
            return string.Format("ActorPropertiesRequest TargetActorNumber:{0} PropertiesSent:{1} ExpectedPropertiesSent:{2}", this.TargetActorNumber, this.properties.PrintParams(typeof(GamePropertyKey)), this.expectedProperties.PrintParams(typeof(GamePropertyKey)));
        }
    }
}