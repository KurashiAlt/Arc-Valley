using Arc;
using Pastel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
namespace ArcInstance
{
    public partial class Instance
    {
        public static readonly string directory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly Dictionary<string, string> Localisation = new();
        public static readonly List<string> Vanilla = new();
#pragma warning disable CS8618
        public static string[] Args;
#pragma warning restore CS8618
        public static readonly string headers = @"
/replace p@ with provinces:
/replace c@ with countries:
/replace a@ with areas:
/replace r@ with regions:
/replace s@ with superregions:
";
        /*
/replace $ with local:
delete local
object local = { }
        */
        public static Dictionary<string, ILintCommand> linterCommands = new();
        public static bool Lint = true;
        public void Run(string[] args, ref Stopwatch timer)
        {
            //args = new string[] { "--lint", "--nudge", "C:\\Users\\Jesse\\Documents\\Paradox Interactive\\Europa Universalis IV" };

            Args = args;
            Lint = Args.Contains("--lint");
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en");

            Compiler.directory = directory;
            Compiler.owner = this;



            string[] locations = File.ReadAllLines(Path.Combine(Compiler.directory, "arc.defines"));

            foreach (string location in locations)
            {
                TimeSpan start = timer.Elapsed;
                LoadTarget(location);
                TimeSpan end = timer.Elapsed;
                Console.WriteLine($"{$"Finished Loading {location}".PadRight(50).Pastel(ConsoleColor.Yellow)}{$"{(end - start).Milliseconds.ToString().PadLeft(5)} Milliseconds".Pastel(ConsoleColor.Red)}");
            }

            if (Lint)
            {
                if (Args.Contains("--nudge"))
                {
                    string path = Args[Array.IndexOf(Args, "--nudge") + 1];
                    Walker g;

                    string tradenodePath = $"{path}/common/tradenodes/00_tradenodes.txt";

                    if (File.Exists(tradenodePath))
                    {
                        string tradenodeFile = File.ReadAllText(tradenodePath);
                        tradenodeFile = LintEqualFixer().Replace(tradenodeFile, "$1 = $2");
                        Block tradenodeCode = Parser.ParseCode(tradenodeFile);

                        g = new(tradenodeCode);
                        do
                        {
                            string key = g.Current;
                            g = Arc.Args.GetArgs(g, out Args tradenode, multiKey: true);

                            Block outgoings = new("{");
                            if (tradenode.keyValuePairs == null) throw new Exception();
                            foreach (KeyValuePair<string, Block> kvp in tradenode.keyValuePairs)
                            {
                                if (!kvp.Key.StartsWith("outgoing")) continue;


                                Walker v = new(kvp.Value);
                                v = Arc.Args.GetArgs(v, out Args outArgs, 2);

                                LinkedListNode<Word>? tradenodeName = outArgs.Get("name").First;
                                if (tradenodeName == null) throw new Exception();
                                string tradenodeId = tradenodeName.Value.value.Trim('"')[..^3];
                                outgoings.Add(
                                    "{",
                                        "node", "=", tradenodeId,
                                        "path", "=", "{"
                                );
                                foreach (Word w in outArgs.Get("path"))
                                {
                                    if (w.value == "{" || w.value == "}") continue;
                                    outgoings.Add(GetProvinceById(int.Parse(w.value) - 1).Item1);
                                }
                                outgoings.Add(
                                        "}",
                                        "control", "=", "{"
                                );
                                foreach (Word w in outArgs.Get("control"))
                                {
                                    if (w.value == "{" || w.value == "}") continue;
                                    outgoings.Add(w.value);
                                }
                                outgoings.Add(
                                        "}",
                                    "}"
                                );

                            }
                            outgoings.Add("}");
                            ((LintAdd)linterCommands[$"tradenode~{key[..^3]}"]).Args.keyValuePairs["outgoings"] = outgoings;
                        } while (g.MoveNext());

                        File.Delete(path + "/common/tradenodes/00_tradenodes.copy");
                        File.Copy(tradenodePath, path + "/common/tradenodes/00_tradenodes.copy");
                        File.Delete(tradenodePath);
                    }

                    string positionPath = $"{path}/map/positions.txt";

                    if (File.Exists(positionPath))
                    {
                        string positionFile = File.ReadAllText(positionPath);
                        positionFile = LintEqualFixer().Replace(positionFile, "$1 = $2");
                        Block positionCode = Parser.ParseCode(positionFile);

                        g = new(positionCode);
                        do
                        {
                            string key = g.Current;
                            g = Arc.Args.GetArgs(g, out Args pos);
                            LintAdd ar = GetProvinceById(int.Parse(key)-1).Item2;
                            if (pos.keyValuePairs == null) throw new Exception();
                            ar.Args["position"] = pos.keyValuePairs["position"];
                            ar.Args["rotation"] = pos.keyValuePairs["rotation"];
                            ar.Args["height"] = pos.keyValuePairs["height"];
                        } while (g.MoveNext());

                        File.Delete(path + "/map/positions.copy");
                        File.Copy(positionPath, path + "/map/positions.copy");
                        File.Delete(positionPath);
                    }

                    static (string, LintAdd) GetProvinceById(int i)
                    {
                        int h = 0;
                        foreach(KeyValuePair<string, ILintCommand> v in linterCommands)
                        {
                            if (v.Key.StartsWith("province"))
                            {
                                if (h == i)
                                {
                                    return (v.Key.Split('~')[1], (LintAdd)v.Value);
                                }

                                h++;
                            }
                        }
                        throw new Exception();
                    }
                }

                List<string> nLocations = new();
                foreach(string location in locations)
                {
                    if (location.EndsWith("/"))
                    {
                        string[] files = Directory.GetFiles(location);
                        foreach (string file in files)
                        {
                            nLocations.Add(file);
                        }
                    }
                    else
                    {
                        nLocations.Add(location);
                    }
                }

                foreach(string location in nLocations)
                {
                    string rel = Path.GetRelativePath(directory, location);

                    StringBuilder sb = new();
                    foreach(KeyValuePair<string, ILintCommand> command in from lin in linterCommands where lin.Value.GetFile() == rel select lin)
                    {
                        sb.Append($"{command.Value.Lint(command.Key)} ");
                    }
                    OverwriteFile(rel, FormatArc(sb.ToString()), false);
                }
            }
            else
            {
                Func<string>[] Transpilers =
                {
                    ReligionGroup.Transpile,
                    PersonalDeity.Transpile,
                    Event.Transpile,
                    Incident.Transpile,
                    Adjacency.Transpile,
                    Area.Transpile,
                    Bookmark.Transpile,
                    Region.Transpile,
                    Superregion.Transpile,
                    Province.Transpile,
                    TradeGood.Transpile,
                    Terrain.Transpile,
                    Blessing.Transpile,
                    Building.Transpile,
                    Country.Transpile,
                    ChurchAspect.Transpile,
                    AdvisorType.Transpile,
                    TradeNode.Transpile,
                    IdeaGroup.Transpile,
                    Relation.Transpile,
                    CultureGroup.Transpile,
                    MissionSeries.Transpile,
                    EstateAgenda.Transpile,
                    EstatePrivilege.Transpile,
                    Estate.Transpile,
                    GovernmentReform.Transpile,
                    Unit.Transpile,
                    GreatProject.Transpile,
                    MercenaryCompany.Transpile,
                    Advisor.Transpile,
                    TranspileOnActions,
                    SpecialUnitTranspile,

                    EventModifier.Transpile,
                    TranspileLocalisations,
                };

                foreach(Func<string> transpiler in Transpilers)
                {
                    TimeSpan start = timer.Elapsed;
                    string type = transpiler();
                    TimeSpan end = timer.Elapsed;
                    Console.WriteLine($"{$"Finished Transpiling {type}".PadRight(50).Pastel(ConsoleColor.Cyan)}{$"{(end - start).Milliseconds.ToString().PadLeft(5)} Milliseconds".Pastel(ConsoleColor.Red)}");
                }
            }

            return;
        }
        public static string SpecialUnitTranspile()
        {
            IArcObject carolean = (IArcObject)((IArcObject)Compiler.global["special_units"]).Get("carolean");

            OverwriteFile("target/common/static_modifiers/special_units.txt", string.Join(' ', new Block(
                    ((ArcBlock)carolean.Get("modifier")).Compile("carolean_regiment")
                )));

            Block defines = new()
            {
                "NDefines.NMilitary.CAROLEAN_STARTING_STRENGTH", "=", carolean.Get("starting_strength"),
                "NDefines.NMilitary.CAROLEAN_STARTING_MORALE", "=", carolean.Get("starting_morale"),
                "NDefines.NMilitary.CAROLEAN_BASE_COST_MODIFIER", "=", carolean.Get("base_cost_modifier"),
                "NDefines.NMilitary.CAROLEAN_USES_CONSTRUCTION", "=", carolean.Get("uses_construction"),
            };
            OverwriteFile("target/common/defines/special_units.lua", string.Join(' ', defines));

            Walker i = new(((ArcBlock)carolean.Get("localisation")).Value);
            do
            {
                string key = i.Current;
                if (!i.MoveNext()) throw new Exception();
                if (i.Current != "=") throw new Exception();
                if (!i.MoveNext()) throw new Exception();
                string value = i.Current;
                Localisation.Add(key, value);
            } while (i.MoveNext());
            return "Special Units";
        }
        public static string FormatArc(string s)
        {
            try
            {
                List<(List<string> line, int tabs)> sb = new();
                Block code = Parser.ParseCode(s);
                int tab = 0;
                bool eq = false;

                sb.Add((new(), 0));
                foreach (Word w in code)
                {
                    if (eq)
                    {
                        if (w == "{") tab++;
                        if (w == "}") tab--;

                        eq = false;
                        sb.Last().line.Add(w);
                        sb.Add((new(), tab));
                        continue;
                    }

                    if (w == "}")
                    {
                        tab--;

                        sb.Add((new(), tab));
                        sb.Last().line.Add(w);
                        sb.Add((new(), tab));
                        continue;
                    }

                    if (w == "{")
                    {

                        sb.Add((new(), tab));
                        tab++;
                        sb.Last().line.Add(w);
                        sb.Add((new(), tab));
                        continue;
                    }

                    if (w == "=" || w == ":=" || w == "+=" || w == "-=") eq = true;
                    sb.Last().line.Add(w);
                }
                var a = from c in sb select (new string('\t', c.tabs) + string.Join(' ', c.line));
                return string.Join(Environment.NewLine, from f in a where !string.IsNullOrWhiteSpace(f) select f);
            }
            catch (Exception)
            {
                Console.WriteLine(s);
                throw;
            }
        }
        public static void OverwriteFile(string path, string text, bool AllowFormatting = true)
        {
            path = Path.Combine(directory, path);
            try
            {
                if (AllowFormatting && Args.Contains("--format")) text = Parser.FormatCode(text);
            }
            catch(Exception)
            {
                Console.WriteLine(path);
                throw;
            }

            if(File.Exists(path))
            {
                string old = File.ReadAllText(path);
                if(text != old) File.WriteAllText(path, text);
            }
            else
            {
                File.WriteAllText(path, text);
            }
        }
        private static string TranspileLocalisations()
        {
            StringBuilder sb = new("l_english:\n");
            foreach (KeyValuePair<string, string> loc in Localisation)
            {
                sb.Append($" {loc.Key}: \"{loc.Value.Trim('"')}\"\n");
            }
            OverwriteFile("target/localisation/replace/arc5_l_english.yml", Parser.ConvertStringToUtf8Bom(sb.ToString()), false);
            return "Localisations";
        }
        private static string TranspileOnActions()
        {
            foreach (KeyValuePair<string, ArcBlock> OnAction in ((Dict<ArcBlock>)Compiler.global["on_actions"]))
            {
                string s = $"{OnAction.Key} = {{ {OnAction.Value.Compile()}}} ";
                OverwriteFile($"target/common/on_actions/{OnAction.Key}.txt", s);
            }
            return "On Actions";
        }
        public static void LoadTarget(string path)
        {
            string fileLocation = Path.Combine(directory, path);

            if (fileLocation.EndsWith("/"))
            {
                string[] files = Directory.GetFiles(fileLocation);
                foreach (string file in files)
                {
                    try
                    {
                        Compiler comp = new();
                        string fileContent = File.ReadAllText(file);
                        if (Lint) comp.LintLoad(ref linterCommands, fileContent + headers, file, true);
                        else comp.ObjectDeclare(fileContent + headers, true);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(Path.GetRelativePath(directory, file).Pastel(ConsoleColor.Red));
                        throw;
                    }
                }
            }
            else
            {
                try
                {
                    Compiler comp = new();
                    string file = File.ReadAllText(fileLocation);
                    if (Lint) comp.LintLoad(ref linterCommands, file + headers, path, true);
                    else comp.ObjectDeclare(file + headers, true);
                }
                catch (Exception)
                {
                    Console.WriteLine(Path.GetRelativePath(directory, fileLocation).Pastel(ConsoleColor.Red));
                    throw;
                }
            }
        }

        [GeneratedRegex("(\\S)=(\\S)")]
        private static partial Regex LintEqualFixer();
    }
}
