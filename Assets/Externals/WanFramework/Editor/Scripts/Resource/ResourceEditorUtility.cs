//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    ResourceEditorUtil.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   01/11/2024 14:46
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using WanFramework.Data;
using WanFramework.Resource;
using Object = UnityEngine.Object;

namespace WanFramework.Editor.Resource
{
    public static class ResourceEditorUtility
    {
        public static readonly string SupportedResourceTypes;

        private sealed class ResourceInitializer
        {
            public string Name { get; }
            public Action Load { get; }

            private ResourceInitializer(string name, Action load)
            {
                Name = name;
                Load = load;
            }

            public static ResourceInitializer FromMethodInfo(MethodInfo methodInfo)
            {
                if (methodInfo.IsAbstract ||
                    methodInfo.IsGenericMethod ||
                    !methodInfo.IsStatic)
                    return null;
                return new ResourceInitializer(
                    methodInfo.GetCustomAttribute<ResourceInitializerAttribute>().Name,
                    () => methodInfo.Invoke(null, Array.Empty<object>()));
            }
        }

        private static readonly List<ResourceInitializer> _resourceInitializer;

        static ResourceEditorUtility()
        {
            _resourceInitializer = new();
            var methods = TypeCache.GetMethodsWithAttribute<ResourceInitializerAttribute>();
            foreach (var method in methods)
            {
                var initializer = ResourceInitializer.FromMethodInfo(method);
                if (initializer != null)
                    _resourceInitializer.Add(initializer);
            }
            _resourceInitializer.Sort((a, b) => 
                string.Compare(a.Name, b.Name, StringComparison.Ordinal));

            SupportedResourceTypes = "";
            foreach (var ri in _resourceInitializer)
            {
                if (!string.IsNullOrEmpty(SupportedResourceTypes))
                    SupportedResourceTypes += ", ";
                SupportedResourceTypes += ri.Name;
            }
        }
        
        [MenuItem("WanFramework/Resource/Load all resources")]
        public static void InitializeResource()
        {
            if (_resourceInitializer == null) return;
            foreach (var initializer in _resourceInitializer)
            {
                Debug.Log($"Loading {initializer.Name}");
                initializer.Load();
            }
        }
        
        public static void RemoveResourceByCategory(string category)
        {
            RemoveResource(null, category);
        }
        
        public static void RemoveResource(string name, string category)
        {
            if (ResourceSystem.Instance == null)
            {
                Debug.LogError("ResourceSystem is null!");
                return;
            }
            var serializedObj = new SerializedObject(ResourceSystem.Instance);
            
            using var entriesProperty = serializedObj.FindProperty("entries");
            for (var i = 0; i < entriesProperty.arraySize;)
            {
                using var entryObject = entriesProperty.GetArrayElementAtIndex(i);
                using var nameProperty = entryObject.FindPropertyRelative("name");
                if (!string.IsNullOrEmpty(name) && nameProperty.stringValue != name)
                {
                    ++i;
                    continue;
                }
                using var categoryProperty = entryObject.FindPropertyRelative("category");
                if (categoryProperty.stringValue != category)
                {
                    ++i;
                    continue;
                }
                using var objProperty = entryObject.FindPropertyRelative("obj");
                // 删除后不需要++i
                entriesProperty.DeleteArrayElementAtIndex(i);
            }
            serializedObj.ApplyModifiedProperties();
        }

        public static UnityEngine.Object FindResource(string name, string category)
        {
            if (ResourceSystem.Instance == null)
            {
                Debug.LogError("ResourceSystem is null!");
                return null;
            }
            using var serializedObj = new SerializedObject(ResourceSystem.Instance);
            
            using var entriesProperty = serializedObj.FindProperty("entries");
            for (var i = 0; i < entriesProperty.arraySize; ++i)
            {
                using var entryObject = entriesProperty.GetArrayElementAtIndex(i);
                using var nameProperty = entryObject.FindPropertyRelative("name");
                if (nameProperty.stringValue != name) continue;
                using var categoryProperty = entryObject.FindPropertyRelative("category");
                if (categoryProperty.stringValue != category) continue;
                using var objProperty = entryObject.FindPropertyRelative("obj");
                return objProperty.objectReferenceValue;
            }

            return null;
        }
        
        public static void AddResource(string name, string category, UnityEngine.Object obj)
        {
            if (ResourceSystem.Instance == null)
            {
                Debug.LogError("ResourceSystem is null!");
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Resource name cannot be null");
                return;
            }
            using var serializedObj = new SerializedObject(ResourceSystem.Instance);
            serializedObj.SetIsDifferentCacheDirty();
            
            using var entriesProperty = serializedObj.FindProperty("entries");
            var count = entriesProperty.arraySize;
            entriesProperty.InsertArrayElementAtIndex(count);
            using var entryObject = entriesProperty.GetArrayElementAtIndex(count);
            using var nameProperty = entryObject.FindPropertyRelative("name");
            nameProperty.stringValue = name;
            using var categoryProperty = entryObject.FindPropertyRelative("category");
            categoryProperty.stringValue = category;
            using var objProperty = entryObject.FindPropertyRelative("obj");
            objProperty.objectReferenceValue = obj;
            serializedObj.ApplyModifiedProperties();
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (Directory.Exists(path)) return;
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }
        
        public static void LoadAllResourceAtPath<T>(string rootPath, string searchPattern, string category) where T : ScriptableObject
        {
            EnsureDirectoryExists(rootPath);
            var objs = 
                Directory.EnumerateFiles(rootPath, searchPattern, SearchOption.AllDirectories)
                    .Select(AssetDatabase.LoadMainAssetAtPath)
                    .Select(o => o as T)
                    .Where(o => o);
            RemoveResourceByCategory(category);
            foreach (var obj in objs)
            {
                var resourceKey = GetResourceKey(rootPath, obj);
                AddResource(resourceKey, category, obj);
            }
        }
        
        public static void LoadAllPrefabAtPath<T>(string rootPath, string category) where T : MonoBehaviour
        {
            EnsureDirectoryExists(rootPath);
            var behaviours =
                Directory.EnumerateFiles(rootPath, "*.prefab", SearchOption.AllDirectories)
                    .Select(AssetDatabase.LoadMainAssetAtPath)
                    .Select(o => o as GameObject)
                    .Where(o => o)
                    .Select(o => o.GetComponent<T>())
                    .Where(c => c);
            RemoveResourceByCategory(category);
            foreach (var behaviour in behaviours)
            {
                var resourceKey = GetResourceKey(rootPath, behaviour.gameObject);
                AddResource(resourceKey, category, behaviour);
            }
        }

        private static string GetResourceKey(string rootPath, Object asset)
        {
            var fullPath = AssetDatabase.GetAssetPath(asset);
            var relPath = Path.GetRelativePath(rootPath, fullPath);
            return relPath[..relPath.LastIndexOf('.')].Replace('/', '.').Replace('\\', '.');
        }
    }
}