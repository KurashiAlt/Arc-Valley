using Arc;
using Pastel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
internal partial class Program
{
    public static Stopwatch timer = Stopwatch.StartNew();
    public static string directory = ArcDirectory.directory;
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
    public static bool OnlyCustomTranspilers;
    public static bool CheckForVariableDefinitions;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public static bool Format = false;
    private static int Main(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("en");

        if (args.Length > 0 && Path.Exists(args[0]))
        {
            directory = args[0];
            if (!directory.EndsWith('\\')) directory += "\\";
            ArcDirectory.directory = directory;
        }

        Args arcDefines = Args.GetArgsFromFile(Path.Combine(directory, "arc.defines"));
        headers = arcDefines.Get(ArcString.Constructor, "headers").Value;
        UnsortedFolder = arcDefines.Get(ArcString.Constructor, "unsorted_target").Value;
        TranspileTarget = arcDefines.Get(ArcString.Constructor, "transpile_target").Value;
        GfxFolder = arcDefines.Get(ArcString.Constructor, "gfx_folder").Value;
        MapFolder = arcDefines.Get(ArcString.Constructor, "map_folder").Value;
        SelectorFolder = arcDefines.Get(ArcString.Constructor, "selector_folder").Value;
        LoadOrder = from c in arcDefines.Get(ArcCode.Constructor, "load_order").Value select new string(c.Value);
        PartialMod = (from a in arcDefines.Get(ArcCode.Constructor, "partial_mod", new()).Value select a.Value).ToArray();
        OnlyCustomTranspilers = arcDefines.Get(ArcBool.Constructor, "only_custom_transpilers", new(false)).Value;
        CheckForVariableDefinitions = arcDefines.Get(ArcBool.Constructor, "check_for_variable_definitions", new(true)).Value;

        if (args.Length == 0)
        {
            Console.WriteLine("You provided no arguments for arc, would you like to transpile all? y/n");
            string? response = Console.ReadLine();
            if (response == "y") args = new string[] { "all" };
        }
        if (
            !args.Contains("no-vdir") &&
            (!File.Exists(Path.Combine(directory, ".arc/script.vdir")) ||
            !File.Exists(Path.Combine(directory, ".arc/unsorted.vdir")) ||
            !File.Exists(Path.Combine(directory, ".arc/map.vdir")) ||
            !File.Exists(Path.Combine(directory, ".arc/gfx.vdir")))
        ) {
            Console.WriteLine("One of the virtual directory caches was missing, arguments have been edited to transpile all");
            Array.Resize(ref args, args.Length + 1);
            args[^1] = "all";
        }

        Format = args.Contains("format");

        if (args.Contains("script") || args.Contains("all"))
        {
            foreach (ArcPath file in ArcDirectory.GetFiles($"{SelectorFolder}"))
            {
                if (!file.value.EndsWith(".png")) continue;
                ArcPath name = Path.GetRelativePath($"{directory}/{SelectorFolder}", file).Split('.')[0];

                _ = new ProvinceGroup(name, new())
                {
                    { "id", new ArcString(name) },
                    { "provinces", new ArcList<Province>() }
                };
            }

            foreach (string location in LoadOrder)
            {
                TimeSpan start = timer.Elapsed;
                LoadTarget(location);
                TimeSpan end = timer.Elapsed;
                Console.WriteLine($"{$"Finished Loading {location}".PadRight(50).Pastel(ConsoleColor.Yellow)}{$"{(end - start).Milliseconds,7:0} Milliseconds".Pastel(ConsoleColor.Red)}");
            }

            if (args.Contains("selector"))
            {
#pragma warning disable CA1416 // Validate platform compatibility
                TimeSpan start = timer.Elapsed;
                using System.Drawing.Bitmap provmap = new($"{directory}/{MapFolder}/provinces.bmp");
                foreach (string file in ArcDirectory.GetFiles($"{SelectorFolder}"))
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

                    List<Province?> list = new();
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
                    ProvinceGroup.ProvinceGroups[name].Get<ArcList<Province>>("provinces").Values = list;
                }
#pragma warning restore CA1416 // Validate platform compatibility
                TimeSpan end = timer.Elapsed;
                Console.WriteLine($"{$"Finished Loading {SelectorFolder}".PadRight(50).Pastel(ConsoleColor.Yellow)}{$"{(end - start).TotalMilliseconds,7:0} Milliseconds".Pastel(ConsoleColor.Red)}");
            }
            else
            {
                TimeSpan start = timer.Elapsed;

                foreach (string file in ArcDirectory.GetFiles($"{SelectorFolder}"))
                {
                    if (!file.EndsWith(".gen")) continue;

                    string name = Path.GetRelativePath($"{directory}/{SelectorFolder}", file).Split('.')[0];

                    List<Province?> list = new();
                    foreach (Word w in Parser.ParseFile(file))
                    {
                        try
                        {
                            list.Add(Province.Provinces[w]);
                        }
                        catch
                        {
                            Console.WriteLine($"Tried to add province to selector province group: '{file}' selector in question, '{w}' unknown province identifier in question.");
                        }
                    }

                    ProvinceGroup.ProvinceGroups[name].Get<ArcList<Province>>("provinces").Values = list;
                }
                TimeSpan end = timer.Elapsed;
                Console.WriteLine($"{$"Finished Loading Cached {SelectorFolder}".PadRight(50).Pastel(ConsoleColor.Yellow)}{$"{(end - start).TotalMilliseconds,7:0} Milliseconds".Pastel(ConsoleColor.Red)}");
            }

            ArcBlock.PastDefineStep = true;
            Compiler.CompileRightAway += 1;
            
            TimeSpan tstart = timer.Elapsed;
            var vList = (from c in CompileList.list where c.ShouldBeCompiled && c.Compiled == null select c).ToArray();
            int ti = 0;
            while (vList.Length != 0)
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

                vList = (from c in CompileList.list where c.ShouldBeCompiled && c.Compiled == null select c).ToArray();
            }
            TimeSpan tend = timer.Elapsed;
            Console.WriteLine($"{$"Finished Compiling Code".PadRight(50).Pastel(ConsoleColor.Magenta)}{$"{(tend - tstart).TotalMilliseconds,7:0} Milliseconds".Pastel(ConsoleColor.Magenta)}");
        }


        (string, string, Func<string>)[] Transpilers =
        {
        ("script", "", Incident.Transpile),
        ("script", "", ReligionGroup.Transpile),
        ("script", "", PersonalDeity.Transpile),
        ("script", "", Decision.Transpile),
        ("script", "", Event.Transpile),
        ("script", "", Province.Transpile),
        ("script", "", Terrain.Transpile),
        ("script", "", Building.Transpile),
        ("script", "", Country.Transpile),
        ("script", "", TradeNode.Transpile),
        ("script", "", Policy.Transpile),
        ("script", "", Relation.Transpile),
        ("script", "", CultureGroup.Transpile),
        ("script", "", MissionSeries.Transpile),
        ("script", "", EstateAgenda.Transpile),
        ("script", "", EstatePrivilege.Transpile),
        ("script", "", Estate.Transpile),
        ("script", "", GovernmentReform.Transpile),
        ("script", "", GovernmentMechanic.Transpile),
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
        ("script", "", SubjectType.Transpile),
        ("script", "", CustomButton.Transpile),
        ("script", "", CustomIcon.Transpile),
        ("script", "", TranspilerClass.TranspileCustomTextBoxes),
        ("script", "", InterfaceNode.Transpile),
        ("script", "", Transpiler.TranspileSimples),
        ("script", "", TechnologyGroups),
        ("script", "", TranspileDefines),
        ("script", "", TranspileLocalisations),
        ("script", "", GfxFolders),
        ("map", "", Map),
        ("unsorted", "", Unsorted),
    };

        foreach ((string, string, Func<string>) transpiler in Transpilers)
        {
            if (!(args.Contains(transpiler.Item1) || transpiler.Item1 == "" || args.Contains("all"))) continue;
            if (OnlyCustomTranspilers && transpiler.Item3 != Transpiler.TranspileSimples) continue;

            TimeSpan start = timer.Elapsed;
            string type = transpiler.Item3();
            start = timer.Elapsed - start;
            Console.WriteLine($"{$"Finished Transpiling{transpiler.Item2} {type}".PadRight(50).Pastel(ConsoleColor.Cyan)}{$"{start.TotalMilliseconds,7:0} Milliseconds".Pastel(ConsoleColor.Red)}");
        }

        if (!args.Contains("no-vdir")) 
        {
            TimeSpan start = timer.Elapsed;
            ArcDirectory.VDirPopulate(args);
            start = timer.Elapsed - start;
            Console.WriteLine($"{$"Virtual Directory Populated".PadRight(50).Pastel(ConsoleColor.Cyan)}{$"{start.TotalMilliseconds,7:0} Milliseconds".Pastel(ConsoleColor.Red)}");
        }

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] != "-u") continue;
            if (args.Length < i + 1) continue;
            string file = args[i + 1];
            Unsorted(file);
        }
        
        Console.WriteLine($"Transpilation took: {(double)timer.ElapsedMilliseconds / 1000:0.000} seconds".Pastel(ConsoleColor.Red));

        ArcDirectory.CheckFolderForUncategorizedFiles(TranspileTarget);
        if (ArcDirectory.ExtraFiles.Count == 0) Console.WriteLine("All files recognized");
        else if (!args.Contains("no-vdir"))
        {
            Console.WriteLine($"{ArcDirectory.ExtraFiles.Count} unknown files found in {TranspileTarget}");
            foreach (string extraFile in ArcDirectory.ExtraFiles)
            {
                Console.WriteLine($"\t{extraFile}".Pastel(ConsoleColor.Gray));
            }
            Console.WriteLine($"Would you like to delete these files? y/n");
            if (args.Contains("-Y"))
            {
                foreach (string extraFile in ArcDirectory.ExtraFiles)
                {
                    ArcDirectory.TryDelete(Path.Combine(directory, extraFile));
                }
                Console.WriteLine("Deleted all unrecognized files");
            }
            else if (args.Contains("-N"))
            {
                Console.WriteLine("Did not delete unrecognized files");
            }
            else
            {
                string? response = Console.ReadLine();
                if (response == "y")
                {
                    foreach (string extraFile in ArcDirectory.ExtraFiles)
                    {
                        ArcDirectory.TryDelete(Path.Combine(directory, extraFile));
                    }
                    Console.WriteLine("Deleted all unrecognized files");
                }
                else Console.WriteLine("Did not delete unrecognized files");
            }
        }

        return 0;
    }
    static string TechnologyGroups()
    {
        if (!Compiler.global.CanGet("technology_groups")) return "";

        Block b = new("groups", "=", "{");
        foreach (ArcObject obj in Compiler.GetVariable<Dict<IVariable>>(new Word("technology_groups")).Values())
        {
            string id = obj.Get<ArcString>("id").ToString();
            b.Add(
                id, "=", "{",
                    "start_level", "=", obj.Get("start_level"),
                    "start_cost_modifier", "=", obj.Get("start_cost_modifier")
            );
            if (obj.Get<ArcBool>("is_primitive")) b.Add("is_primitive", "=", "yes");
            b.Add(
                    obj.Get<ArcBlock>("nation_designer_trigger").Compile("nation_designer_trigger"),
                    obj.Get<ArcBlock>("nation_designer_cost").Compile("nation_designer_cost"),
                "}"
            );
        }
        b.Add(
            "}",
            "tables", "=", "{", 
                "adm_tech", "=", "\"technologies/adm.txt\"", 
                "dip_tech", "=", "\"technologies/dip.txt\"", 
                "mil_tech", "=", "\"technologies/mil.txt\"", 
            "}"
        );
        OverwriteFile($"{TranspileTarget}/common/technology.txt", b.ToString());
        return "Technology Groups";
    }
    static string NavalDoctrines()
    {
        if (!Compiler.global.CanGet("naval_doctrines")) return "";

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
            IEnumerable<ArcPath> Folders = ArcDirectory.GetFolders(fold);
            foreach (ArcPath folder in Folders)
            {
                RFold(folder);
            }

            string tfold = $"{TranspileTarget}\\map\\{Path.GetRelativePath($"{directory}/{MapFolder}", fold)}".Replace('\\', '/');
            CreateTillFolder(tfold);
            IEnumerable<ArcPath> files = ArcDirectory.GetFiles(fold);
            foreach (ArcPath file in files)
            {
                if (file.value.EndsWith("colormap.dds"))
                {
                    ArcPath cfile = Path.GetRelativePath(directory, file).Replace('\\', '/');
                    frw(cfile, $"{TranspileTarget}/map/terrain/colormap_autumn.dds");
                    frw(cfile, $"{TranspileTarget}/map/terrain/colormap_spring.dds");
                    frw(cfile, $"{TranspileTarget}/map/terrain/colormap_summer.dds");
                    frw(cfile, $"{TranspileTarget}/map/terrain/colormap_winter.dds");
                }
                else
                {
                    frw(
                        Path.GetRelativePath(directory, file), 
                        $"{TranspileTarget}/map/{Path.GetRelativePath($"{directory}/{MapFolder}", file)}"
                    );
                }
            }
        }

        void frw(ArcPath cfile, ArcPath tfile)
        {
            ArcDirectory.MapVDir.Add(tfile);
            ArcDirectory.Copy(cfile, tfile);
        }

        return "Map";
    }
    class ArcFile
    {
        string FullFileName;
        public string File() => Path.GetFileName(FullFileName).Replace('\\', '/');
        public string Name() => Path.GetFileNameWithoutExtension(FullFileName).Replace('\\', '/');
        public string Relative() => Path.GetRelativePath(directory, FullFileName).Replace('\\', '/');
        public string Relative(string s) => Path.GetRelativePath(Path.Combine(directory, s), FullFileName).Replace('\\', '/');
        public string Extension() => Path.GetExtension(FullFileName).Replace('\\', '/');
        public ArcFile(ArcPath fullFileName) { FullFileName = fullFileName; }
        public static implicit operator ArcFile(ArcPath s) => new(s);
        public static implicit operator ArcFile(string s) => new(s);
    }
    
    /// <summary>
    /// Handles the Gfx folder transpilation.
    /// </summary>
    /// <returns>The name of the function.</returns>
    static string GfxFolders()
    {
        // Initialize a new block for sprite types
        Block b = new("spriteTypes", "=", "{");

        // Get the list of graphics folders from the compiler
        foreach (ArcObject gfxFolder in Compiler.GetVariable<Dict<IVariable>>(new("gfx_folders")).Values())
        {
            string id = $"{gfxFolder.Get("id")}";
            string target = $"{gfxFolder.Get("target")}";
            string fileExtension = $".{gfxFolder.Get("restrict_type")}";
            string treatType = $"{gfxFolder.Get("treat_type")}";
            ArcBlock name = gfxFolder.Get<ArcBlock>("name");

            // Create target folder if it does not exist
            string targetFolder = Path.Combine(TranspileTarget, target);
            ArcDirectory.CreateTillDirectory(targetFolder);

            // Get the folder path and iterate over files
            string gfxFolderPath = Path.Combine(GfxFolder, id);
            foreach (ArcFile file in ArcDirectory.GetFiles(gfxFolderPath, gfxFolder.Get<ArcBool>("include_sub").Value))
            {
                ArgList.Add("this", new ArcString(file.Name()));

                // Check file extension
                if (file.Extension() != fileExtension) continue;

                // Prepare the sprite type entry
                string textureFilePath = file.Relative(gfxFolderPath);
                if (Path.HasExtension(textureFilePath))
                {
                    int pos = textureFilePath.LastIndexOf('.');
                    textureFilePath = textureFilePath[..pos] + "." + treatType;
                }
                else textureFilePath += $".{treatType}";
                textureFilePath = ArcDirectory.Combine(ArcDirectory.Relative(TranspileTarget, targetFolder), textureFilePath);

                b.Add(
                    "spriteType", "=", "{",
                        "name", "=", $"\"{name.Compile()}\"",
                        "texturefile", "=", $"\"{textureFilePath}\"",
                        gfxFolder.Get<ArcBlock>("type_info").Compile(),
                    "}"
                );

                // Add file to the virtual directory and copy it
                string relativePath = Path.Combine(target, file.Relative(gfxFolderPath));
                ArcDirectory.ScriptVDir.Add(Path.Combine(TranspileTarget, relativePath));
                ArcDirectory.Copy(file.Relative(), Path.Combine(TranspileTarget, relativePath));

                // Drop the argument list entry for "this"
                ArgList.Drop("this");
            }

            // Handle "modifiers" special case
            if (id == "modifiers")
            {
                foreach (ArcFile file in ArcDirectory.GetFile($"{GfxFolder}/modifiers/files.txt"))
                {
                    b.Add(
                        "spriteType", "=", "{",
                            "name", "=", $"\"GFX_modifier_{file.Name()}\"",
                            "texturefile", "=", $"\"gfx/interface/ideas_EU4/{file.File()}\"",
                        "}"
                    );
                }
            }
        }

        // Process special graphics
        string specialFolderPath = Path.Combine(TranspileTarget, "gfx", "special");
        ArcDirectory.CreateTillDirectory(specialFolderPath);
        foreach (ArcFile file in ArcDirectory.GetFiles(Path.Combine(GfxFolder, "special")))
        {
            ArcDirectory.GfxVDir.Add(Path.Combine(specialFolderPath, file.File()));
            ArcDirectory.Copy(file.Relative(), Path.Combine(specialFolderPath, file.File()));
        }

        // Process flags
        string flagsFolderPath = Path.Combine(TranspileTarget, "gfx", "flags");
        ArcDirectory.CreateTillDirectory(flagsFolderPath);
        foreach (string filePath in ArcDirectory.GetFiles(Path.Combine(GfxFolder, "flags")))
        {
            string fileName = Path.GetFileName(filePath);
            string oldPath = Path.Combine(GfxFolder, "flags", fileName);
            string newPath = Path.Combine(flagsFolderPath, $"{Country.Countries[Path.GetFileNameWithoutExtension(fileName)].Tag}.tga");

            ArcDirectory.GfxVDir.Add(newPath);
            ArcDirectory.Copy(oldPath, newPath);
        }

        // Finalize the block
        b.Add("}");

        // Overwrite the configuration file with the generated content
        OverwriteFile(Path.Combine(TranspileTarget, "interface", "arc5.gfx"), string.Join(' ', b), vdirOverride: ArcDirectory.GfxVDir);

        return "Gfx Folders";
    }
    
    static string Unsorted()
    {
        return Unsorted("");
    }
    /// <summary>
    /// Handles the Unsorted folder transpilation.
    /// </summary>
    /// <param name="fileEnd">The file extension to filter the files that should be processed.</param>
    /// <returns>The name of the function indicating the completion status of processing unsorted files.</returns>
    static string Unsorted(string fileEnd)
    {
        // Start processing from the root unsorted folder
        ProcessFolder(UnsortedFolder);

        // Recursive method to process each folder and its contents
        void ProcessFolder(string folderPath)
        {
            // Get subdirectories within the current folder
            IEnumerable<ArcPath> subFolders = ArcDirectory.GetFolders(folderPath);
            foreach (ArcPath subFolder in subFolders)
            {
                ProcessFolder(subFolder); // Recursive call for subfolders
            }

            // Construct the target folder path for the transpiled files
            string relativePath = Path.GetRelativePath(UnsortedFolder, folderPath);
            string targetFolderPath = Path.Combine(TranspileTarget, relativePath).Replace('\\', '/');
            ArcDirectory.CreateTillDirectory(targetFolderPath);

            // Get files within the current folder
            IEnumerable<ArcPath> files = ArcDirectory.GetFiles(folderPath);
            foreach (ArcPath file in files)
            {
                // Process only files that match the specified extension
                if (file.value.EndsWith(fileEnd, StringComparison.OrdinalIgnoreCase))
                {
                    ProcessFile(file);
                }
            }
        }

        // Method to process an individual file
        void ProcessFile(string filePath)
        {
            string relativeFilePath = filePath.Replace('\\', '/'); // Ensure path consistency
            string targetFilePath = Path.Combine(TranspileTarget, Path.GetRelativePath(UnsortedFolder, relativeFilePath)).Replace('\\', '/');

            ArcDirectory.UnsortedVDir.Add(targetFilePath); // Add to virtual directory
            ArcDirectory.Copy(relativeFilePath, targetFilePath); // Copy the file to the target location
        }

        return "Unsorted Files";
    }
    static void UnsortedSingleFile(ArcFile file)
    {
        string cfile = file.Relative();
        string tfile = $"{TranspileTarget}\\{file.Relative(UnsortedFolder)}";
        ArcDirectory.UnsortedVDir.Add(tfile);
        ArcDirectory.Copy(cfile, tfile);
    }
    public static string SpecialUnitTranspile()
    {
        IArcObject specialUnits = (IArcObject)Compiler.global["special_units"];

        IArcObject galleass = (IArcObject?)specialUnits.Get("galleass") ?? throw new Exception();
        IArcObject musketeer = (IArcObject?)specialUnits.Get("musketeer") ?? throw new Exception();
        IArcObject rajput = (IArcObject?)specialUnits.Get("rajput") ?? throw new Exception();

        Block staticModifiers = new()
            {
                ((ArcModifier?)galleass.Get("modifier") ?? throw new Exception()).Compile("galleass_modifier"),
                ((ArcModifier?)galleass.Get("ship") ?? throw new Exception()).Compile("galleass_ship"),
                ((ArcModifier?)musketeer.Get("modifier") ?? throw new Exception()).Compile("musketeer_modifier"),
                ((ArcModifier?)musketeer.Get("regiment") ?? throw new Exception()).Compile("musketeer_regiment"),
                ((ArcModifier?)rajput.Get("regiment") ?? throw new Exception()).Compile("rajput_regiment"),
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
    public static void CreateTillFolder(string fold) => ArcDirectory.CreateTillDirectory(fold);
    public static void OverwriteFile(string path, string text, bool AllowFormatting = true, bool BOM = false, VDirType? vdirOverride = null, bool ForceFormatting = false) => ArcDirectory.OverwriteFile(path, text, AllowFormatting, BOM, vdirOverride, ForceFormatting);
    public static string ReplaceBlocks(string text)
    {
        Regex blockSpot = ArcBlocks();
        while (text.Contains("__ARC.BLOCK__"))
        {
            text = blockSpot.Replace(text, new MatchEvaluator((m) =>
            {
                int id = int.Parse(m.Groups[1].Value);
                return CompileList.list[id].Compiled;
            }));
        }
        return text;
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
        OverwriteFile($"{TranspileTarget}/localisation/replace/z_arc_valley_l_english.yml", sb.ToString(), false, true);
        return "Localisations";
    }
    public static void LoadTarget(string path)
    {
        string fileLocation = Path.Combine(directory, path);

        if (fileLocation.EndsWith("/*"))
        {
            string current = fileLocation[..^1];

            LoadTarget(current);

            foreach (string folder in ArcDirectory.GetDirectories(current))
            {   
                string next = Path.GetRelativePath(directory, folder) + "/*";

                LoadTarget(next);
            }
        }
        else if (fileLocation.EndsWith("/"))
        {
            ArcPath[] files = ArcDirectory.GetFiles(fileLocation);
            foreach (ArcPath file in files)
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

    [GeneratedRegex("__ARC\\.BLOCK__ = (\\d+)")]
    private static partial Regex ArcBlocks();
}