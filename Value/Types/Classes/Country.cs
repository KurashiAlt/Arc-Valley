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
    public ArcCode HistoricalIdeaGroups { get; set; }
    public ArcCode HistoricalUnits { get; set; }
    public ArcCode MonarchNames { get; set; }
    public ArcCode LeaderNames { get; set; }
    public ArcCode ShipNames { get; set; }
    public ArcCode ArmyNames { get; set; }
    public ArcCode FleetNames { get; set; }
    public ArcString Tag { get; set; }
    public ArcString Name { get; set; }
    public ArcString Adj { get; set; }
    public ArcCode Color { get; set; }
    public ArcString Government { get; set; }
    public ArcInt GovernmentRank { get; set; }
    public ArcInt Mercantilism { get; set; }
    public ArcString TechnologyGroup { get; set; }
    public LazyPointer<Religion> iReligion { get; set; }
    public Culture PrimaryCulture { get; set; }
    public ArcString GraphicalCulture { get; set; }
    public ArcCode Definitions { get; set; }
    public ArcEffect History { get; set; }
    public Province Capital { get; set; }
    public LazyPointer<GovernmentReform>? StartingReform { get; set; }
    public Dict<IVariable?> keyValuePairs { get; set; }
    public Country(string key, ArcCode historicalIdeaGroups, ArcCode historicalUnits, ArcCode monarchNames, ArcCode leaderNames, ArcCode shipNames, ArcCode armyNames, ArcCode fleetNames, ArcString tag, ArcString name, ArcString adj, ArcCode color, ArcString government, ArcInt governmentRank, ArcInt mercantilism, ArcString technologyGroup, LazyPointer<Religion> religion, Culture primaryCulture, ArcString graphicalCulture, ArcCode definitions, ArcEffect history, Province capital, LazyPointer<GovernmentReform>? startingReform)
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
        Block fNames = new();
        foreach (Word w in primaryCulture.MaleNames.Value)
        {
            fNames.Add($"{w.value.Trim('"')}");
        }
        foreach (Word w in primaryCulture.CultureGroup.MaleNames.Value)
        {
            fNames.Add($"{w.value.Trim('"')}");
        }
        foreach (Word w in primaryCulture.FemaleNames.Value)
        {
            fNames.Add($"{w.value.Trim('"')}");
        }
        foreach (Word w in primaryCulture.CultureGroup.FemaleNames.Value)
        {
            fNames.Add($"{w.value.Trim('"')}");
        }
        if (leaderNames.IsEmpty()) LeaderNames = new(fNames);
        else LeaderNames = leaderNames;
        if (shipNames.IsEmpty()) ShipNames = new(fNames);
        else ShipNames = shipNames;
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
            args.GetDefault(ArcCode.Constructor, "historical_idea_groups", new()),
            args.GetDefault(ArcCode.Constructor, "historical_units", new()),
            args.GetDefault(ArcCode.Constructor, "monarch_names", new()),
            args.GetDefault(ArcCode.Constructor, "leader_names", new()),
            args.GetDefault(ArcCode.Constructor, "ship_names", new()),
            args.GetDefault(ArcCode.Constructor, "army_names", new("\"Army of $PROVINCE$\"")),
            args.GetDefault(ArcCode.Constructor, "fleet_names", new("\"Fleet of $PROVINCE$\"")),
            args.Get(ArcString.Constructor, "tag"),
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "adj"),
            args.Get(ArcCode.Constructor, "color"),
            args.GetDefault(ArcString.Constructor, "government", new("monarchy")),
            args.GetDefault(ArcInt.Constructor, "government_rank", new(1)),
            args.GetDefault(ArcInt.Constructor, "mercantilism", new(1)),
            args.Get(ArcString.Constructor, "technology_group"),
            args.GetLazyFromList(Religion.Religions, "religion"),
            args.GetFromList(Culture.Cultures, "primary_culture"),
            args.Get(ArcString.Constructor, "graphical_culture"),
            args.GetDefault(ArcCode.Constructor, "definitions", new()),
            args.GetDefault(ArcEffect.Constructor, "history", new()),
            args.GetFromList(Province.Provinces, "capital"),
            args.GetLazyFromListNullable(GovernmentReform.GovernmentReforms, "starting_reform")
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
        Instance.OverwriteFile($"{Instance.TranspileTarget}/common/countries/{Tag}.txt", string.Join(' ', countryDef));

        Block countryHistory = new()
        {
            "government", "=", Government,
            "government_rank", "=", GovernmentRank,
            "mercantilism", "=", Mercantilism,
            "technology_group", "=", TechnologyGroup,
            "religion", "=", iReligion.Get().Id,
            "primary_culture", "=", PrimaryCulture.Id,
            "capital", "=", Capital.Id,
        };



        if (GovernmentRank.Value > 6 || GovernmentRank.Value < 1) throw new Exception();

        if (StartingReform != null) countryHistory.Add("add_government_reform", "=", StartingReform.Get().Id);

        countryHistory.Add(History.Compile());

        Instance.OverwriteFile($"{Instance.TranspileTarget}/history/countries/{Tag}.txt", string.Join(' ', countryHistory));

        Instance.Localisation.Add($"{Tag}", $"{Name}");
        Instance.Localisation.Add($"{Tag}_ADJ", $"{Adj}");
    }
    public static string Transpile()
    {
        Block countryDefinitions = new()
        {
            "REB", "=", "\"countries/REB.txt\"",
            "NAT", "=", "\"countries/NAT.txt\"",
            "PIR", "=", "\"countries/PIR.txt\""
        };
        foreach (KeyValuePair<string, Country> ctr in Countries)
        {
            ctr.Value.Transpile(ref countryDefinitions);

            //Instance.Warn($"(\"add_core = {ctr.Value.Tag}\", \"add_core = {ctr.Key}\"),");
        }

        Instance.OverwriteFile($"{Instance.TranspileTarget}/common/countries/REB.txt", "graphical_culture = westerngfx color = { 30 30 30 } historical_idea_groups = { administrative_ideas quantity_ideas defensive_ideas humanist_ideas trade_ideas quality_ideas economic_ideas maritime_ideas } monarch_names = { \"Corneles #0\" = 10 \"Moise #0\" = 10 \"Mahieu #0\" = 10 \"Daniel #0\" = 10 \"Jacob #0\" = 10 \"Piet #0\" = 10 \"Hendrik #0\" = 10 \"David #0\" = 10 \"John #0\" = 10 \"Eric #0\" = 10 \"Boel #0\" = -1 \"Alexandra #0\" = -1 \"Regina #0\" = -1 \"Miriam #0\" = -1 } leader_names = { Lowther Quelch Condent Dalzeel Spriggs Condon Angre Anstis Chivers Searle Hout Coxon Vynne Vane Worley }");
        Instance.OverwriteFile($"{Instance.TranspileTarget}/common/countries/NAT.txt", "graphical_culture = westerngfx color = { 203 164 103 } historical_idea_groups = { expansion_ideas trade_ideas plutocracy_ideas economic_ideas quality_ideas maritime_ideas quantity_ideas administrative_ideas } monarch_names = { \"Mapi #0\" = 10 \"Quando #0\" = 10 \"Illa #0\" = 10 \"Rimac #0\" = 10 \"Tiso #0\" = 10 \"Cusi #0\" = 10 \"Mayta #0\" = 10 \"Roca #0\" = 10 \"Zope #0\" = 10 \"Curiatao #0\" = 10 \"Guacra #0\" = 10 \"Maila #0\" = 10 \"Tanqui #0\" = 10 \"Taipi #0\" = 10 \"Sanga #0\" = -1 } leader_names = { Tywan Toto Iawi Gasana Wyoh Illa Maila Quizo Tanqui Taraque Tari Pacon } ship_names = { Yamro Sanga Mani Mutara Gasana Yuhi Kigeri Kemba Adero Ayan Mahdi Gukunda Nalungo Kilolo Gahiji }");
        Instance.OverwriteFile($"{Instance.TranspileTarget}/common/countries/PIR.txt", "graphical_culture = westerngfx color = { 10 10 10 } monarch_names = { \"Woodes #0\" = 10 \"Black #0\" = 10 \"Howell #0\" = 10 \"Calico #0\" = 10 \"Lessone #0\" = 10 \"Lawrence #0\" = 10 \"Lewis #0\" = 10 \"John #0\" = 10 \"Roche #0\" = 10 \"James #0\" = 10 \"Assan #0\" = 10 \"Dirck #0\" = 10 \"Jane #0\" = -1 } leader_names = { Alvel Cavendish \"de Bouff\" Brower Barton Hein Noort Easton Rais Verney Rous Reis Davis Collier Greaves Bellamy Every Vane Rogers Taylor Vorley Kelly Howard Halsey } ship_names = { \"Captain's Horror\" \"Death\" \"Disgraceful Strumpet\" \"Dragon's Gold\" \"Executioner\" \"Killer's Fearful Storm\" \"Murderer's Death\" \"Privateer's Strumpet\" \"The Damned Killer\" \"The Dark Dagger\" \"The Dirty Blade\" \"The Dirty Scream\" \"The Doom of the Ocean\" \"The Dreaming Demon\" \"The Fallen Raider\" \"The Fear of the Demon\" \"The Foul Whore\" \"The Gold Cutlass\" \"The Hell-born\" \"The Horrid Compass\" \"The Horrible Raider\" \"The Howling Wolf\" \"The Lustful Hangman\" \"The Nightmare\" \"The Poison Death\" \"The Serpent\" \"The Vicious Murderer\" \"The Vile Saber\" }");

        Instance.OverwriteFile($"{Instance.TranspileTarget}/history/countries/REB.txt", "technology_group = nord_tg");
        Instance.OverwriteFile($"{Instance.TranspileTarget}/history/countries/NAT.txt", "technology_group = nord_tg");
        Instance.OverwriteFile($"{Instance.TranspileTarget}/history/countries/PIR.txt", "technology_group = nord_tg");

        Instance.OverwriteFile($"{Instance.TranspileTarget}/common/country_tags/countries.txt", string.Join(' ', countryDefinitions));
        return "Countries";
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) => Tag.Call(i, ref result);
}
