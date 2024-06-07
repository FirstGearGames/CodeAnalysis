namespace GenerateTest
{
	public static class GeneratedWriters
	{
// Creating DeltaWriter for ClientAssembly.MyStruct
   // Type ClientAssembly.MyStruct is supported.
   // Already created System.Int32
       //Member fullName is System.Int32, Value
		public static void WriteDelta_ClientAssembly_MyStruct(this FishNet.Serializing.Writer writer, ClientAssembly.MyStruct valueA,  ClientAssembly.MyStruct valueB)
		{

           //Member fullName is System.Int32, ClientAssembly.MyStruct
          writer.WriteDeltaInt32(valueA, valueB);
		}

	}
}
