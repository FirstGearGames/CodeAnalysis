using System;
using System.Collections.Generic;
using System.Text;
using FirstGearGames.CodeAnalysis.CodeBuilding;
using FirstGearGames.CodeAnalysis.Extensions;
using FirstGearGames.CodeAnalysis.Helpers;
using FirstGearGames.FishNet.CodeAnalysis.CodeBuilding.Serializers;
using FirstGearGames.FishNet.CodeAnalysis.Constants;
using FirstGearGames.FishNet.CodeAnalysis.Helpers.RemoteProcedureCalls;
using FirstGearGames.FishNet.CodeAnalysis.Misc;
using FirstGearGames.FishNet.CodeAnalysis.Receivers;
using FirstGearGames.FishNet.CodeAnalysis.RemoteProcedureCalls;
using FirstGearGames.FishNet.CodeAnalysis.SourceGenerators;
using FishNetTypes.Managing.Logging;
using FishNetTypes.Object;
using Microsoft.CodeAnalysis;

namespace FirstGearGames.FishNet.CodeAnalysis.CodeBuilding.RemoteProcedureCalls
{
    public class RpcWriter_Builder
    {
        public const string GENERATED_PAREMETER_PREFIX = "p___";

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

        public void CreateRpcMethods() => CreateRpcMethods(_context, _rootSyntaxReceiver);

        /// <summary>
        /// Creates SerializerMethod for each type in need.
        /// </summary>
        private void CreateRpcMethods(GeneratorExecutionContext context, GeneratorSyntaxReceiver syntaxReceiver)
        {
            foreach (RpcMethodDatas item in syntaxReceiver.RpcFinder.RpcMethodDatas)
                CreateRpcMethod(context, item);
        }

