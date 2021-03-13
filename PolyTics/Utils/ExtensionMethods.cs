namespace PolyTics.Utils
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static class ExtensionMethods
    {
        /// TODO: make IsNullOrEmpty a more generic extension for object type
        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return array == null || array.Length == 0;
        }

        public static bool IsNull<T>(this T o)
        {
            return o == null;
        }

        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        // TODO: remove deprecated / obsolete fields/properties
        public static string Dump<T>(this T obj,
            bool privateIncluded = false, // TODO: complete implementation of add/remove private members
            bool printType = false,
            bool removeNullOrEmpty = true, // TODO: complete implementation of empty containers
            string[] ignoreList = null) where T : class
        {
            Type type = obj.GetType();
            //BindingFlags flags = BindingFlags.Default;
            FieldInfo[] fields = type.GetFields();
            PropertyInfo[] properties = type.GetProperties();
            StringBuilder builder = new StringBuilder(/*"Fields:\n"*/);
            foreach (FieldInfo field in fields)
            {
                if (ignoreList.IsNullOrEmpty() || Array.IndexOf(ignoreList, field.Name) == -1)
                {
                    if (removeNullOrEmpty)
                    {
                        if (field.GetValue(obj).IsNull())
                        {
                            continue;
                        }
                        // TODO: remove empty containers
                        /*else if (field.GetType().IsArray && (field.GetValue(obj) as Array).IsNullOrEmpty()) {
                           continue;
                        }*/
                    }
                    if (printType)
                    {
                        builder.AppendFormat("{0}({2})={1}:", field.Name, field.GetValue(obj).Stringify(printType), field.GetType());
                    }
                    else
                    {
                        builder.AppendFormat("{0}={1}:", field.Name, field.GetValue(obj).Stringify(printType));
                    }
                }
            }
            //builder.Append("Properties:");
            // TODO: handle properties with indexers
            foreach (PropertyInfo property in properties)
            {
                if (ignoreList.IsNullOrEmpty() || Array.IndexOf(ignoreList, property.Name) == -1)
                {
                    if (removeNullOrEmpty)
                    {
                        if (property.GetValue(obj, null).IsNull())
                        {
                            continue;
                        }
                        // TODO: remove empty containers
                        /*else if (field.GetType().IsArray && (field.GetValue(obj) as Array).IsNullOrEmpty()) {
                           continue;
                        }*/
                    }
                    if (printType)
                    {
                        builder.AppendFormat("{0}({2})={1}:", property.Name, property.GetValue(obj, null).Stringify(printType), property.GetType());
                    }
                    else
                    {
                        builder.AppendFormat("{0}={1}:", property.Name, property.GetValue(obj, null).Stringify(printType));
                    }
                }
            }
            return builder.ToString().TrimEnd(':');
        }

        public static string Stringify<T>(this T data, bool printType = false)
        {
            if (data.IsNull())
            {
                return "null";
            }
            Type type = data.GetType();
            StringBuilder builder = new StringBuilder();
            if (type == typeof(String))
            {
                if (printType)
                {
                    builder.AppendFormat("({0})\"{1}\"", type.Name, data);
                }
                else
                {
                    builder.AppendFormat("\"{0}\"", data);
                }
            }
            else if (type.IsSimpleType())
            {
                if (printType)
                {
                    builder.AppendFormat("({0}){1}", type.Name, data);
                }
                else
                {
                    builder.AppendFormat("{0}", data);
                }
            }
            else if (type.IsArray)
            {
                var tmp = data as Array;
                builder.Append(tmp.Stringify(printType));
            }
            else if (type.IsEnum)
            {
                return Enum.GetName(type, data);
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                var tmp1 = data as IList;
                builder.Append(tmp1.Stringify(printType));
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var tmp2 = data as IDictionary;
                builder.Append(tmp2.Stringify(printType));
            }
            /*else { // TODO: Dump other types
                builder.Append(data.Stringify(printType));
            }*/
            return builder.ToString();
        }

        public static string Stringify(this Enum enumVal)
        {
            return Enum.GetName(enumVal.GetType(), enumVal);
        }

        /// <summary>
        /// Pretty print objects that implements IDictionary interface
        /// </summary>
        /// <param name="data"></param>
        /// <param name="printType">option to print values' types</param>
        /// <returns></returns>
        /// TODO: make Stringify a more generic extension for object type
        public static string Stringify(this IDictionary data, bool printType)
        {
            if (data == null)
            {
                return "null";
            }
            if (data.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("{");
                foreach (var key in data.Keys)
                {
                    var value = data[key];
                    if (value == null)
                    {
                        builder.AppendFormat("{0}:null,", key);
                    }
                    else
                    {
                        Type type = value.GetType();
                        if (printType)
                        {
                            if (type.IsGenericType /* || type.IsGenericTypeDefinition */)
                            {
                                Console.WriteLine(type);
                                string[] tmp = type.Name.Split('`');
                                int length = int.Parse(tmp[1]); //type.GetGenericArguments().Length
                                if (length == 1)
                                {
                                    builder.AppendFormat("\"({0}<{1}>){2}\":", tmp[0], type.GetGenericArguments()[0].Name, key);
                                }
                                else if (length == 2)
                                {
                                    builder.AppendFormat("\"({0}<{1},{3}>){2}\":", tmp[0], type.GetGenericArguments()[0].Name, key, type.GetGenericArguments()[1].Name);
                                }
                                else
                                {
                                    builder.AppendFormat("\"({0}){1}\":", tmp[0], key);
                                }
                            }
                            else
                            {
                                builder.AppendFormat("\"({0}){1}\":", type.Name, key);
                            }
                        }
                        else
                        {
                            builder.AppendFormat("{0}:", key);
                        }
                        builder.AppendFormat("{0},", value.Stringify(printType));
                    }
                }
                builder.Remove(builder.Length - 1, 1); // remove last ,
                builder.Append("}");
                return builder.ToString();
            }
            return "{}";

        }

        public static string Stringify(this IList data, bool printType)
        {
            if (data.IsNull())
            {
                return "null";
            }
            if (data.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                Type type = data.GetType();
                if (printType)
                {
                    builder.AppendFormat("({0})[", type.ToString().Replace("System.", string.Empty));
                }
                else
                {
                    builder.Append("[");
                }
                for (int i = 0; i < data.Count; i++)
                {
                    builder.AppendFormat("{0},", data[i].Stringify(printType && (type.GetElementType() == null || type.GetElementType().IsAssignableFrom(typeof(object)))));
                }
                builder.Remove(builder.Length - 1, 1); // remove last ,
                builder.Append("]");
                return builder.ToString();
            }
            return "[]";
        }

        public static string Stringify(this Array data, bool printType)
        {
            return Stringify(data as IList, printType);
        }

        public static string Stringify<T>(this T[] data, bool printType)
        {
            return Stringify(data as IList, printType);
        }

        /// <summary>
        /// Determine whether a type is simple (String, Decimal, DateTime, etc) 
        /// or complex (i.e. custom class with public properties and methods).
        /// or complex (i.e. custom class with public properties and methods).
        /// </summary>
        /// <see cref="http://stackoverflow.com/questions/2442534/how-to-test-if-type-is-primitive"/>
        public static bool IsSimpleType(
            this Type type)
        {
            //return (type == typeof(object) || Type.GetTypeCode(type) != TypeCode.Object);
            return
                type.IsValueType ||
                type.IsPrimitive ||
                new[] {
                typeof(String),
                typeof(Decimal),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(Guid)
            }.Contains(type) ||
                Convert.GetTypeCode(type) != TypeCode.Object ||
                type == typeof(object);
        }
    }

}
