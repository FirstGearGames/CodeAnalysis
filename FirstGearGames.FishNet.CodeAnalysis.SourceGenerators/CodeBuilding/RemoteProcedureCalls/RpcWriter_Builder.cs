using System.Collections.Generic;
using System.Text;
using FirstGearGames.CodeAnalysis.CodeBuilding;
using FirstGearGames.CodeAnalysis.Extensions;
using FirstGearGames.CodeAnalysis.Helpers;
using FirstGearGames.FishNet.CodeAnalysis.CodeBuilding.Serializers;
using FirstGearGames.FishNet.CodeAnalysis.Helpers.RemoteProcedureCalls;
using FirstGearGames.FishNet.CodeAnalysis.Receivers;
using FirstGearGames.FishNet.CodeAnalysis.SourceGenerators;
using Microsoft.CodeAnalysis;

namespace FirstGearGames.FishNet.CodeAnalysis.CodeBuilding.RemoteProcedureCalls
{
    public class RpcWriter_Builder
    {
        public const string GENERATED_METHOD_PREFIX = "SendRpc_";
        public const string GENERATED_PAREMETER_PREFIX = "p___";

        private StringBuilder _stringBuilder = new();
        private List<string> _stringList = new();
        private MainGenerator _generator;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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

        public void CreateEmptyRpcMethods() => CreateEmptyRpcMethods(_context, _rootSyntaxReceiver);
        public void CreateSerializerBodies() => CreateSerializerBodies(_context, _rootSyntaxReceiver);

        /// <summary>
        /// Creates SerializerMethod for each type in need.
        /// </summary>
        private void CreateEmptyRpcMethods(GeneratorExecutionContext context, GeneratorSyntaxReceiver syntaxReceiver)
        {
            foreach (RpcMethodDatas item in syntaxReceiver.RpcFinder.MethodsNeedingSerializers)
                CreateEmptyRpcMethod(context, item);
        }

        /// <summary>
        /// Creates an empty delta serializer method for a type.
        /// </summary>
        private void CreateEmptyRpcMethod(GeneratorExecutionContext context, RpcMethodDatas methodData)
        {
            Log($"Processing rpc method name {methodData.MethodName}.");

            const int indent = 3;

            string header = CreateMethodHeader();
            string writeBody = CreateBody();

            RpcMethodContent methodContent = new(header, writeBody);
            methodData.MethodContent = methodContent;

            Log(methodContent.ToString(indent));

            //Creates the header for the method.
            string CreateMethodHeader()
            {
                _stringBuilder.Clear();
                //Prefix_MethodName(
                string returnType = methodData.MethodSymbol.ReturnType.GetTypeSymbolFullName(metadataName: false);
                
                _stringBuilder.Append(indent, $"{CodeBuilder.GetDeclaredAccessibility(methodData.MethodSymbol)} {returnType} {GENERATED_METHOD_PREFIX}{methodData.MethodName}(");

                _stringList.Clear();
                //Add parameters to list as: ParameterType.FullName p___variableName.
                foreach (IParameterSymbol symbol in methodData.SerializableParameters)
                {
                    string symbolTypeName = symbol.Type.GetTypeSymbolFullNameWithNamedArguments(metadataName: false);
                    _stringList.Add($"{symbolTypeName} {GENERATED_PAREMETER_PREFIX}{symbol.Name}");
                }

                //Add parameters to header.
                _stringBuilder.Append(string.Join(", ", _stringList));
                //Close header off and return it.
                _stringBuilder.Append(")");

                return _stringBuilder.ToString();
            }

            //Creates calls to Write<T> for each parameter, and calls send rpc.
            string CreateBody()
            {
                _stringBuilder.Clear();

                _stringBuilder.AppendLine(indent + 1, GeneralBuilder.CallGetPooledWriter(out string writerVariableName));
                _stringBuilder.AppendLine();

                GeneratedWriter_Builder generatedWriterBuilder = _generator.GeneratedWriterBuilder;
                SerializerMethods serializerMethods = _generator.SerializerMethods;

                foreach (IParameterSymbol symbol in methodData.SerializableParameters)
                {
                    ITypeSymbol typeSymbol = symbol.Type;
                    //Get built in serializer. If it does not exist get generic.
                    SerializerMethodData smd = serializerMethods.GetWriteMethod(typeSymbol, GetSerializerType.Full, metadataName: false, out _);
                    if (!smd.IsValid())
                        smd = serializerMethods.CreateWriteGenericSerializerMethod(typeSymbol, metadataName: false);

                    _stringBuilder.AppendLine(indent + 1, generatedWriterBuilder.GetWriteCall(smd, writerVariableName, typeSymbol, $"{GENERATED_PAREMETER_PREFIX}{symbol.Name}", closeCall: true));
                }

                //todo: call the 'sendRpc' method before pooling.

                _stringBuilder.AppendLine();
                _stringBuilder.AppendLine(indent + 1, GeneralBuilder.CallStorePooledWriter(writerVariableName, closeCall: true));

                return _stringBuilder.ToString();
            }
            
        }

        /// <summary>
        /// Creates bodies for empty delta serializer methods.
        /// </summary>
        private void CreateSerializerBodies(GeneratorExecutionContext context, GeneratorSyntaxReceiver SyntaxReceiver) { }

        private void Log(string txt)
        {
            if (txt.Length == 0)
                Debugg.Log(txt);
            else
                Debugg.Log($"   [RemoteProcedureCalls_Writer_Builder] {txt}");
        }
    }
}