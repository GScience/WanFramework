//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    DataTable.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   12/24/2023 10:43
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace WanFramework.Data
{
    public interface IDataTableAsset
    {
        public string TableName { get; }
        public DataEntry Get(int id);
    }
    
    public interface IDataTableAsset<out T> where T : DataEntry
    {
        public string TableName { get; }
        public T Get(int id);
    }
    
    /// <summary>
    /// 通用数据表资源
    /// </summary>
    public class DataTableAsset : ScriptableObject, IDataTableAsset
    {
        public string TableName => name;
        public virtual DataEntry Get(int id)
        {
            return null;
        }
    }
    
    /// <summary>
    /// 数据表泛型，源生成器生成的表资源均继承于此
    /// </summary>
    [Serializable]
    public class DataTableAsset<T> : DataTableAsset, IDataTableAsset<T> where T : DataEntry
    {
        [SerializeField] private T[] data;
        T IDataTableAsset<T>.Get(int id) => data[id];
        public override DataEntry Get(int id) => data[id];

        public int Length => data.Length;

        public void SetData(T[] d)
        {
            data = d;
        }
    }

    [Serializable]
    public abstract class DataEntry
    {
    }
}