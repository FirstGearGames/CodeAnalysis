using FirstGearGames.FishNet.CodeAnalysis.Constants;
using FirstGearGames.FishNet.CodeAnalysis.Helpers.Serializing;
using FirstGearGames.FishNet.CodeAnalysis.Receivers;
using FirstGearGames.FishNet.CodeAnalysis.SourceGenerators;
using System.Collections.Generic;
using System.Text;
using FirstGearGames.CodeAnalysis.CodeBuilding;
using FirstGearGames.CodeAnalysis.Constants;
using FirstGearGames.CodeAnalysis.Extensions;
using FirstGearGames.CodeAnalysis.Helpers;
using Microsoft.CodeAnalysis;

namespace FirstGearGames.FishNet.CodeAnalysis.CodeBuilding.Serializers
{
    public class GeneratedDeltaReader_Builder
    {
        private static readonly string InitializeOnLoad_Method_Name = GeneratedDeltaWriter_Builder.InitializeOnLoad_Method_Name;
        private static readonly string Generated_Class_Name = "Generated_DeltaReaders";
        private static readonly string Generated_Method_Prefix = $"{FishNetConstants.GeneratedReaderPrefix}ReadDelta";
        private static readonly string Generated_ReaderParameter_Name = "reader";
        private static readonly string Generated_DeltaSerializerOption_Name = GeneratedDeltaWriter_Builder.Generated_DeltaSerializerOption_Name;
        private static readonly StringBuilder _stringBuilder = new();
        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private SerializerMethods _serializerMethods => _generator.SerializerMethods;
        private MainGenerator _generator;
        private GeneratorExecutionContext _context;
        private GeneratorSyntaxReceiver _generatorSyntaxReceiver;
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void Initialize(GeneratorExecutionContext context, GeneratorSyntaxReceiver generatorSyntaxReceiver, MainGenerator generator)
        {
            Log("");
            Log("Initialize.");

            _context = context;
            _generatorSyntaxReceiver = generatorSyntaxReceiver;
            _generator = generator;
        }

        public void CreateEmptySerializerMethods() => CreateEmptySerializerMethods(_context, _generatorSyntaxReceiver);
        public void CreateSerializerBodies() => CreateSerializerBodies(_context, _generatorSyntaxReceiver);
        public void CreateGeneratedSerializersClass() => CreateGeneratedSerializersClass(_context);

        /// <summary>
        /// Creates SerializerMethod for each type in need.
        /// </summary>
        private void CreateEmptySerializerMethods(GeneratorExecutionContext context, GeneratorSyntaxReceiver syntaxReceiver)
        {
            foreach (SerializableType item in syntaxReceiver.SerializableFinder.TypesNeedingSerializers)
                CreateEmptyDeltaSerializerMethod(context, item);
        }

        /// <summary>
        /// Creates an empty serializer method for a type.
        /// </summary>
        private void CreateEmptyDeltaSerializerMethod(GeneratorExecutionContext context, SerializableType serializableType, int recursiveCount = 1)
        {
            string typeFullName = serializableType.FullName;

            //Already exist either in FishNet or already created.
            if (_serializerMethods.GetReadMethod(serializableType.NamedTypeSymbol, GetSerializerType.Delta, metadataName: false, out _).IsValid())
                return;

            INamedTypeSymbol namedTypeSymbol = context.Compilation.GetTypeByMetadataName(serializableType.FullMetadataName);
            if (namedTypeSymbol is null)
                return;
            if (!_serializerMethods.CanCreateDeltaSerializer(namedTypeSymbol))
                return;

            List<IFieldSymbol> serializableFields = _serializerMethods.GetSerializableFieldSymbols(namedTypeSymbol);
            foreach (IFieldSymbol item in serializableFields)
            {
                if (item.Type is not INamedTypeSymbol fieldNamedTypeSymbol)
                    continue;

                CreateEmptyDeltaSerializerMethod(context, new(fieldNamedTypeSymbol), recursiveCount + 1);
            }

            string header = CreateSignature(out string methodName);
            //Add to readers.
            _serializerMethods.AddReadMethod(new GeneratedDeltaSerializerMethod(namedTypeSymbol, methodName, header, string.Empty), AddSerializerType.Delta);

            string CreateSignature(out string mName)
            {
                mName = $"{Generated_Method_Prefix}{typeFullName.RemovePeriods("_")}";
                StringBuilder sb = new();
                sb.Append(2, $"public static {typeFullName} {mName}" + $"(this {FishNetConstants.Reader_FullName} {Generated_ReaderParameter_Name}," + $" {typeFullName} {_serializerMethods.GetValueParameterName(0)})");

                return sb.ToString();
            }
        }

