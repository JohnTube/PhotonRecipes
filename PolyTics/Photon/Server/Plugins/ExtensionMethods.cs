JohnTubenamespace JohnTube.Photon.Server.Plugins
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
            return actor != default;
        }

        public static bool TryGetActorByUserId(this IPluginHost pluginHost, string userId, out IActor actor)
        {
            actor = default;
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }
            actor = pluginHost.GameActors.FirstOrDefault(a => userId.Equals(a.UserId));
            return actor != default;
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
            return pluginHost.SetActorProperties(actorNr, new Hashtable {{ (byte)255, nickname}}, null, broadcast);
        }
        
        public static bool SetActorNickname(this IPluginHost pluginHost, int actorNr, string nickname, string oldNickname, bool broadcast)
        {
            return pluginHost.SetActorProperties(actorNr, new Hashtable {{ (byte)255, nickname}}, new Hashtable {{ (byte)255, oldNickname}}, broadcast);
        }

        public static bool SetCustomActorProperty<T>(this IPluginHost pluginHost, int actorNr, string propKey, T propValue, bool broadcast)
        {
            return pluginHost.SetActorProperties(actorNr, new Hashtable {{propKey, propValue}}, null, broadcast);
        }

        public static bool SetCustomActorProperty<T>(this IPluginHost pluginHost, int actorNr, string propKey, T propValue, T expectedValue, bool broadcast)
        {
            return pluginHost.SetActorProperties(actorNr, new Hashtable {{propKey, propValue}}, new Hashtable {{propKey, expectedValue}}, broadcast);
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

        public static bool SetInitialRoomProperties(this ICreateGameCallInfo info, Hashtable properties, bool replaceAll = false)
        {
            if (info.IsProcessed || properties == null)
            {
                return false;
            }
            if (replaceAll || info.Request.GameProperties == null)
            {
                info.Request.GameProperties = properties;
            }
            else
            {
                foreach (object key in properties.Keys)
                {
                    info.Request.GameProperties[key] = properties[key];
                }
            }
            return true;
        }

        public static bool SetInitialActorProperties(this ICreateGameCallInfo info, Hashtable properties, bool replaceAll = false)
        {
            if (info.IsProcessed || properties == null)
            {
                return false;
            }
            if (replaceAll || info.Request.ActorProperties == null)
            {
                info.Request.ActorProperties = properties;
            }
            else
            {
                foreach (object key in properties.Keys)
                {
                    info.Request.ActorProperties[key] = properties[key];
                }
            }
            return true;
        }

        public static bool SetInitialActorProperties(this IBeforeJoinGameCallInfo info, Hashtable properties, bool replaceAll = false)
        {
            if (info.IsProcessed || properties == null)
            {
                return false;
            }
            if (replaceAll || info.Request.ActorProperties == null)
            {
                info.Request.ActorProperties = properties;
            }
            else
            {
                foreach (object key in properties.Keys)
                {
                    info.Request.ActorProperties[key] = properties[key];
                }
            }
            return true;
        }

        public static bool SetInitialActorNickname(this ICreateGameCallInfo info, string nickname)
        {
            if (info.IsProcessed)
            {
                return false;
            }
            if (info.Request.ActorProperties == null)
            {
                info.Request.ActorProperties = new Hashtable();
            }
            info.Request.ActorProperties[(byte)255] = nickname;
            return true;
        }

        public static bool SetInitialActorNickname(this IBeforeJoinGameCallInfo info, string nickname)
        {
            if (info.IsProcessed)
            {
                return false;
            }
            if (info.Request.ActorProperties == null)
            {
                info.Request.ActorProperties = new Hashtable();
            }
            info.Request.ActorProperties[(byte)255] = nickname;
            return true;
        }

        public static bool SetInitialCustomActorProperty<T>(this ICreateGameCallInfo info, string propKey, T propValue)
        {
            if (info.IsProcessed)
            {
                return false;
            }
            if (info.Request.ActorProperties == null)
            {
                info.Request.ActorProperties = new Hashtable();
            }
            info.Request.ActorProperties[propKey] = propValue;
            return true;
        }

        public static bool SetInitialCustomActorProperty<T>(this IBeforeJoinGameCallInfo info, string propKey, T propValue)
        {
            if (info.IsProcessed)
            {
                return false;
            }
            if (info.Request.ActorProperties == null)
            {
                info.Request.ActorProperties = new Hashtable();
            }
            info.Request.ActorProperties[propKey] = propValue;
            return true;
        }

        public static bool SetInitialRoomProperty<T>(this ICreateGameCallInfo info, byte propKey, T propValue)
        {
            if (info.IsProcessed)
            {
                return false;
            }
            if (info.Request.GameProperties == null)
            {
                info.Request.GameProperties = new Hashtable();
            }
            info.Request.GameProperties[propKey] = propValue;
            return true;
        }

        public static bool SetInitialCustomRoomProperty<T>(this ICreateGameCallInfo info, string propKey, T propValue)
        {
            if (info.IsProcessed)
            {
                return false;
            }
            if (info.Request.GameProperties == null)
            {
                info.Request.GameProperties = new Hashtable();
            }
            info.Request.GameProperties[propKey] = propValue;
            return true;
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
            return pluginHost.TryGetActorByNumber(actorNr, out IActor actor) && actor.TryGetActorProperty(key, out property);
        }

        public static bool TryGetCustomActorProperty<T>(this IPluginHost pluginHost, int actorNr, string key, out T property)
        {
            property = default;
            return pluginHost.TryGetActorByNumber(actorNr, out IActor actor) && actor.TryGetCustomActorProperty(key, out property);
        }

        public static bool TryGetActorProperty<T>(this IPluginHost pluginHost, string userId, byte key, out T property)
        {
            property = default;
            return pluginHost.TryGetActorByUserId(userId, out IActor actor) && actor.TryGetActorProperty(key, out property);
        }

        public static bool TryGetCustomActorProperty<T>(this IPluginHost pluginHost, string userId, string key, out T property)
        {
            property = default;
            return pluginHost.TryGetActorByUserId(userId, out IActor actor) && actor.TryGetCustomActorProperty(key, out property);
        }

        public static bool TryGetActorProperty<T>(this IActor actor, byte key, out T property)
        {
            property = default;
            if (actor.Properties.TryGetValue(key, out object temp) && temp is T)
            {
                property = (T)temp;
            }
            return false;
        }

        public static bool TryGetCustomActorProperty<T>(this IActor actor, string key, out T property)
        {
            property = default;
            if (actor.Properties.TryGetValue(key, out object temp) && temp is T)
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
       
        public static void RemoveActor(this IPluginHost pluginHost, ICallInfo info, int actorNr, string reason)
        {
            object eventData = reason;
            pluginHost.RaiseEvent(RemoveActorEventCode, eventData, new List<int> { actorNr });
            object timer = null;
            timer = pluginHost.CreateOneTimeTimer(info, () =>
            {
                pluginHost.RemoveActor(actorNr, reason);
                pluginHost.StopTimer(timer);
            }, RemoveActorTimerDelay);
        }

        public static void RemoveActor(this IPluginHost pluginHost, ICallInfo info, int actorNr, byte reasonCode, string reason)
        {
            object[] eventData = { reasonCode, reason };
            pluginHost.RaiseEvent(RemoveActorEventCode, eventData, new List<int> { actorNr });
            object timer = null;
            timer = pluginHost.CreateOneTimeTimer(info, () =>
            {
                pluginHost.RemoveActor(actorNr, reasonCode, reason);
                pluginHost.StopTimer(timer);
            }, RemoveActorTimerDelay);
        }

        #endregion

        #region Alter Operation Request

        public static bool TrySetParameter<T>(this IOperationRequest request, byte parameterKeyCode, T parameterValue, bool mustExist = false)
        {
            if (request.Parameters == null)
            {
                return false;
            }
            if (!mustExist || request.Parameters.ContainsKey(parameterKeyCode))
            {
                request.Parameters[parameterKeyCode] = parameterValue;
                return true;
            }
            return false;
        }

        public static bool TrySetParameter<T>(this ICallInfo info, byte parameterKeyCode, T parameterValue, bool mustExist = false)
        {
            return info.OperationRequest.TrySetParameter(parameterKeyCode, parameterValue, mustExist);
        }

        public static bool TryGetParameter<T>(this IOperationRequest request, byte parameterKeyCode, out T parameterValue)
        {
            parameterValue = default;
            if (request.Parameters == null)
            {
                return false;
            }
            if (!request.Parameters.TryGetValue(parameterKeyCode, out object parameter) || !(parameter is T))
            {
                return false;
            }
            parameterValue = (T)parameter;
            return true;
        }

        public static bool TryGetParameter<T>(this ICallInfo info, byte parameterKeyCode, out T parameterValue)
        {
            return info.OperationRequest.TryGetParameter(parameterKeyCode, out parameterValue);
        }

        #endregion
    }
}