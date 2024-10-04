using System.Collections.Generic;
using System.Text;
using FirstGearGames.Roslyn.Extensions;
using FirstGearGames.Roslyn.FishNet;
using FirstGearGames.Roslyn.FishNet.CodeBuilding;
using FirstGearGames.Roslyn.FishNet.Constants;
using FirstGearGames.Roslyn.FishNet.Helpers;
using FirstGearGames.Roslyn.FishNet.Receivers;
using FirstGearGames.Roslyn.FishNet.Serializing;
using FirstGearGames.Roslyn.Native.Constants;
using Microsoft.CodeAnalysis;
using RoslynCodeBuilder = FirstGearGames.Roslyn.CodeBuilding.CodeBuilder;

namespace Roslyn.FishNet.CodeBuilding
{
    public class DeltaWriter_Builder
    {
        public const string InitializeOnLoad_Method_Name = Writer_Builder.InitializeOnLoad_Method_Name;
        public const string Generated_DeltaSerializerOption_Name = "options";
        private const string Generated_Class_Name = "Generated_DeltaWriters";
        private const string Generated_Method_Prefix = $"{FishNetConstants.GeneratedWriterPrefix}WriteDelta";
        private const string Generated_WriterParameter_Name = Writer_Builder.Generated_WriterParameter_Name;

        private static StringBuilder _stringBuilder = new();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Serializers _serializers => _generator.Serializers;
        private SerializableGenerator _generator;
        private GeneratorExecutionContext _context;
        private GeneratorSyntaxReceiver _rootSyntaxReceiver;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        
        public void Initialize(GeneratorExecutionContext context, GeneratorSyntaxReceiver rootSyntaxReceiver, SerializableGenerator generator)
        {
            _context = context;
            _rootSyntaxReceiver = rootSyntaxReceiver;
            _generator = generator;
        }

        public void Execute()
        {
            //Create all stub(empty) delta methods.
            CreateEmptySerializerMethods(_context, _rootSyntaxReceiver);
            //Create all bodies for delta methods.
            CreateDeltaSerializerBodies(_context, _rootSyntaxReceiver);
            //Create delta serializers class adding generated serializers.
            CreateGeneratedDeltaSerializersClass(_context);
        }

        /// <summary>
        /// Creates SerializerMethod for each type in need.
        /// </summary>
        private void CreateEmptySerializerMethods(GeneratorExecutionContext context, GeneratorSyntaxReceiver syntaxReceiver)
        {
            foreach (SerializableType item in syntaxReceiver.SerializableReceiver.SerializableTypes)
                CreateEmptyDeltaSerializerMethod(context, item);
        }

        /// <summary>
        /// Creates an empty delta serializer method for a type.
        /// </summary>
        private void CreateEmptyDeltaSerializerMethod(GeneratorExecutionContext context, SerializableType serializableType, int recursiveCount = 1)
        {
            string typeFullName = serializableType.FullName;
            Debugg.Log($"{recursiveCount.ToIndent()}Trying to create writer for {typeFullName}.");
            //Already exist either in FishNet or already created.
            if (_serializers.GetWriteMethod(typeFullName, GetSerializerType.Delta).IsValid())
            {
                Debugg.Log($"{recursiveCount.ToIndent()}   Serializer already exists.");
                return;
            }
            Debugg.Log($"{recursiveCount.ToIndent()}    A   {serializableType.FullMetadataName}");
            //Debugg.Log("Meta name is " + serializableType.FullMetadataName);
            INamedTypeSymbol? namedTypeSymbol = context.Compilation.GetTypeByMetadataName(serializableType.FullMetadataName);
            if (namedTypeSymbol == null)
            {
                Debugg.Log($"{recursiveCount.ToIndent()}   Named symbol is null.");
                return;
            }
            Debugg.Log($"{recursiveCount.ToIndent()}    B");
            if (!_serializers.CanCreateDeltaSerializer(namedTypeSymbol))
                return;
            Debugg.Log($"{recursiveCount.ToIndent()}    C");
            List<IFieldSymbol> serializableFields = _serializers.GetSerializableFieldSymbols(namedTypeSymbol);
            foreach (IFieldSymbol item in serializableFields)
            {
                ITypeSymbol typeSymbol = item.Type;
                CreateEmptyDeltaSerializerMethod(context, new SerializableType(typeSymbol), recursiveCount + 1);
            }

            Debugg.Log($"{recursiveCount.ToIndent()}    Creating header and footer.");
            string header = GetMethodHeader(out string methodName);
            //Add to writers.

            _serializers.AddWriteMethod(new GeneratedDeltaSerializerMethod(namedTypeSymbol, methodName, header, string.Empty), AddSerializerType.Delta);
            Debugg.Log($"{recursiveCount.ToIndent()}Added for type {typeFullName}.");

            string GetMethodHeader(out string mName)
            {
                mName = $"{Generated_Method_Prefix}{typeFullName.RemovePeriods("_")}";
                StringBuilder sb = new();
                string inText = namedTypeSymbol.IsUserDefinedStruct() ? "in " : string.Empty;
                inText = "";
                sb.Append(2, $"public static bool {mName}" + $"(this {FishNetConstants.Writer_FullName} {Generated_WriterParameter_Name}," + $" {inText}{typeFullName} {_serializers.GetValueParameterName(0)}, {inText}{typeFullName} {_serializers.GetValueParameterName(1)}" + $", {FishNetConstants.DeltaSerializerOption_FullName} {Generated_DeltaSerializerOption_Name} = {FishNetConstants.DeltaSerializerOption_Unset_FullName})");

                return sb.ToString();
            }
        }

