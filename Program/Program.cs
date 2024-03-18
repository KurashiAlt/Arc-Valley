using Arc;
using Pastel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
internal class Program
{
    public static Stopwatch timer = Stopwatch.StartNew();
    public static string directory = AppDomain.CurrentDomain.BaseDirectory;
    public static Dictionary<string, string> Localisation = new();
    public static List<string> Vanilla = new();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public static string headers;
    public static string TranspileTarget;
    public static string GfxFolder;
    public static string UnsortedFolder;
    public static string MapFolder;
    public static string SelectorFolder;
    public static IEnumerable<string> LoadOrder;
    public static string[] PartialMod;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public static List<string> warnings = new();
    public static bool Format = false;
    public static void Warn(string s)
    {
        warnings.Add(s);
    }
    private static int Main(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("en");

        Args arcDefines = Args.GetArgsFromFile(Path.Combine(directory, "arc.defines"));
        headers = arcDefines.Get(ArcString.Constructor, "headers").Value;
        UnsortedFolder = arcDefines.Get(ArcString.Constructor, "unsorted_target").Value;
        TranspileTarget = arcDefines.Get(ArcString.Constructor, "transpile_target").Value;
        GfxFolder = arcDefines.Get(ArcString.Constructor, "gfx_folder").Value;
        MapFolder = arcDefines.Get(ArcString.Constructor, "map_folder").Value;
        SelectorFolder = arcDefines.Get(ArcString.Constructor, "selector_folder").Value;
        LoadOrder = from c in arcDefines.Get(ArcCode.Constructor, "load_order").Value select new string(c.Value);
        PartialMod = (from a in arcDefines.Get(ArcCode.Constructor, "partial_mod", new()).Value select a.Value).ToArray();

        if (Debugger.IsAttached)
        {
            args = new string[]
            {
                "format", "test", "script"
            };
        }

        Format = args.Contains("format");

        if (args.Contains("script") || args.Contains("all"))
        {
            foreach (string location in LoadOrder)
            {
                TimeSpan start = timer.Elapsed;
                LoadTarget(location);
                TimeSpan end = timer.Elapsed;
                Console.WriteLine($"{$"Finished Loading {location}".PadRight(50).Pastel(ConsoleColor.Yellow)}{$"{(end - start).Milliseconds,7:0} Milliseconds".Pastel(ConsoleColor.Red)}");
            }

            if (args.Contains("selector"))
            {
                TimeSpan start = timer.Elapsed;
                using System.Drawing.Bitmap provmap = new($"{directory}/{MapFolder}/provinces.bmp");
                foreach (string file in GetFiles($"{SelectorFolder}"))
                {
                    if (!file.EndsWith(".png")) continue;
                    List<System.Drawing.Color> colors = new();

                    System.Drawing.Bitmap img = new(file);

                    string name = Path.GetRelativePath($"{directory}/{SelectorFolder}", file).Split('.')[0];

                    for (int x = 0; x < img.Width; x++)
                    {
                        for (int y = 0; y < img.Height; y++)
                        {
                            if (img.GetPixel(x, y).A == 0) continue;

                            System.Drawing.Color color = provmap.GetPixel(x, y);

                            if (!colors.Contains(color)) colors.Add(color);
                        }
                    }

                    ArcList<Province> list = new();
                    List<string> keys = new();
                    foreach (KeyValuePair<string, Province> v in 
                        from prov in Province.Provinces where 
                            colors.Contains(System.Drawing.Color.FromArgb(
                                byte.Parse(prov.Value.Color.Value.ElementAt(0)), 
                                byte.Parse(prov.Value.Color.Value.ElementAt(1)), 
                                byte.Parse(prov.Value.Color.Value.ElementAt(2))
                            )) select prov) {
                        list.Add(v.Value);
                        keys.Add(v.Key);
                    }

                    File.WriteAllText($"{file}.gen", string.Join(' ', from p in keys select p));
                    _ = new ProvinceGroup(name, new())
                {
                    { "id", new ArcString(name) },
                    { "provinces", list }
                };
                }
                TimeSpan end = timer.Elapsed;
                Console.WriteLine($"{$"Finished Loading {SelectorFolder}".PadRight(50).Pastel(ConsoleColor.Yellow)}{$"{(end - start).Milliseconds,7:0} Milliseconds".Pastel(ConsoleColor.Red)}");
            }
            else
            {
                TimeSpan start = timer.Elapsed;

                foreach (string file in GetFiles($"{SelectorFolder}"))
                {
                    if (!file.EndsWith(".gen")) continue;

                    string name = Path.GetRelativePath($"{directory}/{SelectorFolder}", file).Split('.')[0];

                    ArcList<Province> list = new();
                    foreach (Word w in Parser.ParseFile(file))
                    {
                        list.Add(Province.Provinces[w]);
                    }

                    _ = new ProvinceGroup(name, new())
                {
                    { "id", new ArcString(name) },
                    { "provinces", list }
                };
                }
                TimeSpan end = timer.Elapsed;
                Console.WriteLine($"{$"Finished Loading Cached {SelectorFolder}".PadRight(50).Pastel(ConsoleColor.Yellow)}{$"{(end - start).Milliseconds,7:0} Milliseconds".Pastel(ConsoleColor.Red)}");
            }

            TimeSpan tstart = timer.Elapsed;
            var vList = (from c in ArcBlock.CompileList where c.ShouldBeCompiled && c.Compiled == null select c).ToArray();
            int ti = 0;
            while (vList.Any())
            {
                foreach (ArcBlock v in vList)
                {
                    try
                    {
                        v.Compile();
                    }
                    catch (Exception e)
                    {
                        v.Compiled = "ERROR";
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                    }
                    ti++;
                    if (ti % 1000 == 0) Console.WriteLine($"Has finished compiling {ti} blocks".Pastel(ConsoleColor.Magenta));
                }

                vList = (from c in ArcBlock.CompileList where c.ShouldBeCompiled && c.Compiled == null select c).ToArray();
            }
            TimeSpan tend = timer.Elapsed;
            Console.WriteLine($"{$"Finished Compiling Code".PadRight(50).Pastel(ConsoleColor.Magenta)}{$"{(tend - tstart).Milliseconds,7:0} Milliseconds".Pastel(ConsoleColor.Magenta)}");
        }

        (string, string, Func<string>)[] Transpilers =
        {
        ("script", "", Incident.Transpile),
        ("script", "", ReligionGroup.Transpile),
        ("script", "", PersonalDeity.Transpile),
        ("script", "", Decision.Transpile),
        ("script", "", Event.Transpile),
        ("script", "", Adjacency.Transpile),
        ("script", "", Area.Transpile),
        ("script", "", Bookmark.Transpile),
        ("script", "", Region.Transpile),
        ("script", "", Superregion.Transpile),
        ("script", "", Province.Transpile),
        ("script", "", TradeGood.Transpile),
        ("script", "", Terrain.Transpile),
        ("script", "", Blessing.Transpile),
        ("script", "", Building.Transpile),
        ("script", "", Country.Transpile),
        ("script", "", ChurchAspect.Transpile),
        ("script", "", AdvisorType.Transpile),
        ("script", "", TradeNode.Transpile),
        ("script", "", IdeaGroup.Transpile),
        ("script", "", Policy.Transpile),
        ("script", "", Relation.Transpile),
        ("script", "", CultureGroup.Transpile),
        ("script", "", MissionSeries.Transpile),
        ("script", "", EstateAgenda.Transpile),
        ("script", "", EstatePrivilege.Transpile),
        ("script", "", Estate.Transpile),
        ("script", "", GovernmentReform.Transpile),
        ("script", "", GovernmentMechanic.Transpile),
        ("script", "", Unit.Transpile),
        ("script", "", GreatProject.Transpile),
        ("script", "", MercenaryCompany.Transpile),
        ("script", "", Advisor.Transpile),
        ("script", "", Age.Transpile),
        ("script", "", DiplomaticAction.Transpile),
        ("script", "", NavalDoctrines),
        ("script", "", SpecialUnitTranspile),
        ("script", "", Government.Transpile),
        ("script", "", GovernmentNames.Transpile),
        ("script", "", HolyOrder.Transpile),
        ("script", "", ProvinceTriggeredModifier.Transpile),
        ("script", "", CasusBelli.Transpile),
        ("script", "", WarGoal.Transpile),
        ("script", "", ProvinceGroup.Transpile),
        ("script", "", AiPersonalities),
        ("script", "", CenterOfTrade),
        ("script", "", RulerPersonality.Transpile),
        ("script", "", OpinionModifier.Transpile),
        ("script", "", StaticModifier.Transpile),
        ("script", "", TranspilerClass.TranspileEventModifiers),
        ("script", "", SubjectType.Transpile),
        ("script", "", CustomButton.Transpile),
        ("script", "", CustomIcon.Transpile),
        ("script", "", TranspilerClass.TranspileCustomTextBoxes),
        ("script", "", InterfaceNode.Transpile),
        ("script", "", CustomizableLocalization.Transpile),
        ("script", "", TranspileOnActions),
        ("script", "", TranspileDefines),
        ("script", "", TranspileLocalisations),
        ("gfx", "", Gfx),
        ("map", "", Map),
        ("unsorted", "", Unsorted),
    };

        foreach ((string, string, Func<string>) transpiler in Transpilers)
        {
            if (!(args.Contains(transpiler.Item1) || args.Contains("all"))) continue;

            TimeSpan start = timer.Elapsed;
            string type = transpiler.Item3();
            start = timer.Elapsed - start;
            Console.WriteLine($"{$"Finished Transpiling{transpiler.Item2} {type}".PadRight(50).Pastel(ConsoleColor.Cyan)}{$"{start.TotalMilliseconds,7:0} Milliseconds".Pastel(ConsoleColor.Red)}");
        }

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] != "-u") continue;
            if (args.Length < i + 1) continue;
            string file = args[i + 1];
            Unsorted(file);
        }
        
        OverwriteFile($"warnings.txt", string.Join('\n', warnings));

        Console.WriteLine($"Transpilation took: {(double)timer.ElapsedMilliseconds / 1000:0.000} seconds".Pastel(ConsoleColor.Red));

        return 0;
    }
    static string NavalDoctrines()
    {
        Block b = new();
        foreach (ArcObject obj in Compiler.GetVariable<Dict<IVariable>>(new Word("naval_doctrines")).Values())
        {
            string id = obj.Get<ArcString>("id").ToString();
            b.Add(
                id, "=", "{",
                    obj.Get<ArcBlock>("can_select").Compile("can_select", CanBeEmpty: false),
                    obj.Get<ArcBlock>("country_modifier").Compile("country_modifier", CanBeEmpty: false),
                    obj.Get<ArcBlock>("effect").Compile("effect", CanBeEmpty: false),
                    obj.Get<ArcBlock>("removed_effect").Compile("removed_effect", CanBeEmpty: false),
                    "cost", "=", obj.Get<ArcFloat>("cost"),
                    "button_gfx", "=", obj.Get<ArcInt>("button_gfx"),
                "}"
            );
        }
        OverwriteFile($"{TranspileTarget}/common/naval_doctrines/arc.txt", b.ToString());
        return "Naval Doctrines";
    }

    static string CenterOfTrade()
    {
        Block COTFile = new();
        foreach (KeyValuePair<string, ArcCode> cot in Compiler.GetVariable<Dict<ArcCode>>(new Word("centers_of_trade")))
        {
            cot.Value.Compile(cot.Key, ref COTFile);
        }
        OverwriteFile($"{TranspileTarget}/common/centers_of_trade/arc.txt", string.Join(' ', COTFile));
        return "Center of Trades";
    }
    static string AiPersonalities()
    {
        Block AiPersonalityFile = new();
        foreach (KeyValuePair<string, ArcTrigger> personality in Compiler.GetVariable<Dict<ArcTrigger>>(new Word("ai_personalities")))
        {
            personality.Value.Compile(personality.Key, ref AiPersonalityFile);
        }
        OverwriteFile($"{TranspileTarget}/common/ai_personalities/arc.txt", string.Join(' ', AiPersonalityFile));
        return "Ai Personalities";
    }
    static string Map()
    {
        RFold(MapFolder);
        void RFold(string fold)
        {
            IEnumerable<string> Folders = GetFolders(fold);
            foreach (string folder in Folders)
            {
                RFold(folder);
            }

            string tfold = $"{TranspileTarget}\\map\\{Path.GetRelativePath($"{directory}/{MapFolder}", fold)}".Replace('\\', '/');
            CreateTillFolder(tfold);
            IEnumerable<string> files = GetFiles(fold);
            foreach (string file in files)
            {
                if (file.EndsWith("colormap.dds"))
                {
                    string cfile = Path.GetRelativePath(directory, file).Replace('\\', '/');
                    frw(cfile, $"{TranspileTarget}\\map\\terrain\\colormap_autumn.dds".Replace('\\', '/'));
                    frw(cfile, $"{TranspileTarget}\\map\\terrain\\colormap_spring.dds".Replace('\\', '/'));
                    frw(cfile, $"{TranspileTarget}\\map\\terrain\\colormap_summer.dds".Replace('\\', '/'));
                    frw(cfile, $"{TranspileTarget}\\map\\terrain\\colormap_winter.dds".Replace('\\', '/'));
                }
                else
                {
                    frw(
                        Path.GetRelativePath(directory, file).Replace('\\', '/'), 
                        $"{TranspileTarget}\\map\\{Path.GetRelativePath($"{directory}/{MapFolder}", file)}".Replace('\\', '/')
                    );
                }
            }
        }

        void frw(string cfile, string tfile)
        {
            if (File.Exists(tfile)) File.Delete(tfile);
            File.Copy(cfile, tfile);
        }

        return "Map";
    }
    static string Gfx()
    {
        Block b = new("spriteTypes", "=", "{");

        CreateTillFolder($"{TranspileTarget}/gfx/special");
        foreach (string c in GetFiles($"{GfxFolder}/special"))
        {
            string s = c.Split('\\').Last();

            if (File.Exists($"{TranspileTarget}/gfx/special/{s}")) File.Delete($"{TranspileTarget}/gfx/special/{s}");
            File.Copy(c, $"{TranspileTarget}/gfx/special/{s}");
        }

        CreateTillFolder($"{TranspileTarget}/gfx/loose");
        foreach (string c in GetFiles($"{GfxFolder}/loose"))
        {
            string s = c.Split('\\').Last();

            b.Add(
                "spriteType", "=", "{",
                    "name", "=", $"\"{s.Split('.').First()}\"",
                    "texturefile", "=", $"\"gfx/loose/{s}\"",
                "}"
            );

            File.Delete($"{TranspileTarget}/gfx/loose/{s}");
            File.Copy(c, $"{TranspileTarget}/gfx/loose/{s}");
        }

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

        CreateTillFolder($"{TranspileTarget}/gfx/flags");
        foreach (string c in GetFiles($"{GfxFolder}/flags"))
        {
            string s = c.Split('\\').Last();
            string oldPath = $"{GfxFolder}/flags/{s}";
            string newPath = $"{TranspileTarget}/gfx/flags/{Country.Countries[s.Split('.')[0]].Tag}.tga";

            File.Delete(newPath);
            File.Copy(oldPath, newPath);
        }

        b.Add("}");

        OverwriteFile($"{TranspileTarget}/interface/arc5.gfx", string.Join(' ', b));

        return "GFX folder";
    }
    static string Unsorted()
    {
        return Unsorted("");
    }
    static string Unsorted(string fileEnd)
    {
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
                if(file.EndsWith(fileEnd)) UnsortedSingleFile(file);
            }
        }

        return "Unsorted Files";
    }
    static void UnsortedSingleFile(string file)
    {
        string cfile = Path.GetRelativePath(directory, file).Replace('\\', '/');
        string tfile = $"{TranspileTarget}\\{Path.GetRelativePath($"{directory}/{UnsortedFolder}", file)}".Replace('\\', '/');
        File.Delete(tfile);
        File.Copy(cfile, tfile);
    }
    public static IEnumerable<string> GetFolders(string path)
    {
        string location = Path.Combine(directory, path);

        return from s in GetDirectories(location) select Path.GetRelativePath(directory, s);
    }
    public static string[] GetDirectories(string path)
    {
        if (!path.StartsWith(directory)) path = Path.Combine(directory, path);

        return Directory.GetDirectories(path).OrderBy(d => d).ToArray();
    }
    public static string[] GetFiles(string path)
    {
        if (!Path.Exists(Path.Combine(directory, path))) return new string[] { };
        if(!path.StartsWith(directory)) path = Path.Combine(directory, path);

        return Directory.GetFiles(path).OrderBy(d => d).ToArray();
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
        text = ReplaceBlocks(text);

        bool v = PartialMod.Length == 0;
        foreach (string PartialModFile in PartialMod)
        {
            if (path.EndsWith(PartialModFile)) v = true;
        }
        if (!v) return;
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
            if (AllowFormatting && Format) text = Parser.FormatCode(text);
        }
        catch (Exception)
        {
            Console.WriteLine(path);
            throw;
        }

        CreateTillFolder(pathOrg);

        if (File.Exists(path))
        {
            string old = File.ReadAllText(path);
            if (text != old) File.WriteAllText(path, text);
        }
        else
        {
            File.WriteAllText(path, text);
        }

        string ReplaceBlocks(string text)
        {
            Regex blockSpot = new Regex("__ARC\\.BLOCK__ = (\\d+)");
            while (text.Contains("__ARC.BLOCK__"))
            {
                text = blockSpot.Replace(text, new MatchEvaluator((Match m) =>
                {
                    int id = int.Parse(m.Groups[1].Value);
                    return ArcBlock.CompileList[id].Compiled;
                }));
            }
            return text;
        }
    }
    private static string TranspileDefines()
    {
        StringBuilder sb = new();
        foreach (var v in Compiler.global.Get<ArcObject>("defines"))
        {
            sb.AppendLine($"{v.Key} = {v.Value}");
        }
        OverwriteFile($"{TranspileTarget}/common/defines/arc.lua", sb.ToString(), false);
        return "Defines";
    }
    private static string TranspileLocalisations()
    {
        StringBuilder sb = new("l_english:\n");
        foreach (KeyValuePair<string, string> loc in Localisation)
        {
            string value = loc.Value.Trim('"');
            if (Environment.NewLine != "\n") value = value.Replace(Environment.NewLine, "\\n");
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
        foreach (Word w in BiYearlyEvents)
        {
            if (w == "=") continue;

            if (weight == null)
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

        BiYearlyEvents.Add(BiYearlySum / 10, "=", "0");

        ArcEffect OnBiYearlyPulse = OnActions["on_bi_yearly_pulse"];

        OnBiYearlyPulse.Compiled += $"random_events = {{ {BiYearlyEvents} }}";

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

            foreach (string folder in GetDirectories(current))
            {   
                string next = Path.GetRelativePath(directory, folder) + "/*";

                LoadTarget(next);
            }
        }
        else if (fileLocation.EndsWith("/"))
        {
            string[] files = GetFiles(fileLocation);
            foreach (string file in files)
            {
                string fileContent = File.ReadAllText(file);
                Compiler.ObjectDeclare(fileContent + headers, file, true);
            }
        }
        else
        {
            string file = File.ReadAllText(fileLocation);
            Compiler.ObjectDeclare(file + headers, fileLocation, true);
        }
    }
}