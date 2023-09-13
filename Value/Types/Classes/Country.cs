using ArcInstance;
using Pastel;
using System.Diagnostics.Metrics;
using System.Runtime.ConstrainedExecution;
using System.Text;

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
    public LazyPointer<Religion> iReligion { get; set; }
    public Culture PrimaryCulture { get; set; }
    public ArcString GraphicalCulture { get; set; }
    public ArcBlock Definitions { get; set; }
    public ArcBlock History { get; set; }
    public Province Capital { get; set; }
    public GovernmentReform? StartingReform { get; set; }
    public Dict<IVariable?> keyValuePairs { get; set; }
    public Country(string key, ArcBlock historicalIdeaGroups, ArcBlock historicalUnits, ArcBlock monarchNames, ArcBlock leaderNames, ArcBlock shipNames, ArcBlock armyNames, ArcBlock fleetNames, ArcString tag, ArcString name, ArcString adj, ArcBlock color, ArcString government, ArcInt governmentRank, ArcInt mercantilism, ArcString technologyGroup, LazyPointer<Religion> religion, Culture primaryCulture, ArcString graphicalCulture, ArcBlock definitions, ArcBlock history, Province capital, GovernmentReform? startingReform)
    {
        HistoricalIdeaGroups = historicalIdeaGroups;
        HistoricalUnits = historicalUnits;
        if (monarchNames.IsEmpty())
        {
            Block names = new();
            foreach (Word w in primaryCulture.MaleNames.Value)
            {
                names.Add($"\"{w.value.Trim('"')}\" = 1");
            }
            foreach (Word w in primaryCulture.CultureGroup.MaleNames.Value)
            {
                names.Add($"\"{w.value.Trim('"')}\" = 1");
            }
            foreach (Word w in primaryCulture.FemaleNames.Value)
            {
                names.Add($"\"{w.value.Trim('"')}\" = -1");
            }
            foreach (Word w in primaryCulture.CultureGroup.FemaleNames.Value)
            {
                names.Add($"\"{w.value.Trim('"')}\" = -1");
            }
            MonarchNames = new(names);
        }
        else
        {
            MonarchNames = monarchNames;
        }
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
        iReligion = religion;
        PrimaryCulture = primaryCulture;
        GraphicalCulture = graphicalCulture;
        Definitions = definitions;
        History = history;
        Capital = capital;
        StartingReform = startingReform;
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
            { "religion", iReligion },
            { "primary_culture", PrimaryCulture },
            { "graphical_culture", GraphicalCulture },
            { "definitions", Definitions },
            { "history", History },
            { "capital", Capital },
            { "starting_reform", StartingReform },
        };

        Countries.Add(key, this);
    }
    public bool CanGet(string indexer) => keyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => keyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string key = i.Current;

        i = Args.GetArgs(i, out Args args);

        Country countr = new(
            key,
            args.GetDefault(ArcBlock.Constructor, "historical_idea_groups", new()),
            args.GetDefault(ArcBlock.Constructor, "historical_units", new()),
            args.GetDefault(ArcBlock.Constructor, "monarch_names", new()),
            args.GetDefault(ArcBlock.Constructor, "leader_names", new()),
            args.GetDefault(ArcBlock.Constructor, "ship_names", new()),
            args.GetDefault(ArcBlock.Constructor, "army_names", new("\"Army of $PROVINCE$\"")),
            args.GetDefault(ArcBlock.Constructor, "fleet_names", new("\"Fleet of $PROVINCE$\"")),
            args.Get(ArcString.Constructor, "tag"),
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "adj"),
            args.Get(ArcBlock.Constructor, "color"),
            args.GetDefault(ArcString.Constructor, "government", new("monarchy")),
            args.GetDefault(ArcInt.Constructor, "government_rank", new(1)),
            args.GetDefault(ArcInt.Constructor, "mercantilism", new(1)),
            args.Get(ArcString.Constructor, "technology_group"),
            args.GetLazyFromList(Religion.Religions, "religion"),
            args.GetFromList(Culture.Cultures, "primary_culture"),
            args.Get(ArcString.Constructor, "graphical_culture"),
            args.GetDefault(ArcBlock.Constructor, "definitions", new()),
            args.GetDefault(ArcBlock.Constructor, "history", new()),
            args.GetFromList(Province.Provinces, "capital"),
            args.GetFromListNullable(GovernmentReform.GovernmentReforms, "starting_reform")
        );

        return i;
    }
    public void Transpile(ref Block countryDefinitions)
    {
        countryDefinitions.Add(Tag, "=", $"\"countries/{Tag}.txt\"");
        Block countryDef = new()
        {
            "graphical_culture", "=", GraphicalCulture,
            "color", "=", "{", Color, "}",
            "historical_idea_groups", "=", "{", HistoricalIdeaGroups, "}",
            "historical_units", "=", "{", HistoricalUnits, "}",
            "monarch_names", "=", "{", MonarchNames, "}",
            "leader_names", "=", "{", LeaderNames, "}",
            "ship_names", "=", "{", ShipNames, "}",
            "army_names", "=", "{", ArmyNames, "}",
            "fleet_names", "=", "{", FleetNames, "}",
            Definitions.Compile()
        };
        Instance.OverwriteFile($"target/common/countries/{Tag}.txt", string.Join(' ', countryDef));

        Block countryHistory = new()
        {
            "government", "=", Government,
            "government_rank", "=", GovernmentRank,
            "mercantilism", "=", Mercantilism,
            "technology_group", "=", TechnologyGroup,
            "religion", "=", iReligion.Get().Id,
            "primary_culture", "=", PrimaryCulture.Id,
            "capital", "=", Capital.Id,
            History.Compile()
        };



        if (GovernmentRank.Value > 6 || GovernmentRank.Value < 1) throw new Exception();

        if (StartingReform != null) countryHistory.Add("add_government_reform", "=", StartingReform.Id);

        Instance.OverwriteFile($"target/history/countries/{Tag}.txt", string.Join(' ', countryHistory));

        Instance.Localisation.Add($"{Tag}", $"{Name}");
        Instance.Localisation.Add($"{Tag}_ADJ", $"{Adj}");
    }
    public static string Transpile()
    {
        Block countryDefinitions = new();
        foreach (Country ctr in Country.Countries.Values())
        {
            ctr.Transpile(ref countryDefinitions);
        }
        Instance.OverwriteFile("target/common/country_tags/countries.txt", string.Join(' ', countryDefinitions));
        return "Countries";
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result, Compiler comp) => Tag.Call(i, ref result, comp);
}
