using ArcInstance;
using System.Text;

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
        if ((from s in Provinces where string.Join(' ',s.Value.Color) == string.Join(' ', Color) select s.Key).Any())
        {
            throw new Exception($"Existing Color {string.Join(' ', Color)}, on creating province: {Name}");
        }

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
    public Walker Call(Walker i, ref Block result, Compiler comp) { result.Add(Id.Value.ToString()); return i; }
    public static string Transpile()
    {
        Compiler comp = new();
        StringBuilder Positions = new();
        StringBuilder ProvinceDefines = new();
        StringBuilder Impassibles = new();
        StringBuilder SeaTiles = new();
        StringBuilder LakeTiles = new();
        StringBuilder Continent = new("Tamriel = {");
        foreach (KeyValuePair<string, Province> province in Province.Provinces)
        {
            int id = province.Value.Id.Value;
            string name = province.Value.Name.Value;
            Block color = province.Value.Color.Value;
            Block history = province.Value.History.Value;
            Instance.Localisation.Add($"PROV{id}", name);
            Instance.Localisation.Add($"PROV_ADJ{id}", name);
            ProvinceDefines.Append($"{id};{string.Join(';', color)};;x\n");

            if (province.Value.Impassible.Value)
            {
                Impassibles.Append($"{id} ");
            }
            if (province.Value.Sea.Value)
            {
                SeaTiles.Append($"{id} ");
            }
            if (province.Value.Lake.Value)
            {
                LakeTiles.Append($"{id} ");
            }

            Block res = new();
            if (province.Value.IsLand()) res.Add(SplitToDev(province.Value.BaseDevelopment.Value));
            if (history.Count > 0)
            {
                res.Add(comp.Compile(history));
            }
            res.Add(
                "1.1.1", "=", "{",
                    string.Join(' ', from ctr in Country.Countries select $"discover_country = {ctr.Value.Tag.Value}"),
                "}"
            );
            Positions.Append($"{province.Value.Id} = {{ position = {{ {province.Value.Position} }} rotation = {{ {province.Value.Rotation} }} height = {{ {province.Value.Height} }} }} ");

            if (province.Value.IsLand() || province.Value.Impassible) Continent.Append($" {province.Value.Id}");

            Instance.OverwriteFile($"target/history/provinces/{id} - ARC.txt", res.ToString());
        }
        Continent.Append(" }");

        string SplitToDev(int i)
        {
            int quotient = i / 3;
            int remainder = i % 3;

            int first = quotient + (remainder >= 1 ? 1 : 0);
            int second = quotient + (remainder >= 2 ? 1 : 0);
            int third = quotient;

            return $"base_tax = {first} base_production = {second} base_manpower = {third} ";
        }

        Instance.OverwriteFile("target/map/continent.txt", Continent.ToString());
        Instance.OverwriteFile("target/map/positions.txt", Positions.ToString());
        Instance.OverwriteFile("target/map/definition.csv", ProvinceDefines.ToString(), false);
        Instance.OverwriteFile("target/map/climate.txt", $@"
tropical = {{
    
}}

arid = {{
    
}}

arctic = {{
	
}}

mild_winter = {{
    
}}


normal_winter = {{
    
}}

severe_winter = {{
    
}}

impassable = {{
	{Impassibles}
}}

mild_monsoon = {{
    
}}

normal_monsoon = {{
    
}}

severe_monsoon = {{
    
}}

equator_y_on_province_image = 224");
        Instance.OverwriteFile("target/map/default.map", $@"
width = 4096
height = 2816

max_provinces = {Provinces.Count + 1}
sea_starts = {{ 
	{SeaTiles}
}}

only_used_for_random = {{
}}

lakes = {{
	{LakeTiles}
}}

force_coastal = {{
}}

definitions = ""definition.csv""
provinces = ""provinces.bmp""
positions = ""positions.txt""
terrain = ""terrain.bmp""
rivers = ""rivers.bmp""
terrain_definition = ""terrain.txt""
heightmap = ""heightmap.bmp""
tree_definition = ""trees.bmp""
continent = ""continent.txt""
adjacencies = ""adjacencies.csv""
climate = ""climate.txt""
region = ""region.txt""
superregion = ""superregion.txt""
area = ""area.txt""
provincegroup = ""provincegroup.txt""
ambient_object = ""ambient_object.txt""
seasons = ""seasons.txt""
trade_winds = ""trade_winds.txt""

# Define which indices in trees.bmp palette which should count as trees for automatic terrain assignment
tree = {{ 3 4 7 10 }}
");
        return "Provinces";
    }
}
