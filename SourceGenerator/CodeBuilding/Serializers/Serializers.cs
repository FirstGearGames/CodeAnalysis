using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Linq;
using FishNet.CodeAnalysis.Extensions;
using RoslynLearning.Helpers;
using SourceGenerating.Constants;
using SourceGenerator.Extensions;
using System;

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

        //Consts.
        public const string Generated_ValueParameter_Prefix = "value";

        public void Initialize(IAssemblySymbol runtimeAssemblySymbol)
        {
            AddDefaultWriteMethods(runtimeAssemblySymbol);
            AddDefaultReadMethods(runtimeAssemblySymbol);
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
        public IReadOnlyDictionary<string, SerializerMethod> GetReadDeltaMethods() => _readDeltaMethods;

        #endregion


        /// <summary>
        /// Adds default normal writers.
        /// </summary>
        private void AddDefaultWriteMethods(IAssemblySymbol runtimeAssemblySymbol)
        {
            if (!runtimeAssemblySymbol.GetINamedTypeSymbol(FishNetConstants.Writer_FullName, out INamedTypeSymbol? nameTypeSymbol)) return;
            if (nameTypeSymbol == null) return;

            foreach (IMethodSymbol methodSymbol in nameTypeSymbol.GetMembers().OfType<IMethodSymbol>())
            {
                AddSerializerType addType = AddSerializerType.Unset;
                //Full write.
                if (methodSymbol.HasAttribute(FishNetConstants.DefaultWriterAttribute_FullName, out _))
                    addType = AddSerializerType.Full;
                //Delta write.
                else if (methodSymbol.HasAttribute(FishNetConstants.DefaultDeltaWriterAttribute_FullName, out _))
                    addType = AddSerializerType.Delta;

                if (addType != AddSerializerType.Unset)
                {
                    string typeFullName = methodSymbol.Parameters.First().Type.GetTypeFullName();
                    AddWriteMethod(new DeltaSerializerMethod(typeFullName, methodSymbol.Name), addType);
                }
            }
        }

        /// <summary>
        /// Adds default normal writers.
        /// </summary>
        private void AddDefaultReadMethods(IAssemblySymbol runtimeAssemblySymbol)
        {
            if (!runtimeAssemblySymbol.GetINamedTypeSymbol(FishNetConstants.Reader_FullName, out INamedTypeSymbol? nameTypeSymbol)) return;
            if (nameTypeSymbol == null) return;

            Debugg.Log("Runnnnninggngng");

            foreach (IMethodSymbol methodSymbol in nameTypeSymbol.GetMembers().OfType<IMethodSymbol>())
            {
                AddSerializerType addType = AddSerializerType.Unset;
                //Full read.
                if (methodSymbol.HasAttribute(FishNetConstants.DefaultReaderAttribute_FullName, out _))
                    addType = AddSerializerType.Full;
                //Deltaread.
                else if (methodSymbol.HasAttribute(FishNetConstants.DefaultDeltaReaderAttribute_FullName, out _))
                    addType = AddSerializerType.Delta;

                if (addType != AddSerializerType.Unset)
                {
                    string typeFullName = methodSymbol.ReturnType.GetTypeFullName();
                    AddReadMethod(new DeltaSerializerMethod(typeFullName, methodSymbol.Name), addType);
                }
            }
        }


        /// <summary>
        /// Returns the parameter name used for values within a generated delta writer.
        /// </summary>
        public string GetValueParameterName(int parameterIndex) => $"{Generated_ValueParameter_Prefix}{parameterIndex}";

        /// <summary>
        /// Returns if a serializer can be generated for a symbol.
        /// </summary>
        public bool CanGenerateFieldSerializer(ISymbol symbol, out IFieldSymbol? fieldSymbol)
        {
            fieldSymbol = symbol as IFieldSymbol;
            if (fieldSymbol == null) return false;
            if (fieldSymbol.HasAttribute(FishNetConstants.ExcludeSerializationAttribute_FullName)) return false;

            return true;
        }

        /// <summary>
        /// Gets fields which can be serialized over the network.
        /// </summary>
        /// <param name="namedTypeSymbol"></param>
        /// <returns></returns>
        public List<IFieldSymbol> GetSerializableFieldSymbols(INamedTypeSymbol? namedTypeSymbol)
        {
            List<IFieldSymbol> results = new();
            if (namedTypeSymbol == null) return results;

            //Call write for all members.
            foreach (ISymbol symbol in namedTypeSymbol.GetMembers())
            {
                if (CanGenerateFieldSerializer(symbol, out IFieldSymbol? fieldSymbol))
#pragma warning disable CS8604 // Possible null reference argument.
                    results.Add(fieldSymbol);
#pragma warning restore CS8604 // Possible null reference argument.
            }

            return results;
        }

        /// <summary>
        /// Returns if a type qualifies for a delta serializer.
        /// </summary>
        public bool CanCreateDeltaSerializer(INamedTypeSymbol? namedTypeSymbol, bool throwCritical)
        {
            //Not a supported type. Must be a user defined struct or class.
            if (namedTypeSymbol == null || !namedTypeSymbol.IsUserDefinedClassOrStruct())
                return false;

            //Too many parameters to process as a delta writer due to not enough flags.
            if (namedTypeSymbol.MemberNames.Count() >= 63)
            {
                if (throwCritical)
                    throw new Exception($"Type {namedTypeSymbol.GetTypeFullName()} exceeds the maximum of 63 field members. Reduce the amount of field members or encapsulate members.");
                return false;
            }

            return true;
        }
    }
}