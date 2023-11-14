using ArcInstance;
using Pastel;
using System.Text;

namespace Arc;
public class Adjacency : ArcObject
{
    public static readonly Dict<Adjacency> Adjacencies = new();
    public Adjacency(string id)
    {
        Adjacencies.Add(id, this);
    }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static Adjacency Constructor(string id, Args args) => new(id)
    {
        { "id", new ArcString(id) },
        { "from", args.GetFromList(Province.Provinces, "from") },
        { "to", args.GetFromList(Province.Provinces, "to") },
        { "type", args.Get(ArcString.Constructor, "type") },
        { "through", args.GetFromList(Province.Provinces, "through") },
        { "start_x", args.GetDefault(ArcInt.Constructor, "start_x", new(-1)) },
        { "start_y", args.GetDefault(ArcInt.Constructor, "start_y", new(-1)) },
        { "stop_x", args.GetDefault(ArcInt.Constructor, "stop_x", new(-1)) },
        { "stop_y", args.GetDefault(ArcInt.Constructor, "stop_y", new(-1)) },
        { "comment", args.Get(ArcString.Constructor, "comment", new(id)) }
    };
    public void Transpile(ref StringBuilder sb)
    {
        sb.Append($"{Get<Province>("from").Id};{Get<Province>("to").Id};{Get<ArcString>("type")};{Get<Province>("through").Id};{Get<ArcInt>("start_x")};{Get<ArcInt>("start_y")};{Get<ArcInt>("stop_x")};{Get<ArcInt>("stop_y")};{Get<ArcString>("id")}\n");
    }
    public static string Transpile()
    {
        StringBuilder sb = new("From;To;Type;Through;start_x;start_y;stop_x;stop_y;Comment\n");
        foreach (Adjacency adjacency in Adjacencies.Values())
        {
            adjacency.Transpile(ref sb);
        }
        sb.Append("-1;-1;;-1;-1;-1;-1;-1;-1");
        Program.OverwriteFile($"{Program.TranspileTarget}/map/adjacencies.csv", sb.ToString(), false);
        return "Adjacencies";
    }
    public override Walker Call(Walker i, ref Block result) => throw new Exception();
}
