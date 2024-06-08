namespace RoslynLearning.Helpers.CodeBuilding
{
    [System.Flags]
    public enum DeltaWriterOption
    {
        Unset = 0,
        FullWrite = 1,
        RootWrite = 2,
    }

    public static class DeltaWriterOptionExtensions
    {
        public static bool Contains(this DeltaWriterOption whole, DeltaWriterOption part) => (whole & part) == part;
    }
}