using ArcInstance;
using System.Text;

namespace Arc;
public class Region : IArcObject
{
	public static readonly Dict<Region> Regions = new();
	public bool IsObject() => true;
	public string Class => "Region";
	public ArcString Name { get; set; }
	public ArcString Adj { get; set; }
	public ArcString Id { get; set; }
	public Superregion Superregion { get; set; }

	public Dict<IVariable> KeyValuePairs { get; set; }
	public Region(ArcString name, Superregion region, ArcString id, ArcString adj)
	{
		Name = name;
		Superregion = region;
		Id = id;
		Adj = adj;
		KeyValuePairs = new()
		{
			{ "name", Name },
			{ "id", Id },
			{ "superregion", Superregion },
			{ "adj", Adj },
		};
	}
	public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
	public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
	public static Walker Call(Walker i)
	{
		if (!i.MoveNext()) throw new Exception();

		string id = i.Current;

		i = Args.GetArgs(i, out Args args);

		Region Region = new(
			args.Get(ArcString.Constructor, "name"),
			args.GetFromList(Superregion.Superregions, "superregion"),
			new($"{id}_region"),
			args.GetDefault(ArcString.Constructor, "adj", args.Get(ArcString.Constructor, "name"))
		);

		Regions.Add(id, Region);

		return i;
	}
	public static string Transpile()
    {
        StringBuilder sb = new();
        foreach (Region region in Region.Regions.Values())
        {
            sb.Append($"{region.Id} = {{ areas = {{ {string.Join(' ', from Area in Area.Areas.Values() where Area.Region == region select Area.Id)} }} }} ");
            Instance.Localisation.Add($"{region.Id.Value}", region.Name.Value);
            Instance.Localisation.Add($"{region.Id.Value}_name", region.Name.Value);
            Instance.Localisation.Add($"{region.Id.Value}_adj", region.Adj.Value);
        }
        Instance.OverwriteFile("target/map/region.txt", sb.ToString());
        return "Regions";
    }
    public override string ToString() => Name.Value;
	public Walker Call(Walker i, ref Block result) { result.Add(Id.Value); return i; }
}