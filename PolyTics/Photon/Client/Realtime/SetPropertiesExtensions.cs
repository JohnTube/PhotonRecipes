using Photon.Realtime;

namespace PolyTics.Photon.Client.Realtime
{
    using System.Collections.Generic;
    using ExitGames.Client.Photon;

    public static partial class ExtensionMethods
    {
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
}
