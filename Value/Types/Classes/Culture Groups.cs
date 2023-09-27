using ArcInstance;
using Pastel;
using System.IO;
using System.Text;

namespace Arc;
public class CultureGroup : IArcObject
{
    public static readonly Dict<CultureGroup> CultureGroups = new();
    public bool IsObject() => true;
    public ArcString Id { get; set; }
    public ArcString GraphicalCulture { get; set; }
    public ArcString Name { get; set; }
    public ArcCode MaleNames { get; set; }
    public ArcCode FemaleNames { get; set; }
    public ArcCode DynastyNames { get; set; }
    public ArcModifier CountryModifier { get; set; }
    public ArcModifier ProvinceModifier { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public CultureGroup(string key, ArcString graphicalCulture, ArcString name, ArcCode maleNames, ArcCode femaleNames, ArcCode dynastyNames, ArcModifier country, ArcModifier province)
    {
        Id = new($"{key}_culture_group");
        GraphicalCulture = graphicalCulture;
        Name = name;
        MaleNames = maleNames;
        FemaleNames = femaleNames;
        DynastyNames = dynastyNames;
        CountryModifier = country;
        ProvinceModifier = province;

        KeyValuePairs = new()
        {
            { "id", Id },
            { "graphical_culture", GraphicalCulture },
            { "name", Name },
            { "male_names", MaleNames },
            { "female_names", FemaleNames },
            { "dynasty_names", DynastyNames },
            { "country", CountryModifier },
            { "province", ProvinceModifier },
        };

        CultureGroups.Add(key, this);
    }

    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);

        CultureGroup cg = new(
            id,
            args.Get(ArcString.Constructor, "graphical_culture"),
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcCode.Constructor, "male_names", new()),
            args.Get(ArcCode.Constructor, "female_names", new()),
            args.Get(ArcCode.Constructor, "dynasty_names", new()),
            args.Get(ArcModifier.Constructor, "country", new()),
            args.Get(ArcModifier.Constructor, "province", new())
        );

        return i;
    }
    public static string Transpile()
    {
        StringBuilder sb = new();
        foreach (CultureGroup cg in CultureGroups.Values())
        {
            Instance.Localisation.Add(cg.Id.Value, cg.Name.Value);

            sb.Append($"{cg.Id} = {{ ");
            foreach (Culture Culture in from rel in Culture.Cultures where rel.Value.CultureGroup == cg select rel.Value)
            {
                Culture.Transpile(sb);
            }
            sb.Append($"{cg.MaleNames.Compile("male_names")} {cg.FemaleNames.Compile("female_names")} {cg.DynastyNames.Compile("dynasty_names")} {cg.CountryModifier.Compile("country")} {cg.ProvinceModifier.Compile("province")} }} ");
        }

        Instance.OverwriteFile($"{Instance.TranspileTarget}/common/cultures/arc.txt", sb.ToString());
        return "Cultures";
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}
