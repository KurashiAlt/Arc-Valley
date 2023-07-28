using Arc;
namespace ArcTests;
public static partial class Tests
{
    static bool ResultMatches(string result, string expected)
    {
        result = result.RegRep("\\s+", " ").Trim();
        expected = expected.RegRep("\\s+", " ").Trim();
        return result == expected;
    }
}