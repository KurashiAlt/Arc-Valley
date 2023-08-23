using Arc;
using ArcInstance;
using Pastel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

Instance arc = new();

//if (args.Contains("--kurashi"))
//{
//    Dictionary<string, string> Loc = new();
//
//    LoadFolder("C:\\SteamLibrary\\steamapps\\common\\Europa Universalis IV\\localisation", "vanilla\\", "english");
//    LoadFolder(Instance.directory+"\\target\\localisation", "\\target\\localisation", "english");
//
//    Dictionary<string, IdeaGroup> ideagroups = new();
//
//    string[] paths = new string[]
//    {
//        Instance.directory + "\\target\\common\\ideas\\00_base_ideas.txt",
//        Instance.directory + "\\target\\common\\ideas\\01_economic_ideas.txt",
//        Instance.directory + "\\target\\common\\ideas\\02_destruction_ideas.txt",
//        Instance.directory + "\\target\\common\\ideas\\03_conjuration_ideas.txt",
//        Instance.directory + "\\target\\common\\ideas\\04_standing_army_ideas.txt",
//        Instance.directory + "\\target\\common\\ideas\\05_trade_ideas.txt",
//        Instance.directory + "\\target\\common\\ideas\\06_diplomatic_ideas.txt",
//        Instance.directory + "\\target\\common\\ideas\\07_illusion_ideas.txt",
//        Instance.directory + "\\target\\common\\ideas\\08_alteration_ideas.txt",
//        Instance.directory + "\\target\\common\\ideas\\09_state_administration_ideas.txt",
//        Instance.directory + "\\target\\common\\ideas\\10_mixed_ideas.txt",
//        //Instance.directory + "\\target\\common\\ideas\\11_racial_ideas.txt",
//        Instance.directory + "\\target\\common\\ideas\\12_hidden_ideas.txt",
//    };
//    //Directory.GetFiles(Instance.directory + "\\target\\common\\ideas")
//    foreach (string path in paths)
//    {
//        try
//        {
//            Block code = Parser.ParseCode(Parser.Preprocessor(File.ReadAllText(path)));
//
//            Walker i = new(code);
//            do
//            {
//                string id = i.Current;
//                i = Args.GetArgs(i, out Args nArgs);
//
//                ArcList<Idea> n = new();
//                string[] expected = new string[]
//                {
//            "category",
//            "trigger",
//            "start",
//            "bonus",
//            "ai_will_do"
//                };
//                int f = 1;
//                foreach (var v in from r in nArgs.keyValuePairs where !expected.Contains(r.Key) select r)
//                {
//                    try
//                    {
//                        n.Values.Add(new(
//                            $"{id}_{f}",
//                            new(Loc[$"{v.Key}"]),
//                            new(Loc[$"{v.Key}_desc"]),
//                            new(v.Value)
//                        ));
//                    }
//                    catch(Exception)
//                    {
//                        Console.WriteLine(v.Key);
//                        throw;
//                    }
//                    f++;
//                }
//
//                ideagroups.Add(id, new(
//                    id,
//                    new(1),
//                    new(Loc[$"{id}"]),
//                    nArgs.Get(ArcString.Constructor, "category"),
//                    n,
//                    nArgs.Get(ArcBlock.Constructor, "start", new()),
//                    nArgs.Get(ArcBlock.Constructor, "bonus"),
//                    nArgs.Get(ArcBlock.Constructor, "trigger", new()),
//                    nArgs.Get(ArcBlock.Constructor, "ai_will_do", new())
//                ));
//            } while (i.MoveNext());
//        }
//        catch(Exception)
//        {
//            Console.WriteLine(path);
//            throw;
//        }
//    }
//
//    foreach(IdeaGroup? i in ideagroups.Values)
//    {
//        Console.WriteLine($@"new idea_group {i.Id} = {{ 
//    name = {i.Name}
//
//    category = {i.Category}
//    trigger = {{
//{qFormat(i.Trigger, 2)}
//    }}
//
//    ideas = {{".Replace("    ", "\t"));
//        foreach(Idea? k in i.Ideas.Values)
//        {
//            Console.WriteLine($"\t\t{{");
//            Console.WriteLine($"\t\t\tname = {k.Name}");
//            Console.WriteLine($"\t\t\tdesc = {k.Desc}");
//            Console.WriteLine($"\t\t\tmodifier = {{");
//            Console.WriteLine(qFormat(k.Modifier, 4));
//            Console.WriteLine($"\t\t\t}}");
//            Console.WriteLine($"\t\t}}");
//        }
//    Console.WriteLine($@"    }}
//
//    bonus = {{
//{qFormat(i.Bonus, 2)}
//    }}
//}}".Replace("    ", "\t"));
//    }
//
//    string qFormat(ArcBlock v, int tab)
//    {
//        return IndentString(Parser.FormatCode(string.Join(' ', v.Value)).TrimEnd(), tab).TrimEnd();
//    }
//    string IndentString(string input, int amount)
//    {
//        if (amount < 0)
//        {
//            throw new ArgumentException("Indent amount must be non-negative.");
//        }
//
//        string[] lines = input.Split('\n');
//        StringBuilder indentedString = new StringBuilder();
//
//        foreach (string line in lines)
//        {
//            indentedString.Append(new string('\t', amount));
//            indentedString.AppendLine(line);
//        }
//
//        return indentedString.ToString();
//    }
//
//    void LoadFolder(string path, string tellpath, string language)
//    {
//        //Console.WriteLine($"Reading Folder {tellpath}".Pastel(ConsoleColor.Cyan));
//        string[] Folders = Directory.GetDirectories(path);
//        string[] Files = Directory.GetFiles(path);
//
//        foreach (string file in Files)
//        {
//            if (file.Contains($"{language}"))
//                LoadFile(file, file.Replace(path, ""));
//        }
//
//        foreach (string folder in Folders)
//        {
//            LoadFolder(folder, folder.Replace(Instance.directory, ""), language);
//        }
//    }
//    void LoadFile(string path, string tellpath)
//    {
//        //Console.WriteLine($"Reading File \t\t{tellpath}".Pastel(ConsoleColor.Yellow));
//        string[] file = File.ReadAllLines(path);
//
//        for (int i = 0; i < file.Length; i++)
//        {
//            string s = file[i];
//            Match m = new Regex(" (\\S+):\\d*\\s*(\"[^\\n]*\")").Match(s);
//            if (m.Success)
//            {
//                if (Loc.ContainsKey(m.Groups[1].Value))
//                {
//                    Loc[m.Groups[1].Value] = m.Groups[2].Value;
//                }
//                else
//                {
//                    Loc.Add(m.Groups[1].Value, m.Groups[2].Value);
//                }
//            }
//        }
//    }
//
//    return 0;
//}

Stopwatch s = Stopwatch.StartNew();
try
{
	arc.Run(args, ref s);
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