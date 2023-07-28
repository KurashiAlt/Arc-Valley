using ArcInstance;
using Pastel;
using System.Diagnostics;
Instance arc = new();
Stopwatch s = Stopwatch.StartNew();
try
{
	arc.Run(args);
}
catch (Exception e)
{
	Console.WriteLine(e);
}
Console.WriteLine($"Transpilation took: {(((double)s.ElapsedMilliseconds) / 1000).ToString("0.000")} seconds".Pastel(ConsoleColor.Red));

if (!IsRunningInConsoleOrPowerShell())
{
    Console.WriteLine("Press any key to exit");
    Console.ReadKey();
}

return 0;

bool IsRunningInConsoleOrPowerShell()
{
    string title = Console.Title;

    // Check if the console or PowerShell window title contains certain keywords
    if (title.Contains("cmd.exe") || title.Contains("powershell.exe"))
    {
        return true;
    }

    return false;
}