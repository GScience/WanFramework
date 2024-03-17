//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    DataTableImporter.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   01/01/2024 19:07
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ExcelDataReader;
using Microsoft.CSharp;
using UnityEditor;
using UnityEngine;
using WanFramework.Data;

namespace WanFramework.Editor.Data
{
    public class DataTableImportException : Exception
    {
        public DataTableImportException(string message, ref DataTableRaw dataTableRaw, ref DataTableRaw.DataBlock block, Exception inner)
            : base($"At {dataTableRaw.Path}({block.Row}:{block.Column}). {message}", inner)
        {
        }

        public DataTableImportException(string message, ref DataTableRaw dataTableRaw, Exception inner)
            : this(message, ref dataTableRaw, ref DataTableRaw.DataBlock.Empty, inner)
        {
        }
        
        public DataTableImportException(ref DataTableRaw dataTableRaw, Exception inner)
            : this("", ref dataTableRaw, ref DataTableRaw.DataBlock.Empty, inner)
        {
        }
        
        public DataTableImportException(string message, ref DataTableRaw dataTableRaw, ref DataTableRaw.DataBlock block)
            : base($"At {dataTableRaw.Path}({block.Row}:{block.Column}). {message}")
        {
        }

        public DataTableImportException(string message, ref DataTableRaw dataTableRaw)
            : this(message, ref dataTableRaw, ref DataTableRaw.DataBlock.Empty)
        {
        }
        
        public DataTableImportException(ref DataTableRaw dataTableRaw)
            : this("", ref dataTableRaw, ref DataTableRaw.DataBlock.Empty)
        {
        }
    }
    public class DataTableRaw
    {
        public string Path { get; private set; }

        public struct DataBlock
        {
            public static DataBlock Empty = new DataBlock("", 0, 0);
            public string Data;
            public int Row;
            public int Column;

            public DataBlock(string data, int row, int column)
            {
                Data = data;
                Row = row;
                Column = column;
            }
        }

        private DataBlock[][] _data;

        public DataBlock[] GetRow(int rowId)
        {
            return _data.Length <= rowId
                ? Array.Empty<DataBlock>() : _data[rowId];
        }

        public int GetRowCount()
        {
            return _data.Length;
        }

        public bool OnValid()
        {
            var name = GetRow(0);
            var type = GetRow(1);
            if (name.Length != type.Length) return false;
            for (var i = 0; i < name.Length; ++i)
                if (name[i].Data == "Name" && 
                    (type[i].Data == "string" || type[i].Data == "System.String"))
                    return true;
            return false;
        }
        
        public static DataTableRaw FromExcel(string path)
        {
            using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            
            var dataInRow = new List<DataBlock[]>(reader.RowCount);
            var rawRowIdx = 0;
            while (reader.Read())
            {
                ++rawRowIdx;
                var fieldCount = reader.FieldCount;
                // 空行跳过
                if (fieldCount == 0) continue;
                var first = reader.GetString(0);
                if (string.IsNullOrEmpty(first)) continue;
                // 注释行和空行跳过
                if (first.StartsWith("#") || string.IsNullOrEmpty(first)) continue;
                // 开始读row
                var row = new DataBlock[fieldCount];
                dataInRow.Add(row);
                for (var i = 0; i < fieldCount; ++i)
                {
                    var data = reader.GetValue(i)?.ToString() ?? "";
                    row[i] = new DataBlock(data, rawRowIdx, i + 1);
                }
            }
            return new DataTableRaw
            {
                _data = dataInRow.ToArray(),
                Path = path
            };
        }
    }
    
    /// <summary>
    /// 代码生成器
    /// </summary>
    public class DataTableAssetSourceGenerator
    {
        private static Type FindTypeByName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                Debug.LogError("Empty type is not allowed");
                return null;
            }
            // 对Array则需要递归展开
            var isArray = typeName.EndsWith("[]");
            if (isArray)
            {
                var innerTypeName = typeName[..^2];
                var innerType = FindTypeByName(innerTypeName);
                return innerType?.MakeArrayType();
            }
            
