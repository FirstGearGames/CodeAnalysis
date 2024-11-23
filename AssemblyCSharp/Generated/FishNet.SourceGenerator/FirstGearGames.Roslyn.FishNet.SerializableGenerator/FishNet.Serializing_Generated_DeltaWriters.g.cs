//FishNet generated file.
namespace FishNet.Serializing
{
	public static class Generated_DeltaWriters
	{

		public static bool GWrite___WriteDeltaClientAssembly_Player_NestedStruct(this FishNet.Serializing.Writer writer, ClientAssembly.Player.NestedStruct value0, ClientAssembly.Player.NestedStruct value1, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
		{
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				System.UInt64 optionsFlags = (System.UInt64)options;
				writer.WriteUnsignedPackedWhole(optionsFlags);
				writer.GWrite___WriteClientAssembly_Player_NestedStruct(value1);
				return true;
			}

			System.UInt64 totalFlags = (ulong)options;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

//>>>  GenericName T[]. IsGeneric True TypeFullNameWithGenericArguments System.Byte[] 
//typeFullName System.Byte[]. Valid? True. MethodName WriteDeltaArray
//Generated? False
			if (pooledWriter.WriteDeltaArray<System.Byte>(value0.ByteArr, value1.ByteArr))
				totalFlags += 4;

//>>>  GenericName T[]. IsGeneric True TypeFullNameWithGenericArguments ClientAssembly.Player.NestedStruct[] 
//typeFullName ClientAssembly.Player.NestedStruct[]. Valid? True. MethodName WriteDeltaArray
//Generated? False
			if (pooledWriter.WriteDeltaArray<ClientAssembly.Player.NestedStruct>(value0.StructArr, value1.StructArr))
				totalFlags += 8;

//>>>  GenericName System.Collections.Generic.List<T>. IsGeneric True TypeFullNameWithGenericArguments System.Collections.Generic.List<ClientAssembly.Player.NestedStruct> 
//typeFullName System.Collections.Generic.List<ClientAssembly.Player.NestedStruct>. Valid? False. MethodName 
			//Delta writer could not be found for type System.Collections.Generic.List<ClientAssembly.Player.NestedStruct>. A full serializer will be used. Please report this note.
			pooledWriter.WriteList(value1.StructLst);
			totalFlags += 16;

//>>>  GenericName . IsGeneric False TypeFullNameWithGenericArguments System.String 
//typeFullName System.String. Valid? False. MethodName 
			//Delta writer could not be found for type System.String. A full serializer will be used. Please report this note.
			pooledWriter.WriteString(value1.String);
			totalFlags += 32;

//>>>  GenericName System.Collections.Generic.List<T>. IsGeneric True TypeFullNameWithGenericArguments System.Collections.Generic.List<ClientAssembly.Player.NestedStruct> 
//typeFullName System.Collections.Generic.List<ClientAssembly.Player.NestedStruct>. Valid? False. MethodName 
			//Delta writer could not be found for type System.Collections.Generic.List<ClientAssembly.Player.NestedStruct>. A full serializer will be used. Please report this note.
			pooledWriter.WriteList(value1.LstStruct);
			totalFlags += 64;

//>>>  GenericName . IsGeneric False TypeFullNameWithGenericArguments System.ArraySegment<System.Byte> 
//typeFullName System.ArraySegment<System.Byte>. Valid? True. MethodName WriteDeltaArraySegment
//Generated? False
			if (pooledWriter.WriteDeltaArraySegment<System.Byte>(value0.ArrSegment, value1.ArrSegment))
				totalFlags += 128;

//>>>  GenericName . IsGeneric False TypeFullNameWithGenericArguments System.Object 
//typeFullName System.Object. Valid? False. MethodName 
			//Delta writer could not be found for type System.Object. A full serializer will be used. Please report this note.
			//Full serializer not found for System.Object. This will cause failure at runtime.
			pooledWriter.Write<System.Object>(value1.ObjectType);
			totalFlags += 256;

//>>>  GenericName . IsGeneric False TypeFullNameWithGenericArguments ClientAssembly.AnyType<System.Boolean> 
//typeFullName ClientAssembly.AnyType<System.Boolean>. Valid? False. MethodName 
			//Delta writer could not be found for type ClientAssembly.AnyType<System.Boolean>. A full serializer will be used. Please report this note.
			//Full serializer not found for ClientAssembly.AnyType<System.Boolean>. This will cause failure at runtime.
			pooledWriter.Write<ClientAssembly.AnyType<System.Boolean>>(value1.GenericObjectType);
			totalFlags += 512;

			System.Boolean changed = (totalFlags != 0);
			if (changed)
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.WriteArraySegment(pooledWriter.GetArraySegment());
			}
			pooledWriter.Store();

			return changed;
		}

		public static bool GWrite___WriteDeltaClientAssembly_Player_MyStructC(this FishNet.Serializing.Writer writer, ClientAssembly.Player.MyStructC value0, ClientAssembly.Player.MyStructC value1, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
		{
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				System.UInt64 optionsFlags = (System.UInt64)options;
				writer.WriteUnsignedPackedWhole(optionsFlags);
				writer.GWrite___WriteClientAssembly_Player_MyStructC(value1);
				return true;
			}

			System.UInt64 totalFlags = (ulong)options;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

//>>>  GenericName . IsGeneric False TypeFullNameWithGenericArguments System.Boolean 
//typeFullName System.Boolean. Valid? False. MethodName 
			//Delta writer could not be found for type System.Boolean. A full serializer will be used. Please report this note.
			pooledWriter.WriteBoolean(value1.Works);
			totalFlags += 4;

			System.Boolean changed = (totalFlags != 0);
			if (changed)
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.WriteArraySegment(pooledWriter.GetArraySegment());
			}
			pooledWriter.Store();

			return changed;
		}

		[UnityEngine.RuntimeInitializeOnLoadMethod]
		public static void InitializeSerializers()
		{
			
			
		}
	}
}
