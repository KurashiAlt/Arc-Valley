
using Pastel;
using System.IO;
using System.Text;

namespace Arc;
public class AspectsName : IArcObject
{
    public bool IsObject() => true;
    public ArcString Id { get; set; }
    public ArcString Short { get; set; }
    public ArcString Long { get; set; }
    public ArcString PowerName { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public AspectsName(ArcString id, ArcString @short, ArcString @long, ArcString powerName)
    {
        Id = id;
        Short = @short;
        Long = @long;
        PowerName = powerName;
        KeyValuePairs = new()
        {
            { "id", Id },
            { "short", Short },
            { "long", Long },
            { "power_name", PowerName },
        };
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public void Transpile(ref StringBuilder b)
    {
        Program.Localisation.Add($"{Id}_SHORT", Short.Value);
        Program.Localisation.Add($"{Id}_LONG", Long.Value);
        Program.Localisation.Add($"{Id}_POWER_NAME", PowerName.Value);
        b.Append($"aspects_name = {Id} ");
    }
    public static AspectsName Constructor(Block block, string id)
    {
        Walker i = new(block);

        i = Args.GetArgs(i, out Args args, 2);

        return new AspectsName(
            new(id.ToUpper()),
            args.Get(ArcString.Constructor, "short"),
            args.Get(ArcString.Constructor, "long"),
            args.Get(ArcString.Constructor, "power_name", new("Church Power"))
        );
    }
    public Walker Call(Walker i, ref Block result)
    {
        result.Add(Id);
        return i;
    }
}
public class Religion : IArcObject
{
    public static readonly Dict<Religion> Religions = new();
    public bool IsObject() => true;
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }

    public ArcString Id { get; set; }
    public ArcInt Icon { get; set; }
    public ArcBlock Color { get; set; }
    public ArcBlock Heretic { get; set; }
    public ArcBlock CountryModifier { get; set; }
    public ArcBlock ProvinceModifier { get; set; }
    public ArcBlock CountryAsSecondary { get; set; }
    public ArcBlock AllowedConversions { get; set; }
    public ArcBlock OnConvert { get; set; }
    public ArcString Date { get; set; }
    public ArcBool HreReligion { get; set; }
    public ArcBool HreHereticReligion { get; set; }
    public ArcBool AllowFemaleDefenderOfFaith { get; set; }
    public ArcBool PersonalDeity { get; set; }
    public ArcList<Province>? HolySites { get; set; }
    public ArcList<Blessing>? Blessings { get; set; }
    public ArcList<ChurchAspect>? Aspects { get; set; }
    public ArcBool UsesChurchPower { get; set; }
    public ArcBool UsesKarma { get; set; }
    public ArcBool UsesPiety { get; set; }
    public ArcBool UsesAnglicanPower { get; set; }
    public ArcBool Fervor { get; set; }                   
/* Disabled for now since each one has so many special implementations
    public ArcBlock Papacy { get; set; }
    public ArcBool FetishistCult { get; set; }
    public ArcBool HasPatriachs { get; set; }
    public ArcBool Authority { get; set; }
    public ArcBool ReligiousReforms { get; set; }
    public ArcBool Doom { get; set; }
    public ArcBool UsesIsolationism { get; set; }
    public ArcBool Ancestors { get; set; }
    public ArcBlock Gurus { get; set; }
*/
    public ArcBool MisguidedHeretic { get; set; }
    public ArcBool DeclareWarInRegency { get; set; }
    public ArcBool CanHaveSecondaryReligion { get; set; }
    public ReligionGroup ReligionGroup { get; set; }
    public AspectsName? AspectsName { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public Religion(ArcString name, ArcString desc, ArcString id, ArcInt icon, ArcBlock color, ArcBlock heretic, ArcBlock country, ArcBlock province, ArcBlock countryAsSecondary, ArcBlock allowedConversions, ArcBlock onConvert, ArcBool hreReligion, ArcBool hreHereticReligion, ArcString date, ArcBool misguidedHeretic, ArcBool declareWarInRegency, ArcBool canHaveSecondaryReligion, ReligionGroup religionGroup, ArcBool allowFemaleDefenderOfFaith, ArcBool personalDeity, ArcList<Province>? holySites, ArcList<Blessing>? blessings, ArcBool usesChurchPower, ArcList<ChurchAspect>? aspects, ArcBool usesKarma, ArcBool usesPiety, ArcBool usesAnglicanPower
        , AspectsName? aspectsName, ArcBool fervor)
    {
        Name = name;
        Desc = desc;
        Id = id;
        Icon = icon;
        Color = color;
        Heretic = heretic;
        CountryModifier = country;
        ProvinceModifier = province;
        CountryAsSecondary = countryAsSecondary;
        AllowedConversions = allowedConversions;
        OnConvert = onConvert;
        HreReligion = hreReligion;
        HreHereticReligion = hreHereticReligion;
        Date = date;
        MisguidedHeretic = misguidedHeretic;
        DeclareWarInRegency = declareWarInRegency;
        CanHaveSecondaryReligion = canHaveSecondaryReligion;
        ReligionGroup = religionGroup;
        AllowFemaleDefenderOfFaith = allowFemaleDefenderOfFaith;
        PersonalDeity = personalDeity;
        HolySites = holySites;
        Blessings = blessings;
        UsesChurchPower = usesChurchPower;
        Aspects = aspects;
        UsesKarma = usesKarma;
        UsesPiety = usesPiety;
        UsesAnglicanPower = usesAnglicanPower;
        AspectsName = aspectsName;
        Fervor = fervor;
        KeyValuePairs = new()
        {
            { "name", Name },
            { "desc", Desc },
            { "id", Id },
            { "icon", Icon },
            { "color", Color },
            { "heretic", Heretic },
            { "country", CountryModifier },
            { "province", ProvinceModifier },
            { "country_as_secondary", CountryAsSecondary },
            { "allowed_conversions", AllowedConversions },
            { "on_convert", OnConvert },
            { "hre_religion", HreReligion },
            { "hre_heretic_religion", HreHereticReligion },
            { "date", Date },
            { "misguided_heretic", MisguidedHeretic },
            { "declare_war_in_regency", DeclareWarInRegency },
            { "can_have_secondary_religion", CanHaveSecondaryReligion },
            { "religious_group", ReligionGroup },
            { "allow_female_defenders_of_the_faith", AllowFemaleDefenderOfFaith },
            { "personal_deity", PersonalDeity },
            { "holy_sites", HolySites },
            { "blessing", Blessings },
            { "uses_church_power", UsesChurchPower },
            { "aspects", Aspects },
            { "uses_karma", UsesKarma },
            { "uses_piety", UsesPiety },
            { "uses_anglican_power", UsesAnglicanPower },
            { "aspects_name", AspectsName },
            { "fervor", Fervor },
        };
    }

    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        i.ForceMoveNext();

        string id = Compiler.GetId(i.Current);

        i = Args.GetArgs(i, out Args args);
        ArcModifier country = args.Get(ArcModifier.Constructor, "country");

        var a = args.Get((Block s) => new ArcList<Province>(s, Province.Provinces), "holy_sites", null);

        Religion religion = new(
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc"),
            new(id),
            args.Get(ArcInt.Constructor, "icon"),
            args.Get(ArcCode.Constructor, "color"),
            args.Get(ArcCode.Constructor, "heretic"),
            country,
            args.Get(ArcModifier.Constructor, "province"),
            args.Get(ArcModifier.Constructor, "country_as_secondary", country),
            args.Get(ArcCode.Constructor, "allowed_conversions", new()),
            args.Get(ArcEffect.Constructor, "on_convert", new()),
            args.Get(ArcBool.Constructor, "hre_religion", new(false)),
            args.Get(ArcBool.Constructor, "hre_heretic_religion", new(false)),
            args.Get(ArcString.Constructor, "date", new("")),
            args.Get(ArcBool.Constructor, "misguided_heretic", new(false)),
            args.Get(ArcBool.Constructor, "declare_war_in_regency", new(false)),
            args.Get(ArcBool.Constructor, "can_have_secondary_religion", new(false)),
            args.GetFromList(ReligionGroup.ReligionGroups, "religious_group"),
            args.Get(ArcBool.Constructor, "allow_female_defenders_of_the_faith", new(true)),
            args.Get(ArcBool.Constructor, "personal_deity", new(false)),
            a,
            args.Get((Block s) => new ArcList<Blessing>(s, Blessing.Blessings), "blessings", null),
            args.Get(ArcBool.Constructor, "uses_church_power", new(false)),
            args.Get((Block s) => new ArcList<ChurchAspect>(s, ChurchAspect.ChurchAspects), "aspects", null),
            args.Get(ArcBool.Constructor, "uses_karma", new(false)),
            args.Get(ArcBool.Constructor, "uses_piety", new(false)),
            args.Get(ArcBool.Constructor, "uses_anglican_power", new(false)),
            args.Get((Block b) => AspectsName.Constructor(b, id), "aspects_name", null),
            args.Get(ArcBool.Constructor, "fervor", new ArcBool(false))
        );

        Religions.Add(id, religion);

        return i;
    }
    public void Transpile(StringBuilder sb)
    {
        Program.Localisation.Add($"{Id}", Name.Value);
        Program.Localisation.Add($"{Id}_religion_desc", Desc.Value);
        Program.Localisation.Add($"{Id}_demand", "");
        Program.Localisation.Add($"{Id}_demand_desc", "");

        sb.Append($"{Id} = {{ color = {{ {Color} }} icon = {Icon} heretic = {{ {Heretic} }} country = {{ {CountryModifier} }} province = {{ {ProvinceModifier} }} country_as_secondary = {{ {CountryAsSecondary} }} ");
        if (!AllowedConversions.IsEmpty()) sb.Append($"allowed_conversion = {{ {AllowedConversions} }} ");
        if (!OnConvert.IsEmpty()) sb.Append($"on_convert = {{ {OnConvert} }} ");
        if (MisguidedHeretic) sb.Append("misguided_heretic = yes ");
        if (DeclareWarInRegency) sb.Append("declare_war_in_regency = yes ");
        if (CanHaveSecondaryReligion) sb.Append("can_have_secondary_religion = yes ");
        if (AllowFemaleDefenderOfFaith) sb.Append("allow_female_defenders_of_the_faith = yes ");
        if (HreReligion) sb.Append("hre_religion = yes ");
        if (HreHereticReligion) sb.Append("hre_heretic_religion = yes ");
        if (PersonalDeity) sb.Append("personal_deity = yes ");
        if (UsesChurchPower) sb.Append("uses_church_power = yes ");
        if (UsesKarma) sb.Append("uses_karma = yes ");
        if (UsesPiety) sb.Append("uses_piety = yes ");
        if (Fervor) sb.Append("fervor = yes ");
        if (UsesAnglicanPower) sb.Append("uses_anglican_power = yes ");
        if (HolySites != null) sb.Append($"holy_sites = {{ {string.Join(' ', from b in HolySites.Values select b.Id)} }} ");
        if (Blessings != null) sb.Append($"blessings = {{ {string.Join(' ', from b in Blessings.Values select b.Id)} }} ");
        if (Aspects != null) sb.Append($"aspects = {{ {string.Join(' ', from b in Aspects.Values select b.Id)} }} ");
        AspectsName?.Transpile(ref sb);
        sb.Append("} ");
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}
