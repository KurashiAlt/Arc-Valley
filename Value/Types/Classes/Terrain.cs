using ArcInstance;
using System.Text;

namespace Arc;
public class Terrain : IArcObject
{
    public static readonly Dict<Terrain> Terrains = new();
    public bool IsObject() => true;
    public string Class => "Terrain";
    public ArcString Name { get; set; }
    public ArcString Description { get; set; }
    public ArcBlock Color { get; set; }
    public ArcString SoundType { get; set; }
    public ArcBool IsWater { get; set; }
    public ArcBool InlandSea { get; set; }
    public ArcString? Type { get; set; }
    public ArcFloat MovementCost { get; set; }
    public ArcInt Defence { get; set; }
    public ArcBlock Modifier { get; set; }
    public ArcString Id { get; set; }
    public ArcInt BaseDevelopment { get; set; }
    public Dict<IVariable> KeyValuePairs { get; set; }
    public Terrain(ArcString name, ArcString description, ArcBlock color, ArcString soundType, ArcBool isWater, ArcBool inlandSea, ArcString? type, ArcFloat movementCost, ArcInt defence, ArcBlock modifier, ArcString id, ArcInt baseDevelopment)
    {
        Name = name;
        Description = description;
        Color = color;
        SoundType = soundType;
        IsWater = isWater;
        InlandSea = inlandSea;
        Type = type;
        MovementCost = movementCost;
        Defence = defence;
        Modifier = modifier;
        Id = id;
        BaseDevelopment = baseDevelopment;
        KeyValuePairs = new()
        {
            { "name", Name },
            { "desc", Description },
            { "color", Color },
            { "sound_type", SoundType },
            { "is_water", IsWater },
            { "movement_cost", MovementCost },
            { "defence", Defence },
            { "modifier", Modifier },
            { "id", Id },
        };
        if (Type != null)
            KeyValuePairs.Add("type", Type);
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);

        Terrain Terrain = new(
            args.Get(ArcString.Constructor, "name"),
            args.GetDefault(ArcString.Constructor, "desc", new("")),
            args.Get(ArcCode.Constructor, "color"),
            args.Get(ArcString.Constructor, "sound_type"),
            args.GetDefault(ArcBool.Constructor, "is_water", new(false)),
            args.GetDefault(ArcBool.Constructor, "inland_sea", new(false)),
            args.GetDefault(ArcString.Constructor, "type", null),
            args.GetDefault(ArcFloat.Constructor, "movement_cost", new(1)),
            args.GetDefault(ArcInt.Constructor, "defence", new(0)),
            args.GetDefault(ArcModifier.Constructor, "modifier", new()),
            new(id),
            args.GetDefault(ArcInt.Constructor, "base_development", new(0))
        );

        Terrains.Add(id, Terrain);

        return i;
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value); return i; }
    public static string Transpile()
    {
        StringBuilder sb = new("categories = { pti = { type = pti } ");
        foreach (KeyValuePair<string, Terrain> terrain in Terrain.Terrains)
        {
            sb.Append($"{terrain.Key} = {{ color = {{ {terrain.Value.Color} }} sound_type = {terrain.Value.SoundType} {(terrain.Value.IsWater ? "is_water = yes" : "")}  {(terrain.Value.InlandSea ? "inland_sea = yes" : "")} {(terrain.Value.Type != null ? $"type = {terrain.Value.Type}" : "")} movement_cost = {terrain.Value.MovementCost} {(terrain.Value.Defence.Value == 0 ? "" : $"defence = {terrain.Value.Defence}")} {terrain.Value.Modifier.Compile()} terrain_override = {{ {(string.Join(' ', from Province in Province.Provinces.Values() where Province.Terrain == terrain.Value select Province.Id))} }} }} ");
            Instance.Localisation.Add(terrain.Value.Id.Value, terrain.Value.Name.Value);
            Instance.Localisation.Add($"{terrain.Value.Id}_desc", terrain.Value.Description.Value);
        }
        sb.Append(" } ");
        sb.Append($"terrain = {{ {Compiler.global["terrain_declarations"]} }}");
        sb.Append($"tree = {{ {Compiler.global["tree"]} }}");
        Instance.OverwriteFile($"{Instance.TranspileTarget}/map/terrain.txt", sb.ToString());
        return "Terrains";
    }
}