        /// <summary>
        /// Creates bodies for empty delta serializer methods.
        /// </summary>
        private void CreateSerializerBodies(GeneratorExecutionContext context, GeneratorSyntaxReceiver generatorSyntaxReceiver)
        {
            //Iterate all serializers and if they are generated then complete them.
            foreach (KeyValuePair<string, SerializerMethodData> item in _serializerMethods.GetReadDeltaMethods())
            {
                //Skip built in serializers.
                if (!item.Value.IsValid() || item.Value is not GeneratedDeltaSerializerMethod gsm)
                    continue;

                const int bodyIndent = 3;
                StringBuilder sb = new();

                //Make a new instance of the type to return.
                const string resultVariableName = "result";
                sb.AppendLine(bodyIndent, CodeBuilder.CreateLocalVariable(item.Key, resultVariableName, "new()"));

                /* ulong totalFlags = reader.ReadPackedWhole(); */
                string totalFlagsVariableName = "totalFlags";
                sb.Append(bodyIndent, CodeBuilder.CreateLocalVariable(NativeConstants.UInt64_FullName, totalFlagsVariableName, string.Empty, false));
                sb.AppendLine($" = {CodeBuilder.CallMethod(FishNetConstants.Reader_ReadUnsignedPackedWhole_Name, Generated_ReaderParameter_Name)}{NativeConstants.LineFeed}");

                /* DeltaSerializerOption options = (DeltaSerializerOption)totalFlags;
                 * if (options.FastContains(DeltaSerializerOption.FullSerializer))
                 *      return reader.Read<type>(); */
                CreateReadFullIf();

                void CreateReadFullIf()
                {
                    ITypeSymbol typeSymbol = item.Value.TypeSymbol;
                    SerializerMethodData sm = _serializerMethods.GetReadMethod(typeSymbol, GetSerializerType.Full, metadataName: false, out _);

                    string optionsVariableName = $"options";
                    sb.Append(bodyIndent, CodeBuilder.CreateLocalVariable(FishNetConstants.DeltaSerializerOption_FullName, optionsVariableName, closeLine: false));
                    sb.AppendLine($" = ({FishNetConstants.DeltaSerializerOption_FullName}){totalFlagsVariableName};");

                    sb.AppendLine(bodyIndent, $"if ({optionsVariableName}.{FishNetConstants.FastContains_Name}({FishNetConstants.DeltaSerializerOption_FullSerialize_FullName}))");
                    sb.AppendLine(bodyIndent, "{");

                    if (!sm.IsValid())
                        sm = _serializerMethods.CreateReadGenericSerializerMethod(typeSymbol, metadataName: false);
                    // {
                    //     sb.AppendLine(bodyIndent + 1, GeneralBuilder.GetMissingSerializerComment(deltaSerializer: false, item.Value.TypeSymbol));
                    // }
                    // else
                    sb.AppendLine(bodyIndent + 1, $"{resultVariableName} = {Generated_ReaderParameter_Name}.{sm.MethodName}();{NativeConstants.LineFeed}");

                    sb.AppendLine(bodyIndent + 1, $"return {resultVariableName};");
                    sb.AppendLine(bodyIndent, "}" + NativeConstants.LineFeed);
                }

                List<IFieldSymbol> serializableFieldSymbols = _serializerMethods.GetSerializableFieldSymbols(gsm.TypeSymbol);

                ulong fieldFlag = FishNetConstants.DeltaSerializerOption_MaxValue * 2;
                foreach (IFieldSymbol fieldSymbol in serializableFieldSymbols)
                {
                    ITypeSymbol typeSymbol = fieldSymbol.Type;

                    SerializerMethodData sm = _serializerMethods.GetReadMethod(typeSymbol, GetSerializerType.Delta, metadataName: false, out _);

                    //Neither delta nor full could be found.
                    if (!sm.IsValid())
                        sm = _serializerMethods.CreateReadDeltaGenericSerializerMethod(typeSymbol, metadataName: false);

                    // {
                    //     sb.AppendLine(bodyIndent, GeneralBuilder.GetMissingSerializerComment(deltaSerializer: false, item.Value.TypeSymbol, fieldSymbol));
                    //     continue;
                    // }

                    //if ((totalFlags & flag) == flag)
                    sb.AppendLine(bodyIndent, $"if (({totalFlagsVariableName} & {fieldFlag}) == {fieldFlag})");

                    // //If is a full serializer.
                    // if (!sm.IsDeltaSerializer())
                    // {
                    //     sb.AppendLine(bodyIndent + 1, GeneralBuilder.GetMissingSerializerComment(deltaSerializer: true, item.Value.TypeSymbol, fieldSymbol));
                    //     sb.AppendLine(bodyIndent + 1, _generator.GeneratedReaderBuilder.GetReadCall(sm, fieldSymbol, Generated_ReaderParameter_Name, $"{resultVariableName}.{fieldSymbol.Name}", closeCall: true));
                    // }
                    // else
                    // {
                    //If here then is a delta serializer.
                    DeltaSerializerMethod dsm = sm as DeltaSerializerMethod;

                    string previousValueName = $"{_serializerMethods.GetValueParameterName(0)}.{fieldSymbol.Name}";
                    sb.AppendLine(bodyIndent + 1, GetReadDeltaCall(dsm, fieldSymbol, Generated_ReaderParameter_Name, $"{resultVariableName}.{fieldSymbol.Name}", previousValueName, closeCall: true));
                    //}

                    /* else
                     *      result.FieldName = previousResult.FieldName; */
                    sb.AppendLine(bodyIndent, "else");
                    sb.AppendLine(bodyIndent + 1, $"{resultVariableName}.{fieldSymbol.Name} = {_serializerMethods.GetValueParameterName(0)}.{fieldSymbol.Name};{NativeConstants.LineFeed}");
                    fieldFlag *= 2;
                }

                sb.Append(bodyIndent, $"return {resultVariableName};");

                gsm.MethodContent.Body = sb;
            }
        }

