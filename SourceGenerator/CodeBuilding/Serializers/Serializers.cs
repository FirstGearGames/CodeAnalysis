using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Linq;
using FishNet.CodeAnalysis.Extensions;
using RoslynLearning.Helpers;
using SourceGenerating.Constants;
using SourceGenerator.Extensions;

namespace SourceGenerator.CodeBuilding.Serializers
{
    internal enum AddSerializerType
    {
        Unset,
        Full,
        Delta,
    }

    internal enum GetSerializerType
    {
        Full,
        Delta,
        FavorFull,
        FavorDelta,
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
            AddDefaultWriteMethods(runtimeAssemblySymbol);
        }

        #region Add/get serializers

        /// <summary>
        /// Adds to writers.
        /// </summary>
        public void AddWriteMethod(SerializerMethod sm, AddSerializerType addType)
        {
            Dictionary<string, SerializerMethod> dict = (addType == AddSerializerType.Full) ? _writeMethods : _writeDeltaMethods;
            if (dict.ContainsKey(sm.TypeFullName))
                Debugg.Log($"{addType} writer has already exists for {sm.TypeFullName}.");
            else
            {
                dict[sm.TypeFullName] = sm;
                Debugg.Log($"-- Added {sm.TypeFullName} to writer {addType}. New Count is {dict.Count}.");
            }
        }

        /// <summary>
        /// Adds to readers.
        /// </summary>
        public void AddReadMethod(SerializerMethod sm, AddSerializerType addType)
        {
            Dictionary<string, SerializerMethod> dict = (addType == AddSerializerType.Full) ? _readMethods : _readDeltaMethods;
            if (dict.ContainsKey(sm.TypeFullName))
                Debugg.Log($"{addType} reader has already exists for {sm.TypeFullName}.");
            else
                dict[sm.TypeFullName] = sm;
        }

        /// <summary>
        /// Returns a writer.
        /// </summary>
        public SerializerMethod GetWriteMethod(string typeFullName, GetSerializerType getType) => GetSerializerMethod(typeFullName, getType, true);

        /// <summary>
        /// Returns a reader.
        /// </summary>
        public SerializerMethod GetReadMethod(string typeFullName, GetSerializerType getType) => GetSerializerMethod(typeFullName, getType, false);

        /// <summary>
        /// Returns a reader or writer.
        /// </summary>
        private SerializerMethod GetSerializerMethod(string typeFullName, GetSerializerType getType, bool writer)
        {
            SerializerMethod result;
            if (getType == GetSerializerType.Full)
            {
                Dictionary<string, SerializerMethod> dict = (writer) ? _writeMethods : _readMethods;
                dict.TryGetValue(typeFullName, out result);
            }
            else if (getType == GetSerializerType.Delta)
            {
                Dictionary<string, SerializerMethod> dict = (writer) ? _writeDeltaMethods : _readDeltaMethods;
                dict.TryGetValue(typeFullName, out result);
            }
            else if (getType == GetSerializerType.FavorFull || getType == GetSerializerType.FavorDelta)
            {
                Dictionary<string, SerializerMethod> dictA;
                Dictionary<string, SerializerMethod> dictB;
                if (getType == GetSerializerType.FavorFull)
                {
                    dictA = (writer) ? _writeMethods : _readMethods;
                    dictB = (writer) ? _writeDeltaMethods : _readDeltaMethods;
                }
                else
                {
                    dictA = (writer) ? _writeDeltaMethods : _readDeltaMethods;
                    dictB = (writer) ? _writeMethods : _readMethods;
                }

                //Try A first, then B.
                if (!dictA.TryGetValue(typeFullName, out result))
                    dictB.TryGetValue(typeFullName, out result);
            }
            else
            {
                Debugg.Log($"SerializerType {getType} is unhandled.");
                result = null;
            }

            return result;
        }

        /// <summary>
        /// Returns the collection containing all delta writers.
        /// </summary>
        public IReadOnlyDictionary<string, SerializerMethod> GetWriteMethods() => _writeMethods;

        /// <summary>
        /// Returns the collection containing all delta writers.
        /// </summary>
        public IReadOnlyDictionary<string, SerializerMethod> GetWriteDeltaMethods() => _writeDeltaMethods;

        /// <summary>
        /// Returns the collection containing all delta readers.
        /// </summary>
        public IReadOnlyDictionary<string, SerializerMethod> GeReadMethods() => _readMethods;

        /// <summary>
        /// Returns the collection containing all delta readers.
        /// </summary>
        public IReadOnlyDictionary<string, SerializerMethod> GeReadDeltaMethods() => _readDeltaMethods;

        #endregion


        /// <summary>
        /// Adds default normal writers.
        /// </summary>
        private void AddDefaultWriteMethods(IAssemblySymbol runtimeAssemblySymbol)
        {
            if (!runtimeAssemblySymbol.GetINamedTypeSymbol(FishNetConstants.Writer_FullName, out INamedTypeSymbol? nameTypeSymbol))
                return;

            foreach (IMethodSymbol methodSymbol in nameTypeSymbol!.GetMembers().OfType<IMethodSymbol>())
            {
                AddSerializerType addType = AddSerializerType.Unset;
                string typeFullName = string.Empty;
                //Full write.
                if (methodSymbol.HasAttribute(FishNetConstants.WriterAttribute_FullName, out _))
                {
                    typeFullName = methodSymbol.Parameters.First().Type.GetTypeFullName();
                    addType = AddSerializerType.Full;
                }
                //Delta write.
                else if (methodSymbol.HasAttribute(FishNetConstants.DeltaWriterAttribute_FullName, out _))
                {
                    typeFullName = methodSymbol.Parameters.First().Type.GetTypeFullName();
                    addType = AddSerializerType.Delta;
                }

                if (addType != AddSerializerType.Unset)
                    AddWriteMethod(new DeltaSerializerMethod(typeFullName, methodSymbol.Name), addType);
            }
        }

    }
}