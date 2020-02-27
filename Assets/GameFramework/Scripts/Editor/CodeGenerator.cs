using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AzurGames.GameServer.StatesSchema;
using Common.Utils.Logging;
using UnityEditor;
using UnityEngine;

namespace AzurGames.GameServer.CodeGenerator
{
    public static class Codegen
    {
        private static void SerializeField(FieldInfo field, StreamWriter writer)
        {
            var pair = GetSizeAndBitsCountText(field.FieldType);
                            
            var minmax = (MinMaxValue)field.GetCustomAttributes().FirstOrDefault(el => el.GetType() == typeof(MinMaxValue));
            var compression = (Compression)field.GetCustomAttributes().FirstOrDefault(el => el.GetType() == typeof(Compression));
            if (minmax != null)
            {
                var range = minmax.MaxValue - minmax.MinValue;
                var bitsCount = GetBitsCount(range);
                writer.Write($"packer.Write{pair.Key}({field.Name}, {bitsCount});\n");
            }
            else if (compression != null)
            {
                var writeString = "";
                if(compression.CompressionRate == Compression.Rate.ToShort)
                {
                    writeString = "Short";
                }
                else if(compression.CompressionRate == Compression.Rate.ToByte)
                {
                    writeString = "Byte";
                }
                                
                writer.Write($"var {field.Name}Compressed = FloatCompression.FloatTo{writeString}({field.Name});\n");
                writer.Write($"packer.Write{writeString}({field.Name}Compressed);\n");
            }
            else
            {
                writer.Write($"packer.Write{pair.Key}({(field.FieldType.IsEnum ? $"({pair.Key.ToString().ToLower()})" : "")}{field.Name}{(string.IsNullOrEmpty(pair.Value) ? "" : $", {pair.Value}")});\n");
            }
        }
        
        private static void DeserializeField(FieldInfo field, StreamWriter writer)
        {
            var pair = GetSizeAndBitsCountText(field.FieldType);
                            
            var minmax = (MinMaxValue)field.GetCustomAttributes().FirstOrDefault(el => el.GetType() == typeof(MinMaxValue));
            var compression = (Compression)field.GetCustomAttributes().FirstOrDefault(el => el.GetType() == typeof(Compression));
            if (minmax != null)
            {
                var range = minmax.MaxValue - minmax.MinValue;
                var bitsCount = GetBitsCount(range);
                writer.Write($"{field.Name} = packer.Read{pair.Key}({bitsCount});\n");
            }
            else if (compression != null)
            {
                var writeString = "";
                if(compression.CompressionRate == Compression.Rate.ToShort)
                {
                    writeString = "Short";
                }
                else if(compression.CompressionRate == Compression.Rate.ToByte)
                {
                    writeString = "Byte";
                }
                                                                
                writer.Write($"var {field.Name}Compressed = packer.Read{writeString}();\n");
                writer.Write($"{field.Name} = FloatCompression.FloatFrom{writeString}({field.Name}Compressed);\n");
            }
            else
            {
                writer.Write($"{field.Name} = {(field.FieldType.IsEnum ? $"({field.FieldType})" : "")}packer.Read{pair.Key}({(string.IsNullOrEmpty(pair.Value) ? "" : $"{pair.Value}")});\n");
            }
        }
        
        public static int GetBitsCount(long num)
        {
            var bits = 0;
            while (num > 0)
            {
                num = num / 2;
                bits++;
            }

            return bits;
        }
        
