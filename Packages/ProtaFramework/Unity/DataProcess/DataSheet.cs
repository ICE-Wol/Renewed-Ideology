using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DL.FastCsvParser;
using Unity.Collections;
using UnityEngine;

namespace Prota.Unity
{
    
    public unsafe class DataSheet
    {
        public enum DataType : byte
        {
            String = 1,
            Int = 2,
            Float = 3,
            Bool = 4,
            Vec2 = 5,
            Vec3 = 6,
            Color = 7,
        }
        
        public static bool IsValidDataType(string str)
        {
            switch(str)
            {
                case "string":
                case "int":
                case "float":
                case "bool":
                case "vec2":
                case "vec3":
                case "color":
                    return true;
                default:
                    return false;
            }
        }
        
        public DataType ParseDataType(string str, int? i = null, int? j = null)
        {
            switch(str)
            {
                case "string": return DataType.String;
                case "int": return DataType.Int;
                case "float": return DataType.Float;
                case "bool": return DataType.Bool;
                case "vec2": return DataType.Vec2;
                case "vec3": return DataType.Vec3;
                case "color": return DataType.Color;
                default:
                    Debug.LogError($"table[{this.name}] Unknown data type {str} at ({i}, {j})");
                    return DataType.String;
            }
        }
        
        public string DataTypeName(DataType type)
        {
            switch(type)
            {
                case DataType.String: return "string";
                case DataType.Int: return "int";
                case DataType.Float: return "float";
                case DataType.Bool: return "bool";
                case DataType.Vec2: return "vec2";
                case DataType.Vec3: return "vec3";
                case DataType.Color: return "color";
                default:
                    Debug.LogError($"Unknown data type {type}");
                    return "string";
            }
        }
        
