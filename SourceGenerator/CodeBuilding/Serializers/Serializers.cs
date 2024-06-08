using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using System.Linq;
using FishNet.CodeAnalysis.Extensions;
using RoslynLearning.Helpers;
using SourceGenerating.Constants;
using SourceGenerator.Extensions;

namespace SourceGenerator.CodeBuilding.Serializers
{
    // internal readonly struct SerializerMethod
    // {
    //     /// <summary>
    //     /// Name of the method.
    //     /// </summary>
    //     public readonly string MethodName;
    //
    //     /// <summary>
    //     /// Full name of the type the serializer is for.
    //     /// </summary>
    //     public readonly string TypeFullName;
    //
    //     public SerializerMethod(string methodFullName, string typeFullName)
    //     {
    //         MethodName = methodFullName;
    //         TypeFullName = typeFullName;
    //     }
    // }

    internal struct SerializerMethod
    {
        /// <summary>
        /// NamedTypeSymbol of the method.
        /// </summary>
        public readonly INamedTypeSymbol? NamedTypeSymbol;

        /// <summary>
        /// Full name of the type the serializer is for.
        /// </summary>
        public readonly string TypeFullName;

        /// <summary>
        /// Name of the generated method.
        /// </summary>
        public readonly string MethodName;

        /// <summary>
        /// True if this serializer was generated.
        /// </summary>
        public readonly bool Generated;

        public SerializerMethod(string typeFullName, string methodName, bool generated)
        {
            TypeFullName = typeFullName;
            MethodName = methodName;
        }

        public SerializerMethod(INamedTypeSymbol namedTypeSymbol, string typeFullName, string methodName, bool generated) : this(typeFullName, methodName, generated)
        {
            NamedTypeSymbol = namedTypeSymbol;
        }

        public bool IsValid() => (TypeFullName != string.Empty);
    }

    internal class Serializers
    {
        //Writers.
        private readonly Dictionary<string, SerializerMethod> _writeMethods = new();

        private readonly Dictionary<string, SerializerMethod> _writeDeltaMethods = new();

        //Readers
        private readonly Dictionary<string, SerializerMethod> _readMethods = new();
        private readonly Dictionary<string, SerializerMethod> _readDeltaMethods = new();

        public void Initialize(IAssemblySymbol runtimeAssemblySymbol)
        {
            SetWriterMethods(runtimeAssemblySymbol);
            SetDeltaWriterMethods(runtimeAssemblySymbol);
        }

        #region Add/get delta serializers

        /// <summary>
        /// Adds to delta writers.
        /// </summary>
        public void AddDeltaWriter(SerializerMethod sm)
        {
            _writeDeltaMethods.Add(sm.TypeFullName, sm);
        }

        /// <summary>
        /// Adds to delta readers.
        /// </summary>
        public void AddDeltaReader(SerializerMethod sm)
        {
            _readDeltaMethods.Add(sm.TypeFullName, sm);
        }

        /// <summary>
        /// Returns a delta writer.
        /// </summary>
        public SerializerMethod GetDeltaWriter(string typeFullName)
        {
            if (_writeDeltaMethods.TryGetValue(typeFullName, out SerializerMethod result))
                return result;

            return default;
        }
        /// <summary>
        /// Returns a delta reader.
        /// </summary>
        public SerializerMethod GetDeltaReader(string typeFullName)
        {
            if (_readDeltaMethods.TryGetValue(typeFullName, out SerializerMethod result))
                return result;

            return default;
        }
        #endregion


        /// <summary>
        /// Sets default writer references.
        /// </summary>
        private void SetWriterMethods(IAssemblySymbol runtimeAssemblySymbol)
        {
            if (!runtimeAssemblySymbol.GetINamedTypeSymbol(FishNetConstants.Writer_FullName, out INamedTypeSymbol? nameTypeSymbol))
                return;

            foreach (IMethodSymbol methodSymbol in nameTypeSymbol!.GetMembers().OfType<IMethodSymbol>())
            {
                // Does not have writer attribute.
                if (!methodSymbol.HasAttribute(FishNetConstants.WriterAttribute_FullName, out _)) continue;

                //Type will always be the first parameter.
                string typeFullName = methodSymbol.Parameters.First().Type.GetTypeFullName();

                if (_writeMethods.ContainsKey(typeFullName))
                    Debugg.Log($"A writer has already been found for {typeFullName}.");
                else
                    _writeMethods.Add(typeFullName, new SerializerMethod(methodSymbol.Name, typeFullName));
            }
        }

        /// <summary>
        /// Sets default writer references.
        /// </summary>
        public void SetDeltaWriterMethods(IAssemblySymbol runtimeAssemblySymbol)
        {
            if (!runtimeAssemblySymbol.GetINamedTypeSymbol(FishNetConstants.Writer_FullName, out INamedTypeSymbol? nameTypeSymbol))
                return;

            foreach (IMethodSymbol methodSymbol in nameTypeSymbol!.GetMembers().OfType<IMethodSymbol>())
            {
                // Does not have writer attribute.
                if (!methodSymbol.HasAttribute(FishNetConstants.DeltaWriterAttribute_FullName, out _)) continue;

                //Type will always be the first parameter.
                string typeFullName = methodSymbol.Parameters.First().Type.GetTypeFullName();

                if (_writeDeltaMethods.ContainsKey(typeFullName))
                    Debugg.Log($"A delta writer has already been found for {typeFullName}.");
                else
                    _writeDeltaMethods.Add(typeFullName, new SerializerMethod(methodSymbol.Name, typeFullName));
            }
        }
    }
}