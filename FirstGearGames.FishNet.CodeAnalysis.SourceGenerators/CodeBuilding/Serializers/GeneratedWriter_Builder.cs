using FirstGearGames.FishNet.CodeAnalysis.Constants;
using System.Collections.Generic;
using System.Text;
using FirstGearGames.CodeAnalysis.CodeBuilding;
using FirstGearGames.CodeAnalysis.Extensions;
using FirstGearGames.CodeAnalysis.Helpers;
using FirstGearGames.FishNet.CodeAnalysis.Helpers.Serializing;
using FirstGearGames.FishNet.CodeAnalysis.Receivers;
using FirstGearGames.FishNet.CodeAnalysis.SourceGenerators;
using Microsoft.CodeAnalysis;

namespace FirstGearGames.FishNet.CodeAnalysis.CodeBuilding.Serializers
{
    public class GeneratedWriter_Builder
    {
        public const string Generated_WriterParameter_Name = "writer";
        public const string InitializeOnLoad_Method_Name = "InitializeSerializers";
        private const string Generated_Class_Name = "Generated_Writers";
        private const string Generated_Method_Prefix = $"{FishNetConstants.GeneratedWriterPrefix}Write";

        private static StringBuilder _stringBuilder = new();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private SerializerMethods _serializerMethods => _generator.SerializerMethods;
        private MainGenerator _generator;
        private GeneratorExecutionContext _context;
        private GeneratorSyntaxReceiver _rootSyntaxReceiver;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void Initialize(GeneratorExecutionContext context, GeneratorSyntaxReceiver rootSyntaxReceiver, MainGenerator generator)
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
            foreach (SerializableType item in syntaxReceiver.SerializableFinder.TypesNeedingSerializers)
                CreateEmptySerializerMethod(context, item);
        }

        /// <summary>
        /// Creates an empty delta serializer method for a type.
        /// </summary>
        private void CreateEmptySerializerMethod(GeneratorExecutionContext context, SerializableType serializableType)
        {
            string typeFullName = serializableType.FullName;
            
            //Already exist either in FishNet or already created.
            if (_serializerMethods.GetWriteMethod(serializableType.NamedTypeSymbol, GetSerializerType.Full, metadataName: false, out _).IsValid())
                return;
            
            INamedTypeSymbol? namedTypeSymbol = context.Compilation.GetTypeByMetadataName(serializableType.FullMetadataName);
            if (namedTypeSymbol == null)
                return;

            if (!_serializerMethods.CanCreateSerializer(namedTypeSymbol))
                return;

            List<IFieldSymbol> serializableFields = _serializerMethods.GetSerializableFieldSymbols(namedTypeSymbol);
            foreach (IFieldSymbol item in serializableFields)
            {
                if (item.Type is not INamedTypeSymbol fieldNamedTypeSymbol) continue;
                
                CreateEmptySerializerMethod(context, new SerializableType(fieldNamedTypeSymbol));
            }

            string header = CreateSignature(out string methodName);
            //Add to writers.
            _serializerMethods.AddWriteMethod(new GeneratedSerializerMethod(namedTypeSymbol, methodName, header, string.Empty), AddSerializerType.Full);

            string CreateSignature(out string mName)
            {
                mName = $"{Generated_Method_Prefix}{typeFullName.RemovePeriods("_")}";
                StringBuilder sb = new();
                string inText = namedTypeSymbol.IsUserDefinedStruct() ? "in " : string.Empty;
                inText = "";
                //public static bool WriteType(Type valueA) { }
                sb.Append(2, $"public static void {mName}" + $"(this {FishNetConstants.Writer_FullName} {Generated_WriterParameter_Name}," + $" {inText}{typeFullName} {_serializerMethods.GetValueParameterName(0)})");

                return sb.ToString();
            }
        }

        /// <summary>
        /// Creates bodies for empty delta serializer methods.
        /// </summary>
        private void CreateSerializerBodies(GeneratorExecutionContext context, GeneratorSyntaxReceiver SyntaxReceiver)
        {
            //Iterate all serializers and if they are generated then complete them.
            foreach (KeyValuePair<string, SerializerMethodData> item in _serializerMethods.GetWriteMethods())
            {
                //Skip built in serializers.
                if (!item.Value.IsValid() || item.Value is not GeneratedSerializerMethod gsm)
                    continue;

                const int bodyIndent = 3;
                StringBuilder sb = new();

                List<IFieldSymbol> serializableFieldSymbols = _serializerMethods.GetSerializableFieldSymbols(gsm.TypeSymbol);

                //Call write for all members.
                foreach (IFieldSymbol fieldSymbol in serializableFieldSymbols)
                {
                    ITypeSymbol typeSymbol = fieldSymbol.Type;

                    //Get serializer method for the field.
                    SerializerMethodData sm = _serializerMethods.GetWriteMethod(typeSymbol, GetSerializerType.Full, metadataName: false, out _);

                    //Serializer not found.
                    if (!sm.IsValid())
                        sm = _serializerMethods.CreateWriteGenericSerializerMethod(typeSymbol, metadataName: false);
                    //     sb.AppendLine(bodyIndent, GeneralBuilder.GetMissingSerializerComment(deltaSerializer: false, item.Value.TypeSymbol, fieldSymbol));
                    // //Serializer found.
                    // else
                        sb.AppendLine(bodyIndent, GetWriteCall(sm, Generated_WriterParameter_Name, fieldSymbol, $"{_serializerMethods.GetValueParameterName(0)}.{fieldSymbol.Name}", closeCall: true));
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

            string clsText = CodeBuilder.CreatePublicStaticClass(Generated_Class_Name, out string footer, FishNetConstants.Serializing_Namespace);
            sb.AppendLine($"//FishNet generated file.");
            sb.AppendLine(clsText);

            const int initializeIndent = 2;
            SerializerMethodContent initializeMethod = GeneralBuilder.CreatePublicRuntimeInitializeOnLoadMethod(initializeIndent, InitializeOnLoad_Method_Name);

            int addedSerializers = 0;

            foreach (KeyValuePair<string, SerializerMethodData> item in _serializerMethods.GetWriteMethods())
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
            sb.Append($"{CodeBuilder.CreateAction(FishNetConstants.Writer_FullName, dsm.TypeFullName)}");
            sb.Append($"({dsm.MethodName}));");

            return sb.ToString();
        }

        /// <summary>
        /// Returns a call to Read/WriteMethodName or Read/Write<T> for a field.
        /// </summary>
        public string GetWriteCall(SerializerMethodData sm, string writerVariableName, IFieldSymbol fieldSymbol, string variableName, bool closeCall)
        {
            return GetWriteCall(sm, writerVariableName, fieldSymbol.Type, $"{variableName}", closeCall);
        }

        /// <summary>
        /// Returns a call to Read/WriteMethodName or Read/Write<T> for a field.
        /// </summary>
        public string GetWriteCall(SerializerMethodData sm, string writerVariableName, ITypeSymbol typeSymbol, string variableName, bool closeCall)
        {
            if (!sm.IsValid())
                return string.Empty;

            string arguments = sm.GetReadOrWriteArgumentString(typeSymbol);

            //Uses Read/Write<T>.
            if (sm.IsGenericReadOrWriteMethod()) 
                return CodeBuilder.CallMethod($"{FishNetConstants.Writer_Write_Name}<{arguments}>", writerVariableName, closeCall, $"{variableName}");
            
            //Uses generated or built-in serializer.
            return CodeBuilder.CallMethod($"{sm.MethodName}{arguments}", writerVariableName, closeCall, $"{variableName}");   
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