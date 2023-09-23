using ArcInstance;
using Pastel;
using System.IO;
using System.Text;

namespace Arc;
public class Relation : ArcBlock
{
    public static readonly ArcList<Relation> Relations = new();
    public Relation(Block block)
    {
        Value = block;
    }
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;
        if (!i.MoveNext()) throw new Exception();
        if (i.Current != "=") throw new Exception();
        if (!i.MoveNext()) throw new Exception();
        i = Compiler.GetScope(i, out Block scope);

        scope.Prepend(new("="));
        scope.Prepend(new(id));

        Relation Relation = new(
            scope
        );

        Relations.Values.Add(Relation);

        return i;
    }
    public static string Transpile()
    {
        StringBuilder sb = new("");

        sb.Append(string.Join(' ', from rel in Relations.Values select rel.Value));
        sb.Append(' ');

        Instance.OverwriteFile("target/history/diplomacy/arc.txt", sb.ToString());
        return "Relations";
    }

    public override string ToString() => "[Arc Relation]";
    public Walker Call(Walker i, ref List<string> result) => throw new Exception();
}
