//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    DataTableImporter.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   12/24/2023 11:44
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using ExcelDataReader;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.Compilation;
using UnityEngine;
using WanFramework.Data;

namespace WanFramework.Editor.Data
{
    [ScriptedImporter(1, "xlsx")]
    public class ExcelDataTableImporter : ScriptedImporter
    {
        public MonoScript dataTableScript;
        public string @namespace = "Game.Data";
        
        private bool RegenerateScript(DataTableRaw tableRaw)
        {
            // Generate source code
            var sourceGenerator = new DataTableAssetSourceGenerator();
            var scriptCode = sourceGenerator.Generate(@namespace, ref tableRaw);
            if (scriptCode == dataTableScript.text)
                return false;
            var scriptPath = AssetDatabase.GetAssetPath(dataTableScript);
            File.WriteAllText(scriptPath, scriptCode);
            EditorUtility.SetDirty(this);
            SaveAndReimport();
            return true;
        }

        private DataTableAsset LoadDataTableAsset(DataTableRaw tableRaw)
        {
            // Check source generation
            var assetType = dataTableScript.GetClass();
            var assetEntryType = assetType?.GetNestedType("Entry");
            if (assetEntryType == null)
            {
                Debug.LogWarning("Not an valid asset table script");
                return null;
            }
                
            // Create data table asset
            var readerType = typeof(DataTableAssetReader<,>).MakeGenericType(assetEntryType, assetType);
            var reader = Activator.CreateInstance(readerType);
            var asset = readerType.GetMethod("Load")!.Invoke(reader, new object[] { tableRaw }) as DataTableAsset;
            return asset;
        }

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var assetFileName = Path.GetFileName(assetPath);
            if (assetFileName.StartsWith("~$"))
                return;

            if (dataTableScript == null)
                return;
            ctx.DependsOnArtifact(AssetDatabase.GetAssetPath(dataTableScript));
            // Load raw table
            var tableRaw = DataTableRaw.FromExcel(ctx.assetPath);
            if (!tableRaw.OnValid())
                throw new DataTableImportException(
                    "Require Name column in data table and name column should be \"string\"", ref tableRaw);

            if (!RegenerateScript(tableRaw))
            {
                var asset = LoadDataTableAsset(tableRaw);
                if (asset != null)
                {
                    ctx.AddObjectToAsset("DataTable", asset);
                    ctx.SetMainObject(asset);
                }
            }
        }
    }
}