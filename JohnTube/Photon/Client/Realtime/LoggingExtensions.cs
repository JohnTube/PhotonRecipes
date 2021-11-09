using Photon.Realtime;

namespace JohnTube.Photon.Client.Realtime
{
    using ExitGames.Client.Photon;
    using Utils;
    using System.Collections.Generic;
    using System.Text;
    using Hashtable = ExitGames.Client.Photon.Hashtable;

    public static partial class ExtensionMethods
    {
        public static string PrettyPrint(this OperationResponse operationResponse, bool printTypes)
        {
            return string.Format("OpResponse={0}:{1}:{2}{3}",
                UtilityClass.GetConstantNameFromValue(typeof(OperationCode), operationResponse.OperationCode),
                UtilityClass.GetConstantNameFromValue(typeof(ErrorCode), (int)operationResponse.ReturnCode),
                string.IsNullOrEmpty(operationResponse.DebugMessage) ? string.Empty : string.Format("{0}:", operationResponse.DebugMessage),
                operationResponse.Parameters.PrintOpResponseParams(printTypes));
        }

        public static string PrettyPrint(this EventData eventData, bool printTypes)
        {
            return string.Format("Event={0}:{1}",
                UtilityClass.GetConstantNameFromValue(typeof(EventCode), eventData.Code),
                eventData.Parameters.PrintEventParams(printTypes));
        }

        public static string PrintEventParams(this Dictionary<byte, object> data, bool printType = false)
        {
            StringBuilder builder = new StringBuilder("{");
            if (!data.IsNull() && data.Count > 0)
            {
                foreach (var param in data)
                {
                    string name = string.Empty, value = string.Empty;
                    if (param.Key == ParameterCode.PlayerProperties)
                    {
                        name = "PlayerProperties";
                        Hashtable props = param.Value as Hashtable;
                        if (!props.IsNull())
                        {
                            builder.AppendFormat("{0}:{1},", name, props.PrintParams(typeof(ActorProperties), printType));
                            continue;
                        }
                    }
                    else if (param.Key == ParameterCode.GameProperties)
                    {
                        name = "GameProperties";
                        value = (param.Value as Hashtable).PrintParams(typeof(GamePropertyKey), printType);
                    }
                    else if (param.Key == ParameterCode.Properties)
                    {
                        name = "Properties";
                        int targetActorNr = 0;
                        object temp;
                        if (data.TryGetValue(ParameterCode.TargetActorNr, out temp))
                        {
                            targetActorNr = (int)temp;
                        }
                        if (targetActorNr == 0)
                        {
                            value = (param.Value as Hashtable).PrintParams(typeof(GamePropertyKey), printType);
                        }
                        else
                        {
                            value = (param.Value as Hashtable).PrintParams(typeof(ActorProperties), printType);
                        }
                    }
                    else if (param.Key == ParameterCode.GameList)
                    {
                        name = "GameList";
                        Hashtable hashtable = param.Value as Hashtable;
                        if (hashtable.Count > 0)
                        {
                            builder.AppendFormat("{0}:{{", name);
                            foreach (string room in hashtable.Keys)
                            {
                                Hashtable hash = hashtable[room] as Hashtable;
                                builder.AppendFormat("{0}:{1},", room, hash.PrintParams(typeof(GamePropertyKey), printType));
                            }
                            builder.Remove(builder.Length - 1, 1);
                            builder.Append("}}");
                        }
                        else
                        {
                            builder.AppendFormat("{0}:{{}}}}", name);
                        }
                        continue;
                    }
                    else
                    {
                        name = UtilityClass.GetConstantNameFromValue(typeof(ParameterCode), param.Key);
                        if (name.IsNullOrEmpty())
                        {
                            name = param.Key.ToString();
                        }
                        value = param.Value.Stringify(printType);
                    }
                    builder.AppendFormat("{0}:{1},", name, value);
                }
                builder.Remove(builder.Length - 1, 1);
            }
            builder.Append("}");
            return builder.ToString();
        }

        public static string PrintOpResponseParams(this ParameterDictionary data, bool printType = false)
        {
            return data.paramDict.PrintOpResponseParams(printType);
        }

        public static string PrintEventParams(this ParameterDictionary data, bool printType = false)
        {
            return data.paramDict.PrintOpResponseParams(printType);
        }

