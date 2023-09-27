using ArcInstance;
using Pastel;
using System.Text;

namespace Arc;
public class MercenaryCompany : IArcObject
{
    public static readonly Dict<MercenaryCompany> Companies = new();
    public bool IsObject() => true;
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public Province? HomeProvince { get; set; }
    public Dict<ArcCode> Attributes { get; set; }
    public static string[] Implemented = new string[]
    {
        "name", "home_province"
    };
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public MercenaryCompany(
        string id, 
        ArcString name, 
        Province? homeProvince, 
        Dict<ArcCode> attributes
    ) {
        Id = new(id);
        Name = name;
        HomeProvince = homeProvince;
        Attributes = attributes;
        KeyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "home_province", HomeProvince },
        };

        Companies.Add(id, this);
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);
        MercenaryCompany adj = new(
            id,
            args.Get(ArcString.Constructor, "name"),
            args.GetFromListNullable(Province.Provinces, "home_province"),
            args.GetAttributes(Implemented)
        );

        return i;
    }
    public static string Transpile()
    {
        Block s = new();
        foreach(MercenaryCompany company in Companies.Values())
        {
            s.Add(company.Id, "=", "{");
            if(company.HomeProvince != null) s.Add("home_province", "=", company.HomeProvince.Id);
            foreach(KeyValuePair<string, ArcCode> attribute in company.Attributes)
            {
                attribute.Value.Compile(attribute.Key, ref s, true, true);
            }
            s.Add("}");
            Instance.Localisation.Add(company.Id.Value, company.Name.Value);
        }

        Instance.OverwriteFile($"{Instance.TranspileTarget}/common/mercenary_companies/arc.txt", string.Join(' ', s));
        return "Mercenary Companies";
    }
    public Walker Call(Walker i, ref Block result) => throw new Exception();
}
