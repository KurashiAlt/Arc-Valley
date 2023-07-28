using Arc;
namespace ArcTests;
public static partial class Tests
{
    public static void PreprocessorTest()
    {
        Compiler comp = new();

        string result = comp.Compile($@"
/replace p@ with globals:provinces:
/replace :: with globals:

p@ = 1
:: = 1
		", true).Trim();

        if (!ResultMatches(result, @"
globals:provinces: = 1
globals: = 1
"))
            throw new Exception("Failure on Preprocessor Test");


        Console.WriteLine("Success on Preprocessor Test");
    }
}