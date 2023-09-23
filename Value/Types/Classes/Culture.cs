using ArcInstance;
using Pastel;
using System.IO;
using System.Text;

namespace Arc;
public class Culture : IArcObject
{
    public static readonly Dict<Culture> Cultures = new();
    public bool IsObject() => true;
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcCode MaleNames { get; set; }
    public ArcCode FemaleNames { get; set; }
    public ArcCode DynastyNames { get; set; }
    public ArcModifier CountryModifier { get; set; }
    public ArcModifier ProvinceModifier { get; set; }
    public Country? Primary { get; set; }
    public CultureGroup CultureGroup { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public Culture(string key, ArcString name, ArcCode maleNames, ArcCode femaleNames, ArcCode dynastyNames, ArcModifier country, ArcModifier province, Country? primary, CultureGroup cultureGroup)
    {
        Id = new($"{key}_culture");
        Name = name;
        MaleNames = maleNames;
        FemaleNames = femaleNames;
        DynastyNames = dynastyNames;
        CountryModifier = country;
        ProvinceModifier = province;
        Primary = primary;
        CultureGroup = cultureGroup;

        KeyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "male_names", MaleNames },
            { "female_names", FemaleNames },
            { "dynasty_names", DynastyNames },
            { "country", CountryModifier },
            { "province", ProvinceModifier },
            { "primary", Primary },
            { "culture_group", CultureGroup },
        };

        Cultures.Add(key, this);
    }

    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);

        Culture Culture = new(
            id,
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcCode.Constructor, "male_names", new()),
            args.Get(ArcCode.Constructor, "female_names", new()),
            args.Get(ArcCode.Constructor, "dynasty_names", new()),
            args.Get(ArcModifier.Constructor, "country", new()),
            args.Get(ArcModifier.Constructor, "province", new()),
            args.GetFromListNullable(Country.Countries, "primary"),
            args.GetFromList(CultureGroup.CultureGroups, "culture_group")
        );

        return i;
    }
    public void Transpile(StringBuilder sb)
    {
        Instance.Localisation.Add($"{Id}", Name.Value);

        sb.Append($"{Id} = {{ {CountryModifier.Compile("country")} {ProvinceModifier.Compile("province")} ");
        if (Primary != null) sb.Append($"primary = {Primary.Tag}");
        sb.Append($" {MaleNames.Compile("male_names")} {FemaleNames.Compile("female_names")} {DynastyNames.Compile("dynasty_names")} }}");
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}
