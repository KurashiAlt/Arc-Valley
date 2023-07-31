using ArcInstance;
using Pastel;
using System.IO;
using System.Text;

namespace Arc;
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
    public ArcBlock Manufactory { get; set; }
    public ArcBlock BuildTrigger { get; set; }
    public ArcBlock Modifier { get; set; }
    public ArcBlock AiWillDo { get; set; }
    public ArcBlock OnBuilt { get; set; }
    public ArcBlock OnDestroyed { get; set; }
    public ArcBlock OnObsolete { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public Building(ArcString id, ArcString name, ArcString desc, ArcInt cost, ArcInt time, Building? makeObsolete, ArcBool onePerCountry, ArcBool allowInGoldProvinces, ArcBool indestructible, ArcBool onMap, ArcBool influencingFort, ArcBlock manufactory, ArcBlock buildTrigger, ArcBlock modifier, ArcBlock aiWillDo, ArcBlock onBuilt, ArcBlock onDestroyed, ArcBlock onObsolete)
    {
        Id = id;
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
        BuildTrigger = buildTrigger;
        Modifier = modifier;
        AiWillDo = aiWillDo;
        OnBuilt = onBuilt;
        OnDestroyed = onDestroyed;
        OnObsolete = onObsolete;
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
            { "build_trigger", BuildTrigger },
            { "modifier", Modifier },
            { "ai_will_do", AiWillDo },
            { "on_built", OnBuilt },
            { "on_destroyed", OnDestroyed },
            { "on_obsolete", OnObsolete },
        };
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);
        Building building = new(
            new(id),
            args.Get                (ArcString.Constructor, "name"),
            args.Get                (ArcString.Constructor, "desc"),
            args.Get                (ArcInt.Constructor,    "cost"),
            args.Get                (ArcInt.Constructor,    "time"),
            args.GetFromListNullable(Buildings,             "make_obsolete"),
            args.Get         (ArcBool.Constructor,   "one_per_country", new(false)),
            args.Get         (ArcBool.Constructor,   "allow_in_gold_provinces", new(false)),
            args.Get         (ArcBool.Constructor,   "indestructible", new(false)),
            args.Get         (ArcBool.Constructor,   "onmap", new(false)),
            args.Get         (ArcBool.Constructor,   "influencing_fort", new(false)),
            args.Get         (ArcBlock.Constructor,  "manufactory", new()),
            args.Get         (ArcBlock.Constructor,  "build_trigger", new()),
            args.Get         (ArcBlock.Constructor,  "modifier", new()),
            args.Get         (ArcBlock.Constructor,  "ai_will_do", new("factor = 1")),
            args.Get         (ArcBlock.Constructor,  "on_built", new()),
            args.Get         (ArcBlock.Constructor,  "on_destroyed", new()),
            args.Get         (ArcBlock.Constructor, "on_obsolete", new())
        );

        Buildings.Add(id, building);

        return i;
    }
    public static void Transpile()
    {
        StringBuilder sb = new("");
        foreach (Building building in Buildings.Values())
        {
            sb.Append($"{building.Id} = {{ ");
            if (building.Manufactory.Value.Count == 0) sb.Append($" cost = {building.Cost}");
            if (building.Manufactory.Value.Count == 0) sb.Append($" time = {building.Time}");
            if (building.MakeObsolete != null) sb.Append($" make_obsolete = {building.MakeObsolete.Id}");
            if (building.OnePerCountry.Value) sb.Append($" one_per_country = yes");
            if (building.AllowInGoldProvinces.Value) sb.Append($" allow_in_gold_provinces = yes");
            if (building.Indestructible.Value) sb.Append($" indestructible = yes");
            if (building.OnMap.Value) sb.Append($" onmap = yes");
            if (building.InfluencingFort.Value) sb.Append($" influencing_fort = yes");
            if (building.Manufactory.Value.Count != 0) sb.Append($" manufactory = {{ {building.Manufactory} }}");
            if (building.BuildTrigger.Value.Count != 0) sb.Append($" build_trigger = {{ {building.BuildTrigger} }}");
            sb.Append($" modifier = {{ {building.Modifier} }}");
            if (building.AiWillDo.Value.Count != 0 && building.Id.Value != "manufactory") sb.Append($" ai_will_do = {{ {building.AiWillDo} }}");
            if (building.OnBuilt.Value.Count != 0) sb.Append($" on_built = {{ {building.OnBuilt} }}");
            if (building.OnDestroyed.Value.Count != 0) sb.Append($" on_destroyed = {{ {building.OnDestroyed} }}");
            if (building.OnObsolete.Value.Count != 0) sb.Append($" on_obsolete = {{ {building.OnObsolete} }}");
            sb.Append(" } ");

            Instance.Localisation.Add($"building_{building.Id}", building.Name.Value);
            Instance.Localisation.Add($"building_{building.Id}_desc", building.Desc.Value);
        }
        Instance.OverwriteFile("target/common/buildings/arc.txt", sb.ToString());
        Console.WriteLine($"Finished Transpiling Buildings".Pastel(ConsoleColor.Cyan));
    }

    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref List<string> result, Compiler comp) { result.Add(Id.Value.ToString()); return i; }
}
