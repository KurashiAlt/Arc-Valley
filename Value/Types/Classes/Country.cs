namespace Arc;
public class Country : IArcObject
{
    public static readonly Dict<Country> Countries = new();
    public bool IsObject() => true;
    public ArcBlock HistoricalIdeaGroups { get; set; }
    public ArcBlock HistoricalUnits { get; set; }
    public ArcBlock MonarchNames { get; set; }
    public ArcBlock LeaderNames { get; set; }
    public ArcBlock ShipNames { get; set; }
    public ArcBlock ArmyNames { get; set; }
    public ArcBlock FleetNames { get; set; }
    public ArcString Tag { get; set; }
    public ArcString Name { get; set; }
    public ArcString Adj { get; set; }
    public ArcBlock Color { get; set; }
    public ArcString Government { get; set; }
    public ArcInt GovernmentRank { get; set; }
    public ArcInt Mercantilism { get; set; }
    public ArcString TechnologyGroup { get; set; }
    public ArcString Religion { get; set; }
    public ArcString PrimaryCulture { get; set; }
    public ArcString GraphicalCulture { get; set; }
    public ArcBlock Definitions { get; set; }
    public ArcBlock History { get; set; }
    public Province Capital { get; set; }
    public Dict<IVariable?> keyValuePairs { get; set; }
    public Country(ArcBlock historicalIdeaGroups, ArcBlock historicalUnits, ArcBlock monarchNames, ArcBlock leaderNames, ArcBlock shipNames, ArcBlock armyNames, ArcBlock fleetNames, ArcString tag, ArcString name, ArcString adj, ArcBlock color, ArcString government, ArcInt governmentRank, ArcInt mercantilism, ArcString technologyGroup, ArcString religion, ArcString primaryCulture, ArcString graphicalCulture, ArcBlock definitions, ArcBlock history, Province capital)
    {
        HistoricalIdeaGroups = historicalIdeaGroups;
        HistoricalUnits = historicalUnits;
        MonarchNames = monarchNames;
        LeaderNames = leaderNames;
        ShipNames = shipNames;
        ArmyNames = armyNames;
        FleetNames = fleetNames;
        Tag = tag;
        Name = name;
        Adj = adj;
        Color = color;
        Government = government;
        GovernmentRank = governmentRank;
        Mercantilism = mercantilism;
        TechnologyGroup = technologyGroup;
        Religion = religion;
        PrimaryCulture = primaryCulture;
        GraphicalCulture = graphicalCulture;
        Definitions = definitions;
        History = history;
        Capital = capital;
        keyValuePairs = new()
        {
            { "historical_idea_groups", HistoricalIdeaGroups },
            { "historical_units", HistoricalUnits },
            { "monarch_names", MonarchNames },
            { "leader_names", LeaderNames },
            { "ship_names", ShipNames },
            { "army_names", ArmyNames },
            { "fleet_names", FleetNames },
            { "tag", Tag },
            { "name", Name },
            { "adj", Adj },
            { "color", Color },
            { "government", Government },
            { "government_rank", GovernmentRank },
            { "mercantilism", Mercantilism },
            { "technology_group", TechnologyGroup },
            { "religion", Religion },
            { "primary_culture", PrimaryCulture },
            { "graphical_culture", GraphicalCulture },
            { "definitions", Definitions },
            { "history", History },
            { "capital", Capital },
        };
    }
    public bool CanGet(string indexer) => keyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => keyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string key = i.Current;

        i = Args.GetArgs(i, out Args args);

        Country countr = new(
            args.GetDefault(ArcBlock.Constructor, "historical_idea_groups", new()),
            args.GetDefault(ArcBlock.Constructor, "historical_units", new()),
            args.GetDefault(ArcBlock.Constructor, "monarch_names", new()),
            args.GetDefault(ArcBlock.Constructor, "leader_names", new()),
            args.GetDefault(ArcBlock.Constructor, "ship_names", new()),
            args.GetDefault(ArcBlock.Constructor, "army_names", new("Army of $PROVINCE$")),
            args.GetDefault(ArcBlock.Constructor, "fleet_names", new("Fleet of $PROVINCE$")),
            args.Get(ArcString.Constructor, "tag"),
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "adj"),
            args.Get(ArcBlock.Constructor, "color"),
            args.GetDefault(ArcString.Constructor, "government", new("monarchy")),
            args.GetDefault(ArcInt.Constructor, "government_rank", new(1)),
            args.GetDefault(ArcInt.Constructor, "mercantilism", new(1)),
            args.Get(ArcString.Constructor, "technology_group"),
            args.Get(ArcString.Constructor, "religion"),
            args.Get(ArcString.Constructor, "primary_culture"),
            args.Get(ArcString.Constructor, "graphical_culture"),
            args.GetDefault(ArcBlock.Constructor, "definitions", new()),
            args.GetDefault(ArcBlock.Constructor, "history", new()),
            args.GetFromList(Province.Provinces, "capital")
        );

        Countries.Add(key, countr);

        return i;
    }

    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref List<string> result, Compiler comp) { result.Add(Tag.Value.ToString()); return i; }
}
