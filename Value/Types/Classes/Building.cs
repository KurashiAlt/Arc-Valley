using ArcInstance;
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

        if (args.keyValuePairs == null) throw new Exception();
        if (!args.keyValuePairs.ContainsKey("unlock_tier")) args.keyValuePairs.Add("unlock_tier", new());
        if (!args.keyValuePairs.ContainsKey("x")) args.keyValuePairs.Add("x", new());
        if (!args.keyValuePairs.ContainsKey("y")) args.keyValuePairs.Add("y", new());

        int startOffset = args.Get(ArcInt.Constructor, "start_offset", new(0)).Value;
        int x = args.Get(ArcInt.Constructor, "x").Value;
        int y = args.Get(ArcInt.Constructor, "y").Value;

        ArcList<Building> buildings = new();
        foreach (KeyValuePair<string, ArcCode> tier in tiers)
        {
            if (int.TryParse(tier.Key, out int i))
            {
                args.keyValuePairs["unlock_tier"] = new($"{i-startOffset}");
                args.keyValuePairs["x"] = new($"{x + (i-1) * 60}");
                args.keyValuePairs["y"] = new($"{y}");
            }
            else continue;
            

            Args tArgs = Args.GetArgs(tier.Value.Value, args);

            buildings.Add(Building.Constructor($"{id}_{tier.Key}", tArgs));
        }

        return new(
            id,
            args.Get(ArcString.Constructor, "name"),
            buildings
        );
    }
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

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
            { "allow_in_gold_provinces", args.Get(ArcBool.Constructor, "allow_in_gold_provinces", new(false)) },
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
            { "idea_group_unlocks", args.Get(ArcList<IdeaGroup>.GetConstructor(IdeaGroup.IdeaGroups), "idea_group_unlocks", null) },
            { "reform_unlocks", args.Get(ArcList<GovernmentReform>.GetConstructor(GovernmentReform.GovernmentReforms), "reform_unlocks", null) },
            { "unlock_tier", args.Get(ArcInt.Constructor, "unlock_tier", null) },
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
        b.Add("allow_in_gold_provinces", Get<ArcBool>("allow_in_gold_provinces"));
        b.Add("indestructible", Get<ArcBool>("indestructible"));
        b.Add("onmap", Get<ArcBool>("onmap"));
        b.Add("influencing_fort", Get<ArcBool>("influencing_fort"));
        Manufactory.Compile("manufactory", ref b);

        Block c = new Block
        {
            "build_trigger", "=", "{"
        };
        ArcInt? UnlockTier = GetNullable<ArcInt>("unlock_tier");
        ArcList<IdeaGroup>? IdeaGroupUnlocks = GetNullable<ArcList<IdeaGroup>?>("idea_group_unlocks");
        ArcList<GovernmentReform>? ReformUnlocks = GetNullable<ArcList<GovernmentReform>?>("reform_unlocks");
        if (UnlockTier != null && UnlockTier.Value > 0)
        {
            c.Add(
                "FROM", "=", "{",
                    "calc_true_if", "=", "{",
                        "amount", "=", UnlockTier
            );
            if (IdeaGroupUnlocks != null)
            {
                foreach (IdeaGroup? ideaGroup in IdeaGroupUnlocks.Values)
                {
                    if (ideaGroup == null) continue;
                    c.Add("full_idea_group", "=", ideaGroup.Id);
                }
            }
            if (ReformUnlocks != null)
            {
                foreach (GovernmentReform? reform in ReformUnlocks.Values)
                {
                    if (reform == null) continue;
                    c.Add("has_reform", "=", reform.Id);
                }
            }
            c.Add(
                    "}",
                "}"
            );
        }
        Get<ArcTrigger>("build_trigger").Compile(ref c);
        c.Add("}");
        if (c.Count > 4) b.Add(c);

        Get<ArcModifier>("modifier").Compile("modifier", ref b);
        if (id != "manufactory") Get<ArcCode>("ai_will_do").Compile("ai_will_do", ref b);
        Get<ArcEffect>("on_built").Compile("on_built", ref b);
        Get<ArcEffect>("on_destroyed").Compile("on_destroyed", ref b);
        Get<ArcEffect>("on_obsolete").Compile("on_obsolete", ref b);
        b.Add("}");

        string desc = Get<ArcString>("desc").Value;
        if (UnlockTier != null && UnlockTier.Value > 0)
        {
            if (desc != "") desc += "\n\n";
            desc += $"At least {UnlockTier}:";

            if(IdeaGroupUnlocks != null)
            {
                desc += string.Join(' ', from ide in IdeaGroupUnlocks.Values select $"\n\tHas Completed §Y{ide.Name}§! Ideas");
            }
            if(ReformUnlocks != null)
            {
                desc += string.Join(' ', from ide in ReformUnlocks.Values select $"\n\tHas Enacted §Y{ide.Name}§!");
            }
        }

        Program.Localisation.Add($"building_{id}", Get<ArcString>("name").Value);
        Program.Localisation.Add($"building_{id}_desc", desc);

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