            // 内置类型关键字
            Type type;
            switch (typeName)
            {
                // System内置
                case "string":
                    type = typeof(string);
                    break;
                case "sbyte":
                    type = typeof(sbyte);
                    break;
                case "byte":
                    type = typeof(byte);
                    break;
                case "short":
                    type = typeof(short);
                    break;
                case "ushort":
                    type = typeof(ushort);
                    break;
                case "int":
                    type = typeof(int);
                    break;
                case "uint":
                    type = typeof(uint);
                    break;
                case "long":
                    type = typeof(long);
                    break;
                case "ulong":
                    type = typeof(ulong);
                    break;
                case "float":
                    type = typeof(float);
                    break;
                case "double":
                    type = typeof(double);
                    break;
                case "decimal":
                    type = typeof(decimal);
                    break;
                case "bool":
                    type = typeof(bool);
                    break;
                // Unity内置别名
                case "Vector2":
                    type = typeof(Vector2);
                    break;
                case "Vector3":
                    type = typeof(Vector3);
                    break;
                case "Vector4":
                    type = typeof(Vector4);
                    break;
                case "Vector2Int":
                    type = typeof(Vector2Int);
                    break;
                case "Vector3Int":
                    type = typeof(Vector3Int);
                    break;
                default:
                    type = null;
                    break;
            }
            if (type != null) return type;
            // 查找当前程序集中的类型
            type = Type.GetType(typeName);
            if (type != null) return type;
            // 从其他程序集中查找
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = asm.GetType(typeName);
                if (type != null) return type;
            }
            // 没找到
            return null;
        }

        /// <summary>
        /// 插入表头
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="dataTable"></param>
        public string Generate(string @namespace, ref DataTableRaw dataTable)
        {
            var headerArray = dataTable.GetRow(0);
            var typeArray = dataTable.GetRow(1);
            if (typeArray.Length != headerArray.Length)
                throw new DataTableImportException("Table header count is not equal to type count.", ref dataTable);
            var fields = new (Type type, string name)[headerArray.Length];
            for (var i = 0; i < headerArray.Length; ++i)
            {
                fields[i].name = headerArray[i].Data;
                var typeName = typeArray[i].Data;
                var type = FindTypeByName(typeName);
                if (type == null)
                    throw new DataTableImportException($"Type {typeName} not found.", ref dataTable);
                fields[i].type = type;
            }
            
            // 表名
            var tableName = Path.GetFileNameWithoutExtension(dataTable.Path);

            // 所有Key，保证每个Key的唯一性
            var keyColumnId = -1;
            for (var i = 0; i < headerArray.Length; ++i)
                if (headerArray[i].Data == "Name")
                {
                    keyColumnId = i;
                    break;
                }
            if (keyColumnId == -1)
                throw new DataTableImportException($"Column \"Name\" not found.", ref dataTable);
            var keys = new string[dataTable.GetRowCount() - 2];
            var keyHashSet = new HashSet<string>();
            for (var i = 0; i < dataTable.GetRowCount() - 2; ++i)
            {
                var block = dataTable.GetRow(i + 2)[keyColumnId];
                var key = block.Data;
                keys[i] = key;
                if (keyHashSet.Contains(key))
                    throw new DataTableImportException($"Name \"{key}\" already exists.", ref dataTable, ref block);
                keyHashSet.Add(key);
            }
            
            // 生成代码
            return Generate(@namespace, tableName, fields, keys);
        }

        public static string ToFieldName(string name)
        {
            return name[..1].ToLower() + name[1..];
        }
        
        public static string ToPropertyName(string name)
        {
            return name[..1].ToUpper() + name[1..];
        }

        private void GenerateAssetKeyEnum(CodeNamespace @namespace, string assetTypeName, string[] keys)
        {
            var enumTypeName = $"{assetTypeName}Names";
            var enumType = new CodeTypeDeclaration(enumTypeName)
            {
                IsEnum = true
            };
            for (var i = 0; i < keys.Length; ++i)
                enumType.Members.Add(
                    new CodeMemberField(enumTypeName, keys[i])
                    {
                        InitExpression = new CodeSnippetExpression(i.ToString())
                    });
            @namespace.Types.Add(enumType);
        }
        
        private void GenerateAssetType(CodeNamespace @namespace, string assetTypeName, (Type type, string name)[] fields)
        {
            var serializeFieldAttribute
                = new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializeField)));
            var serializableAttribute
                = new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializableAttribute)));
            var codeAssetEntryType = new CodeTypeDeclaration("Entry")
            {
                Attributes = MemberAttributes.Public,
                BaseTypes = { new CodeTypeReference(typeof(DataEntry)) },
                CustomAttributes = { serializableAttribute }
            };
            foreach (var (type, name) in fields)
            {
                codeAssetEntryType.Members.Add(new CodeMemberField(type, ToFieldName(name))
                {
                    Attributes = MemberAttributes.Private,
                    CustomAttributes = { serializeFieldAttribute }
                });
                codeAssetEntryType.Members.Add(new CodeMemberProperty
                {
                    Attributes = MemberAttributes.Final | MemberAttributes.Public,
                    Name = ToPropertyName(name),
                    Type = new CodeTypeReference(type),
                    GetStatements =
                    {
                        new CodeMethodReturnStatement(
                            new CodeFieldReferenceExpression(
                                new CodeThisReferenceExpression(), 
                                ToFieldName(name)))
                    }
                });
            }
            var codeAssetGetMethod = new CodeMemberMethod()
            {
                Attributes = MemberAttributes.Final | MemberAttributes.Public,
                Name = "Get",
                ReturnType = new CodeTypeReference($"{@namespace.Name}.{assetTypeName}.Entry"),
                Parameters = { new CodeParameterDeclarationExpression($"{@namespace.Name}.{assetTypeName}Names", "id")},
                Statements = 
                { 
                    new CodeMethodReturnStatement(
                        new CodeCastExpression($"{@namespace.Name}.{assetTypeName}.Entry",
                            new CodeMethodInvokeExpression(
                                new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "Get"), 
                                new CodeCastExpression("System.Int32", new CodeArgumentReferenceExpression("id"))
                                )
                            )
                        )
                }
            };
            var codeAssetType = new CodeTypeDeclaration(assetTypeName)
            {
                Attributes = MemberAttributes.Public,
                BaseTypes = { new CodeTypeReference("WanFramework.Data.DataTableAsset", new CodeTypeReference($"{assetTypeName}.Entry")) },
                Members =
                {
                    codeAssetEntryType,
                    codeAssetGetMethod
                },
                CustomAttributes = { serializableAttribute }
            };
            @namespace.Types.Add(codeAssetType);
        }
        private string Generate(string @namespace, string assetTypeName, (Type type, string name)[] fields, string[] keys)
        {
            var codeNamespace = new CodeNamespace(@namespace);
            GenerateAssetKeyEnum(codeNamespace, assetTypeName, keys);
            GenerateAssetType(codeNamespace, assetTypeName, fields);
            var compileUnit = new CodeCompileUnit()
            {
                Namespaces =
                {
                    codeNamespace
                }
            };
            using var provider = new CSharpCodeProvider();
            using var writer = new StringWriter();
            provider.GenerateCodeFromCompileUnit(compileUnit, writer, new CodeGeneratorOptions()
            {
                
            });
            return writer.ToString();
        }
    }

    public class DataTableAssetReader<TEntry, TDataTable>
        where TDataTable : DataTableAsset<TEntry>
        where TEntry : DataEntry, new()
    {
        public TDataTable Load(ref DataTableRaw dataTableRaw)
        {
            var header = dataTableRaw.GetRow(0);
            var table = ScriptableObject.CreateInstance<TDataTable>();
            var fields = new FieldInfo[header.Length];
            for (var col = 0; col < header.Length; ++col)
            {
                var fieldName = DataTableAssetSourceGenerator.ToFieldName(header[col].Data);
                fields[col] = typeof(TEntry).GetField(
                    fieldName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (fields[col] == null)
                {
                    throw new DataTableImportException($"Failed to find field with name {fieldName}", ref dataTableRaw);
                }
            }

            var entryCount = dataTableRaw.GetRowCount() - 2;
            var entries = new TEntry[entryCount];
            for (var row = 0; row < entryCount; ++row)
            {
                var rowData =  dataTableRaw.GetRow(row + 2);
                var entry = new TEntry();
                for (var col = 0; col < fields.Length; ++col)
                {
                    var field = fields[col];
                    object value;
                    try
                    {
                        value = ConvertTo(rowData[col].Data, field.FieldType);
                    }
                    catch (Exception e)
                    {
                        throw new DataTableImportException($"Failed to convert data", ref dataTableRaw, e);
                    }
                    field.SetValue(entry, value);
                }
                entries[row] = entry;
            }
            table.SetData(entries);
            return table;
        }
        
        private object ConvertTo(string data, Type type)
        {
            // Unity object
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                return AssetDatabase.LoadAssetAtPath(data, type);
            
            // Unity vector
            if (type == typeof(Vector2))
            {
                data = data.Trim();
                var args = data.Split(',');
                if (!data.StartsWith('(') || !data.EndsWith(')') || args.Length != 2)
                    throw new Exception($"Vector2 should be written in (x, y) format but get {data}");
                return new Vector2(float.Parse(args[0][1..]), float.Parse(args[1][..^1]));
            }
            if (type == typeof(Vector3))
            {
                data = data.Trim();
                var args = data.Split(',');
                if (!data.StartsWith('(') || !data.EndsWith(')') || args.Length != 3)
                    throw new Exception($"Vector3 should be written in (x, y, z) format but get {data}");
                return new Vector3(float.Parse(args[0][1..]), float.Parse(args[1]), float.Parse(args[2][..^1]));
            }
            if (type == typeof(Vector4))
            {
                data = data.Trim();
                var args = data.Split(',');
                if (!data.StartsWith('(') || !data.EndsWith(')') || args.Length != 4)
                    throw new Exception($"Vector4 should be written in (x, y, z, w) format but get {data}");
                return new Vector4(float.Parse(args[0][1..]), float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3][..^1]));
            }
            if (type == typeof(Vector2Int))
            {
                data = data.Trim();
                var args = data.Split(',');
                if (!data.StartsWith('(') || !data.EndsWith(')') || args.Length != 2)
                    throw new Exception($"Vector2Int should be written in (x, y) format but get {data}");
                return new Vector2Int(int.Parse(args[0][1..]), int.Parse(args[1][..^1]));
            }
            if (type == typeof(Vector3Int))
            {
                data = data.Trim();
                var args = data.Split(',');
                if (!data.StartsWith('(') || !data.EndsWith(')') || args.Length != 3)
                    throw new Exception($"Vector3Int should be written in (x, y, z) format but get {data}");
                return new Vector3(int.Parse(args[0][1..]), int.Parse(args[1]), int.Parse(args[2][..^1]));
            }
            
            // Enum
            if (type.IsEnum)
            {
                if (string.IsNullOrEmpty(data))
                    return Enum.GetValues(type).GetValue(0);
                if (Enum.TryParse(type, data, out var result))
                    return result;
                throw new Exception($"Failed to convert {data} to enum type {type}");
            }

            // Array
            if (type.IsArray)
            {
                var elementType = type.GetElementType()!;
                var depth = 0;
                var beginIndex = 1;
                if (data == "[]" || string.IsNullOrEmpty(data)) return Array.CreateInstance(elementType, 0);
                var list = new List<object>();
                for (var i = 0; i < data.Length; ++i)
                {
                    if (data[i] == '[')
                        ++depth;
                    if (data[i] == ',' || data[i] == ']' && depth == 1)
                    {
                        var subData = data.Substring(beginIndex, i - beginIndex);
                        var element = ConvertTo(subData, elementType);
                        list.Add(element);
                        beginIndex = i + 1;
                    }
                    if (data[i] == ']')
                        --depth;
                    
                    if (depth <= 0) break;
                }

                var array = Array.CreateInstance(elementType, list.Count);
                for (var i = 0; i < list.Count; ++i)
                    array.SetValue(list[i], i);
                return array;
            }
            return string.IsNullOrEmpty(data) ? 
                Activator.CreateInstance(type) : 
                Convert.ChangeType(data, type);
        }
    }
}