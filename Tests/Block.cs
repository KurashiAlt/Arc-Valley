using Arc;

namespace ArcTests;
public static partial class Tests
{
    public static void BlockTest()
    {
        Compiler comp = new();

        string result = comp.Compile(@"
block primary_culture = {
	`primary_culture` = args
}
primary_culture = finnish
		").Trim();

        if (!ResultMatches(result, @"
primary_culture = finnish
"))
            throw new Exception("Failure on Block Test");

        Console.WriteLine("Success on Variable Test");
    }
}