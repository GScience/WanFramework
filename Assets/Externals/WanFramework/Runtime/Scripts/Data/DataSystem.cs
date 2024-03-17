//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    DataSystem.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   12/24/2023 10:43
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using System;
using System.Collections.Generic;
using UnityEngine;
using WanFramework.Base;
using WanFramework.Resource;

namespace WanFramework.Data
{
    [SystemPriority(SystemPriorities.DataSystem)]
    public class DataSystem : SystemBase<DataSystem>
    {
        public const string DefaultTableCategory = "Table";
        
        public DataTableAsset Load(string tableName)
        {
            var table = ResourceSystem.Instance.Load<DataTableAsset>(tableName, DefaultTableCategory);
            if (table == null)
                Debug.LogError($"Table {tableName} not found");
            return table;
        }

        public T Load<T>(string tableName) where T : DataTableAsset
        {
            return (T)Load(tableName);
        }
    }
}