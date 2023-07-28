using Arc;
namespace ArcTests;
public static partial class Tests
{
    public static void InheritTest()
    {
        Compiler comp = new();

        string result = comp.Compile($@"
object FirstLayer = {{
	object SecondLayer = {{
		int A = 1
		int B = 2
		int C = 3
	}}
	inherit = SecondLayer
}}
inherit = FirstLayer
A
B
C
		").Trim();

        if (!ResultMatches(result, @"1 2 3"))
            throw new Exception("Failure on Inherit Test");

        Console.WriteLine("Success on Inherit Test");
    }
}