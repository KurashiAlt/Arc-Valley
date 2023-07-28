namespace Arc;
public class Province : IArcObject
{
    public static readonly Dict<Province> Provinces = new();
    public bool IsObject() => true;
    public ArcString Name { get; set; }
    public ArcBlock Color { get; set; }
    public ArcBlock History { get; set; }
    public ArcBool Sea { get; set; }
    public ArcBool Lake { get; set; }
    public ArcBool Impassible { get; set; }
    public Terrain Terrain { get; set; }
    public Area? Area { get; set; }
    public ArcInt Id { get; set; }
    public ArcInt BaseDevelopment { get; set; }
    public ArcBlock Position { get; set; }
    public ArcBlock Rotation { get; set; }
    public ArcBlock Height { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public bool IsLand() => !(Sea.Value || Lake.Value || Impassible.Value);
    public Province(ArcString Name, ArcBlock Color, ArcBlock History, ArcBool Sea, ArcBool Lake, ArcBool Impassible, Area? Area, Terrain terrain, ArcInt basedevelopment, ArcBlock position, ArcBlock rotation, ArcBlock height)
    {
        this.Name = Name;
        this.Color = Color;
        this.History = History;
        this.Sea = Sea;
        this.Lake = Lake;
        this.Impassible = Impassible;
        this.Area = Area;
        Id = new(Provinces.Count + 1);
        Terrain = terrain;
        BaseDevelopment = basedevelopment;
        Position = position;
        Rotation = rotation; 
        Height = height;
        KeyValuePairs = new()
        {
            { "name", this.Name },
            { "color", this.Color },
            { "history", this.History },
            { "sea", this.Sea },
            { "lake", this.Lake },
            { "impassible", this.Impassible },
            { "area", this.Area },
            { "id", this.Id },
            { "terrain", Terrain }
        };
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);
        Terrain terrain = Terrain.Terrains[args.GetDefault(ArcString.Constructor, "terrain", new("grasslands")).Value];
        Province prov = new(
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcBlock.Constructor, "color"),
            args.Get(ArcBlock.Constructor, "history"),
            args.GetDefault(ArcBool.Constructor, "sea", new(false)),
            args.GetDefault(ArcBool.Constructor, "lake", new(false)),
            args.GetDefault(ArcBool.Constructor, "impassible", new(false)),
            args.GetFromListNullable(Area.Areas, "area"),
            terrain,
            args.GetDefault(ArcInt.Constructor, "base_development", terrain.BaseDevelopment),
            args.Get(ArcBlock.Constructor, "position"),
            args.Get(ArcBlock.Constructor, "rotation"),
            args.Get(ArcBlock.Constructor, "height")
        );

        Provinces.Add(id, prov);

        return i;
    }

    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref List<string> result, Compiler comp) { result.Add(Id.Value.ToString()); return i; }
}
