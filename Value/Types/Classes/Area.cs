using ArcInstance;
using Pastel;
using System.Text;

namespace Arc;
public class Area : IArcObject
{
    public static readonly Dict<Area> Areas = new();
    public string Class => "Area";
    public ArcString Name { get; set; }
    public ArcString Id { get; set; }
    public Region Region { get; set; }
    public Dict<IVariable> keyValuePairs { get; set; }
    public Area(ArcString name, Region region, ArcString id)
    {
        Name = name;
        Region = region;
        Id = id;
        keyValuePairs = new()
        {
            { "name", Name },
            { "id", Id },
            { "region", Region }
        };
    }
    public bool CanGet(string indexer) => keyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => keyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);

        Area area = new(
            args.Get(ArcString.Constructor, "name"),
            args.GetFromList(Region.Regions, "region"),
            new($"{id}_area")
        );

        Areas.Add(id, area);

        return i;
    }

    public override string ToString() => Name.Value;
    public static string Transpile()
    {
        StringBuilder sb = new();
        foreach (Area area in Area.Areas.Values())
        {
            sb.Append($"{area.Id} = {{ {string.Join(' ', from Province in Province.Provinces.Values() where Province.Area == area select Province.Id)} }} ");
            Instance.Localisation.Add($"{area.Id.Value}", area.Name.Value);
        }
        Instance.OverwriteFile($"{Instance.TranspileTarget}/map/area.txt", sb.ToString());
        return "Areas";
    }
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}