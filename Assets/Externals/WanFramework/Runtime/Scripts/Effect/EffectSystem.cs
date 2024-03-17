//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    EffectSystem.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   01/12/2024 16:18
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using WanFramework.Base;
using WanFramework.Data;
using WanFramework.Resource;
using Object = UnityEngine.Object;

namespace WanFramework.Effect
{
    /// <summary>
    /// 特效系统，负责实例化并播放特效
    /// </summary>
    public class EffectSystem : SystemBase<EffectSystem>
    {
        public const string DefaultEffectCategory = "Effect";
        
        /// <summary>
        /// 特效池
        /// </summary>
        private Dictionary<string, EffectObjectPool> _effectPoolDict = new();
        
        public EffectBase Play<T>(string effectName, T data) 
            where T : struct, Enum, IConvertible
        {
            if (!_effectPoolDict.TryGetValue(effectName, out var pool))
            {
                var template = ResourceSystem.Instance.Load<EffectBase>(effectName, DefaultEffectCategory);
                pool = new EffectObjectPool(template, transform);
                _effectPoolDict[effectName] = pool;
            }

            var instance = pool.Get();
            instance.Play(pool, data);
            return instance;
        }
        
        public TEffectBase Play<TEffectBase, T>(string effectName, T data) 
            where TEffectBase : EffectBase 
            where T : struct, Enum, IConvertible
        {
            return (TEffectBase)Play(effectName, data);
        }
    }

    /// <summary>
    /// 特效对象池
    /// </summary>
    internal sealed class EffectObjectPool : IDisposable
    {
        private readonly ObjectPool<EffectBase> _effectPool;

        public EffectBase Get()
        {
            return _effectPool.Get();
        }

        public void Release(EffectBase effect)
        {
            _effectPool.Release(effect);
        }
        public EffectObjectPool(EffectBase template, Transform root)
        {
            _effectPool = new ObjectPool<EffectBase>(Create, OnGet, OnRelease, OnViewDestroy);
            return;
            
            EffectBase Create()
            {
                return Object.Instantiate(template, root);
            }

            void OnGet(EffectBase view)
            {
                view.gameObject.SetActive(true);
            }
            
            void OnRelease(EffectBase view)
            {
                view.gameObject.SetActive(false);
            }
            
            void OnViewDestroy(EffectBase view)
            {
                Object.Destroy(view.gameObject);
            }
        }

        public void Dispose()
        {
            _effectPool?.Dispose();
        }
    }
}