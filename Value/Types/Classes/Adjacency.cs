using ArcInstance;
using Pastel;
using System.Text;

namespace Arc;
public class Adjacency : IArcObject
{
    public static readonly Dict<Adjacency> Adjacencies = new();
    public bool IsObject() => true;
    public Province From { get; set; }
    public Province To { get; set; }
    public ArcString Type { get; set; }
    public Province Through { get; set; }
    public ArcInt StartX { get; set; }
    public ArcInt StartY { get; set; }
    public ArcInt StopX { get; set; }
    public ArcInt StopY { get; set; }
    public Dict<IVariable> KeyValuePairs { get; set; }
    public Adjacency(Province from, Province to, ArcString type, Province through, ArcInt startX, ArcInt startY, ArcInt stopX, ArcInt stopY)
    {
        From = from;
        To = to;
        Type = type;
        if (!new string[] { "sea", "land", "lake", "canal", "river" }.Contains(type.Value)) throw new Exception();
        Through = through;
        StartX = startX;
        StartY = startY;
        StopX = stopX;
        StopY = stopY;
        KeyValuePairs = new()
        {
            { "from", From },
            { "to", To },
            { "type", Type },
            { "through", Through },
            { "start_x", StartX },
            { "start_y", StartY },
            { "stop_x", StopX },
            { "stop_y", StopY },
        };
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);
        Adjacency adj = new(
            Province.Provinces[args.Get(ArcString.Constructor, "from").Value],
            Province.Provinces[args.Get(ArcString.Constructor, "to").Value],
            args.Get(ArcString.Constructor, "type"),
            Province.Provinces[args.Get(ArcString.Constructor, "through").Value],
            args.GetDefault(ArcInt.Constructor, "start_x", new(-1)),
            args.GetDefault(ArcInt.Constructor, "start_y", new(-1)),
            args.GetDefault(ArcInt.Constructor, "stop_x", new(-1)),
            args.GetDefault(ArcInt.Constructor, "stop_y", new(-1))
        );

        Adjacencies.Add(id, adj);

        return i;
    }
    public static string Transpile()
    {
        StringBuilder sb = new("From;To;Type;Through;start_x;start_y;stop_x;stop_y;Comment;\n");
        foreach (Adjacency adjacency in Adjacency.Adjacencies.Values())
        {
            sb.Append($"{adjacency.From.Id};{adjacency.To.Id};{adjacency.Type};{adjacency.Through.Id};{adjacency.StartX};{adjacency.StartY};{adjacency.StopX};{adjacency.StopY};{adjacency.From.Name.Value.Trim('"')} to {adjacency.To.Name.Value.Trim('"')} through {adjacency.Through.Name.Value.Trim('"')};\n");
        }
        sb.Append("-1;-1;;-1;-1;-1;-1;-1;-1;");
        Instance.OverwriteFile("target/map/adjacencies.csv", sb.ToString(), false);
        return "Adjacencies";
    }
    public Walker Call(Walker i, ref Block result, Compiler comp) => throw new Exception();
}
