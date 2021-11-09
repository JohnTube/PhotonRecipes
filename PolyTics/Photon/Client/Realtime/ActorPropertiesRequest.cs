JohnTubeusing Photon.Realtime;

namespace PolyTics.Photon.Client.Realtime
{
    /// <summary>
    /// A class that holds parameters of an outgoing SetProperties request that tries to set actor properties.
    /// </summary>
    public class ActorPropertiesRequest : SetPropertiesRequest
    {
        /// <summary>
        /// Actor number of the actor whose properties should be set.
        /// </summary>
        public int TargetActorNumber;
        /// <summary>
        /// Nickname to set for the target actor.
        /// </summary>
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
            return
                $"ActorPropertiesRequest TargetActorNumber:{this.TargetActorNumber} Properties:{this.properties.PrintParams(typeof(GamePropertyKey))} ExpectedProperties:{this.expectedProperties.PrintParams(typeof(GamePropertyKey))}";
        }
    }
}