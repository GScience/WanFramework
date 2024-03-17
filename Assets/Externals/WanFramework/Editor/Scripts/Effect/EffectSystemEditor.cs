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
using WanFramework.Effect;

namespace WanFramework.Editor.Effect
{
    [CustomEditor(typeof(EffectSystem))]
    public class EffectSystemEditor : UnityEditor.Editor
    {
        private const string DefaultEffectPath = "Assets/Game/Runtime/Effect/";

        private bool _showResource = true;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.HelpBox(DefaultEffectPath, MessageType.Info);
            if (GUILayout.Button("Load all effect"))
                InitializeResource();
            _showResource = ResourceGUIHelper.ShowResource(EffectSystem.DefaultEffectCategory, _showResource);
        }

        [ResourceInitializer("EffectSystem")]
        private static void InitializeResource()
        {
            ResourceEditorUtility.LoadAllPrefabAtPath<EffectBase>(DefaultEffectPath, EffectSystem.DefaultEffectCategory);
        }
    }
}