        /// <summary>
        /// Creates and returns a full serialize call.
        /// </summary>
        internal string CreateFullSerializeCheck(int indent, string optionsVariableName, string body)
        {
            _stringBuilder.Clear();

            _stringBuilder.AppendLine(indent, $"if ({optionsVariableName}.{FishNetConstants.FastContains_Name}({FishNetConstants.DeltaSerializerOption_FullSerialize_FullName}))");
            _stringBuilder.AppendLine(indent, "{");
            /* ulong flags = (ulong)options;
             * writer.WriteUnsignedPackedWhole(flags); */
            string flagsVariable = "optionsFlags";
            _stringBuilder.Append(indent + 1, RoslynCodeBuilder.CreateLocalVariable(NativeConstants.UInt64_FullName, flagsVariable, string.Empty, false));
            _stringBuilder.AppendLine($" = ({NativeConstants.UInt64_FullName}){Generated_DeltaSerializerOption_Name};");
            _stringBuilder.AppendLine(indent + 1, RoslynCodeBuilder.CallMethod(FishNetConstants.Writer_WriteUnsignedPackedWhole_Name, Generated_WriterParameter_Name, true, flagsVariable));
            _stringBuilder.AppendLine(body);
            _stringBuilder.AppendLine(indent, "}");

            return _stringBuilder.ToString();
        }

