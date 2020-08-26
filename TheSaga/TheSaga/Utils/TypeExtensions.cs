using System;

namespace TheSaga.Utils
{
    internal static class TypeExtensions
    {
        internal static string GetFriendlyName(this Type type)
        {
            string friendlyName = type.Name;
            if (type.IsGenericType)
            {
                int iBacktick = friendlyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    friendlyName = friendlyName.Remove(iBacktick);
                }
                friendlyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = GetFriendlyName(typeParameters[i]);
                    friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
                }
                friendlyName += ">";
            }

            return friendlyName;
        }

        internal static bool Is<T>(this Type thisType)
        {
            return Is(thisType, typeof(T));
        }

        internal static bool Is(this Type thisType, Type baseType)
        {
            return
                baseType == thisType ||
                baseType.IsAssignableFrom(thisType);
        }
    }
}