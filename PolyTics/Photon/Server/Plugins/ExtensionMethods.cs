namespace PolyTics.Photon.Server.Plugins
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using Photon.Hive.Plugin;
	
    public static partial class ExtensionMethods
    {
        #region TryGetActor

        public static bool TryGetActorByNumber(this IPluginHost pluginHost, int actorNr, out IActor actor)
        {
            actor = pluginHost.GameActors.FirstOrDefault(a => a.ActorNr == actorNr);
            return actor == default;
        }

        public static bool TryGetActorByUserId(this IPluginHost pluginHost, string userId, out IActor actor)
        {
            actor = default;
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }
            actor = pluginHost.GameActors.FirstOrDefault(a => userId.Equals(a.UserId));
            return actor == default;
        }

        #endregion

        #region SetProperties

        public static bool SetRoomProperties(this IPluginHost pluginHost, Hashtable properties, Hashtable expectedProperties, bool broadcast)
        {
            return pluginHost.SetProperties(0, properties, expectedProperties, broadcast);
        }

        public static bool SetActorProperties(this IPluginHost pluginHost, int actorNr, Hashtable properties, Hashtable expectedProperties, bool broadcast)
        {
            return pluginHost.SetProperties(actorNr, properties, expectedProperties, broadcast);
        }

        public static bool SetActorNickname(this IPluginHost pluginHost, int actorNr, string nickname, bool broadcast)
        {
            return pluginHost.SetActorProperty(actorNr, 255, nickname, broadcast);
        }

        public static bool SetActorProperty<T>(this IPluginHost pluginHost, int actorNr, byte propKey, T propValue, bool broadcast)
        {
            return pluginHost.SetActorProperties(actorNr, new Hashtable {{propKey, propValue}}, null, broadcast);
        }

        public static bool SetRoomProperty<T>(this IPluginHost pluginHost, byte propKey, T propValue, bool broadcast)
        {
            return pluginHost.SetRoomProperties(new Hashtable {{propKey, propValue}}, null, broadcast);
        }

        public static bool SetCustomRoomProperty<T>(this IPluginHost pluginHost, string propKey, T propValue, bool broadcast)
        {
            return pluginHost.SetRoomProperties(new Hashtable {{propKey, propValue}}, null, broadcast);
        }

        public static bool SetRoomProperty<T>(this IPluginHost pluginHost, byte propKey, T propValue, T expectedValue, bool broadcast)
        {
            return pluginHost.SetRoomProperties(new Hashtable {{propKey, propValue}}, new Hashtable {{propKey, expectedValue}}, broadcast);
        }

        public static bool SetCustomRoomProperty<T>(this IPluginHost pluginHost, string propKey, T propValue, T expectedValue, bool broadcast)
        {
            return pluginHost.SetRoomProperties(new Hashtable {{propKey, propValue}}, new Hashtable {{propKey, expectedValue}}, broadcast);
        }

        #endregion

        #region GetProperty

        public static bool TryGetRoomProperty<T>(this IPluginHost pluginHost, byte key, out T property)
        {
            property = default;
            if (pluginHost.GameProperties.ContainsKey(key))
            {
                property = (T)pluginHost.GameProperties[key];
            }
            return false;
        }

        public static bool TryGetCustomRoomProperty<T>(this IPluginHost pluginHost, string key, out T property)
        {
            property = default;
            if (pluginHost.GameProperties.ContainsKey(key))
            {
                property = (T)pluginHost.GameProperties[key];
            }
            return false;
        }

        public static bool TryGetActorProperty<T>(this IPluginHost pluginHost, int actorNr, byte key, out T property)
        {
            property = default;
            if (pluginHost.TryGetActorByNumber(actorNr, out IActor actor) && actor.Properties.TryGetValue(key, out object temp))
            {
                property = (T)temp;
            }
            return false;
        }

        public static bool TryGetCustomActorProperty<T>(this IPluginHost pluginHost, int actorNr, string key, out T property)
        {
            property = default;
            if (pluginHost.TryGetActorByNumber(actorNr, out IActor actor) && actor.Properties.TryGetValue(key, out object temp))
            {
                property = (T)temp;
            }
            return false;
        }

        #endregion

        #region RaiseEvent

        public static void RaiseEvent(this IPluginHost pluginHost, byte eventCode, object eventData,
            byte receiverGroup = ReciverGroup.All,
            int senderActorNumber = 0,
            byte cachingOption = CacheOperations.DoNotCache,
            byte interestGroup = 0,
            SendParameters sendParams = default)
        {
            Dictionary<byte, object> parameters = new Dictionary<byte, object>(2)
            {
                { 245, eventData },
                { 254, senderActorNumber }
            };
            pluginHost.BroadcastEvent(receiverGroup, senderActorNumber, interestGroup, eventCode, parameters, cachingOption, sendParams);
        }

        public static void RaiseEvent(this IPluginHost pluginHost, byte eventCode, object eventData, IList<int> targetActorsNumbers,
            int senderActorNumber = 0,
            byte cachingOption = CacheOperations.DoNotCache,
            SendParameters sendParams = default)
        {
            Dictionary<byte, object> parameters = new Dictionary<byte, object>(2)
            {
                { 245, eventData },
                { 254, senderActorNumber }
            };
            pluginHost.BroadcastEvent(targetActorsNumbers, senderActorNumber, eventCode, parameters, cachingOption, sendParams);
        }

        #endregion

        #region RemoveActor

        public const int RemoveActorEventCode = 199;
        public const int RemoveActorTimerDelay = 200;

       
        public static void RemoveActor(this IPluginHost pluginHost, int actorNr, string reason)
        {
            object eventData = reason;
            pluginHost.RaiseEvent(RemoveActorEventCode, eventData, new List<int> { actorNr });
            pluginHost.CreateOneTimeTimer(() => pluginHost.RemoveActor(actorNr, reason), RemoveActorTimerDelay);
        }

        public static void RemoveActor(this IPluginHost pluginHost, int actorNr, byte reasonCode, string reason)
        {
            object[] eventData = { reasonCode, reason };
            pluginHost.RaiseEvent(RemoveActorEventCode, eventData, new List<int> { actorNr });
            pluginHost.CreateOneTimeTimer(() => pluginHost.RemoveActor(actorNr, reasonCode, reason), RemoveActorTimerDelay);
        }

        #endregion
    }
}