        public static void ToExactString (double d)
        {
            // Translate the double into sign, exponent and mantissa.
            long bits = BitConverter.DoubleToInt64Bits(d);
            // Note that the shift is sign-extended, hence the test against -1 not 1
            bool negative = (bits & (1L << 63)) != 0;
            int exponent = (int) ((bits >> 52) & 0x7ffL);
            long mantissa = bits & 0xfffffffffffffL;

            // Subnormal numbers; exponent is effectively one higher,
            // but there's no extra normalisation bit in the mantissa
            if (exponent==0)
            {
                exponent++;
            }
            // Normal numbers; leave exponent as it is but add extra
            // bit to the front of the mantissa
            else
            {
                mantissa = mantissa | (1L << 52);
            }

            // Bias the exponent. It's actually biased by 1023, but we're
            // treating the mantissa as m.0 rather than 0.m, so we need
            // to subtract another 52 from it.
            exponent -= 1075;

            if (mantissa == 0)
            {
                exponent = 0;
            }
            else
            {
                while((mantissa & 1) == 0) 
                {    /*  i.e., Mantissa is even */
                    mantissa >>= 1;
                    exponent++;
                }
            }

            Debug.Log($"{d.ToString()} Negative{negative}|Exponent{exponent}|mantissa{mantissa}");
        }
        
        public static List<Type> FindAllDerivedTypes<T>()
        {
            return FindAllDerivedTypes<T>(Assembly.GetAssembly(typeof(T)));
        }

        public static List<Type> FindAllDerivedTypes<T>(Assembly assembly)
        {
            var derivedType = typeof(T);
            return assembly
                .GetTypes()
                .Where(t =>
                    t != derivedType &&
                    derivedType.IsAssignableFrom(t)
                ).ToList();
        }

        [MenuItem("CodeGenerator/Clear")]
        public static void CleanGenerated()
        {
            string genFolder = Application.dataPath + "/Generated";
            if(Directory.Exists(genFolder))
            {
                Directory.Delete(genFolder, true);
            }
            AssetDatabase.Refresh();
        }
        
        private static KeyValuePair<string,string> GetSizeAndBitsCountText(System.Type typeToUse)
        {
            if (typeToUse == typeof(int))
            {
                return new KeyValuePair<string, string>("Int", (sizeof(int) * 8).ToString());
            }
            
            if (typeToUse == typeof(uint))
            {
                return new KeyValuePair<string, string>("UInt", (sizeof(int) * 8).ToString());
            }
            
            if (typeToUse == typeof(byte))
            {
                return new KeyValuePair<string, string>("Byte", (sizeof(byte) * 8).ToString());
            }
            
            if (typeToUse == typeof(bool))
            {
                return new KeyValuePair<string, string>("Bool", "");
            }
            
            if (typeToUse == typeof(float))
            {
                return new KeyValuePair<string, string>("Float", "");
            }
            
            if (typeToUse == typeof(short))
            {
                return new KeyValuePair<string, string>("Short", (sizeof(short) * 8).ToString());
            }
            
            if (typeToUse == typeof(ushort))
            {
                return new KeyValuePair<string, string>("UShort", (sizeof(ushort) * 8).ToString());
            }
            
            if (typeToUse == typeof(ulong))
            {
                return new KeyValuePair<string, string>("ULong", (sizeof(ulong) * 8).ToString());
            }
            
            if (typeToUse == typeof(long))
            {
                return new KeyValuePair<string, string>("Long", (sizeof(long) * 8).ToString());
            }

            if (typeToUse.IsEnum)
            {
                var result = GetSizeAndBitsCountText(Enum.GetUnderlyingType(typeToUse));
                var vals = Enum.GetValues(typeToUse);

                var maxVal = int.MinValue;
                foreach (var val in vals)
                {
                    if (maxVal < (int) val)
                    {
                        maxVal = (int) val;
                    }
                }
                return new KeyValuePair<string, string>(result.Key, GetBitsCount(maxVal).ToString());
            }
            
            throw new TypeAccessException($"Unsupported type in state {typeToUse.Name}");
        }

