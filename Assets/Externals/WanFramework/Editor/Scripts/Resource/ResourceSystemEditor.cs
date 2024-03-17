//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    ResourceSystemEditor.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   01/11/2024 16:29
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Compilation;
using WanFramework.Resource;

namespace WanFramework.Editor.Resource
{
    [CustomEditor(typeof(ResourceSystem))]
    public class ResourceSystemEditor : UnityEditor.Editor
    {
        private string _supportedResourceTypes;
        
        protected void OnEnable()
        {
            _supportedResourceTypes = "Resource initializers: " + ResourceEditorUtility.SupportedResourceTypes;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Load all resources"))
                ResourceEditorUtility.InitializeResource();
            EditorGUILayout.HelpBox(_supportedResourceTypes, MessageType.Info);
        }
    }

    public static class ResourceGUIHelper
    {
        public static bool ShowResource(string category, bool isFolded)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                var entries = ResourceSystem.Instance.GetAllEntries();
                EditorGUILayout.BeginVertical();
                isFolded = EditorGUILayout.Foldout(isFolded, "Resources");
                if (isFolded)
                    foreach (var entry in entries)
                    {
                        if (entry.Category == category)
                            ShowResourceEntry(entry);
                    }

                EditorGUILayout.EndVertical();
            }

            return isFolded;
        }

        public static void ShowResourceEntry(ResourceEntry entry)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(entry.Name);
            EditorGUILayout.ObjectField(entry.Obj, typeof(UnityEngine.Object), true);
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 自动注册静态方法到资源初始化调用中
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ResourceInitializerAttribute : Attribute
    {
        public string Name { get; }

        public ResourceInitializerAttribute(string name)
        {
            Name = name;
        }
    }
}