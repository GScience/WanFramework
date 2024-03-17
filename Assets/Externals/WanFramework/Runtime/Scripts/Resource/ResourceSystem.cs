//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    ResourceSystem.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   12/24/2023 10:52
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using WanFramework.Base;

namespace WanFramework.Resource
{
    [Serializable]
    public class ResourceEntry
    {
        [SerializeField]
        private string name;
        public string Name => name;

        [SerializeField]
        private string category;
        public string Category => category;
        
        [SerializeField]
        private UnityEngine.Object obj;
        public UnityEngine.Object Obj => obj;

        public ResourceEntry()
        {
        }

        public ResourceEntry(string name, string category, UnityEngine.Object obj)
        {
            this.name = name;
            this.category = category;
            this.obj = obj;
        }
    }

    /// <summary>
    /// 资源分类
    /// </summary>
    class ResourceCategory
    {
        private Dictionary<string, ResourceEntry> _dict = new();

        public UnityEngine.Object Get(string path)
        {
            return _dict.TryGetValue(path, out var result) ? result.Obj : null;
        }
        
        public void Add(ResourceEntry entry)
        {
            _dict[entry.Name] = entry;
        }
    }
    
    [SystemPriority(SystemPriorities.Resource)]
    public class ResourceSystem : SystemBase<ResourceSystem>
    {
        [SerializeField] private ResourceEntry[] entries;
        private readonly Dictionary<string, ResourceCategory> _dict = new();

        public ResourceEntry[] GetAllEntries()
        {
            return entries;
        }
        public T Load<T>(string path, string category) where T : UnityEngine.Object
        {
            if (!_dict.TryGetValue(category, out var resourceCategory))
            {
                Debug.LogError($"Resource category {category} not found");
                return null;
            }

            var result = resourceCategory.Get(path);
            if (result == null)
                Debug.LogError($"{path} not found");
            if (result is T tResult) 
                return tResult;
            Debug.LogError($"{result} is not a {typeof(T)}");
            return null;
        }

        public override void Init()
        {
            BuildResourceIndex();
        }
        
        /// <summary>
        /// 构建资源索引
        /// </summary>
        private void BuildResourceIndex()
        {
            foreach (var entry in entries)
            {
                if (!_dict.TryGetValue(entry.Category, out var category))
                {
                    category = new();
                    _dict[entry.Category] = category;
                }
                category.Add(entry);
            }
        }
    }
}