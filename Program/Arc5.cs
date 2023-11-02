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
        public static string[] iArgs;
#pragma warning restore CS8618
        public static readonly string headers = @"
/replace p@ with provinces:
/replace c@ with countries:
/replace a@ with areas:
/replace r@ with regions:
/replace s@ with superregions:
";
        public static string TranspileTarget;
        public static string GfxFolder;
        public static string UnsortedFolder;
        public static IEnumerable<string> LoadOrder;
        public void Run(string[] args, ref Stopwatch timer)
        {
            iArgs = args;
            CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("en");

            Compiler.owner = this;

            Block va = Parser.ParseCode(File.ReadAllText(Path.Combine(directory, "arc.defines")));
            va.Prepend("{");
            va.Add("}");
            Args arcDefines = Args.GetArgs(va);

            UnsortedFolder = arcDefines.Get(ArcString.Constructor, "unsorted_target").Value;
            TranspileTarget = arcDefines.Get(ArcString.Constructor, "transpile_target").Value;
            GfxFolder = arcDefines.Get(ArcString.Constructor, "gfx_folder").Value;
            LoadOrder = from c in arcDefines.Get(ArcCode.Constructor, "load_order").Value select new string(c.value);

            foreach (string location in LoadOrder)
            {
                TimeSpan start = timer.Elapsed;
                LoadTarget(location);
                TimeSpan end = timer.Elapsed;
                Console.WriteLine($"{$"Finished Loading {location}".PadRight(50).Pastel(ConsoleColor.Yellow)}{$"{(end - start).Milliseconds,7:0} Milliseconds".Pastel(ConsoleColor.Red)}");
            }

            Func<string>[] Transpilers =
            {
                Incident.Transpile,

                TranspileOnActions,
                ReligionGroup.Transpile,
                PersonalDeity.Transpile,
                Decision.Transpile,
                Event.Transpile,
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
                GovernmentMechanic.Transpile,
                Unit.Transpile,
                GreatProject.Transpile,
                MercenaryCompany.Transpile,
                Advisor.Transpile,
                Age.Transpile,
                DiplomaticAction.Transpile,
                SpecialUnitTranspile,
                Government.Transpile,
                GovernmentNames.Transpile,
                HolyOrder.Transpile,
                ProvinceTriggeredModifier.Transpile,
                CasusBelli.Transpile,
                WarGoal.Transpile,
                ProvinceGroup.Transpile,
                AiPersonalities,
                CenterOfTrade,
                RulerPersonality.Transpile,

                OpinionModifier.Transpile,
                EventModifier.Transpile,
                TranspileLocalisations,
                Gfx,
                Unsorted,
            };

            foreach(Func<string> transpiler in Transpilers)
            {
                TimeSpan start = timer.Elapsed;
                string type = transpiler();
                start = timer.Elapsed - start;
                Console.WriteLine($"{$"Finished Transpiling {type}".PadRight(50).Pastel(ConsoleColor.Cyan)}{$"{start.TotalMilliseconds,7:0} Milliseconds".Pastel(ConsoleColor.Red)}");
            }

            OverwriteFile($"warnings.txt", string.Join('\n', warnings));

            return;
        }
        static string CenterOfTrade()
        {
            Block COTFile = new();
            foreach (KeyValuePair<string, ArcCode> cot in Compiler.GetVariable<Dict<ArcCode>>("centers_of_trade"))
            {
                cot.Value.Compile(cot.Key, ref COTFile);
            }
            OverwriteFile($"{TranspileTarget}/common/centers_of_trade/arc.txt", string.Join(' ', COTFile));
            return "Center of Trades";
        }
        static string AiPersonalities()
        {
            Block AiPersonalityFile = new();
            foreach (KeyValuePair<string, ArcTrigger> personality in Compiler.GetVariable<Dict<ArcTrigger>>("ai_personalities"))
            {
                personality.Value.Compile(personality.Key, ref AiPersonalityFile);
            }
            OverwriteFile($"{TranspileTarget}/common/ai_personalities/arc.txt", string.Join(' ', AiPersonalityFile));
            return "Ai Personalities";
        }
        static string Gfx()
        {
            if (!(iArgs.Contains("gfx") || iArgs.Contains("all"))) return "GFX folder";
            Block b = new("spriteTypes", "=", "{");

            CreateTillFolder($"{TranspileTarget}/gfx/event_pictures/arc");
            foreach (string c in GetFiles($"{GfxFolder}/event_pictures"))
            {
                string s = c.Split('\\').Last();

                b.Add(
                    "spriteType", "=", "{",
                        "name", "=", $"\"{s.Split('.').First()}\"",
                        "texturefile", "=", $"\"gfx/event_pictures/arc/{s}\"",
                    "}"
                );

                File.Delete($"{TranspileTarget}/gfx/event_pictures/arc/{s}");
                File.Copy(c, $"{TranspileTarget}/gfx/event_pictures/arc/{s}");
            }

            CreateTillFolder($"{TranspileTarget}/gfx/interface/missions");
            foreach (string c in GetFiles($"{GfxFolder}/missions"))
            {
                string s = c.Split('\\').Last();

                b.Add(
                    "spriteType", "=", "{",
                        "name", "=", $"\"{s.Split('.').First()}\"",
                        "texturefile", "=", $"\"gfx/interface/missions/{s}\"",
                    "}"
                );

                File.Delete($"{TranspileTarget}/gfx/interface/missions/{s}");
                File.Copy(c, $"{TranspileTarget}/gfx/interface/missions/{s}");
            }

            foreach (string folder in GetFolders($"{GfxFolder}/ages"))
            {
                string folderName = folder.Split('\\').Last();
                CreateTillFolder($"{TranspileTarget}/gfx/interface/ages/{folderName}");
                foreach (string file in GetFiles(folder))
                {
                    string s = file.Split('\\').Last();
                    string v = $"gfx/interface/ages/{folderName}/{s}";
                    b.Add(
                        "spriteType", "=", "{",
                            "name", "=", $"\"GFX_{s.Split('.').First()}\"",
                            "texturefile", "=", $"\"{v}\"",
                        "}"
                    );

                    string oldPath = Path.GetRelativePath(directory, file);
                    string newPath = $"{TranspileTarget}/{v}";

                    File.Delete(newPath);
                    File.Copy(oldPath, newPath);
                }
            }

            CreateTillFolder($"{TranspileTarget}/gfx/interface/buildings");
            foreach (string c in GetFiles($"{GfxFolder}/buildings"))
            {
                string s = c.Split('\\').Last();
                string oldPath = $"{GfxFolder}/buildings/{s}";
                string newPath = $"{TranspileTarget}/gfx/interface/buildings/{s}";

                b.Add(
                    "spriteType", "=", "{",
                        "name", "=", $"\"GFX_{s.Split('.').First()}\"",
                        "texturefile", "=", $"\"gfx/interface/buildings/{s.Split('.').First()}.tga\"",
                        "loadType", "=", "\"INGAME\"",
                    "}"
                );

                File.Delete(newPath);
                File.Copy(oldPath, newPath);
            }

            CreateTillFolder($"{TranspileTarget}/gfx/interface/great_projects");
            foreach (string c in GetFiles($"{GfxFolder}/great_projects"))
            {
                string s = c.Split('\\').Last();
                string oldPath = $"{GfxFolder}/great_projects/{s}";
                string newPath = $"{TranspileTarget}/gfx/interface/great_projects/{s}";

                b.Add(
                    "spriteType", "=", "{",
                        "name", "=", $"\"GFX_great_project_{s.Split('.').First()}\"",
                        "texturefile", "=", $"\"gfx/interface/great_projects/{s}\"",
                    "}"
                );

                File.Delete(newPath);
                File.Copy(oldPath, newPath);
            }

            CreateTillFolder($"{TranspileTarget}/gfx/interface/privileges");
            foreach (string c in GetFiles($"{GfxFolder}/privileges"))
            {
                string s = c.Split('\\').Last();
                string oldPath = $"{GfxFolder}/privileges/{s}";
                string newPath = $"{TranspileTarget}/gfx/interface/privileges/{s}";

                b.Add(
                    "spriteType", "=", "{",
                        "name", "=", $"\"{s.Split('.').First()}\"",
                        "texturefile", "=", $"\"gfx/interface/privileges/{s}\"",
                    "}"
                );

                File.Delete(newPath);
                File.Copy(oldPath, newPath);
            }

            CreateTillFolder($"{TranspileTarget}/gfx/interface/holy_orders");
            foreach (string c in GetFiles($"{GfxFolder}/holy_orders"))
            {
                string s = c.Split('\\').Last();
                string oldPath = $"{GfxFolder}/holy_orders/{s}";
                string newPath = $"{TranspileTarget}/gfx/interface/holy_orders/{s}";

                b.Add(
                    "spriteType", "=", "{",
                        "name", "=", $"\"GFX_holy_order_{s.Split('.').First()}\"",
                        "texturefile", "=", $"\"gfx/interface/holy_orders/{s}\"",
                        "noOfFrames", "=", "4",
                    "}"
                );

                File.Delete(newPath);
                File.Copy(oldPath, newPath);
            }

            CreateTillFolder($"{TranspileTarget}/gfx/interface/government_reform_icons");
            foreach (string c in GetFiles($"{GfxFolder}/government_reforms"))
            {
                string s = c.Split('\\').Last();
                string oldPath = $"{GfxFolder}/government_reforms/{s}";
                string newPath = $"{TranspileTarget}/gfx/interface/government_reform_icons/{s}";

                b.Add(
                    "spriteType", "=", "{",
                        "name", "=", $"\"government_reform_{s.Split('.').First()}\"",
                        "texturefile", "=", $"\"gfx/interface/government_reform_icons/{s}\"",
                    "}"
                );

                File.Delete(newPath);
                File.Copy(oldPath, newPath);
            }

            b.Add("}");

            OverwriteFile($"{TranspileTarget}/interface/arc5.gfx", string.Join(' ', b));

            return "GFX folder";
        }
        static string Unsorted()
        {
            if(!(iArgs.Contains("unsorted") || iArgs.Contains("all"))) return "Unsorted Files";
            RFold(UnsortedFolder);
            void RFold(string fold)
            {
                IEnumerable<string> Folders = GetFolders(fold);
                foreach (string folder in Folders)
                {
                    RFold(folder);
                }

                string tfold = $"{TranspileTarget}\\{Path.GetRelativePath($"{directory}/{UnsortedFolder}", fold)}".Replace('\\', '/');
                CreateTillFolder(tfold);
                IEnumerable<string> files = GetFiles(fold);
                foreach (string file in files)
                {
                    string cfile = Path.GetRelativePath(directory, file).Replace('\\', '/');
                    string tfile = $"{TranspileTarget}\\{Path.GetRelativePath($"{directory}/{UnsortedFolder}", file)}".Replace('\\', '/');
                    File.Delete(tfile);
                    File.Copy(cfile, tfile);
                }
            }

            return "Unsorted Files";
        }
        static bool FileCompare(string file1, string file2)
        {
            if (!File.Exists(file1)) return false;
            if (!File.Exists(file2)) return false;
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;
            if (file1 == file2)
            {
                return true;
            }
            fs1 = new FileStream(file1, FileMode.Open, FileAccess.Read);
            fs2 = new FileStream(file2, FileMode.Open, FileAccess.Read);
            if (fs1.Length != fs2.Length)
            {
                fs1.Close();
                fs2.Close();
                return false;
            }
            do
            {
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));
            fs1.Close();
            fs2.Close();
            return ((file1byte - file2byte) == 0);
        }
        static List<string> warnings = new();
        public static void Warn(string s)
        {
            warnings.Add(s);
        }
        public static IEnumerable<string> GetFolders(string path)
        {
            string location = Path.Combine(directory, path);

            return from s in Directory.GetDirectories(location) select Path.GetRelativePath(directory, s);
        }
        public static string[] GetFiles(string path)
        {
            string location = Path.Combine(directory, path);

            return Directory.GetFiles(location);
        }
        public static string SpecialUnitTranspile()
        {
            IArcObject specialUnits = (IArcObject)Compiler.global["special_units"];

            IArcObject galleass = (IArcObject)specialUnits.Get("galleass");
            IArcObject musketeer = (IArcObject)specialUnits.Get("musketeer");
            IArcObject rajput = (IArcObject)specialUnits.Get("rajput");

            Block staticModifiers = new()
            {
                ((ArcModifier)galleass.Get("modifier")).Compile("galleass_modifier"),
                ((ArcModifier)galleass.Get("ship")).Compile("galleass_ship"),
                ((ArcModifier)musketeer.Get("modifier")).Compile("musketeer_modifier"),
                ((ArcModifier)musketeer.Get("regiment")).Compile("musketeer_regiment"),
                ((ArcModifier)rajput.Get("regiment")).Compile("rajput_regiment"),
            };

            OverwriteFile($"{TranspileTarget}/common/static_modifiers/special_units.txt", string.Join(' ', staticModifiers));

            Block defines = new()
            {
                "NDefines.NMilitary.GALLEASS_USES_CONSTRUCTION", "=", galleass.Get("uses_construction"),
                "NDefines.NMilitary.GALLEASS_BASE_COST_MODIFIER", "=", galleass.Get("base_cost_modifier"),
                "NDefines.NMilitary.GALLEASS_SAILORS_COST_MODIFIER", "=", galleass.Get("sailors_cost_modifier"),
                "NDefines.NMilitary.GALLEASS_STARTING_STRENGTH", "=", galleass.Get("starting_strength"),
                "NDefines.NMilitary.GALLEASS_STARTING_MORALE", "=", galleass.Get("starting_morale"),

                "NDefines.NMilitary.MUSKETEER_USES_CONSTRUCTION", "=", musketeer.Get("uses_construction"),
                "NDefines.NMilitary.MUSKETEER_BASE_COST_MODIFIER", "=", musketeer.Get("base_cost_modifier"),
                "NDefines.NMilitary.MUSKETEER_MANPOWER_COST_MODIFIER", "=", musketeer.Get("manpower_cost_modifier"),
                "NDefines.NMilitary.MUSKETEER_PRESTIGE_COST", "=", musketeer.Get("prestige_cost"),
                "NDefines.NMilitary.MUSKETEER_ABSOLUTISM_COST", "=", musketeer.Get("absolutism_cost"),
                "NDefines.NMilitary.MUSKETEER_STARTING_STRENGTH", "=", musketeer.Get("starting_strength"),
                "NDefines.NMilitary.MUSKETEER_STARTING_MORALE", "=", musketeer.Get("starting_morale"),

                "NDefines.NMilitary.RAJPUT_USES_CONSTRUCTION", "=", rajput.Get("uses_construction"),
                "NDefines.NMilitary.RAJPUT_BASE_COST_MODIFIER", "=", rajput.Get("base_cost_modifier"),
                "NDefines.NMilitary.RAJPUT_MAXIMUM_RATIO", "=", rajput.Get("maximum_ratio"),
                "NDefines.NMilitary.RAJPUT_STARTING_STRENGTH", "=", rajput.Get("starting_strength"),
            };
            OverwriteFile($"{TranspileTarget}/common/defines/special_units.lua", string.Join(' ', defines));

            Block LocBlock = new()
            {
                ((ArcBlock)galleass.Get("localisation")).Value,
                ((ArcBlock)musketeer.Get("localisation")).Value,
                ((ArcBlock)rajput.Get("localisation")).Value,
            };
            if (LocBlock.Any())
            {
                Walker i = new(LocBlock);
                do
                {
                    string key = i.Current;
                    i.ForceMoveNext();
                    i.Asssert("=");
                    i.ForceMoveNext();
                    string value = i.Current;
                    Localisation.Add(key, value);
                } while (i.MoveNext());
            }
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
        public static void CreateTillFolder(string fold)
        {
            string[] paths = fold.Split('/');
            string newPath = directory;
            foreach (string s in paths)
            {
                if (s.Contains('.')) continue;

                newPath = Path.Combine(newPath, s);
                if (!Directory.Exists(newPath))
                {
                    Console.WriteLine($"\tCreating {Path.GetRelativePath(directory, newPath)}".Pastel(ConsoleColor.Magenta));
                    Directory.CreateDirectory(newPath);
                }
            }
        }
        public static void OverwriteFile(string path, string text, bool AllowFormatting = true, bool BOM = false)
        {
            if (BOM)
            {
                byte[] data = Encoding.UTF8.GetBytes(text);
                byte[] result = Encoding.UTF8.GetPreamble().Concat(data).ToArray();
                UTF8Encoding encoder = new(true);

                text = encoder.GetString(result);
            }
            string pathOrg = path;
            path = Path.Combine(directory, path);
            try
            {
                if (AllowFormatting && iArgs.Contains("format")) text = Parser.FormatCode(text);
            }
            catch(Exception)
            {
                Console.WriteLine(path);
                throw;
            }

            CreateTillFolder(pathOrg);

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
                string value = loc.Value.Trim('"');
                if(Environment.NewLine != "\n") value = value.Replace(Environment.NewLine, "\\n");
                value = value.Replace("\n", "\\n");
                value = value.Replace("\t", "    ");

                sb.Append($" {loc.Key}: \"{value}\"\n");
            }
            OverwriteFile($"{TranspileTarget}/localisation/replace/z_arc_valley_l_english.yml", (sb.ToString()), false, true);
            return "Localisations";
        }
        private static string TranspileOnActions()
        {
            Dict<ArcEffect> OnActions = (Dict<ArcEffect>)Compiler.global["on_actions"];

            Block BiYearlyEvents = ((ArcBlock)Compiler.global["bi_yearly_events"]).Value;

            int BiYearlySum = 0;

            int? weight = null;
            foreach(Word w in BiYearlyEvents)
            {
                if(w == "=") continue;

                if(weight == null)
                {
                    weight = int.Parse(w);
                    continue;
                }
                else
                {
                    if (!Event.Events.CanGet(w)) Console.WriteLine($"bi_yearly_events: {w} does not exist".Pastel(ConsoleColor.Magenta));
                    BiYearlySum += (int)weight;
                    weight = null;
                    continue;
                }
            }

            BiYearlyEvents.Add(BiYearlySum/10, "=", "0");

            ArcEffect OnBiYearlyPulse = OnActions["on_bi_yearly_pulse"];

            OnBiYearlyPulse.Value.Add("random_events", "=", "{");
            OnBiYearlyPulse.Value.Add(BiYearlyEvents);
            OnBiYearlyPulse.Value.Add("}");

            foreach (KeyValuePair<string, ArcEffect> OnAction in OnActions)
            {
                string s = $"{OnAction.Key} = {{ {OnAction.Value.Compile()}}} ";
                OverwriteFile($"{TranspileTarget}/common/on_actions/{OnAction.Key}.txt", s);
            }
            return "On Actions";
        }
        public static void LoadTarget(string path)
        {
            string fileLocation = Path.Combine(directory, path);

            if (fileLocation.EndsWith("/*"))
            {
                string current = fileLocation[..^1];

                LoadTarget(current);

                foreach(string folder in Directory.GetDirectories(current))
                {
                    string next = Path.GetRelativePath(directory, folder) + "/*";

                    LoadTarget(next);
                }
            }
            else if (fileLocation.EndsWith("/"))
            {
                string[] files = Directory.GetFiles(fileLocation);
                foreach (string file in files)
                {
                    try
                    {
                        string fileContent = File.ReadAllText(file);
                        Compiler.ObjectDeclare(fileContent + headers, true);
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
                    string file = File.ReadAllText(fileLocation);
                    Compiler.ObjectDeclare(file + headers, true);
                }
                catch (Exception)
                {
                    Console.WriteLine(Path.GetRelativePath(directory, fileLocation).Pastel(ConsoleColor.Red));
                    throw;
                }
            }
        }
    }
}