        public static string PrintOpResponseParams(this NonAllocDictionary<byte, object> data, bool printType = false)
        {
            StringBuilder builder = new StringBuilder("{");
            if (!data.IsNull() && data.Count > 0)
            {
                foreach (var param in data)
                {
                    string name = string.Empty, value = string.Empty;
                    if (param.Key == ParameterCode.PlayerProperties)
                    {
                        name = "PlayerProperties";
                        Hashtable props = param.Value as Hashtable;
                        if (!props.IsNull())
                        {
                            builder.AppendFormat("{0}:{{", name);
                            if (props.Count > 0)
                            {
                                foreach (var p in props.Keys)
                                {
                                    Hashtable tmp = props[p] as Hashtable;
                                    builder.AppendFormat("{0}:{1},", p, tmp.PrintParams(typeof(ActorProperties), printType));
                                }
                                builder.Remove(builder.Length - 1, 1);
                            }
                            builder.Append("},");
                            continue;
                        }
                    }
                    else if (param.Key == ParameterCode.GameProperties)
                    {
                        name = "GameProperties";
                        value = (param.Value as Hashtable).PrintParams(typeof(GamePropertyKey), printType);
                    }
                    else if (param.Key == ParameterCode.Properties)
                    {
                        name = "Properties";
                        value = (param.Value as Hashtable).PrintParams(typeof(GamePropertyKey), printType);
                    }
                    else if (param.Key == ParameterCode.GameList)
                    {
                        name = "GameList";
                        Hashtable hashtable = param.Value as Hashtable;
                        if (hashtable.Count > 0)
                        {
                            builder.AppendFormat("{0}:{{", name);
                            foreach (string room in hashtable.Keys)
                            {
                                Hashtable hash = hashtable[room] as Hashtable;
                                builder.AppendFormat("{0}:{1},", room, hash.PrintParams(typeof(GamePropertyKey), printType));
                            }
                            builder.Remove(builder.Length - 1, 1);
                            builder.Append("}}");
                        }
                        else
                        {
                            builder.AppendFormat("{0}:{{}}}}", name);
                        }
                        continue;
                    }
                    else
                    {
                        name = UtilityClass.GetConstantNameFromValue(typeof(ParameterCode), param.Key);
                        if (name.IsNullOrEmpty())
                        {
                            name = param.Key.ToString();
                        }
                        value = param.Value.Stringify(printType);
                    }
                    builder.AppendFormat("{0}:{1},", name, value);
                }
                builder.Remove(builder.Length - 1, 1);
            }
            builder.Append("}");
            return builder.ToString();
        }

        public static string PrintOpResponseParams(this Dictionary<byte, object> data, bool printType = false)
        {
            StringBuilder builder = new StringBuilder("{");
            if (!data.IsNull() && data.Count > 0)
            {
                foreach (var param in data)
                {
                    string name = string.Empty, value = string.Empty;
                    if (param.Key == ParameterCode.PlayerProperties)
                    {
                        name = "PlayerProperties";
                        Hashtable props = param.Value as Hashtable;
                        if (!props.IsNull())
                        {
                            builder.AppendFormat("{0}:{{", name);
                            if (props.Count > 0)
                            {
                                foreach (var p in props.Keys)
                                {
                                    Hashtable tmp = props[p] as Hashtable;
                                    builder.AppendFormat("{0}:{1},", p, tmp.PrintParams(typeof(ActorProperties), printType));
                                }
                                builder.Remove(builder.Length - 1, 1);
                            }
                            builder.Append("},");
                            continue;
                        }
                    }
                    else if (param.Key == ParameterCode.GameProperties)
                    {
                        name = "GameProperties";
                        value = (param.Value as Hashtable).PrintParams(typeof(GamePropertyKey), printType);
                    }
                    else if (param.Key == ParameterCode.Properties)
                    {
                        name = "Properties";
                        value = (param.Value as Hashtable).PrintParams(typeof(GamePropertyKey), printType);
                    }
                    else if (param.Key == ParameterCode.GameList)
                    {
                        name = "GameList";
                        Hashtable hashtable = param.Value as Hashtable;
                        if (hashtable.Count > 0)
                        {
                            builder.AppendFormat("{0}:{{", name);
                            foreach (string room in hashtable.Keys)
                            {
                                Hashtable hash = hashtable[room] as Hashtable;
                                builder.AppendFormat("{0}:{1},", room, hash.PrintParams(typeof(GamePropertyKey), printType));
                            }
                            builder.Remove(builder.Length - 1, 1);
                            builder.Append("}}");
                        }
                        else
                        {
                            builder.AppendFormat("{0}:{{}}}}", name);
                        }
                        continue;
                    }
                    else
                    {
                        name = UtilityClass.GetConstantNameFromValue(typeof(ParameterCode), param.Key);
                        if (name.IsNullOrEmpty())
                        {
                            name = param.Key.ToString();
                        }
                        value = param.Value.Stringify(printType);
                    }
                    builder.AppendFormat("{0}:{1},", name, value);
                }
                builder.Remove(builder.Length - 1, 1);
            }
            builder.Append("}");
            return builder.ToString();
        }
    }
}
