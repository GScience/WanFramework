//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    DataTableAssetEditor.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   03/07/2024 15:18
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using UnityEditor;
using WanFramework.Data;

namespace WanFramework.Editor.Data
{
    [CustomEditor(typeof(DataTableAsset<>), true)]
    public class DataTableAssetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}