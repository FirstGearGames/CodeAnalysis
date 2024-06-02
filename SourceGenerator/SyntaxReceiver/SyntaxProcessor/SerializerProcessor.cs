using FishNet.CodeAnalysis.Extensions;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Serializing;
using FishNet.Transporting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynLearning.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace SourceGenerator.SyntaxReceiver.SyntaxProcessor
{
    public class SerializerProcessor
    {
        public HashSet<string> SerializableTypes = new();

        public List<MethodDeclarationSyntax> Methods = new();

        public static string Writer_FullName => typeof(Writer).FullName;
        public static string ServerRpcAttribute_FullName => typeof(ServerRpcAttribute).FullName;
        public static string TargetRpcAttribute_FullName => typeof(TargetRpcAttribute).FullName;
        public static string ObserversRpcAttribute_FullName => typeof(ObserversRpcAttribute).FullName;
        public static string Channel_FullName => typeof(Channel).FullName;
        public static string NetworkConnection_FullName => typeof(NetworkConnection).FullName;

        //public ClassDeclarationSyntax? GeneratedWriter_Class;

        //public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        //{
        //    //SyntaxNode syntaxNode = context.Node;

        //    //if (syntaxNode is ClassDeclarationSyntax classDeclaration)
        //    //    FindClassSerializables(context, classDeclaration);
        //    //else if (syntaxNode is MethodDeclarationSyntax methodDeclaration)
        //    //    FindRpcSerializables(context, methodDeclaration);

        //}

        //private void FindClassSerializables(GeneratorSyntaxContext context, ClassDeclarationSyntax classDeclarationSyntax)
        //{
        //    //ISymbol? symbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);
        //    //if (symbol is not INamedTypeSymbol namedTypeSymbol) return;

        //    //string fullName = namedTypeSymbol.GetFullName();
        //    //if (fullName == typeof(FishNet.Serializing.GeneratedWriters).FullName)
        //    //    GeneratedWriter_Class = classDeclarationSyntax;
        //}

        //private void FindRpcSerializables(GeneratorSyntaxContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        //{
        //    //ISymbol? symbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax);
        //    //if (symbol is not IMethodSymbol methodSymbol) return;
        //    //if (!symbol.HasAttributes<ServerRpcAttribute, ObserversRpcAttribute, TargetRpcAttribute>(out List<AttributeData> results)) return;

        //    //List<IParameterSymbol> parameters = methodSymbol.Parameters.ToList();
        //    //int parametersCount = parameters.Count;
        //    //Debugg.Log("Parameter count is  " + parametersCount);

        //    //foreach (AttributeData item in results)
        //    //{
        //    //    //ServerRpc.
        //    //    if (item.AttributeClass?.GetFullName() == ServerRpcAttribute_FullName)
        //    //    {
        //    //        RemoveTrailinggNetworkConnection();
        //    //        RemoveTrailingChannel();
        //    //    }
        //    //    //TargetRpc.
        //    //    else if (item.AttributeClass?.GetFullName() == TargetRpcAttribute_FullName)
        //    //    {
        //    //        RemoveTrailingChannel();
        //    //        RemoveLeadingNetworkConnection();
        //    //    }
        //    //    //ObserversRpc.
        //    //    else if (item.AttributeClass?.GetFullName() == TargetRpcAttribute_FullName)
        //    //    {
        //    //        RemoveTrailingChannel();
        //    //        RemoveLeadingNetworkConnection();
        //    //    }

        //    //    //Removes networkConnection if the first parameter.
        //    //    void RemoveLeadingNetworkConnection()
        //    //    {
        //    //        if (parametersCount == 0) return;
        //    //        //Remove channel from serializable.
        //    //        if (parameters[0].Type.GetFullName() == NetworkConnection_FullName)
        //    //            parameters.RemoveAt(--parametersCount);
        //    //    }

        //    //    //Removes networkConnection if the last parameter.
        //    //    void RemoveTrailinggNetworkConnection()
        //    //    {
        //    //        if (parametersCount == 0) return;
        //    //        //Remove channel from serializable.
        //    //        if (parameters[parametersCount - 1].Type.GetFullName() == NetworkConnection_FullName)
        //    //            parameters.RemoveAt(--parametersCount);
        //    //    }


        //    //    //Removes channel if the last parameter.
        //    //    void RemoveTrailingChannel()
        //    //    {
        //    //        if (parametersCount == 0) return;
        //    //        if (!parameters[parametersCount - 1].IsOptional) return;
        //    //        //Remove channel from serializable.
        //    //        if (parameters[parametersCount - 1].Type.GetFullName() == Channel_FullName)
        //    //            parameters.RemoveAt(--parametersCount);
        //    //    }
        //    //}

        //    ////Anything left in parameters is serializable.
        //    //foreach (IParameterSymbol parameter in parameters)
        //    //    SerializableTypes.Add(parameter.Type.GetFullName());
        //}

        ////private void FindMethodSerializables(CompilationUnitSyntax root, MethodDeclarationSyntax methodDeclaration)
        ////{
        ////    methodDeclaration.GetDeclaredSymbol(root);
        ////    if (methodDeclaration.AttributeLists == null) return;

        ////    foreach (AttributeListSyntax attributeList in methodDeclaration.AttributeLists)
        ////    {
        ////        foreach (AttributeSyntax item in attributeList.Attributes)
        ////        {

        ////        }
        ////    }
        ////}


        ////private void FindRpcSerializables(List<ClassDeclarationSyntax> classes)
        ////{
        ////    foreach (ClassDeclarationSyntax classDeclaration in classes)
        ////    {
        ////        foreach (var item in classDeclaration.m)
        ////        {

        ////        }
        ////    }
        ////}
        ///// <summary>
        ///// Creates serializers for any networked methods which need them.
        ///// </summary>
        //private void CreateSerializers_ForMethods(in GeneratorExecutionContext context)
        //{
        //    Debugg.Log($"- CreateSerializers_ForMethods");
        //    foreach (SyntaxTree tree in context.Compilation.SyntaxTrees)
        //    {
        //        CompilationUnitSyntax? root = tree.GetRoot() as CompilationUnitSyntax;
        //        if (root == null) continue;
        //        SemanticModel semanticModel = context.Compilation.GetSemanticModel(tree);
        //        if (semanticModel == null) continue;

        //        IEnumerable<ClassDeclarationSyntax> classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
        //        foreach (ClassDeclarationSyntax classDeclaration in classes)
        //        {
        //            Debugg.Log($"Class: {classDeclaration.Identifier.ToString().Quoted()}");
        //        }

        //        IEnumerable<MethodDeclarationSyntax> methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
        //        foreach (MethodDeclarationSyntax myMethod in methods)
        //        {
        //            IMethodSymbol? methodSymbol = (IMethodSymbol?)semanticModel.GetDeclaredSymbol(myMethod);
        //            if (methodSymbol == null) continue;

        //            Debugg.Log("Method Name " + methodSymbol.Name);
        //            //Does not have any RPC attributes.
        //            if (!methodSymbol.HasRpcAttributes(out List<RpcAttributeData> rpcAttributeDatas)) return;

        //            Debugg.Log("-- RpcAttributeDatas");
        //            foreach (RpcAttributeData item in rpcAttributeDatas)
        //                Debugg.Log($"  {item.RPCType.ToString().Quoted()}");
        //        }

        //    }
        //}
    }
}
