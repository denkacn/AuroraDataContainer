using System;
using System.Linq;
using System.Reflection;
using AuroraDataContainer.Attributes;
using AuroraDataContainer.Interface;

namespace AuroraDataContainer.Utilities
{
    internal static class DataContainerHelper
    {
        internal static bool TryGetKeyInfo(IData data, out Type indexType, out object index)
        {
            foreach (var member in data.GetType().GetMembers())
            {
                if (member.GetCustomAttributes(typeof(DataKeyAttribute), false).Length != 0 &&
                    TryGetValueInfo(member, data, out indexType, out index))
                {
                    return true;
                }
            }

            indexType = null;
            index = null;
            return false;
        }

        internal static bool IsHasKey(Type type) => type.GetMembers()
            .Any(member => member.GetCustomAttributes(typeof(DataKeyAttribute), false).Length != 0);

        internal static bool TryGetValueInfo(MemberInfo member, object target, out Type valueType, out object value)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    var fieldInfo = (FieldInfo)member;
                    value = fieldInfo.GetValue(target);
                    valueType = fieldInfo.FieldType;
                    return true;
                case MemberTypes.Property:
                    var propertyInfo = (PropertyInfo)member;
                    value = propertyInfo.GetValue(target, null);
                    valueType = propertyInfo.PropertyType;
                    return true;
                default:
                    valueType = null;
                    value = null;
                    return false;
            }
        }
    }
}