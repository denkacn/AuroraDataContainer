using System;
using System.Collections.Generic;
using System.Reflection;
using AuroraDataContainer.Attributes;
using AuroraDataContainer.Interface;
using AuroraDataContainer.Utilities;

namespace AuroraDataContainer.Models
{
    internal class DataPack
    {
        internal IData Data => _data;
        
        private readonly Dictionary<Type, object> _keys;

        private IData _data;

        internal DataPack(IData data)
        {
            _data = data;
            _keys = new Dictionary<Type, object>();

            foreach (var member in data.GetType().GetMembers())
            {
                if (member.MemberType == MemberTypes.Field  || member.MemberType == MemberTypes.Property)
                {
                    if (member.GetCustomAttributes(typeof(DataKeyAttribute), false).Length == 0) continue;

                    if (DataContainerHelper.TryGetValueInfo(member, data, out var valueType, out var obj))
                    {
                        _keys.TryAdd(valueType, obj);
                    }
                }
            }
        }

        internal bool HasKeyWithType(Type type) => _keys.ContainsKey(type);

        internal bool HasKeyWithType<T>() => HasKeyWithType(typeof(T));

        internal object GetKeyValue(Type type) => _keys.GetValueOrDefault(type);

        internal T GetKeyValue<T>() => (T)GetKeyValue(typeof(T));

        internal bool TryGetKeyValue(Type type, out object key) => _keys.TryGetValue(type, out key);

        internal bool TryGetKeyValue<T>(out T key)
        {
            if (_keys.TryGetValue(typeof(T), out var value))
            {
                key = (T)value;
                return true;
            }

            key = default;
            return false;
        }

        internal void Set(IData data)
        {
            _data = data;
        }
    }
}