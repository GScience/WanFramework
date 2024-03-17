//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    DataModelEditor.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   03/09/2024 14:20
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using WanFramework.UI.DataComponent;

namespace WanFramework.Editor.Debugger
{
    public class RuntimeDataModelEditor : EditorWindow
    {
        private List<Type> _dataModelTypes = new();
        private string[] _dataModelNames;

        private int _currentIndex = 0;
        private DataModelBase _current;
        
        [MenuItem("Window/WanFramework/RuntimeDataModelEditor")]
        [MenuItem("WanFramework/Window/RuntimeDataModelEditor")]
        private static void ShowWindow()
        {
            var wnd = GetWindow<RuntimeDataModelEditor>();
            wnd.titleContent = new GUIContent("数据模型修改器");
        }

        private void OnEnable()
        {
            var nameCache = new List<string>();
            _dataModelTypes.Clear();
            _dataModelTypes.Add(null);
            nameCache.Add("<NULL>");
            _currentIndex = 0;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in asm.GetTypes())
                if (typeof(DataModelBase).IsAssignableFrom(type) &&
                    !type.IsAbstract &&
                    !type.IsGenericType)
                {
                    _dataModelTypes.Add(type);
                    nameCache.Add(type.Name);
                }
            _dataModelNames = nameCache.ToArray();
        }

        public void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("游戏跑起来才能改", MessageType.Warning);
                return;
            }
            var newIndex = EditorGUILayout.Popup(_currentIndex, _dataModelNames);
            if (newIndex != _currentIndex)
            {
                var instanceType = typeof(DataModel<>).MakeGenericType(_dataModelTypes[newIndex]);
                _current = instanceType.GetProperty("Instance")?.GetValue(null) as DataModelBase;
                _currentIndex = newIndex;
            }

            if (_current != null)
            {
                try
                {
                    ShowObjectEditor(_current);
                }
                catch (Exception e)
                {
                    Debug.LogError($"数据模型{_current}暂不支持编辑");
                    Debug.LogException(e);
                }
            }
        }

        private object ShowArrayEditor(Array obj)
        {
            for (var i = 0; i < obj.Length; ++i)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{i}: ");
                ShowValueEditor(obj.GetValue(i));
                GUILayout.EndHorizontal();
            }

            return obj;
        }
        
        private object ShowListEditor(IList obj)
        {
            // TODO list需要支持插入删除元素
            for (var i = 0; i < obj.Count; ++i)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{i}: ");
                ShowValueEditor(obj[i]);
                GUILayout.EndHorizontal();
            }

            return obj;
        }

        private object ShowValueEditor(object val)
        {
            GUILayout.BeginVertical();
            var newVal = val switch
            {
                int v => EditorGUILayout.IntField(v),
                uint v => EditorGUILayout.IntField((int)v),
                short v => EditorGUILayout.IntField(v),
                ushort v => EditorGUILayout.IntField(v),
                long v => EditorGUILayout.IntField((int)v),
                ulong v => EditorGUILayout.IntField((int)v),
                float v => EditorGUILayout.FloatField(v),
                double v => EditorGUILayout.FloatField((float)v),
                decimal v => EditorGUILayout.FloatField((float)v),
                string v => EditorGUILayout.TextField(v),
                Enum v => EditorGUILayout.EnumPopup(v),
                Array v => ShowArrayEditor(v),
                IList v => ShowListEditor(v),
                UnityEngine.Object v => EditorGUILayout.ObjectField(v, v.GetType(), true),
                _ => ShowObjectEditor(val)
            };
            GUILayout.EndVertical();
            return newVal;
        }
        
        private object ShowObjectEditor(object obj)
        {
            foreach (var property in obj.GetType().GetProperties())
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(property.Name);
                GUILayout.BeginVertical();
                var val = property.GetValue(obj);
                var newVal = ShowValueEditor(val);
                if (!newVal.Equals(val))
                    property.SetValue(obj, newVal);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            return obj;
        }
    }
}