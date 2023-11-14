using ArcInstance;
using System.Collections.Generic;
using System.Text;

namespace Arc;
public class Superregion : IArcObject
{
    public static readonly Dict<Superregion> Superregions = new();
    public bool IsObject() => true;
    public string Class => "Superregion";
    public ArcString Name { get; set; }
    public ArcString Adj { get; set; }
    public ArcString Id { get; set; }
    public Dict<IVariable> keyValuePairs { get; set; }

    public Superregion(ArcString Name, ArcString id, ArcString adj)
    {
        this.Name = Name;
        Id = id;
        Adj = adj;
        keyValuePairs = new()
        {
            { "name", this.Name },
            { "id", Id },
            { "adj", Adj }
        };
    }
    public bool CanGet(string indexer) => keyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => keyValuePairs.Get(indexer);
    public Dictionary<string, IValue> Values()
    {
        return new Dictionary<string, IValue>()
        {
            { "name", Name }
        };
    }
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);

        Superregion Superregion = new(
            args.Get(ArcString.Constructor, "name"),
            new($"{id}_superregion"),
            args.GetDefault(ArcString.Constructor, "adj", args.Get(ArcString.Constructor, "name"))
        );

        Superregions.Add(id, Superregion);

        return i;
    }
    public static string Transpile()
    {
        StringBuilder sb = new();
        foreach (Superregion superregion in Superregion.Superregions.Values())
        {
            sb.Append($"{superregion.Id} = {{ {string.Join(' ', from Region in Region.Regions.Values() where Region.Superregion == superregion select Region.Id)} }} ");
            Program.Localisation.Add($"{superregion.Id.Value}", superregion.Name.Value);
            Program.Localisation.Add($"{superregion.Id.Value}_name", superregion.Name.Value);
            Program.Localisation.Add($"{superregion.Id.Value}_adj", superregion.Adj.Value);
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/map/superregion.txt", sb.ToString());
        return "Superregion";
    }
    public override string ToString() => Name.Value;

    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}