using System.Collections.Generic;
using System.Text;
using FirstGearGames.Roslyn.CodeBuilding;
using FirstGearGames.Roslyn.Extensions;
using FirstGearGames.Roslyn.FishNet.Constants;
using FirstGearGames.Roslyn.FishNet.Helpers;
using FirstGearGames.Roslyn.FishNet.Receivers;
using FirstGearGames.Roslyn.FishNet.Serializing;
using FirstGearGames.Roslyn.Native.Constants;
using Microsoft.CodeAnalysis;
using Roslyn.FishNet.CodeBuilding;
using RoslynCodeBuilder = FirstGearGames.Roslyn.CodeBuilding.CodeBuilder;

namespace FirstGearGames.Roslyn.FishNet.CodeBuilding
{
    public class Reader_Builder
    {
        private const string Generated_Class_Name = "Generated_Readers";
        private const string Generated_Method_Prefix = $"{FishNetConstants.GeneratedReaderPrefix}Read";
        private const string Generated_ReaderParameter_Name = "reader";
        public const string InitializeOnLoad_Method_Name = Writer_Builder.InitializeOnLoad_Method_Name;

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

        public void Execute()
        {
        }

        public SerializerMethod CreateSerializerMethod(ITypeSymbol typeSymbol)
        {
            return new SerializerMethod(typeSymbol, $"{FishNetConstants.Reader_Read_Name}<{typeSymbol.GetTypeSymbolFullName(metadataName: false)}>");
        }
    }
}