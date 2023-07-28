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

    public Walker Call(Walker i, ref List<string> result, Compiler comp)
    {
        throw new NotImplementedException();
    }
}