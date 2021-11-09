using Photon.Realtime;

namespace JohnTube.Photon.Client.Realtime
{
    using System.Collections.Generic;
    using ExitGames.Client.Photon;

    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Tries to send SetProperties operation.
        /// If actorNr param is 0 then it's for room properties.
        /// If actorNr param is higher than 0 then it's for actor properties.
        /// </summary>
        /// <param name="client">LoadBalancingClient instance to send the operation request.</param>
        /// <param name="actorNr">Target actor number of the properties to set. 0 for room properties.</param>
        /// <param name="properties">Properties to set.</param>
        /// <param name="expectedProperties">Properties used in Check-And-Swap or Compare-And-Set</param>
        /// <param name="webFlags">Flags to define PathGameProperties WebHook behaviour.</param>
        /// <param name="notify">Whether or not this operation should result in PropertiesChanged event sent to joined actors to sync their cached properties.</param>
        /// <param name="sendOptions">Generic options affecting the operation request.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Tries to send SetProperties request to set room properties.
        /// </summary>
        /// <param name="client">LoadBalancingClient instance to send the operation request.</param>
        /// <param name="roomProperties">Request declaration.</param>
        /// <returns></returns>
        public static bool OpSetPropertiesOfRoom(this LoadBalancingClient client, RoomPropertiesRequest roomProperties)
        {
            if (roomProperties == null)
            {
                return false;
            }
            return client.OpSetProperties(0, roomProperties.Properties, 
                roomProperties.ExpectedProperties, roomProperties.WebFlags, roomProperties.SendPropertiesChangedEvent, roomProperties.SendOptions);
        }

        /// <summary>
        /// Tries to send SetProperties request to set actor properties.
        /// </summary>
        /// <param name="client">LoadBalancingClient instance to send the operation request.</param>
        /// <param name="actorProperties">Request declaration.</param>
        /// <returns></returns>
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
            return client.OpSetProperties(actorProperties.TargetActorNumber, actorProperties.Properties, 
                actorProperties.ExpectedProperties, actorProperties.WebFlags, actorProperties.SendPropertiesChangedEvent, actorProperties.SendOptions);
        }
    }
}
