//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    IDataCollection.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   01/06/2024 14:03
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace WanFramework.UI.DataComponent
{
    /// <summary>
    /// 数据集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class DataCollection<T> : DataModelBase, IList<T> where T : DataModelBase
    {
        public UnityEvent<DataCollection<T>, int, T> onItemInsert = new();
        public UnityEvent<DataCollection<T>, int> onItemRemove = new();

        private List<T> _data = new();
        
        public int Count => _data.Count;
        public bool IsReadOnly => false;

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_data).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_data).GetEnumerator();
        }

        public bool Contains(T item)
        {
            return _data.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _data.CopyTo(array, arrayIndex);
        }
        
        public int IndexOf(T item)
        {
            return _data.IndexOf(item);
        }
        
        public void Add(T item)
        {
            Insert(_data.Count, item);
        }

        public void Clear()
        {
            var oldCount = _data.Count;
            _data.Clear();
            // 从后往前发送item移除事件
            for (var i = oldCount - 1; i >= 0; --i)
                onItemRemove?.Invoke(this, i);
        }

        public bool Remove(T item)
        {
            var index = _data.IndexOf(item);
            if (index == -1) return false;
            RemoveAt(index);
            return true;
        }

        public T this[int index]
        {
            get => _data[index];
            set
            {
                RemoveAt(index);
                Insert(index, value);
            }
        }

        public void RemoveAt(int index)
        {
            var oldValue = _data[index];
            _data.RemoveAt(index);
            onItemRemove?.Invoke(this, index);
        }
        
        public void Insert(int index, T item)
        {
            _data.Insert(index, item);
            onItemInsert?.Invoke(this, index, item);
        }
    }
}