        /// <summary>
        /// Creates bodies for empty delta serializer methods.
        /// </summary>
        private void CreateDeltaSerializerBodies(GeneratorExecutionContext context, GeneratorSyntaxReceiver SyntaxReceiver)
        {
            //Iterate all serializers and if they are generated delta writers then complete them.
            foreach (KeyValuePair<string, SerializerMethod> item in _serializers.GetWriteDeltaMethods())
            {
                //Skip built in serializers.
                if (!item.Value.IsValid() || item.Value is not GeneratedDeltaSerializerMethod gsm)
                    continue;

                const int bodyIndent = 3;
                StringBuilder sb = new();

                /* If (options.FastContains(DeltaSerializerOption.FullSerialize))
                 * {
                 *      writer.Write(type);
                 *      return;
                 * } */
                CreateWriteFullIf();

                void CreateWriteFullIf()
                {
                    SerializerMethod sm = GetFullWriter(item.Key, out bool serializerFound);
                    if (!serializerFound)
                        sm = _generator.WriterBuilder.CreateSerializerMethod(item.Value.TypeSymbol);
                    
                    StringBuilder ifBody = new();

                    ifBody.AppendLine(bodyIndent + 1, $"{Generated_WriterParameter_Name}.{sm.MethodName}({_serializers.GetValueParameterName(1)});");
                    ifBody.Append(bodyIndent + 1, "return true;");
                    sb.AppendLine(CreateFullSerializeCheck(bodyIndent, Generated_DeltaSerializerOption_Name, ifBody.ToString()));
                }

                //Starting flag for each modified field.
                ulong fieldFlag = (FishNetConstants.DeltaSerializerOption_MaxValue * 2);

                //totalFlags and pooledWriter local variables.
                string totalFlagsVariable = "totalFlags";
                sb.AppendLine(bodyIndent, RoslynCodeBuilder.CreateLocalVariable(NativeConstants.UInt64_FullName, totalFlagsVariable, $"(ulong){Generated_DeltaSerializerOption_Name}"));
                sb.AppendLine(bodyIndent, CodeBuilder.CallGetPooledWriter(out string tmpWriterVariable) + NativeConstants.LineFeed);

                List<IFieldSymbol> serializableFieldSymbols = _serializers.GetSerializableFieldSymbols(gsm.TypeSymbol);

                //Call write for all members.
                foreach (IFieldSymbol fieldSymbol in serializableFieldSymbols)
                {
                    ITypeSymbol typeSymbol = fieldSymbol.Type;
                    string typeFullName = typeSymbol.GetTypeSymbolFullName(metadataName: false);

                    //Get delta writer method for the field.
                    DeltaSerializerMethod? dsm = _serializers.GetWriteMethod(typeFullName, GetSerializerType.Delta) as DeltaSerializerMethod;
                    //Delta writer not found.
                    if (!dsm.IsValid())
                    {
                        SerializerMethod sm = GetFullWriter(typeFullName, out bool serializerFound);
                        if (!serializerFound)
                            sm = _generator.WriterBuilder.CreateSerializerMethod(typeSymbol);

                        sb.AppendLine(bodyIndent, $"//Delta writer could not be found for type {typeFullName}. Please report this note.");
                        
                        string genericArguments = RoslynCodeBuilder.GetCombinedGenericArguments(typeSymbol.GetGenericArgumentsString());
                        sb.AppendLine(bodyIndent, RoslynCodeBuilder.CallMethod($"{sm.MethodName}{genericArguments}", tmpWriterVariable, true, $"{_serializers.GetValueParameterName(1)}.{fieldSymbol.Name}"));
                        AppendIncreaseTotalFlags(3);
                    }
                    //Delta writer found.
                    else
                    {
                        //If a user defined struct then use the in keyword.
                        string inText = typeSymbol.IsUserDefinedStruct() ? "in " : string.Empty;
                        inText = "";
                        /* if (writer.WriteDeltaXYZ(p0, p1))
                            totalFlags += x */

                        string writeDeltaText;

                        //TODO get working to call method directly once delta serializers are fully supported.
                        if (dsm.IsGenerated())
                        {
                            string genericArguments = RoslynCodeBuilder.GetCombinedGenericArguments(typeSymbol.GetGenericArgumentsString());
                            writeDeltaText = RoslynCodeBuilder.CallMethod($"WriteDelta{genericArguments}", tmpWriterVariable, false, $"{inText}{_serializers.GetValueParameterName(0)}.{fieldSymbol.Name}", $"{inText}{_serializers.GetValueParameterName(1)}.{fieldSymbol.Name}", $"{FishNetConstants.DeltaSerializerOption_Unset_FullName}");
                        }
                        else
                        {
                            string genericArguments = RoslynCodeBuilder.GetCombinedGenericArguments(typeSymbol.GetGenericArgumentsString());
                            writeDeltaText = RoslynCodeBuilder.CallMethod($"{dsm.MethodName}{genericArguments}", tmpWriterVariable, false, $"{inText}{_serializers.GetValueParameterName(0)}.{fieldSymbol.Name}", $"{inText}{_serializers.GetValueParameterName(1)}.{fieldSymbol.Name}");
                        }
                        // ifBody.AppendLine(bodyIndent + 1, $"{Generated_WriterParameter_Name}.WriteDelta<{fullSerializerMethod.TypeFullName}>({_serializers.GetValueParameterName(1)});");

                        sb.AppendLine(bodyIndent, $"if ({writeDeltaText})");
                        AppendIncreaseTotalFlags(4);
                    }
                    sb.AppendLine();

                    void AppendIncreaseTotalFlags(int indent)
                    {
                        sb.AppendLine(indent, $"{totalFlagsVariable} += {fieldFlag};");
                    }

                    fieldFlag *= 2;
                }

                string changedVariable = "changed";
                //System.Boolean changed = (totalFlags != 0) || rootSerializer;
                sb.AppendLine(bodyIndent, $"{NativeConstants.Boolean_FullName} {changedVariable} = ({totalFlagsVariable} != 0);");

                /* if (changed)
                 {
                    writer.WritePackedWhole(totalFlags); */
                sb.AppendLine(bodyIndent, $"if ({changedVariable})");
                sb.AppendLine(bodyIndent, "{");
                sb.AppendLine(bodyIndent + 1, RoslynCodeBuilder.CallMethod(FishNetConstants.Writer_WriteUnsignedPackedWhole_Name, Generated_WriterParameter_Name, true, totalFlagsVariable));
                /*  writer.WriteBytes(pooledWriter.GetBuffer(), 0, pooledWriter.Length);
                 } */
                sb.AppendLine(bodyIndent + 1, CodeBuilder.CallWriteArraySegment(Generated_WriterParameter_Name, tmpWriterVariable));
                sb.AppendLine(bodyIndent, "}");
                //store tmpWriter.
                sb.AppendLine(bodyIndent, CodeBuilder.CallStorePooledWriter(tmpWriterVariable) + NativeConstants.LineFeed);
                /* Struct/class writers must always return true. This is so if they are being encapsulated
                 * the flags written will be read, even if that flag is 0. */
                sb.Append(bodyIndent, $"return {changedVariable};");

                gsm.MethodContent.Body = sb;
            }
        }

