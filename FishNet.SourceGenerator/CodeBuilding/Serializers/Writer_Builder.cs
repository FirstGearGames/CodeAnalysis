using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class Writer_Builder
    {
        public const string Generated_WriterParameter_Name = "writer";
        public const string InitializeOnLoad_Method_Name = "InitializeSerializers";
        private const string Generated_Class_Name = "Generated_Writers";
        private const string Generated_Method_Prefix = $"{FishNetConstants.GeneratedWriterPrefix}Write";

        private static StringBuilder _stringBuilder = new();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Serializers _serializers => _generator.Serializers;
        private SerializableGenerator _generator;
        private GeneratorExecutionContext _context;
        private GeneratorSyntaxReceiver _rootSyntaxReceiver;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void Initialize(GeneratorExecutionContext context, GeneratorSyntaxReceiver rootSyntaxReceiver, SerializableGenerator generator)
        {
            Log("");
            Log("Initialize.");

            _context = context;
            _rootSyntaxReceiver = rootSyntaxReceiver;
            _generator = generator;
        }

        public void Execute()
        {
            //Create all stub(empty) methods.
            CreateEmptySerializerMethods(_context, _rootSyntaxReceiver);
            //Create all bodies for methods.
            CreateSerializerBodies(_context, _rootSyntaxReceiver);
            //Create serializers class adding generated serializers.
            CreateGeneratedSerializersClass(_context);
        }

        /// <summary>
        /// Creates a SerializerMethod for Write<T>.
        /// </summary>
        public SerializerMethod CreateSerializerMethod(ITypeSymbol typeSymbol)
        {
            return new SerializerMethod(typeSymbol, $"{FishNetConstants.Writer_Write_Name}<{typeSymbol.GetTypeSymbolFullName(metadataName: false)}>");
        }

        /// <summary>
        /// Creates SerializerMethod for each type in need.
        /// </summary>
        private void CreateEmptySerializerMethods(GeneratorExecutionContext context, GeneratorSyntaxReceiver syntaxReceiver)
        {
            foreach (SerializableType item in syntaxReceiver.SerializableReceiver.TypesNeedingSerializers)
                CreateEmptySerializerMethod(context, item);
        }

        /// <summary>
        /// Creates an empty delta serializer method for a type.
        /// </summary>
        private void CreateEmptySerializerMethod(GeneratorExecutionContext context, SerializableType serializableType, int recursiveCount = 1)
        {
            string typeFullName = serializableType.FullName;
            Log($"Trying to create empty writer for {typeFullName}.");
            //Already exist either in FishNet or already created.
            if (_serializers.GetWriteMethod(typeFullName, GetSerializerType.Full).IsValid())
                return;

            INamedTypeSymbol? namedTypeSymbol = context.Compilation.GetTypeByMetadataName(serializableType.FullMetadataName);
            if (namedTypeSymbol == null)
                return;

            if (!_serializers.CanCreateSerializer(namedTypeSymbol))
                return;

            List<IFieldSymbol> serializableFields = _serializers.GetSerializableFieldSymbols(namedTypeSymbol);
            foreach (IFieldSymbol item in serializableFields)
            {
                ITypeSymbol typeSymbol = item.Type;
                Log($"  Iterating recursively.");
                CreateEmptySerializerMethod(context, new SerializableType(typeSymbol), recursiveCount + 1);
            }

            //Add to writers.
            string header = GetMethodHeader(out string methodName);
            _serializers.AddWriteMethod(new GeneratedDeltaSerializerMethod(namedTypeSymbol, methodName, header, string.Empty), AddSerializerType.Delta);

            string GetMethodHeader(out string mName)
            {
                Log($"   GetMethodName");
                mName = $"{Generated_Method_Prefix}{typeFullName.RemovePeriods("_")}";
                StringBuilder sb = new();
                string inText = namedTypeSymbol.IsUserDefinedStruct() ? "in " : string.Empty;
                inText = "";
                //public static bool WriteType(Type valueA) { }
                sb.Append(2, $"public static vosssid {mName}" + $"(this {FishNetConstants.Writer_FullName} {Generated_WriterParameter_Name}," + $" {inText}{typeFullName} {_serializers.GetValueParameterName(0)})");

                return sb.ToString();
            }
        }

        /// <summary>
        /// Creates bodies for empty delta serializer methods.
        /// </summary>
        private void CreateSerializerBodies(GeneratorExecutionContext context, GeneratorSyntaxReceiver SyntaxReceiver)
        {
            //Iterate all serializers and if they are generated then complete them.
            foreach (KeyValuePair<string, SerializerMethod> item in _serializers.GetWriteDeltaMethods())
            {
                //Skip built in serializers.
                if (!item.Value.IsValid() || item.Value is not GeneratedDeltaSerializerMethod gsm)
                    continue;

                const int bodyIndent = 3;
                StringBuilder sb = new();

                List<IFieldSymbol> serializableFieldSymbols = _serializers.GetSerializableFieldSymbols(gsm.TypeSymbol);

                //Call write for all members.
                foreach (IFieldSymbol fieldSymbol in serializableFieldSymbols)
                {
                    ITypeSymbol typeSymbol = fieldSymbol.Type;
                    bool isArray = (typeSymbol is IArrayTypeSymbol arrayTypeSymbol);

                    string typeFullName = fieldSymbol.Type.GetTypeSymbolFullName(metadataName: false);

                    Log($"Checking type full name {typeFullName}. IsArray {isArray}");
                    //Get delta writer method for the field.
                    SerializerMethod? sm = _serializers.GetWriteMethod(typeFullName, GetSerializerType.Full) as SerializerMethod;
                    //Writer not found, call Write<T>.
                    if (!sm.IsValid())
                    {
                        sm = CreateSerializerMethod(typeSymbol);

                        sb.AppendLine(bodyIndent, $"//Writer could not be found for type {typeFullName}. Please report this note.");

                        string genericArguments = RoslynCodeBuilder.GetCombinedGenericArguments(typeSymbol.GetGenericArgumentsString(), typeSymbol);
                        sb.AppendLine(bodyIndent, RoslynCodeBuilder.CallMethod($"{sm.MethodName}{genericArguments}", Generated_WriterParameter_Name, true, $"{_serializers.GetValueParameterName(1)}.{fieldSymbol.Name}"));
                    }
                    //writer found.
                    else
                    {
                        string genericArguments = RoslynCodeBuilder.GetCombinedGenericArguments(typeSymbol.GetGenericArgumentsString(), typeSymbol);
                        sb.AppendLine(bodyIndent, RoslynCodeBuilder.CallMethod($"{sm.MethodName}{genericArguments}", Generated_WriterParameter_Name, true, $"{_serializers.GetValueParameterName(1)}.{fieldSymbol.Name}"));
                    }
                }

                gsm.MethodContent.Body = sb;
            }
        }

        /// <summary>
        /// Creates a class containing generated delta writers.
        /// </summary>
        private void CreateGeneratedSerializersClass(GeneratorExecutionContext context)
        {
            StringBuilder sb = new();

            string clsText = RoslynCodeBuilder.CreatePublicStaticClass(Generated_Class_Name, out string footer, FishNetConstants.Serializing_Namespace);
            sb.AppendLine($"//FishNet generated file.");
            sb.AppendLine(clsText);

            const int initializeIndent = 2;
            MethodContent initializeMethod = CodeBuilder.CreatePublicRuntimeInitializeOnLoadMethod(initializeIndent, InitializeOnLoad_Method_Name);

            int addedSerializers = 0;

            foreach (KeyValuePair<string, SerializerMethod> item in _serializers.GetWriteMethods())
            {
                if (item.Value is not GeneratedSerializerMethod gsm) continue;

                sb.AppendLine(gsm.MethodContent.ToString(2));

                //Add return if body already contains value. This is just to make neater formatting.
                if (initializeMethod.Body.Length > 0)
                    initializeMethod.Body.AppendLine();

                initializeMethod.Body.Append(initializeIndent + 1, CreateInitializeFunction(gsm));

                addedSerializers++;
            }

            sb.Append(initializeMethod.ToString(initializeIndent));
            sb.AppendLine(footer);

            string fileName = $"{FishNetConstants.Serializing_Namespace}_{Generated_Class_Name}.g.cs";
            context.AddSource($"{fileName}", sb.ToString());

            Log($"Added class {fileName}. Added serializer count is {addedSerializers}.");
        }

        /// <summary>
        /// Creates a call to the set method for generic serializers.
        /// </summary>
        private string CreateInitializeFunction(GeneratedSerializerMethod dsm)
        {
            StringBuilder sb = new();
            //GenericSerializer<Type>.SetWrite(
            sb.Append($"{FishNetConstants.GenericDeltaWriter_FullName}<{dsm.TypeFullName}>.{FishNetConstants.GenericDeltaWriter_SetWrite_Name}(");
            sb.Append($"{RoslynCodeBuilder.CreateFunction(NativeConstants.Boolean_FullName, FishNetConstants.Writer_FullName, dsm.TypeFullName, dsm.TypeFullName, FishNetConstants.DeltaSerializerOption_FullName)}");
            sb.Append($"({dsm.MethodName}));");

            return sb.ToString();
        }

        private void Log(string txt)
        {
            if (txt.Length == 0)
                Debugg.Log(txt);
            else
                Debugg.Log($"   [Write] {txt}");
        }
    }
}