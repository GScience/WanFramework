//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    DataSystemEditor.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   12/24/2023 13:19
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using WanFramework.Data;
using WanFramework.Editor.Resource;

namespace WanFramework.Editor.Data
{
    [CustomEditor(typeof(DataSystem))]
    public class DataSystemEditor : UnityEditor.Editor
    {
        private const string DefaultDataPath = "Assets/Game/Runtime/Data/";

        private bool _showResource = true;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.HelpBox(DefaultDataPath, MessageType.Info);
            if (GUILayout.Button("Load all table"))
                InitializeResource();
            _showResource = ResourceGUIHelper.ShowResource(DataSystem.DefaultTableCategory, _showResource);
        }

        [ResourceInitializer("DataSystem")]
        private static void InitializeResource()
        {
            ResourceEditorUtility.LoadAllResourceAtPath<DataTableAsset>(DefaultDataPath, "*.xlsx", DataSystem.DefaultTableCategory);
        }
    }
}