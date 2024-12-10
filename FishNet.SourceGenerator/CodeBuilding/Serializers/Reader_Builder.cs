using System.Collections.Generic;
using System.Text;
using FirstGearGames.Roslyn.CodeBuilding;
using FirstGearGames.Roslyn.Extensions;
using FirstGearGames.Roslyn.FishNet.Constants;
using FirstGearGames.Roslyn.FishNet.Receivers;
using FirstGearGames.Roslyn.FishNet.Serializing;
using FirstGearGames.Roslyn.FishNet.SyncTypes;
using FirstGearGames.Roslyn.Native.Constants;
using Microsoft.CodeAnalysis;
using RoslynCodeBuilder = FirstGearGames.Roslyn.CodeBuilding.CodeBuilder;

namespace FirstGearGames.Roslyn.FishNet.CodeBuilding.Serializers
{
    public class Reader_Builder
    {
        private const string Generated_Class_Name = "Generated_Readers";
        private const string Generated_Method_Prefix = $"{FishNetConstants.GeneratedReaderPrefix}Read";
        private const string Generated_ReaderParameter_Name = "reader";
        public const string InitializeOnLoad_Method_Name = Writer_Builder.InitializeOnLoad_Method_Name;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Methods _serializers => _generator.SerializerMethods;
        private SerializableGenerator _generator;
        private GeneratorExecutionContext _context;
        private GeneratorSyntaxReceiver _rootSyntaxReceiver;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void Initialize(GeneratorExecutionContext context, GeneratorSyntaxReceiver rootSyntaxReceiver, SerializableGenerator generator)
        {
            Log("");
            Log("Initialize");
            Log("");

            _context = context;
            _rootSyntaxReceiver = rootSyntaxReceiver;
            _generator = generator;
        }

        public void CreateEmptySerializerMethods() => CreateEmptySerializerMethods(_context, _rootSyntaxReceiver);
        public void CreateSerializerBodies() => CreateSerializerBodies(_context, _rootSyntaxReceiver);
        public void CreateGeneratedSerializersClass() => CreateGeneratedSerializersClass(_context);

