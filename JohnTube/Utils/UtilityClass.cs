namespace JohnTube.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class UtilityClass
    {
        // http://stackoverflow.com/a/10261848/1449056
        public static string GetConstantNameFromValue(Type type, object val)
        {
            FieldInfo[] fieldInfos = type.GetFields(
                // Gets all public and static fields
                BindingFlags.Public | BindingFlags.Static |
                // This tells it to get the fields from all base types as well
                BindingFlags.FlattenHierarchy);
            // Go through the list and only pick out the constants
            foreach (FieldInfo fi in fieldInfos)
            {
                // remove deprecated / obsolete fields/properties
                if (fi.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length != 0)
                {
                    continue;
                }
                // IsLiteral determines if its value is written at
                //   compile time and not changeable
                // IsInitOnly determine if the field can be set
                //   in the body of the constructor
                // for C# a field which is readonly keyword would have both true
                //   but a const field would have only IsLiteral equal to true
                if (fi.IsLiteral && !fi.IsInitOnly)
                {
                    object value = fi.GetRawConstantValue();
                    //Console.WriteLine("{0}={1}", fi.Name, value);
                    if (value.Equals(val))
                    {
                        return fi.Name;
                    }
                }
            }
            return val.Stringify();
        }

        public static string GetConstantNameFromValue(IList<Type> types, object val)
        {
            if (types.IsNull())
            {
                return val.Stringify();
            }
            if (val.IsNull())
            {
                return "null";
            }
            string temp = val.Stringify();
            foreach (var type in types)
            {
                temp = GetConstantNameFromValue(type, val);
                if (!temp.Equals(val.ToString()))
                {
                    return temp;
                }
            }
            return temp;
        }
    }
}