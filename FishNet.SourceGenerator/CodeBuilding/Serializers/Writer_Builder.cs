using System.Collections.Generic;
using System.Text;
using FirstGearGames.Roslyn.Extensions;
using FirstGearGames.Roslyn.FishNet;
using FirstGearGames.Roslyn.FishNet.CodeBuilding;
using FirstGearGames.Roslyn.FishNet.Constants;
using FirstGearGames.Roslyn.FishNet.Helpers;
using FirstGearGames.Roslyn.FishNet.Receivers;
using FirstGearGames.Roslyn.FishNet.Serializing;
using FirstGearGames.Roslyn.FishNet.SyncTypes;
using FirstGearGames.Roslyn.Native.Constants;
using Microsoft.CodeAnalysis;
using RoslynCodeBuilder = FirstGearGames.Roslyn.CodeBuilding.CodeBuilder;

namespace FirstGearGames.Roslyn.FishNet.CodeBuilding.Serializers
{
    public class Writer_Builder
    {
        public const string Generated_WriterParameter_Name = "writer";
        public const string InitializeOnLoad_Method_Name = "InitializeSerializers";
        private const string Generated_Class_Name = "Generated_Writers";
        private const string Generated_Method_Prefix = $"{FishNetConstants.GeneratedWriterPrefix}Write";

        private static StringBuilder _stringBuilder = new();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Methods _methods => _generator.SerializerMethods;
        private SerializableGenerator _generator;
        private GeneratorExecutionContext _context;
        private GeneratorSyntaxReceiver _rootSyntaxReceiver;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void Initialize(GeneratorExecutionContext context, GeneratorSyntaxReceiver rootSyntaxReceiver, SerializableGenerator generator)
        {
            Log("");
            Log("Initialize.");
            Log("");

            _context = context;
            _rootSyntaxReceiver = rootSyntaxReceiver;
            _generator = generator;
        }

        public void CreateEmptySerializerMethods() => CreateEmptySerializerMethods(_context, _rootSyntaxReceiver);
        public void CreateSerializerBodies() => CreateSerializerBodies(_context, _rootSyntaxReceiver);
        public void CreateGeneratedSerializersClass() => CreateGeneratedSerializersClass(_context);

        /// <summary>
        /// Creates SerializerMethod for each type in need.
        /// </summary>
        private void CreateEmptySerializerMethods(GeneratorExecutionContext context, GeneratorSyntaxReceiver syntaxReceiver)
        {
            foreach (SerializableType item in syntaxReceiver.SerializableReceiver.TypesNeedingSerializers)
            {
                Log("//////////////////////////");
                Log($"Processing root serializable type {item.FullName}.");
                Log("//////////////////////////");
                CreateEmptySerializerMethod(context, item);
            }
        }

        /// <summary>
        /// Creates an empty delta serializer method for a type.
        /// </summary>
        private void CreateEmptySerializerMethod(GeneratorExecutionContext context, SerializableType serializableType)
        {
            string typeFullName = serializableType.FullName;
            Log($"Checking to create a writer for {typeFullName}.");
            //Already exist either in FishNet or already created.
            if (_methods.GetWriteMethod(typeFullName, GetSerializerType.Full).IsValid())
                return;

            INamedTypeSymbol? namedTypeSymbol = context.Compilation.GetTypeByMetadataName(serializableType.FullMetadataName);
            if (namedTypeSymbol == null)
                return;

            if (!_methods.CanCreateSerializer(namedTypeSymbol))
                return;

            List<IFieldSymbol> serializableFields = _methods.GetSerializableFieldSymbols(namedTypeSymbol);
            foreach (IFieldSymbol item in serializableFields)
            {
                ITypeSymbol typeSymbol = item.Type;
                CreateEmptySerializerMethod(context, new SerializableType(typeSymbol));
            }

            string header = GetMethodHeader(out string methodName);
            //Add to writers.
            _methods.AddWriteMethod(new GeneratedSerializerMethod(namedTypeSymbol, methodName, header, string.Empty), AddSerializerType.Full);

            string GetMethodHeader(out string mName)
            {
                mName = $"{Generated_Method_Prefix}{typeFullName.RemovePeriods("_")}";
                StringBuilder sb = new();
                string inText = namedTypeSymbol.IsUserDefinedStruct() ? "in " : string.Empty;
                inText = "";
                //public static bool WriteType(Type valueA) { }
                sb.Append(2, $"public static void {mName}" + $"(this {FishNetConstants.Writer_FullName} {Generated_WriterParameter_Name}," + $" {inText}{typeFullName} {_methods.GetValueParameterName(0)})");

                return sb.ToString();
            }
        }

        /// <summary>
        /// Creates bodies for empty delta serializer methods.
        /// </summary>
        private void CreateSerializerBodies(GeneratorExecutionContext context, GeneratorSyntaxReceiver SyntaxReceiver)
        {
            //Iterate all serializers and if they are generated then complete them.
            foreach (KeyValuePair<string, MethodData> item in _methods.GetWriteMethods())
            {
                //Skip built in serializers.
                if (!item.Value.IsValid() || item.Value is not GeneratedSerializerMethod gsm)
                    continue;

                Log($"Creating serializer body for {item.Value.TypeFullName}.");
                const int bodyIndent = 3;
                StringBuilder sb = new();

                List<IFieldSymbol> serializableFieldSymbols = _methods.GetSerializableFieldSymbols(gsm.TypeSymbol);

                //Call write for all members.
                foreach (IFieldSymbol fieldSymbol in serializableFieldSymbols)
                {
                    ITypeSymbol typeSymbol = fieldSymbol.Type;

                    //Get serializer method for the field.
                    MethodData sm = _methods.GetWriteMethod(typeSymbol, GetSerializerType.Full, metadataName: false, out _);

                    //Serializer not found.
                    if (!sm.IsValid())
                        sb.AppendLine(bodyIndent, CodeBuilder.GetMissingSerializerComment(deltaSerializer: false, item.Value.TypeSymbol, fieldSymbol));
                    //Serializer found.
                    else
                        sb.AppendLine(bodyIndent, GetWriteCall(sm, Generated_WriterParameter_Name, fieldSymbol.Name, closeCall: true));
                }

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

            foreach (KeyValuePair<string, MethodData> item in _methods.GetWriteMethods())
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
            sb.Append($"{FishNetConstants.GenericWriter_FullName}<{dsm.TypeFullName}>.{FishNetConstants.GenericDeltaWriter_SetWrite_Name}(");
            sb.Append($"{RoslynCodeBuilder.CreateAction(FishNetConstants.Writer_FullName, dsm.TypeFullName)}");
            sb.Append($"({dsm.MethodName}));");

            return sb.ToString();
        }

        public string GetWriteCall(MethodData sm, string writerVariableName, string fieldName, bool closeCall)
        {
            if (!sm.IsValid())
                return string.Empty;

            // if (sm.IsGenerated())
            //     return RoslynCodeBuilder.CallMethod($"Write", writerVariableName, closeCall, $"{_serializers.GetValueParameterName(0)}.{fieldName}");
            // else
            return RoslynCodeBuilder.CallMethod($"{sm.MethodName}", writerVariableName, closeCall, $"{_methods.GetValueParameterName(0)}.{fieldName}");
        }

        private void Log(string txt)
        {
            if (txt.Length == 0)
                Debugg.Log(txt);
            else
                Debugg.Log($"   [Writer_Builder] {txt}");
        }
    }
}