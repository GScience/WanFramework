//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    UISystemEditor.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   01/11/2024 15:57
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using UnityEditor;
using UnityEngine;
using WanFramework.Data;
using WanFramework.Editor.Resource;
using WanFramework.UI;

namespace WanFramework.Editor.UI
{
    [CustomEditor(typeof(UISystem))]
    public class UISystemEditor : UnityEditor.Editor
    {
        private const string DefaultUIViewPath = "Assets/Game/Runtime/UI/";

        private bool _showResource = true;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.HelpBox(DefaultUIViewPath, MessageType.Info);
            if (GUILayout.Button("Load all UI view"))
                InitializeResource();
            _showResource = ResourceGUIHelper.ShowResource(UISystem.DefaultUIViewCategory, _showResource);
        }

        [ResourceInitializer("DataSystem")]
        private static void InitializeResource()
        {
            ResourceEditorUtility.LoadAllPrefabAtPath<UIRootView>(DefaultUIViewPath, UISystem.DefaultUIViewCategory);
        }
    }
}