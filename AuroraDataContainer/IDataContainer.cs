using System;
using System.Collections.Generic;
using AuroraDataContainer.Interface;

namespace AuroraDataContainer
{
    public interface IDataContainer
    {
        bool Contains(IData data);
        void Add(IData data);
        void AddRange(IEnumerable<IData> dataList);
        T Get<T>() where T : class, IData;
        T[] GetAll<T>() where T : class, IData;
        IData GetByKey(Type dataType, Type indexType, object index);
        TData GetByKey<TData, TIndex>(TIndex index) where TData : class, IData;
        bool HasKey(Type dataType);
        bool HasKey<TData>() where TData : IData;
        bool TryGetKeyValueFor(IData data, out object keyValue);
        object GetKeyValueFor(IData data);
        bool TryGetKeyValueFor<TData, TKey>(TData data, out TKey keyValue) where TData : class, IData;
        void Remove<TData, TIndex>(TIndex index);
        void Replace<TData, TIndex>(TIndex index, IData data);
    }
}