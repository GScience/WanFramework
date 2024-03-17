//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    EffectBase.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   01/12/2024 16:20
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using System;
using UnityEngine;
using WanFramework.Data;

namespace WanFramework.Effect
{
    /// <summary>
    /// 特效基类
    /// </summary>
    public abstract class EffectBase : MonoBehaviour
    {
        private static DataTableAsset _dataTable;
        private EffectObjectPool _owner;
        public DataEntry Data { get; private set; }

        /// <summary>
        /// 特效表
        /// </summary>
        protected abstract string DataTableName { get; }

        public virtual void Awake()
        {
            if (_dataTable == null)
                _dataTable = DataSystem.Instance.Load(DataTableName);
        }
        
        internal void Play<T>(EffectObjectPool pool, T data) where T : struct, Enum, IConvertible
        {
            _owner = pool;
            Data = GetData(data.ToInt32(null));
            OnPlay();
        }

        protected virtual void OnPlay()
        {
            
        }

        private DataEntry GetData(int id)
        {
            return _dataTable == null ? null : _dataTable.Get(id);
        }
        
        /// <summary>
        /// 释放特效，主动将自己释放回pool
        /// </summary>
        protected void Release()
        {
            _owner.Release(this);
            _owner = null;
        }
    }
}