        /// <summary>
        /// Creates an empty delta serializer method for a type.
        /// </summary>
        private void CreateRpcMethod(GeneratorExecutionContext context, RpcMethodDatas methodData)
        {
            Log($"Processing rpc method name {methodData.MethodName}.");

            const int indent = 3;
            const string channelVariableName = "channel";

            string header = CreateSignature();
            string writeBody = CreateBody();

            RpcMethodContent methodContent = new(header, writeBody);
            methodData.MethodContent = methodContent;

            Log(Environment.NewLine + methodContent.ToString(indent));

            //Creates the header for the method.
            string CreateSignature()
            {
                StringBuilder sb = PerformanceHelper.RetrieveStringBuilder();

                //Prefix_MethodName(
                string returnType = methodData.MethodSymbol.ReturnType.GetTypeSymbolFullName(metadataName: false);

                string methodPrefix = $"Send{methodData.RpcAttributeData.RPCType.ToString()}Rpc_";
                sb.Append(indent, $"{CodeBuilder.GetDeclaredAccessibility(methodData.MethodSymbol)} {returnType} {methodPrefix}{methodData.MethodName}(");

                _stringList.Clear();
                //Add parameters to list as: ParameterType.FullName p___variableName.
                foreach (IParameterSymbol symbol in methodData.SerializableParameters)
                {
                    string symbolTypeName = symbol.Type.GetTypeSymbolFullNameWithNamedArguments(metadataName: false);
                    _stringList.Add($"{symbolTypeName} {GENERATED_PAREMETER_PREFIX}{symbol.Name}");
                }

                /* Always add channel. The default will be optional as reliable. If the user
                 * specified their own optional then that is used instead. */
                _stringList.Add($"{FishNetConstants.Channel_FullName} {channelVariableName} = {methodData.DefaultChannelValue}");

                //Add parameters to header.
                sb.Append(string.Join(", ", _stringList));
                //Close header off and return it.
                sb.Append(")");

                string result = sb.ToString();
                PerformanceHelper.StoreStringBuilder(sb);
                return result;
            }

            //Creates calls to Write<T> for each parameter, and calls send rpc.
            string CreateBody()
            {
                StringBuilder sb = PerformanceHelper.RetrieveStringBuilder();

                sb.AppendLine(CreateCallerChecks());
                
                sb.AppendLine(indent + 1, GeneralBuilder.CallGetPooledWriter(out string writerVariableName));
                sb.AppendLine();

                GeneratedWriter_Builder generatedWriterBuilder = _generator.GeneratedWriterBuilder;
                SerializerMethods serializerMethods = _generator.SerializerMethods;

                foreach (IParameterSymbol symbol in methodData.SerializableParameters)
                {
                    ITypeSymbol typeSymbol = symbol.Type;
                    //Get built in serializer. If it does not exist get generic.
                    SerializerMethodData smd = serializerMethods.GetWriteMethod(typeSymbol, GetSerializerType.Full, metadataName: false, out _);
                    if (!smd.IsValid())
                        smd = serializerMethods.CreateWriteGenericSerializerMethod(typeSymbol, metadataName: false);

                    sb.AppendLine(indent + 1, generatedWriterBuilder.GetWriteCall(smd, writerVariableName, typeSymbol, $"{GENERATED_PAREMETER_PREFIX}{symbol.Name}", closeCall: true));
                }

                //Call base.Send Rpc.
                sb.AppendLine();
                sb.AppendLine(indent + 1, CreateCallRpc());

                sb.AppendLine();
                sb.AppendLine(indent + 1, GeneralBuilder.CallStorePooledWriter(writerVariableName, closeCall: true));

                string result = sb.ToString();
                PerformanceHelper.StoreStringBuilder(sb);
                return result;

                string CreateCallRpc()
                {
                    StringBuilder fullLSb = PerformanceHelper.RetrieveStringBuilder();

                    StringBuilder lSb = PerformanceHelper.RetrieveStringBuilder();

                    lSb.Append($"base.");

                    if (methodData.RpcAttributeData.RPCType == RPCType.Server)
                        lSb.Append(FishNetConstants.SendServerRpc_Name);
                    else if (methodData.RpcAttributeData.RPCType == RPCType.Observers)
                        lSb.Append(FishNetConstants.SendObserversRpc_Name);
                    else if (methodData.RpcAttributeData.RPCType == RPCType.Target)
                        lSb.Append(FishNetConstants.SendTargetRpc_Name);

                    //TODO needs to be a thing.
                    string hash = "-1";

                    string dataOrderType = methodData.RpcAttributeData.AttributeData.GetNamedArgument(FishNetConstants.RpcAttribute_OrderType_Name, FishNetConstants.Default_DataOrderType).GetEnumName();

                    //The following is used by all RPCs.
                    lSb.Append($"({hash}, {writerVariableName}, {channelVariableName}, {dataOrderType}");

                    RPCType rpcType = methodData.RpcAttributeData.RPCType;
                    if (rpcType == RPCType.Server)
                    {
                        //Nothing else needs to be done for server rpc.
                    }
                    else if (rpcType == RPCType.Observers) { }
                    else if (rpcType == RPCType.Target) { }

                    //Close off call.
                    lSb.Append(");");
                    //observerRpc //bool bufferLast, bool excludeServer, bool excludeOwner) { }
                    //targetRpc //NetworkConnection target, bool excludeServer, bool validateTarget = true) { }

                    string lResult = lSb.ToString();
                    PerformanceHelper.StoreStringBuilder(lSb);
                    return lResult;
                }

                string CreateCallerChecks()
                {
                    StringBuilder fullLSb = PerformanceHelper.RetrieveStringBuilder();

                    bool isServerRpc = (methodData.RpcAttributeData.RPCType == RPCType.Server);

                    fullLSb.Append(CreateInitializedCheck());

                    string ownerCheck = CreateOwnerCheck();
                    if (ownerCheck != string.Empty)
                        fullLSb.Append(ownerCheck);

                    string result = fullLSb.ToString();
                    PerformanceHelper.StoreStringBuilder(fullLSb);
                    return result;
                    
                    /* This is a mandatory initialized check. */
                    string CreateInitializedCheck()
                    {
                        StringBuilder lSb = PerformanceHelper.RetrieveStringBuilder();

                        //Get logging type. If set then add logging text.
                        LoggingType loggingType = methodData.RpcAttributeData.AttributeData.GetNamedArgument(FishNetConstants.RpcAttribute_Logging_Name, FishNetConstants.Default_LoggingType);
                        if (loggingType != LoggingType.Off)
                        {
                            string requiredInitializer = (isServerRpc) ? "Client" : "Server";
                            string text = $"Rpc {methodData.MethodName} cannot be run while {requiredInitializer} has not initialized the object. This commonly occurs when the {requiredInitializer} is not started or has not yet spawned the object.";
                            lSb.AppendLine(indent + 2, loggingType.CreateLog(FishNetConstants.Base_NetworkManager_Field_Name, text));
                        }

                        lSb.Append(indent + 2, "return;");

                        string conditionalStatement = "!";
                        conditionalStatement += (isServerRpc) ? FishNetConstants.Base_IsClient_Initialized_Field_Name : FishNetConstants.Base_IsServer_Initialized_Field_Name;

                        string ifBlock = (CodeBuilder.CreateMultiLineIf(indent + 1, conditionalStatement, lSb));

                        PerformanceHelper.StoreStringBuilder(lSb);

                        return ifBlock;
                    }

                    /* This next check is for if ServerRpc only, and is an owner check, only if
                     * authority is required, which is default. */
                    string CreateOwnerCheck()
                    {
                        return default;
                        //Todo. This needs to have the requireauthority check
                    }
                    
                    
                }
            }
        }

        /// <summary>
        /// Creates bodies for empty delta serializer methods.
        /// </summary>
        private void CreateRpcBodies(GeneratorExecutionContext context, GeneratorSyntaxReceiver SyntaxReceiver) { }

        private void Log(string txt)
        {
            if (txt.Length == 0)
                Debugg.Log(txt);
            else
                Debugg.Log($"   [RemoteProcedureCalls_Writer_Builder] {txt}");
        }
    }
}