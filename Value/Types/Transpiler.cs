public static partial class Transpiler
{
    public static List<ArcEffect> SimpleTranspilers = new();
    public static string TranspileSimples()
    {
        foreach (ArcEffect obj in SimpleTranspilers)
        {
            obj.Compile();
        }

        return "Simple Dynamic Classes";
    }
}
