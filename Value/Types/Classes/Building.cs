using ArcInstance;
using Pastel;
using System.IO;
using System.Text;

namespace Arc;
public class BuildingLine : IArcObject
{
    public static readonly Dict<BuildingLine> BuildingLines = new();
    public ArcString Name { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public BuildingLine(string id, ArcString name)
    {
        Name = name;
        KeyValuePairs = new()
        {
            { "name", Name }
        };

        BuildingLines.Add(id, this);
    }
    public static BuildingLine Constructor(string id, Args args)
    {
        Dict<ArcCode> tiers = args.GetAttributes(new string[] { "name", "start_offset" });

        if (args.keyValuePairs == null) throw new Exception();
        if (!args.keyValuePairs.ContainsKey("unlock_tier")) args.keyValuePairs.Add("unlock_tier", new());

        int startOffset = args.Get(ArcInt.Constructor, "start_offset", new(0)).Value;

        foreach (KeyValuePair<string, ArcCode> tier in tiers)
        {
            if (int.TryParse(tier.Key, out int i))
            {
                args.keyValuePairs["unlock_tier"] = new($"{i-startOffset}");
            }
            else continue;
            

            Args tArgs = Args.GetArgs(tier.Value.Value, args);

            Building.Constructor($"{id}_{tier.Key}", tArgs);
        }

        return new(
            id,
            args.Get(ArcString.Constructor, "name")
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
    public Walker Call(Walker i, ref Block result) => throw new NotImplementedException();
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
}

public class Building : IArcObject
{
    public static readonly Dict<Building> Buildings = new();
    public bool IsObject() => true;
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }
    public ArcInt Cost { get; set; }
    public ArcInt Time { get; set; }
    public Building? MakeObsolete { get; set; }
    public ArcBool OnePerCountry { get; set; }
    public ArcBool AllowInGoldProvinces { get; set; }
    public ArcBool Indestructible { get; set; }
    public ArcBool OnMap { get; set; }
    public ArcBool InfluencingFort { get; set; }
    public ArcCode Manufactory { get; set; }
    public ArcTrigger Potential { get; set; }
    public ArcTrigger BuildTrigger { get; set; }
    public ArcModifier Modifier { get; set; }
    public ArcCode AiWillDo { get; set; }
    public ArcEffect OnBuilt { get; set; }
    public ArcEffect OnDestroyed { get; set; }
    public ArcEffect OnObsolete { get; set; }
    public ArcList<IdeaGroup>? IdeaGroupUnlocks { get; set; }
    public ArcList<GovernmentReform>? ReformUnlocks { get; set; }
    public ArcInt? UnlockTier { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public Building(string id, ArcString name, ArcString desc, ArcInt cost, ArcInt time, Building? makeObsolete, ArcBool onePerCountry, ArcBool allowInGoldProvinces, ArcBool indestructible, ArcBool onMap, ArcBool influencingFort, ArcCode manufactory, ArcTrigger potential, ArcTrigger buildTrigger, ArcModifier modifier, ArcCode aiWillDo, ArcEffect onBuilt, ArcEffect onDestroyed, ArcEffect onObsolete, ArcList<IdeaGroup>? ideaGroupUnlocks, ArcList<GovernmentReform>? reformUnlocks, ArcInt? unlockTier)
    {
        Id = new(id);
        Name = name;
        Desc = desc;
        Cost = cost;
        Time = time;
        MakeObsolete = makeObsolete;
        OnePerCountry = onePerCountry;
        AllowInGoldProvinces = allowInGoldProvinces;
        Indestructible = indestructible;
        OnMap = onMap;
        InfluencingFort = influencingFort;
        Manufactory = manufactory;
        Potential = potential;
        BuildTrigger = buildTrigger;
        Modifier = modifier;
        AiWillDo = aiWillDo;
        OnBuilt = onBuilt;
        OnDestroyed = onDestroyed;
        OnObsolete = onObsolete;
        IdeaGroupUnlocks = ideaGroupUnlocks;
        ReformUnlocks = reformUnlocks;
        UnlockTier = unlockTier;
        KeyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "desc", Desc },
            { "cost", Cost },
            { "time", Time },
            { "make_obsolete", MakeObsolete },
            { "one_per_country", OnePerCountry },
            { "allow_in_gold_provinces", AllowInGoldProvinces },
            { "indestructible", Indestructible },
            { "onmap", OnMap },
            { "influencing_fort", InfluencingFort },
            { "manufactory", Manufactory },
            { "potential", Potential },
            { "build_trigger", BuildTrigger },
            { "modifier", Modifier },
            { "ai_will_do", AiWillDo },
            { "on_built", OnBuilt },
            { "on_destroyed", OnDestroyed },
            { "on_obsolete", OnObsolete },
            { "idea_group_unlocks", IdeaGroupUnlocks },
            { "reform_unlocks", ReformUnlocks },
            { "unlock_tier", UnlockTier },
        };

        Buildings.Add(id, this);
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);

        Constructor(id, args);

        return i;
    }
    public static Building Constructor(string id, Args args)
    {
        return new(
            id,
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc"),
            args.Get(ArcInt.Constructor, "cost"),
            args.Get(ArcInt.Constructor, "time"),
            args.GetFromListNullable(Buildings, "make_obsolete"),
            args.Get(ArcBool.Constructor, "one_per_country", new(false)),
            args.Get(ArcBool.Constructor, "allow_in_gold_provinces", new(false)),
            args.Get(ArcBool.Constructor, "indestructible", new(false)),
            args.Get(ArcBool.Constructor, "onmap", new(false)),
            args.Get(ArcBool.Constructor, "influencing_fort", new(false)),
            args.Get(ArcCode.Constructor, "manufactory", new()),
            args.Get(ArcTrigger.Constructor, "potential", new()),
            args.Get(ArcTrigger.Constructor, "build_trigger", new()),
            args.Get(ArcModifier.Constructor, "modifier", new()),
            args.Get(ArcCode.Constructor, "ai_will_do", new("factor = 1")),
            args.Get(ArcEffect.Constructor, "on_built", new()),
            args.Get(ArcEffect.Constructor, "on_destroyed", new()),
            args.Get(ArcEffect.Constructor, "on_obsolete", new()),
            args.Get(ArcList<IdeaGroup>.GetConstructor(IdeaGroup.IdeaGroups), "idea_group_unlocks", null),
            args.Get(ArcList<GovernmentReform>.GetConstructor(GovernmentReform.GovernmentReforms), "reform_unlocks", null),
            args.Get(ArcInt.Constructor, "unlock_tier", null)
        );
    }
    public void TranspileThis(ref Block b)
    {
        b.Add(Id, "=", "{");
        if (Manufactory.Count() == 0) b.Add(
            "cost", "=", Cost,
            "time", "=", Time
        );
        if (MakeObsolete != null) b.Add("make_obsolete", "=", MakeObsolete.Id);
        b.Add("one_per_country", OnePerCountry);
        b.Add("allow_in_gold_provinces", AllowInGoldProvinces);
        b.Add("indestructible", Indestructible);
        b.Add("onmap", OnMap);
        b.Add("influencing_fort", InfluencingFort);
        Manufactory.Compile("manufactory", ref b);

        Block c = new Block
        {
            "build_trigger", "=", "{"
        };
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
        BuildTrigger.Compile(ref c);
        c.Add("}");
        if (c.Count > 4) b.Add(c);

        Modifier.Compile("modifier", ref b);
        if (Id.Value != "manufactory") AiWillDo.Compile("ai_will_do", ref b);
        OnBuilt.Compile("on_built", ref b);
        OnDestroyed.Compile("on_destroyed", ref b);
        OnObsolete.Compile("on_obsolete", ref b);
        b.Add("}");

        string desc = Desc.Value;
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

        Instance.Localisation.Add($"building_{Id}", Name.Value);
        Instance.Localisation.Add($"building_{Id}_desc", desc);
    }
    public static string Transpile()
    {
        Block b = new();
        foreach (Building building in Buildings.Values())
        {
            building.TranspileThis(ref b);
        }
        Instance.OverwriteFile($"{Instance.TranspileTarget}/common/buildings/arc.txt", string.Join(' ', b));
        return "Buildings";
    }

    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}