        public MethodData CreateSerializerMethod(ITypeSymbol typeSymbol)
        {
            return new MethodData(typeSymbol, $"{FishNetConstants.Reader_Read_Name}<{typeSymbol.GetTypeSymbolFullNameWithNamedArguments(metadataName: false)}>");
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
        /// Creates an empty serializer method for a type.
        /// </summary>
        private void CreateEmptySerializerMethod(GeneratorExecutionContext context, SerializableType serializableType)
        {
            string typeFullName = serializableType.FullName;
            Log($"Checking to create a reader for {typeFullName}.");
            //Already exist either in FishNet or already created.
            if (_serializers.GetReadMethod(typeFullName, GetSerializerType.Full).IsValid())
                return;

            INamedTypeSymbol? namedTypeSymbol = context.Compilation.GetTypeByMetadataName(serializableType.FullMetadataName);
            if (namedTypeSymbol == null)
            {
                Log($"NamedTypeSymbol is null for fullMetaName {serializableType.FullMetadataName}");
                return;
            }
            if (!_serializers.CanCreateSerializer(namedTypeSymbol))
            {
                Log($"Cannot create serializer.");
                return;
            }

            List<IFieldSymbol> serializableFields = _serializers.GetSerializableFieldSymbols(namedTypeSymbol);
            foreach (IFieldSymbol item in serializableFields)
            {
                ITypeSymbol typeSymbol = item.Type;
                Log($"Checking serializable field {item.Name}.");
                CreateEmptySerializerMethod(context, new SerializableType(typeSymbol));
            }

            string header = GetMethodHeader(out string methodName);
            //Add to readers.
            _serializers.AddReadMethod(new GeneratedSerializerMethod(namedTypeSymbol, methodName, header, string.Empty), AddSerializerType.Full);

            string GetMethodHeader(out string mName)
            {
                mName = $"{Generated_Method_Prefix}{typeFullName.RemovePeriods("_")}";
                StringBuilder sb = new();
                string inText = namedTypeSymbol.IsUserDefinedStruct() ? "in " : string.Empty;
                inText = "";
                sb.Append(2, $"public static {typeFullName} {mName}" + $"(this {FishNetConstants.Reader_FullName} {Generated_ReaderParameter_Name})");

                return sb.ToString();
            }
        }

        /// <summary>
        /// Creates bodies for empty serializer methods.
        /// </summary>
        private void CreateSerializerBodies(GeneratorExecutionContext context, GeneratorSyntaxReceiver rootSyntaxReceiver)
        {
            //Iterate all serializers and if they are generated then complete them.
            foreach (KeyValuePair<string, MethodData> item in _serializers.GetReadMethods())
            {
                //Skip built in serializers.
                if (!item.Value.IsValid() || item.Value is not GeneratedSerializerMethod gsm)
                    continue;

                Log($"Creating serializer body for {item.Value.TypeFullName}.");
                const int bodyIndent = 3;
                StringBuilder sb = new();

                //Make a new instance of the type to return.
                const string resultVariableName = "result";
                sb.AppendLine(bodyIndent, $"{RoslynCodeBuilder.CreateLocalVariable(item.Key, resultVariableName, "new()")}{NativeConstants.LineFeed}");

                List<IFieldSymbol> serializableFieldSymbols = _serializers.GetSerializableFieldSymbols(gsm.TypeSymbol);

                foreach (IFieldSymbol fieldSymbol in serializableFieldSymbols)
                {
                    ITypeSymbol typeSymbol = fieldSymbol.Type;

                    //Get serializer method for the field.
                    MethodData sm = _serializers.GetReadMethod(typeSymbol, GetSerializerType.Full, metadataName: false, out _);

                    //Serializer not found.
                    if (!sm.IsValid())
                        sb.AppendLine(bodyIndent, CodeBuilder.GetMissingSerializerComment(deltaSerializer: false, item.Value.TypeSymbol, fieldSymbol));
                    //Serializer found.
                    else
                        sb.AppendLine(bodyIndent, GetReadCall(sm, resultVariableName, Generated_ReaderParameter_Name, fieldSymbol, closeCall: true));
                }

                sb.Append(bodyIndent, $"return {resultVariableName};");

                gsm.MethodContent.Body = sb;
            }
        }

        /// <summary>
        /// Creates a class containing generated serializers.
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

            foreach (KeyValuePair<string, MethodData> item in _serializers.GetReadMethods())
            {
                if (item.Value is not GeneratedSerializerMethod dsm) continue;

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

            Log($"Added class {fileName}. Added serializer count is {addedSerializers}.");
        }

        /// <summary>
        /// Creates a call to the set method for generic serializers.
        /// </summary>
        private string CreateInitializeFunction(GeneratedSerializerMethod dsm)
        {
            StringBuilder sb = new();
            //GenericSerializer<Type>.SetWrite(
            sb.Append($"{FishNetConstants.GenericReader_FullName}<{dsm.TypeFullName}>.{FishNetConstants.GenericReader_SetRead_Name}(");
            sb.Append($"{RoslynCodeBuilder.CreateFunction(dsm.TypeFullName, FishNetConstants.Reader_FullName)}");
            sb.Append($"({dsm.MethodName}));");

            return sb.ToString();
        }

        public string GetReadCall(MethodData sm, string resultVariableName, string readerVariableName, IFieldSymbol fieldSymbol, bool closeCall)
        {
            if (!sm.IsValid())
                return string.Empty;

            ITypeSymbol fieldSymbolType = fieldSymbol.Type;
            string fieldName = fieldSymbol.Name;

            string arguments;
            if (!sm.AreGenericsNamed && sm.GenericArguments.Count > 0)
                arguments = $"{fieldSymbolType.GetGenericArgumentsString(GenericArgumentType.PreferNamed).GetCombinedGenericArguments(fieldSymbolType)}";
            else
                arguments = string.Empty;

            // if (sm.IsGenerated())
            //     return $"{resultVariableName}.{fieldName} = {RoslynCodeBuilder.CallMethod($"Read{arguments}", readerVariableName, closeCall)}";
            // else
            return $"{resultVariableName}.{fieldName} = {RoslynCodeBuilder.CallMethod($"{sm.MethodName}{arguments}", readerVariableName, closeCall)}";
        }

        private void Log(string txt)
        {
            if (txt.Length == 0)
                Debugg.Log(txt);
            else
                Debugg.Log($"   [Reader_Builder] {txt}");
        }
    }
}