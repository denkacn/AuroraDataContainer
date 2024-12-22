using System;
using System.Collections.Generic;
using System.Linq;
using AuroraDataContainer.Interface;
using AuroraDataContainer.Models;
using AuroraDataContainer.Utilities;

namespace AuroraDataContainer
{
    public class DataContainer : IDataContainer
    {
        private readonly List<DataPack> _dataContainers = new List<DataPack>();

        public bool Contains(IData data) => _dataContainers.Find(info => info.Data == data) != null;
        
        public void Add(IData data)
        {
            if (data == null || Contains(data)) return;

            var type = data.GetType();

            if (!DataContainerHelper.IsHasKey(type)) return;
            if (!DataContainerHelper.TryGetKeyInfo(data, out var indexType, out var index)) return;
            if (GetByKey(type, indexType, index) != null) return;

            _dataContainers.Add(new DataPack(data));
        }
        
        public void AddRange(IEnumerable<IData> dataList)
        {
            if (dataList == null) return;

            foreach (var data in dataList) Add(data);
        }
        
        public T Get<T>() where T : class, IData
        {
            return _dataContainers
                .Where(cfg => cfg.Data is T)
                .Select(cfg => cfg.Data as T)
                .ToArray().FirstOrDefault();
        }
        
        public T[] GetAll<T>() where T : class, IData
        {
            return _dataContainers
                .Where(cfg => cfg.Data is T)
                .Select(cfg => cfg.Data as T)
                .ToArray();
        }
        
        public IData GetByKey(Type dataType, Type indexType, object index)
        {
            return _dataContainers
                .Where(container => container.Data.GetType() == dataType)
                .Where(container => container.HasKeyWithType(indexType))
                .Where(container => container.GetKeyValue(indexType).Equals(index))
                .Select(container => container.Data)
                .FirstOrDefault();
        }
        
        public TData GetByKey<TData, TIndex>(TIndex index) where TData : class, IData
        {
            return _dataContainers
                .Where(cfg => cfg.Data is TData)
                .Where(cfg => cfg.HasKeyWithType(typeof(TIndex)))
                .Where(cfg => cfg.GetKeyValue(typeof(TIndex)).Equals(index))
                .Select(cfg => cfg.Data as TData)
                .FirstOrDefault();
        }
        
        public bool HasKey(Type dataType) => DataContainerHelper.IsHasKey(dataType);

        public bool HasKey<TData>() where TData : IData => DataContainerHelper.IsHasKey(typeof(TData));

        public bool TryGetKeyValueFor(IData data, out object keyValue)
        {
            keyValue = null;

            var dataType = data.GetType();
            var dataInfo = _dataContainers.Find(i => i.Data == data && i.HasKeyWithType(dataType));
            if (dataInfo == null)
                return false;

            keyValue = dataInfo.GetKeyValue(dataType);
            return true;
        }

        public object GetKeyValueFor(IData data)
        {
            var dataType = data.GetType();

            return _dataContainers
                .Where(i => i.Data == data)
                .Where(i => i.HasKeyWithType(dataType))
                .Select(i => i.GetKeyValue(dataType))
                .FirstOrDefault();
        }

        public bool TryGetKeyValueFor<TData, TKey>(TData data, out TKey keyValue) where TData : class, IData
        {
            keyValue = default;

            var dataContainer = _dataContainers
                .FirstOrDefault(i => i.Data == data && i.HasKeyWithType<TKey>());

            if (dataContainer == null)
                return false;

            keyValue = dataContainer.GetKeyValue<TKey>();
            return true;
        }

        public void Remove<TData, TIndex>(TIndex index)
        {
            _dataContainers.RemoveAll(cfg => cfg.Data is TData && cfg.GetKeyValue(typeof(TIndex)).Equals(index));
        }
        
        public void Replace<TData, TIndex>(TIndex index, IData data)
        {
            var dataPack =
                _dataContainers.Find(cfg => cfg.Data is TData && cfg.GetKeyValue(typeof(TIndex)).Equals(index));
            
            dataPack.Set(data);
        }
    }
}