using Arc;

namespace ArcTests;
public static partial class Tests
{
    public static void VariableTest()
    {
        Compiler comp = new();

#pragma warning disable IDE0059 // Unnecessary assignment of a value
        string result = comp.Compile(@"
object province = {
	string name = """"
	block color = { }
	block history = { }
	bool impassible = no
	bool sea = no
	bool lake = no
}

object area = {
	list provinces = province[] {
		
	}
}

type stew = string
stew he = ""Ha""

float a = 10.25
int b = 52
bool c = yes
string d = ""Hello""
block e = {
	aba baba
}

object f = {
	float a = 10.25
	int b = 52
	bool c = yes
	string d = ""Hello""
	block e = {
		aba baba
	}
}

f g = {
	a = 5
}

string global:hello = ""Hello""

object aba = {
	string s = global:hello
}
object bebe = {
	string s = global:hello
}
		").Trim();
#pragma warning restore IDE0059 // Unnecessary assignment of a value

        Console.WriteLine("Success on Variable Test");
    }
}