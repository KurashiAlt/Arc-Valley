using Arc;
namespace ArcTests;
public static partial class Tests
{
    public static void ForeachTest()
    {
        Compiler comp = new();

        string result = comp.Compile(@"
list args = [
	kazakh
	khalkha
	korchin
]
OR = {
	foreach $culture in args = {
		`primary_culture` = $culture
	}
}

object obj = {
	int a = 10
	int b = 20
	int c = 30
}
list lst = [
	10 20 30
]
foreach kvp in obj = {
	kvp:value = yes
}
foreach value in lst = {
	value = yes
}
		").Trim();

        if (!ResultMatches(result, @"
OR = {
	primary_culture = kazakh
	primary_culture = khalkha
	primary_culture = korchin
}
10 = yes
20 = yes
30 = yes
10 = yes
20 = yes
30 = yes
"))
            throw new Exception();

        Console.WriteLine("Success on Foreach Test");
    }
}