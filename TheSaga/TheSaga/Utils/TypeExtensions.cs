using System;
using System.Collections.Generic;

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
                if (iBacktick > 0) friendlyName = friendlyName.Remove(iBacktick);
                friendlyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = GetFriendlyName(typeParameters[i]);
                    friendlyName += i == 0 ? typeParamName : "," + typeParamName;
                }

                friendlyName += ">";
            }

            return friendlyName;
        }

        internal static Type ConstructGenericType(this Type generic, params Type[] typeArgs)
        {
            Type constructed = generic.MakeGenericType(typeArgs);
            return constructed;
        }

        internal static Type GetFirstGenericArgument(this Type generic)
        {
            return generic.GetGenericArguments()[0];
        }

        internal static Type GetInterfaceOf(this Type thisType, Type baseInterfaceType)
        {
            foreach (Type interfaceType in thisType.GetInterfaces())
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == baseInterfaceType)
                    return interfaceType;
                else if (baseInterfaceType == interfaceType || baseInterfaceType.IsAssignableFrom(interfaceType))
                    return interfaceType;
            return null;
        }

        internal static bool Is<T>(this object obj)
        {
            if (obj == null)
                return false;
            return obj.GetType().Is(typeof(T));
        }

        internal static bool Is<T>(this Type thisType)
        {
            return Is(thisType, typeof(T));
        }

        internal static bool Is(this object obj, Type baseType)
        {
            if (obj == null)
                return false;
            return obj.GetType().Is(baseType);
        }

        static Dictionary<Type, Dictionary<Type, bool>> isThisTypeCache =
            new Dictionary<Type, Dictionary<Type, bool>>();

        internal static bool Is(this Type thisType, Type baseType)
        {
            lock (isThisTypeCache)
            {
                Dictionary<Type, bool> baseTypeType;
                isThisTypeCache.TryGetValue(thisType, out baseTypeType);
                if (baseTypeType != null)
                {
                    bool isResult;
                    if (baseTypeType.TryGetValue(baseType, out isResult))
                        return isResult;
                }

                bool result =
                    baseType == thisType ||
                    baseType.IsAssignableFrom(thisType);

                if (!result)
                {
                    if (thisType.IsInterface && thisType.IsGenericType)
                    {
                        Type genericType = thisType.GetGenericTypeDefinition();

                        if (genericType != null)
                            result =
                                baseType == genericType ||
                                baseType.IsAssignableFrom(genericType);
                    }
                    else
                    {
                        foreach (Type interfaceType in thisType.GetInterfaces())
                        {
                            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == baseType)
                            {
                                result = true;
                                break;
                            }
                            else if (baseType == interfaceType || baseType.IsAssignableFrom(interfaceType))
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                }

                if (!isThisTypeCache.ContainsKey(thisType))
                    isThisTypeCache[thisType] = new Dictionary<Type, bool>();
                isThisTypeCache[thisType][baseType] = result;

                return result;
            }
        }
    }
}