        private static void GenerateGameState(string genFolder)
        {
            HashSet<Type> states = new HashSet<Type>(); 
            FindAllDerivedTypes<State>().ForEach(el => states.Add(el));
            FindAllDerivedTypes<AbilityState>().ForEach(el => states.Add(el));
            states.RemoveWhere(st => st.IsAbstract);
            
            ISimpleLogger logger = new ConsoleLogger();
            bool failed = false;
            foreach (var state in states)
            {
                if (!state.IsSubclassOf(state))
                {
                    logger?.Error("State types should derive from State class:" + state.FullName);
                    failed = true;
                }
            }
            
            string result = genFolder + "/GameStateGenerated.cs";
            if (File.Exists(result))
            {
                File.Delete(result);
            }
            
            var writer = File.CreateText(result);

            writer.Write($"using System.Collections.Generic;\n");
            writer.Write($"using Common.Utils.Serialization;\n");
            writer.Write($"using AzurGames.GameServer.StatesSchema;\n");
            var namespaceName = "AzurGames.GameServer.Generated";
            writer.Write($"namespace {namespaceName} \n{{\n");
            {
                writer.Write($"public partial class GameState \n{{\n");
                foreach (var state in states)
                {
                    writer.Write(
                        $"public Dictionary<int,{state.Name}Struct> {state.Name}List = new Dictionary<int,{state.Name}Struct>();\n");
                }

                {
                    writer.Write($"public void Deser(ISerializer packer)\n{{\n");
                    foreach (var state in states)
                    {
                        writer.Write($"var {state.Name}count = packer.ReadInt();\n");
                        writer.Write($"for(int i = 0; i < {state.Name}count; i++)\n" +
                                     $"{{\n var id = packer.ReadInt();\n" +
                                     $"var deserdStruct = new {state.Name}Struct();\n" +
                                     $"deserdStruct.Deser(packer);\n" +
                                     $"{state.Name}List[id] = deserdStruct;\n}}\n");
                    }

                    writer.Write($"}}\n");
                }
                
                {
                    writer.Write($"public void Ser(ISerializer packer)\n{{\n");
                    foreach (var state in states)
                    {
                        writer.Write($"packer.WriteInt({state.Name}List.Count);\n");
                        writer.Write($"foreach(var st in {state.Name}List)\n" +
                                     $"{{\npacker.WriteInt(st.Key);\n" +
                                     $"st.Value.Ser(packer);\n}}\n");
                    }
                    
                    writer.Write($"}}\n");
                }
                
                writer.Write($"}}\n");

                foreach (var state in states)
                {
                    writer.Write($"public partial struct {state.Name}Struct \n{{\n");
                    var publicFields = state.GetFields().Where(f => f.IsPublic).ToList();
                    var crossRefFields = publicFields.Where(f =>
                        f.GetCustomAttributes().Any(el => el.GetType() == typeof(CrossReference)));
                    foreach (var field in publicFields)
                    {
                        writer.Write($"public {field.FieldType} {field.Name};\n");
                    }
                    {
                        writer.Write($"public void Deser(ISerializer packer)\n{{\n");
                        foreach (var field in publicFields)
                        {
                            DeserializeField(field, writer);
                        }
                        
                        writer.Write($"}}\n");
                    }
                    {
                        writer.Write($"public void Ser(ISerializer packer)\n{{\n");
                        
                        foreach (var field in publicFields)
                        {
                            SerializeField(field, writer);
                        }
                        
                        writer.Write($"}}\n");
                    }
                    
                    writer.Write($"}}\n");
                }

                writer.Write($"public static class GameStateExtensions \n{{\n");
                foreach (var state in states)
                {
                    var crossRefFields = state.GetFields().Where(f => f.IsPublic).Where(f =>
                        f.GetCustomAttributes().Any(el => el.GetType() == typeof(CrossReference)));
                    //Extensions to get cross references
                    foreach (var cr in crossRefFields)
                    {
                        var crossRef =
                            (CrossReference) cr.GetCustomAttributes()
                                .First(el => el.GetType() == typeof(CrossReference));
                        writer.Write(
                            $"public static {crossRef.ReferencedType.Name}Struct Get{crossRef.ReferencedType.Name}ByRef(this ref {state.Name}Struct referrer, GameState state)\n{{\n");
                        writer.Write($"return state.{crossRef.ReferencedType.Name}List[referrer.{cr.Name}];\n");
                        writer.Write($"}}\n");
                    }
                }

                writer.Write($"}}\n");
            }
            writer.Write($"}}");
            writer.Flush();
            writer.Close();
        }
        
