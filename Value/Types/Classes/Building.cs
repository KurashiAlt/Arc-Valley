
using Microsoft.CSharp.RuntimeBinder;
using Pastel;
using System.IO;
using System.Text;

namespace Arc;
public class BuildingLine : IArcObject
{
    public static readonly Dict<BuildingLine> BuildingLines = new();
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcList<Building> Buildings { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public BuildingLine(string id, ArcString name, ArcList<Building> buildings)
    {
        Id = new(id);
        Name = name;
        Buildings = buildings;
        KeyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "buildings", Buildings }
        };

        BuildingLines.Add(id, this);
    }
    public static BuildingLine Constructor(string id, Args args)
    {
        Dict<ArcCode> tiers = args.GetAttributes(new string[] { "name", "start_offset", "x", "y" });
        ArcString name = args.Get(ArcString.Constructor, "name");

        if (args.keyValuePairs == null) throw ArcException.Create(id, args, tiers);
        if (!args.keyValuePairs.ContainsKey("build_trigger")) args.keyValuePairs.Add("build_trigger", new());
        if (!args.keyValuePairs.ContainsKey("x")) args.keyValuePairs.Add("x", new());
        if (!args.keyValuePairs.ContainsKey("y")) args.keyValuePairs.Add("y", new());

        int startOffset = args.Get(ArcInt.Constructor, "start_offset", new(0)).Value;
        int x = args.Get(ArcInt.Constructor, "x").Value;
        int y = args.Get(ArcInt.Constructor, "y").Value;

        ArcObject tThis = new()
        {
            { "id", new ArcString($"{id}_line") },
            { "name", new ArcString($"Unlocked {name} buildings") },
            { "is_percentage", new ArcBool(false) },
            { "trigger", new ArcTrigger("always", "=", "yes") }
        };
        ArgList.Add("this", tThis);
        Compiler.GetVariable<Dict<IVariable>>(new Word("modifier_definitions")).Add($"{id}_line", tThis);
        ArcClass.Classes["modifier_definition"].OnCreate.Compile();
        ArgList.Drop("this");

        Block buildTrigger = args.keyValuePairs["build_trigger"];
        ArcList<Building> buildings = new();
        foreach (KeyValuePair<string, ArcCode> tier in tiers)
        {
            if (int.TryParse(tier.Key, out int i))
            {
                args.keyValuePairs["x"] = new($"{x + (i-1) * 60}");
                args.keyValuePairs["y"] = new($"{y}");
                Block copyBuildTrigger = new(buildTrigger);
                foreach (Word w in copyBuildTrigger)
                {
                    if (w.Value == "$tier$") w.Value = i.ToString();
                }
                args.keyValuePairs["build_trigger"] = copyBuildTrigger;
            }
            else continue;
            

            Args tArgs = Args.GetArgs(tier.Value.Value, args);

            buildings.Add(Building.Constructor($"{id}_{tier.Key}", tArgs));
        }

        return new(
            id,
            name,
            buildings
        );
    }
    public static Walker Call(Walker i)
    {
        i.ForceMoveNext();

        string id = Compiler.GetId(i.Current);

        i = Args.GetArgs(i, out Args args);

        Constructor(id, args);

        return i;
    }
    public Walker Call(Walker i, ref Block result) 
    {
        result.Add(Id.Value);
        return i;
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
}

