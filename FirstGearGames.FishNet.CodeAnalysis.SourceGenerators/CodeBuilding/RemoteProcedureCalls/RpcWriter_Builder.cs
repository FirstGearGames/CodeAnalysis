using System.Text;
using FirstGearGames.CodeAnalysis.Helpers;
using FirstGearGames.FishNet.CodeAnalysis.Constants;
using FirstGearGames.FishNet.CodeAnalysis.Helpers.Serializing;
using FirstGearGames.FishNet.CodeAnalysis.Receivers;
using Microsoft.CodeAnalysis;

namespace FirstGearGames.FishNet.CodeAnalysis.CodeBuilding.RemoteProcedureCalls
{
    public class RpcWriter_Builder
    {

        private static StringBuilder _stringBuilder = new();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private GeneratorExecutionContext _context;
        private GeneratorSyntaxReceiver _rootSyntaxReceiver;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void Initialize(GeneratorExecutionContext context, GeneratorSyntaxReceiver rootSyntaxReceiver)
        {
            Log("");
            Log("Initialize.");
            Log("");

            _context = context;
            _rootSyntaxReceiver = rootSyntaxReceiver;
        }

        public void CreateEmptySerializerMethods() => CreateEmptySerializerMethods(_context, _rootSyntaxReceiver);
        public void CreateSerializerBodies() => CreateSerializerBodies(_context, _rootSyntaxReceiver);

        /// <summary>
        /// Creates SerializerMethod for each type in need.
        /// </summary>
        private void CreateEmptySerializerMethods(GeneratorExecutionContext context, GeneratorSyntaxReceiver syntaxReceiver)
        {
            foreach (SerializableType item in syntaxReceiver.SerializableFinder.TypesNeedingSerializers)
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
        }

        /// <summary>
        /// Creates bodies for empty delta serializer methods.
        /// </summary>
        private void CreateSerializerBodies(GeneratorExecutionContext context, GeneratorSyntaxReceiver SyntaxReceiver)
        {
        }


        private void Log(string txt)
        {
            if (txt.Length == 0)
                Debugg.Log(txt);
            else
                Debugg.Log($"   [RemoteProcedureCalls_Writer_Builder] {txt}");
        }
    }
}
