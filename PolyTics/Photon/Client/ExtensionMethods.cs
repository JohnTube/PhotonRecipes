namespace PolyTics.Photon.Client
{
    using Utils;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Hashtable = ExitGames.Client.Photon.Hashtable;

    public static partial class ExtensionMethods
    {
        public static string PrintParams(this Hashtable data, Type type, bool printType = false)
        {
            StringBuilder builder = new StringBuilder("{");
            if (!data.IsNull() && data.Count > 0)
            {
                foreach (var prop in data.Keys)
                {
                    string name = UtilityClass.GetConstantNameFromValue(type, prop);
                    if (name.IsNullOrEmpty())
                    {
                        name = prop.ToString();
                    }
                    builder.AppendFormat("{0}:{1},", name, data[prop].Stringify(printType));
                }
                builder.Remove(builder.Length - 1, 1);
            }
            builder.Append("}");
            return builder.ToString();
        }

        public static string PrintParams(this Hashtable data, IList<Type> types, bool printType = false)
        {
            StringBuilder builder = new StringBuilder("{");
            if (!data.IsNull() && data.Count > 0)
            {
                foreach (var prop in data.Keys)
                {
                    string name = UtilityClass.GetConstantNameFromValue(types, prop);
                    if (name.IsNullOrEmpty())
                    {
                        name = prop.ToString();
                    }
                    builder.AppendFormat("{0}:{1},", name, data[prop].Stringify(printType));
                }
                builder.Remove(builder.Length - 1, 1);
            }
            builder.Append("}");
            return builder.ToString();
        }

        public static string PrintParams(this Dictionary<byte, object> data, Type type, bool printType = false)
        {
            StringBuilder builder = new StringBuilder("{");
            if (!data.IsNull() && data.Count > 0)
            {
                foreach (var prop in data.Keys)
                {
                    string name = UtilityClass.GetConstantNameFromValue(type, prop);
                    if (name.IsNullOrEmpty())
                    {
                        name = prop.ToString();
                    }
                    builder.AppendFormat("{0}:{1},", name, data[prop].Stringify(printType));
                }
                builder.Remove(builder.Length - 1, 1);
            }
            builder.Append("}");
            return builder.ToString();
        }

        public static string PrintParams(this Dictionary<byte, object> data, IList<Type> types, bool printType = false)
        {
            StringBuilder builder = new StringBuilder("{");
            if (!data.IsNull() && data.Count > 0)
            {
                foreach (var prop in data.Keys)
                {
                    string name = UtilityClass.GetConstantNameFromValue(types, prop);
                    if (name.IsNullOrEmpty())
                    {
                        name = prop.ToString();
                    }
                    builder.AppendFormat("{0}:{1},", name, data[prop].Stringify(printType));
                }
                builder.Remove(builder.Length - 1, 1);
            }
            builder.Append("}");
            return builder.ToString();
        }

    }
}