        // 每一列一个数组.
        public struct ColumnData
        {
            public readonly string[] strings;
            public readonly int[] ints;
            public readonly float[] floats;
            public readonly bool[] bools;
            public readonly Vector2[] vec2s;
            public readonly Vector3[] vec3s;
            public readonly Color[] colors;
            public ColumnData(DataType type, int size)
            {
                strings = default;
                ints = default;
                floats = default;
                bools = default;
                vec2s = default;
                vec3s = default;
                colors = default;
                switch(type)
                {
                    case DataType.String: strings = new string[size]; break;
                    case DataType.Int: ints = new int[size]; break;
                    case DataType.Float: floats = new float[size]; break;
                    case DataType.Bool: bools = new bool[size]; break;
                    case DataType.Vec2: vec2s = new Vector2[size]; break;
                    case DataType.Vec3: vec3s = new Vector3[size]; break;
                    case DataType.Color: colors = new Color[size]; break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        public readonly struct TypeMap
        {
            public static readonly IReadOnlyList<Type> dataTypeToEngineType;
            public static readonly IReadOnlyDictionary<Type, DataType> engineTypeToDataType;
            static TypeMap()
            {
                var dataTypeToEngineType = new List<Type>();
                var engineTypeToDataType = new Dictionary<Type, DataType>();
                void Add(DataType dataType, Type engineType)
                {
                    dataTypeToEngineType.Add(engineType);
                    engineTypeToDataType[engineType] = dataType;
                }
                Add(DataType.String, typeof(string));
                Add(DataType.Int, typeof(int));
                Add(DataType.Float, typeof(float));
                Add(DataType.Bool, typeof(bool));
                Add(DataType.Vec2, typeof(Vector2));
                Add(DataType.Vec3, typeof(Vector3));
                Add(DataType.Color, typeof(Color));
            }
        }
        
        public readonly struct LineAccessor
        {
            public readonly DataSheet dataSheet;
            public readonly int lineId;
            public string lineName => dataSheet.rowIdToName[lineId];
            public CellAccessor this[int columnId] => new CellAccessor(dataSheet, lineId, columnId);
            public CellAccessor this[string columnName] => new CellAccessor(dataSheet, lineId, dataSheet.columnNameToId[columnName]);
            public LineAccessor(DataSheet dataSheet, int lineId)
            {
                this.dataSheet = dataSheet;
                this.lineId = lineId;
            }
        }
        
        public readonly struct ColumnAccessor
        {
            public readonly DataSheet dataSheet;
            public readonly int columnId;
            public string columnName => dataSheet.columnIdToName[columnId];
            public DataType columnType => dataSheet.columnIdToType[columnId];
            public string columnComment => dataSheet.columnIdToComment[columnId];
            public bool IsType<T>() => TypeMap.dataTypeToEngineType[(int)columnType] == typeof(T);
            public bool IsType(Type type) => TypeMap.dataTypeToEngineType[(int)columnType] == type;
            public ColumnAccessor(DataSheet dataSheet, int columnId)
            {
                this.dataSheet = dataSheet;
                this.columnId = columnId;
            }
        }
        
        public readonly struct CellAccessor
        {
            public readonly DataSheet dataSheet;
            public readonly int lineId;
            public readonly int columnId;
            public LineAccessor line => new LineAccessor(dataSheet, lineId);
            public ColumnAccessor column => new ColumnAccessor(dataSheet, columnId);
            public bool TryGetValue(ref string value)
            {
                if(!column.IsType(typeof(string))) return false;
                return dataSheet.RawGet(lineId, columnId, ref value);
            }
            public bool TryGetValue(ref int value)
            {
                if(!column.IsType(typeof(int))) return false;
                return dataSheet.RawGet(lineId, columnId, ref value);
            }
            public bool TryGetValue(ref float value)
            {
                if(!column.IsType(typeof(float))) return false;
                return dataSheet.RawGet(lineId, columnId, ref value);
            }
            public bool TryGetValue(ref bool value)
            {
                if(!column.IsType(typeof(bool))) return false;
                return dataSheet.RawGet(lineId, columnId, ref value);
            }
            public bool TryGetValue(ref Vector2 value)
            {
                if(!column.IsType(typeof(Vector2))) return false;
                return dataSheet.RawGet(lineId, columnId, ref value);
            }
            public bool TryGetValue(ref Vector3 value)
            {
                if(!column.IsType(typeof(Vector3))) return false;
                return dataSheet.RawGet(lineId, columnId, ref value);
            }
            public bool TryGetValue(ref Color value)
            {
                if(!column.IsType(typeof(Color))) return false;
                return dataSheet.RawGet(lineId, columnId, ref value);
            }
            public CellAccessor(DataSheet dataSheet, int lineId, int columnId)
            {
                this.dataSheet = dataSheet;
                this.lineId = lineId;
                this.columnId = columnId;
            }
        }
        
        // =================================================================================================
        // =================================================================================================
        
        public string name;
        public readonly IReadOnlyDictionary<string, int> rowNameToId;
        public readonly IReadOnlyDictionary<string, int> columnNameToId;
        public int columnCount { get; private set; }
        public int lineCount { get; private set; }
        
        public readonly IReadOnlyList<string> rowIdToName;
        public readonly IReadOnlyList<string> columnIdToName;
        public readonly IReadOnlyList<DataType> columnIdToType;
        public readonly IReadOnlyList<string> columnIdToComment;
        
        public readonly string content;
        public ColumnData[] data;
        
        public static DataSheet FromFile(string csvFileName)
        {
            return FromString(csvFileName, File.ReadAllText(csvFileName));
        }
        
        public static DataSheet FromString(string name, string csvString)
        {
            return new DataSheet(name, Csv.Parse(csvString));
        }
        
        DataSheet(string name, Csv parser)
        {
            parser.RemoveAll(x => x == null || x.Count == 0);
            this.name = name;
            lineCount = parser.Count - 3;    // 三行分别是名称, 注释, 类型.
            columnCount = parser.Count == 0 ? 0 : parser.Max(x => x.Count);
            
            var rowNameToId = new Dictionary<string, int>();
            var columnNameToId = new Dictionary<string, int>();
            var rowIdToName = new string[lineCount];
            var columnIdToName = new string[columnCount];
            var columnIdToType = new DataType[columnCount];
            var columnIdToComment = new string[columnCount];
            var rawDataOffset = new int[lineCount, columnCount];
            
            // ================================
            // 列名, 类型和注释.
            for(int i = 0; i < columnCount; i++)
            {
                columnIdToName[i] = parser[0][i].Trim();
                columnIdToType[i] = ParseDataType(parser[1][i].Trim(), 1, i);
                columnIdToComment[i] = parser[2][i].Trim();
                columnNameToId[columnIdToName[i]] = i;
            }
            this.columnNameToId = columnNameToId;
            this.columnIdToName = columnIdToName;
            this.columnIdToType = columnIdToType;
            this.columnIdToComment = columnIdToComment;
            
            // ================================
            // 行名.p
            for(int i = 0; i < lineCount; i++)
            {
                rowIdToName[i] = parser[i + 3][0].Trim();
                rowNameToId[rowIdToName[i]] = i;
            }
            this.rowNameToId = rowNameToId;
            this.rowIdToName = rowIdToName;
            
            // ================================
            // 数据.
            data = Enumerable.Range(0, columnCount)
                .Select(i => new ColumnData(columnIdToType[i], lineCount))
                .ToArray();
            
            for(int j = 0; j < columnCount; j++)
            {
                var column = data[j];
                var type = columnIdToType[j];
                for(int i = 0; i < lineCount; i++)
                {
                    var str = parser[i + 3][j].Trim();
                    switch(type)
                    {
                        case DataType.String:
                        {
                            column.strings[i] = str;
                            break;
                        }
                        case DataType.Int:
                        {
                            if(int.TryParse(str, out var value))
                            {
                                column.ints[i] = value;
                                break;
                            }
                            
                            Debug.LogError($"Table {name} Failed to parse int at ({i}, {j})");
                            continue;
                        }
                        case DataType.Float:
                        {
                            if(float.TryParse(str, out var value))
                            {
                                column.floats[i] = value;
                                break;
                            }
                            Debug.LogError($"Table {name} Failed to parse float at ({i}, {j})");
                            continue;
                        }
                        case DataType.Bool:
                        {
                            if(bool.TryParse(str, out var value))
                            {
                                column.bools[i] = value;
                                break;
                            }
                            if(int.TryParse(str, out var intValue))
                            {
                                column.bools[i] = intValue != 0;
                                break;
                            }
                            Debug.LogError($"Table {name} Failed to parse bool at ({i}, {j})");
                            break;
                        }
                        case DataType.Vec2:
                        {
                            if(!str.TryParseToVector2(out var value))
                            {
                                Debug.LogError($"Table {name} Failed to parse Vector2 at ({i}, {j})");
                                continue;
                            }
                            column.vec2s[i] = value;
                            break;
                        }
                        case DataType.Vec3:
                        {
                            if(!str.TryParseToVector3(out var value))
                            {
                                Debug.LogError($"Table {name} Failed to parse Vector3 at ({i}, {j})");
                                continue;
                            }
                            column.vec3s[i] = value;
                            break;
                        }
                        case DataType.Color:
                        {
                            if(!str.TryParseToColor(out var value))
                            {
                                Debug.LogError($"Table {name} Failed to parse Color at ({i}, {j})");
                                continue;
                            }
                            column.colors[i] = value;
                            break;
                        }
                        default:
                        {
                            Debug.LogError($"Unknown data type at ({i}, {j}), type is {type}");
                            break;
                        }
                    }
                }
            }
        }
        
        
        
        public bool RawGet(int lineId, int columnId, ref string value)
        {
            value = data[columnId].strings[lineId];
            return true;
        }
        
        public bool RawGet(int lineId, int columnId, ref int value)
        {
            value = data[columnId].ints[lineId];
            return true;
        }
        
        public bool RawGet(int lineId, int columnId, ref float value)
        {
            value = data[columnId].floats[lineId];
            return true;
        }
        
        public bool RawGet(int lineId, int columnId, ref bool value)
        {
            value = data[columnId].bools[lineId];
            return true;
        }
        
        public bool RawGet(int lineId, int columnId, ref Vector2 value)
        {
            value = data[columnId].vec2s[lineId];
            return true;
        }
        
        public bool RawGet(int lineId, int columnId, ref Vector3 value)
        {
            value = data[columnId].vec3s[lineId];
            return true;
        }
        
        public bool RawGet(int lineId, int columnId, ref Color value)
        {
            value = data[columnId].colors[lineId];
            return true;
        }
        
        
    }
    
    
}