public class Building : ArcObject
{
    public static readonly Dict<Building> Buildings = new();
    public Building(string id)
    {
        Buildings.Add(id, this);
    }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static Building Constructor(string id, Args args)
    {
        Building building = new(id)
        {
            { "id", new ArcString(id) },
            { "name", args.Get(ArcString.Constructor, "name") },
            { "desc", args.Get(ArcString.Constructor, "desc") },
            { "cost", args.Get(ArcInt.Constructor, "cost") },
            { "time", args.Get(ArcInt.Constructor, "time") },
            { "make_obsolete", args.GetFromListNullable(Buildings, "make_obsolete") },
            { "one_per_country", args.Get(ArcBool.Constructor, "one_per_country", new(false)) },
            { "allow_in_gold_provinces", args.Get(ArcBool.Constructor, "allow_in_gold_provinces", null) },
            { "indestructible", args.Get(ArcBool.Constructor, "indestructible", new(false)) },
            { "onmap", args.Get(ArcBool.Constructor, "onmap", new(false)) },
            { "influencing_fort", args.Get(ArcBool.Constructor, "influencing_fort", new(false)) },
            { "manufactory", args.Get(ArcCode.Constructor, "manufactory", new()) },
            { "potential", args.Get(ArcTrigger.Constructor, "potential", new()) },
            { "build_trigger", args.Get(ArcTrigger.Constructor, "build_trigger", new()) },
            { "modifier", args.Get(ArcModifier.Constructor, "modifier", new()) },
            { "ai_will_do", args.Get(ArcCode.Constructor, "ai_will_do", new("factor = 1")) },
            { "on_built", args.Get(ArcEffect.Constructor, "on_built", new()) },
            { "on_destroyed", args.Get(ArcEffect.Constructor, "on_destroyed", new()) },
            { "on_obsolete", args.Get(ArcEffect.Constructor, "on_obsolete", new()) },
            { "x", args.Get(ArcInt.Constructor, "x", null) },
            { "y", args.Get(ArcInt.Constructor, "y", null) }
        };
        return building;
    }
    public void TranspileThis(ref Block b, ref Block a)
    {
        string id = Get<ArcString>("id").Value;
        ArcCode Manufactory = Get<ArcCode>("manufactory");
        Building? MakeObsolete = GetNullable<Building>("make_obsolete");
        ArcInt Cost = Get<ArcInt>("cost");
        ArcInt Time = Get<ArcInt>("time");

        b.Add(id, "=", "{");
        if (Manufactory.Count() == 0) b.Add(
            "cost", "=", Cost,
            "time", "=", Time
        );
        if (MakeObsolete != null)
        {
            b.Add("make_obsolete", "=", MakeObsolete.Get<ArcString>("id"));
            ArcInt? obsoX = MakeObsolete.GetNullable<ArcInt>("x");
            ArcInt? obsoY = MakeObsolete.GetNullable<ArcInt>("y");
            ArcInt? thisX = GetNullable<ArcInt>("x");
            ArcInt? thisY = GetNullable<ArcInt>("y");
            if(obsoX != null && obsoY != null && thisX != null && thisY != null)
            {
                a.Add(
                    "iconType", "=", "{",
                        "name", "=", $"{MakeObsolete.Get<ArcString>("id")}_{id}_arrow",
                        "position", "=", "{",
                            "x", "=", (obsoX.Value + thisX.Value) / 2 + 18,
                            "y", "=", (obsoY.Value + thisY.Value) / 2 + 10,
                        "}",
                        "spriteType", "=", "\"GFX_building_upgrade_icon\"",
                    "}"
                );
            }
        }
        b.Add("one_per_country", Get<ArcBool>("one_per_country"));
        if(CanGet("allow_in_gold_provinces")) b.Add("allow_in_gold_provinces", "=", Get<ArcBool>("allow_in_gold_provinces"));
        b.Add("indestructible", Get<ArcBool>("indestructible"));
        b.Add("onmap", Get<ArcBool>("onmap"));
        b.Add("influencing_fort", Get<ArcBool>("influencing_fort"));
        Manufactory.Compile("manufactory", ref b);

        Get<ArcTrigger>("build_trigger").Compile("build_trigger", ref b);

        Get<ArcModifier>("modifier").Compile("modifier", ref b);
        if (id != "manufactory") Get<ArcCode>("ai_will_do").Compile("ai_will_do", ref b);
        Get<ArcEffect>("on_built").Compile("on_built", ref b);
        Get<ArcEffect>("on_destroyed").Compile("on_destroyed", ref b);
        Get<ArcEffect>("on_obsolete").Compile("on_obsolete", ref b);
        b.Add("}");

        Program.Localisation.Add($"building_{id}", Get<ArcString>("name").Value);
        Program.Localisation.Add($"building_{id}_desc", Get<ArcString>("desc").Value);

        ArcInt? x = GetNullable<ArcInt>("x");
        ArcInt? y = GetNullable<ArcInt>("y");
        if(x  != null && y != null)
        {
            a.Add(
                "checkboxType", "=", "{",
                    "name", "=", $"\"build_{id}\"",
                    "position", "=", "{", 
                        "x", "=", x,
                        "y", "=", y,
                    "}",
                    "quadTextureSprite", "=", "\"GFX_building_default\"",
                "}",
                "instantTextBoxType", "=", "{",
                    "name", "=", $"\"{id}_cost\"",
                    "position", "=", "{",
                        "x", "=", x,
                        "y", "=", y.Value+50,
                    "}",
                    "font", "=", "\"vic_18\"",
                    "borderSize", "=", "{",
                        "x", "=", "0",
                        "y", "=", "0",
                    "}",
                    "maxWidth", "=", "128",
                    "maxHeight", "=", "18",
                "}"
            );
        }
    }
    public static string Transpile()
    {
        Block a = Compiler.global.Get<Dict<IValue>>("interface").Get<ArcCode>("buildings").Value;
        Block b = new();
        foreach (Building building in Buildings.Values())
        {
            building.TranspileThis(ref b, ref a);
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/common/buildings/arc.txt", string.Join(' ', b));

        string macro = Compiler.global.Get<Dict<IValue>>("interface").Get<ArcString>("macrobuildinterface").Value;
        string provinceview = Compiler.global.Get<Dict<IValue>>("interface").Get<ArcString>("provinceview").Value;
        macro = macro.Replace("$buildings$", string.Join(' ', a)).Trim('`');
        provinceview = provinceview.Replace("$buildings$", string.Join(' ', a)).Trim('`');

        Program.OverwriteFile($"{Program.TranspileTarget}/interface/macrobuildinterface.gui", macro, false);
        Program.OverwriteFile($"{Program.TranspileTarget}/interface/provinceview.gui", provinceview, false);

        return "Buildings";
    }
    public override string ToString() => Get<ArcString>("name").Value;
    public override Walker Call(Walker i, ref Block result) { result.Add(Get<ArcString>("id").Value.ToString()); return i; }
}