        /// <summary>
        /// Creates a class containing generated delta writers.
        /// </summary>
        private void CreateGeneratedSerializersClass(GeneratorExecutionContext context)
        {
            CreateForPublicTypes();
            //CreateForPartialTypes();

            //Creates file for types that have public scope.
            void CreateForPublicTypes()
            {
                StringBuilder sb = new();

                string clsText = CodeBuilder.CreatePublicStaticClass(Generated_Class_Name, out string footer, FishNetConstants.Serializing_Namespace);
                sb.AppendLine(clsText);

                const int initializeIndent = 2;

                SerializerMethodContent initializeMethod = GeneralBuilder.CreatePublicRuntimeInitializeOnLoadMethod(initializeIndent, InitializeOnLoad_Method_Name);

                foreach (KeyValuePair<string, SerializerMethodData> item in _serializerMethods.GetReadDeltaMethods())
                {
                    if (item.Value.TypeSymbol is not INamedTypeSymbol namedTypeSymbol)
                        continue;
                    if (!namedTypeSymbol.HasPublicAccessibility())
                        continue;

                    if (item.Value is not GeneratedDeltaSerializerMethod dsm)
                        continue;

                    sb.AppendLine(dsm.MethodContent.ToString(2));

                    //Add return if body already contains value. This is just to make neater formatting.
                    if (initializeMethod.Body.Length > 0)
                        initializeMethod.Body.AppendLine();

                    initializeMethod.Body.Append(initializeIndent + 1, CreateInitializeFunction(dsm));
                }

                sb.Append(initializeMethod.ToString(initializeIndent));
                sb.AppendLine(footer);

                context.AddSource($"{FishNetConstants.Serializing_Namespace}_{Generated_Class_Name}.g.cs", sb.ToString());
            }
            //
            // //Creates file for types that have do not have public scope, but the containing type is partial.
            // void CreateForPartialTypes()
            // {
            //     StringBuilder sb = new();
            //
            //     const int initializeIndent = 2;
            //
            //     foreach (KeyValuePair<string, SerializerMethodData> item in _serializerMethods.GetReadDeltaMethods())
            //     {
            //         if (item.Value.TypeSymbol is not INamedTypeSymbol namedTypeSymbol) continue;
            //         if (namedTypeSymbol.HasPublicAccessibility() || !namedTypeSymbol.ContainingType.IsPartial()) continue;
            //
            //         if (item.Value is not GeneratedDeltaSerializerMethod dsm) continue;
            //
            //         /* //todo CreatePublicStaticClass needs to instead create class same as containingType.
            //          * className should be the same.
            //          *
            //          * The only difference is the output file should have the name of the type
            //          * the generate is for. EG: if one containing class has two nonPublic members that need
            //          * serializers and their names are.. S1 and S2, and containing class is CC then this is the result..
            //          *
            //          * //Saved as Generated_CC_S1.g.cs
            //          * //As well for the second type Generated_CC_s2.g.cs
            //          * public partial class CC //assuming containingType is public.
            //          * {
            //          * //Serializers here.
            //          * }
            //          *
            //          */
            //         INamedTypeSymbol containingType = namedTypeSymbol.ContainingType;
            //         string clsText = CodeBuilder.CreateClassCopy(containingType, out string footer, containingType.GetNamespace());
            //
            //         sb.AppendLine(clsText);
            //
            //         SerializerMethodContent initializeMethod = GeneralBuilder.CreatePublicRuntimeInitializeOnLoadMethod(initializeIndent, InitializeOnLoad_Method_Name);
            //
            //         sb.AppendLine(dsm.MethodContent.ToString(2));
            //
            //         //Add return if body already contains value. This is just to make neater formatting.
            //         if (initializeMethod.Body.Length > 0)
            //             initializeMethod.Body.AppendLine();
            //
            //         initializeMethod.Body.Append(initializeIndent + 1, CreateInitializeFunction(dsm));
            //
            //         sb.Append(initializeMethod.ToString(initializeIndent));
            //         sb.AppendLine(footer);
            //
            //         context.AddSource($"{containingType.GetNamespace()}_{containingType.Name}_{namedTypeSymbol.Name}.g.cs", sb.ToString());
            //     }
            // }
        }