        [MenuItem("CodeGenerator/Generate")]
        public static void GenerateCode()
        {
            string genFolder = Application.dataPath + "/Generated";
            if(!Directory.Exists(genFolder))
            {
                Directory.CreateDirectory(genFolder);
            }
            
            GenerateGameState(genFolder);
            GenerateInputState(genFolder);
            GenerateDataStorage(genFolder);
            AssetDatabase.Refresh();
        }

        private static void GenerateDataStorage(string genFolder)
        {
            string result = genFolder + "/DataStorage.cs";
            if (File.Exists(result))
            {
                File.Delete(result); 
            }
            var writer = File.CreateText(result);

            writer.Write($"using System.Collections.Generic;\n");
            writer.Write($"using Common.Utils.Serialization;\n");
            writer.Write($"using AzurGames.GameServer.StatesSchema;\n");
            var namespaceName = "AzurGames.GameServer.Generated";
            writer.Write($"namespace {namespaceName} \n{{\n");
            {
                writer.Write($"public partial class DataStorage \n{{\n");
                {
                    {
                        writer.Write($"public partial class PlayerNetworkData \n{{\n");
                        writer.Write($"public int NetworkPlayerId;\n");
                        writer.Write($"public Sampler<InputStateStruct> InputSampler = new Sampler<InputStateStruct>();\n");
                        writer.Write($"}}\n");
                    }
                    writer.Write($"public Sampler<GameState> GameStateSampler = new Sampler<GameState>();\n");
                    writer.Write($"public Dictionary<int, PlayerNetworkData> PlayersData = new Dictionary<int, PlayerNetworkData>();\n");
                }
                writer.Write($"}}\n");
            }
            
            writer.Write($"}}\n");
            writer.Flush();
            writer.Close();
        }

        private static void GenerateInputState(string genFolder)
        {
            HashSet<Type> inputs = new HashSet<Type>(); 
            FindAllDerivedTypes<InputState>().ForEach(el => inputs.Add(el));
            inputs.RemoveWhere(st => st.IsAbstract);
            
            ISimpleLogger logger = new ConsoleLogger();
            bool failed = false;
            foreach (var state in inputs)
            {
                if (!state.IsSubclassOf(state))
                {
                    logger?.Error("State types should derive from State class:" + state.FullName);
                    failed = true;
                }
            }

            if (inputs.Count > 1)
            {
                throw new Exception("Multiple inputs unsupported");
            }
            
            if (inputs.Count == 0)
            {
                throw new Exception("Declare input schema derived from InputState unsupported");
            }
            
            string result = genFolder + "/InputStateGenerated.cs";
            if (File.Exists(result))
            {
                File.Delete(result); 
            }
            
            var writer = File.CreateText(result);

            writer.Write($"using System.Collections.Generic;\n");
            writer.Write($"using Common.Utils.Serialization;\n");
            writer.Write($"using AzurGames.GameServer.StatesSchema;\n");
            var namespaceName = "AzurGames.GameServer.Generated";
            var inputSchema = inputs.First();
            writer.Write($"namespace {namespaceName} \n{{\n");
            {
                writer.Write($"public partial class InputStateStruct \n{{\n");
                
                //fields
                var publicFields = inputSchema.GetFields().Where(f => f.IsPublic).ToList();
                var crossRefFields = publicFields.Where(f =>
                    f.GetCustomAttributes().Any(el => el.GetType() == typeof(CrossReference)));
                
                foreach (var field in publicFields)
                {
                    writer.Write($"public {field.FieldType} {field.Name};\n");
                }

                {
                    writer.Write($"public void Ser(ISerializer packer)\n{{\n");
                    foreach (var field in publicFields)
                    {
                        SerializeField(field, writer);
                    }

                    writer.Write($"}}\n");
                }
                
                //serd

                {
                    writer.Write($"public void Deser(ISerializer packer)\n{{\n");
                    foreach (var field in publicFields)
                    {
                        DeserializeField(field, writer);
                    }

                    writer.Write($"}}\n");
                }
                
                //deserd
                
                writer.Write($"}}\n");
            }
            writer.Write($"}}\n");
            writer.Flush();
            writer.Close();
        }
    }
}