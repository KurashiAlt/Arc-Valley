
using Pastel;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace Arc;
public class ConditionalModifier : ArcObject
{
    public static ConditionalModifier Constructor(Args args) => new()
    {
        { "trigger", args.Get(ArcTrigger.Constructor, "trigger") },
        { "modifier", args.Get(ArcModifier.Constructor, "modifier") }
    };
    public void Transpile(ref Block b)
    {
        b.Add("conditional_modifier", "=", "{");
        Get<ArcTrigger>("trigger").Compile("trigger", ref b);
        Get<ArcModifier>("modifier").Compile("modifier", ref b);
        b.Add("}");
    }
}
public class Tier : IArcObject
{
    public bool IsObject() => true;
    public ArcInt UpgradeTime { get; set; }
    public ArcInt CostToUpgrade { get; set; }
    public ArcModifier ProvinceModifier { get; set; }
    public ArcModifier AreaModifier { get; set; }
    public ArcModifier CountryModifier { get; set; }
    public ArcEffect OnUpgraded { get; set; }
    public ArcList<ConditionalModifier> ConditionalModifiers { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public Tier(
        ArcInt upgradeTime,
        ArcInt costToUpgrade,
        ArcModifier provinceModifier,
        ArcModifier areaModifier,
        ArcModifier countryModifier,
        ArcEffect onUpgraded,
        ArcList<ConditionalModifier> conditionalModifiers
    )
    {
        UpgradeTime = upgradeTime;
        CostToUpgrade = costToUpgrade;
        ProvinceModifier = provinceModifier;
        AreaModifier = areaModifier;
        CountryModifier = countryModifier;
        OnUpgraded = onUpgraded;
        ConditionalModifiers = conditionalModifiers;
        KeyValuePairs = new()
        {
            { "uprade_time", UpgradeTime },
            { "cost_to_upgrade", CostToUpgrade },
            { "province_modifier", ProvinceModifier },
            { "area_modifier", AreaModifier },
            { "country_modifier", CountryModifier },
            { "on_upgraded", OnUpgraded }
        };
    }
    public static Tier Constructor(Block b, int upgradeTime, int costToUpgrade)
    {
        if(!b.Any())
        {
            return new Tier(
                new(upgradeTime),
                new(costToUpgrade),
                new(),
                new(),
                new(),
                new(),
                new()
            );
        }

        Args args = Args.GetArgs(b);
        return Constructor(args, upgradeTime, costToUpgrade);
    }
    static Tier Constructor(Args args, int upgradeTime, int costToUpgrade) => new(
        args.Get(ArcInt.Constructor, "upgrade_time", new(upgradeTime)),
        args.Get(ArcInt.Constructor, "cost_to_upgrade", new(costToUpgrade)),
        args.Get(ArcModifier.Constructor, "province_modifier", new()),
        args.Get(ArcModifier.Constructor, "area_modifier", new()),
        args.Get(ArcModifier.Constructor, "country_modifier", new()),
        args.Get(ArcEffect.Constructor, "on_upgraded", new()),
        args.Get(ArcList<ConditionalModifier>.GetConstructor(ConditionalModifier.Constructor), "conditional_modifiers", new())
    );
    public void TranspileSingular(string key, ref Block b)
    {
        b.Add(
            key, "=", "{",
                "upgrade_time", "=", "{",
                    "months", "=", UpgradeTime,
                "}",
                "cost_to_upgrade", "=", "{",
                    "factor", "=", CostToUpgrade,
                "}"
        );
        ProvinceModifier.Compile("province_modifiers", ref b, CanBeEmpty: false);
        AreaModifier.Compile("area_modifier", ref b, CanBeEmpty: false);
        CountryModifier.Compile("country_modifiers", ref b, CanBeEmpty: false);
        OnUpgraded.Compile("on_upgraded", ref b, CanBeEmpty: false);
        foreach(ConditionalModifier c in ConditionalModifiers)
        {
            c.Transpile(ref b);
        }
        b.Add("}");
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public Walker Call(Walker i, ref Block result) => throw new Exception();
}
public class GreatProject : IArcObject
{
    public static readonly Dict<GreatProject> GreatProjects = new();
    public bool IsObject() => true;
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public Province Start { get; set; }
    public ArcString Date { get; set; }
    public ArcInt Time { get; set; }
    public ArcInt BuildCost { get; set; }
    public ArcBool CanBeMoved { get; set; }
    public ArcInt StartingTier { get; set; }
    public ArcString Type { get; set; }
    public ArcTrigger BuildTrigger { get; set; }
    public ArcEffect OnBuilt { get; set; }
    public ArcEffect OnDestroyed { get; set; }
    public ArcTrigger CanUseModifiersTrigger { get; set; }
    public ArcTrigger CanUpgradeTrigger { get; set; }
    public ArcTrigger KeepTrigger { get; set; }
    public Tier Tier0 { get; set; }
    public Tier Tier1 { get; set; }
    public Tier Tier2 { get; set; }
    public Tier Tier3 { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public GreatProject(
        string id,
        ArcString name,
        Province start,
        ArcString date,
        ArcInt time,
        ArcInt buildCost,
        ArcBool canBeMoved,
        ArcInt startingTier,
        ArcString type,
        ArcTrigger buildTrigger,
        ArcEffect onBuilt,
        ArcEffect onDestroyed,
        ArcTrigger canUseModifiersTrigger,
        ArcTrigger canUpgradeTrigger,
        ArcTrigger keepTrigger,
        Tier tier0,
        Tier tier1,
        Tier tier2,
        Tier tier3
    )
    {
        Id = new(id);
        Name = name;
        Start = start;
        Date = date;
        Time = time;
        BuildCost = buildCost;
        CanBeMoved = canBeMoved;
        StartingTier = startingTier;
        Type = type;
        BuildTrigger = buildTrigger;
        OnBuilt = onBuilt;
        OnDestroyed = onDestroyed;
        CanUseModifiersTrigger = canUseModifiersTrigger;
        CanUpgradeTrigger = canUpgradeTrigger;
        KeepTrigger = keepTrigger;
        Tier0 = tier0;
        Tier1 = tier1;
        Tier2 = tier2;
        Tier3 = tier3;

        KeyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "start", Start },
            { "date", Date },
            { "time", Time },
            { "build_cost", BuildCost },
            { "can_be_moved", CanBeMoved },
            { "starting_tier", StartingTier },
            { "type", Type },
            { "build_trigger", BuildTrigger },
            { "on_built", OnBuilt },
            { "on_destroyed", OnDestroyed },
            { "can_use_modifiers_trigger", CanUseModifiersTrigger },
            { "can_upgrade_trigger", CanUpgradeTrigger },
            { "keep_trigger", KeepTrigger },
            { "tier_0", Tier0 },
            { "tier_1", Tier1 },
            { "tier_2", Tier2 },
            { "tier_3", Tier3 },
        };

        GreatProjects.Add(id, this);
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = Compiler.GetId(i.Current);

        i = Args.GetArgs(i, out Args args);

        Constructor(id, args);

        return i;
    }
    public static GreatProject Constructor(string id, Args args)
    {
        return new GreatProject(
            id,
            args.Get(ArcString.Constructor, "name"),
            args.GetFromList(Province.Provinces, "start"),
            args.Get(ArcString.Constructor, "date", new("1.1.1")),
            args.Get(ArcInt.Constructor, "time", new(0)),
            args.Get(ArcInt.Constructor, "build_cost", new(0)),
            args.Get(ArcBool.Constructor, "can_be_moved", new(true)),
            args.Get(ArcInt.Constructor, "starting_tier", new(0)),
            args.Get(ArcString.Constructor, "type", new("monument")),
            args.Get(ArcTrigger.Constructor, "build_trigger", args.Get(ArcTrigger.Constructor, "trigger", new())),
            args.Get(ArcEffect.Constructor, "on_built", new()),
            args.Get(ArcEffect.Constructor, "on_destroyed", new()),
            args.Get(ArcTrigger.Constructor, "can_use_modifiers_trigger", args.Get(ArcTrigger.Constructor, "trigger", new())),
            args.Get(ArcTrigger.Constructor, "can_upgrade_trigger", args.Get(ArcTrigger.Constructor, "trigger", new())),
            args.Get(ArcTrigger.Constructor, "keep_trigger", new("always", "=", "yes")),
            Tier.Constructor(args.Get("tier_0", new()), 0, 0),
            Tier.Constructor(args.Get("tier_1", new()), 120, 1000),
            Tier.Constructor(args.Get("tier_2", new()), 240, 2500),
            Tier.Constructor(args.Get("tier_3", new()), 480, 5000)
        );
    }
    private void TranspileSingular(ref Block b)
    {
        b.Add(
            Id, "=", "{",
                "start", "=", Start.Id,
                "date", "=", Date,
                "time", "=", "{", 
                    "months", "=", Time,
                "}",
                "build_cost", "=", BuildCost,
                "can_be_moved", "=", CanBeMoved,
                "starting_tier", "=", StartingTier,
                "type", "=", Type,
                "move_days_per_unit_distance", "=", "100"
        );
        BuildTrigger.Compile("build_trigger", ref b, CanBeEmpty: false);
        OnBuilt.Compile("on_built", ref b, CanBeEmpty: false);
        OnDestroyed.Compile("on_destroyed", ref b, CanBeEmpty: false);
        CanUseModifiersTrigger.Compile("can_use_modifiers_trigger", ref b, CanBeEmpty: false);
        CanUpgradeTrigger.Compile("can_upgrade_trigger", ref b, CanBeEmpty: false);
        KeepTrigger.Compile("keep_trigger", ref b, CanBeEmpty: false);
        Tier0.TranspileSingular("tier_0", ref b);
        Tier1.TranspileSingular("tier_1", ref b);
        Tier2.TranspileSingular("tier_2", ref b);
        Tier3.TranspileSingular("tier_3", ref b);
        b.Add("}");

        Program.Localisation.Add(Id.Value, Name.Value);
    }
    public static string Transpile()
    {
        Block b = new();

        foreach (GreatProject GreatProject in GreatProjects.Values())
        {
            GreatProject.TranspileSingular(ref b);
        }

        Program.OverwriteFile($"{Program.TranspileTarget}/common/great_projects/arc.txt", string.Join(' ', b));
        return "Great Projects";
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}