        /// <summary>
        /// Creates a class containing generated delta writers.
        /// </summary>
        private void CreateGeneratedDeltaSerializersClass(GeneratorExecutionContext context)
        {
            StringBuilder sb = new();

            string clsText = RoslynCodeBuilder.CreatePublicStaticClass(Generated_Class_Name, out string footer, FishNetConstants.Serializing_Namespace);
            sb.AppendLine($"//FishNet generated file.");
            sb.AppendLine(clsText);

            const int initializeIndent = 2;
            MethodContent initializeMethod = CodeBuilder.CreatePublicRuntimeInitializeOnLoadMethod(initializeIndent, InitializeOnLoad_Method_Name);

            int addedSerializers = 0;

            foreach (KeyValuePair<string, SerializerMethod> item in _serializers.GetWriteDeltaMethods())
            {
                if (item.Value is not GeneratedDeltaSerializerMethod dsm) continue;

                sb.AppendLine(dsm.MethodContent.ToString(2));

                //Add return if body already contains value. This is just to make neater formatting.
                if (initializeMethod.Body.Length > 0)
                    initializeMethod.Body.AppendLine();

                initializeMethod.Body.Append(initializeIndent + 1, CreateInitializeFunction(dsm));

                addedSerializers++;
            }

            sb.Append(initializeMethod.ToString(initializeIndent));
            sb.AppendLine(footer);

            string fileName = $"{FishNetConstants.Serializing_Namespace}_{Generated_Class_Name}.g.cs";
            context.AddSource($"{fileName}", sb.ToString());

            Debugg.Log($"Added class {fileName}. Added serializer count is {addedSerializers}.");
        }

        /// <summary>
        /// Creates a call to the set method for generic serializers.
        /// </summary>
        private string CreateInitializeFunction(GeneratedDeltaSerializerMethod dsm)
        {
            StringBuilder sb = new();
            //GenericDeltaSerializer<Type>.SetWrite(
            sb.Append($"{FishNetConstants.GenericDeltaWriter_FullName}<{dsm.TypeFullName}>.{FishNetConstants.GenericDeltaWriter_SetWrite_Name}(");
            sb.Append($"{RoslynCodeBuilder.CreateFunction(NativeConstants.Boolean_FullName, FishNetConstants.Writer_FullName, dsm.TypeFullName, dsm.TypeFullName, FishNetConstants.DeltaSerializerOption_FullName)}");
            sb.Append($"({dsm.MethodName}));");

            return sb.ToString();
        }

        /// <summary>
        /// Returns a SerializerMethod for a DefaultWriter of a type. Optionally returns a call to Write<T> if a DefaultWriter is not found.
        /// </summary>
        private SerializerMethod GetFullWriter(string typeFullName,  out bool found)
        {
            SerializerMethod sm = _serializers.GetWriteMethod(typeFullName, GetSerializerType.Full) as SerializerMethod;
            
            found = sm.IsValid();

            return sm;
        }
    }
}