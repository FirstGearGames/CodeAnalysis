using System.Collections.Generic;
using System.Text;
using FirstGearGames.Roslyn.Extensions;
using FirstGearGames.Roslyn.FishNet;
using FirstGearGames.Roslyn.FishNet.CodeBuilding;
using FirstGearGames.Roslyn.FishNet.Constants;
using FirstGearGames.Roslyn.FishNet.Helpers;
using FirstGearGames.Roslyn.FishNet.Receivers;
using FirstGearGames.Roslyn.FishNet.Serializing;
using FirstGearGames.Roslyn.Native.Constants;
using Microsoft.CodeAnalysis;
using RoslynCodeBuilder = FirstGearGames.Roslyn.CodeBuilding.CodeBuilder;

namespace Roslyn.FishNet.CodeBuilding
{
    public class Writer_Builder
    {
        private const string Generated_Class_Name = "Generated_Writers";
        private const string Generated_Method_Prefix = $"{FishNetConstants.GeneratedWriterPrefix}Write";
        private const string Generated_WriterParameter_Name = "writer";
        public const string InitializeOnLoad_Method_Name = "InitializeSerializers";

        private static StringBuilder _stringBuilder = new();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Serializers _serializers => _generator.Serializers;
        private SerializableGenerator _generator;
        private GeneratorExecutionContext _context;
        private GeneratorSyntaxReceiver _rootSyntaxReceiver;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void Initialize(GeneratorExecutionContext context, GeneratorSyntaxReceiver rootSyntaxReceiver, SerializableGenerator generator)
        {
            _context = context;
            _rootSyntaxReceiver = rootSyntaxReceiver;
            _generator = generator;
        }

        public void Execute() { }

        public SerializerMethod CreateWriteSerializerMethod(ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
            {
                Debugg.Log("XULL TYPE");
                Debugg.Send();
            }
            return new SerializerMethod(typeSymbol, $"{FishNetConstants.Writer_Write_Name}<{typeSymbol.GetTypeSymbolFullName()}>");
        }

    }
}