        /// <summary>
        /// Creates a call to the set method for generic serializers.
        /// </summary>
        private string CreateInitializeFunction(GeneratedDeltaSerializerMethod dsm)
        {
            StringBuilder sb = new();
            //GenericDeltaSerializer<Type>.SetWrite(
            sb.Append($"{FishNetConstants.GenericDeltaReader_FullName}<{dsm.TypeFullName}>.{FishNetConstants.GenericDeltaReader_SetRead_Name}(");
            sb.Append($"{CodeBuilder.CreateFunction(dsm.TypeFullName, FishNetConstants.Reader_FullName, dsm.TypeFullName)}");
            sb.Append($"({dsm.MethodName}));");

            return sb.ToString();
        }

        /// <summary>
        /// Returns a call to Read/WriteMethodName or Read/Write<T> for a field.
        /// </summary>
        public string GetReadDeltaCall(SerializerMethodData sm, IFieldSymbol fieldSymbol, string readerVariableName, string resultVariableName, string previousValueName, bool closeCall)
        {
            // if (fieldSymbol.Name.Contains("Jagged"))
            // {
            //     ITypeSymbol sym = fieldSymbol.Type;
            //     if (sym is IArrayTypeSymbol arr)
            //     {
            //         string elementType = arr.ElementType.GetTypeSymbolFullName(metadataName: false);
            //         Log($"Okay. FieldName {fieldSymbol.Name}. Symbol name {elementType}");
            //     }
            // }
            return GetReadDeltaCall(sm, fieldSymbol.Type, readerVariableName, resultVariableName, previousValueName, closeCall);
        }

        /// <summary>
        /// Returns a call to Read/WriteMethodName or Read/Write<T> for a field.
        /// </summary>
        public string GetReadDeltaCall(SerializerMethodData sm, ITypeSymbol typeSymbol, string readerVariableName, string resultName, string previousValueName, bool closeCall)
        {
            if (!sm.IsValid())
                return string.Empty;

            string arguments = sm.GetReadOrWriteArgumentString(typeSymbol);

            //Uses Read/Write<T>.
            if (sm.IsGenericReadOrWriteMethod())
                return CodeBuilder.CallMethod($"{FishNetConstants.Reader_ReadDelta_Name}<{arguments}>", readerVariableName, closeCall, previousValueName);

            //Uses generated or built-in serializer.
            return $"{resultName} = {CodeBuilder.CallMethod($"{sm.MethodName}{arguments}", readerVariableName, closeCall, previousValueName)}";
        }

        private void Log(string txt)
        {
            if (txt.Length == 0)
                Debugg.Log(txt);
            else
                Debugg.Log($"   [DeltaReader_Builder] {txt}");
        }
    }
}