

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
        i.ForceMoveNext();

        string id = Compiler.GetId(i.Current);
        i.ForceMoveNext();
        i.Asssert("=");
        i.ForceMoveNext();
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
        Block b = new()
        {
            string.Join(' ', from rel in Relations.Values select rel.Compile())
        };

        if(Compiler.TryGetVariable(new Word("hre_defines:emperor"), out IVariable? emperorVar))
        {
            if (emperorVar == null) throw new Exception("No Emperor Defined");
            if (emperorVar is not ArcString) throw new Exception("Emperor of wrong type");
            string emperorKey = ((ArcString)emperorVar).Value;
            if (Country.Countries.CanGet(emperorKey))
            {
                Country emperor = Country.Countries[emperorKey];
                b.Add("2500.1.1", "=", "{", "emperor", "=", emperor.Tag, "}");
            }
        }

        Program.OverwriteFile($"{Program.TranspileTarget}/history/diplomacy/arc.txt", string.Join(' ', b));
        return "Relations";
    }
    public override string Compile()
    {
        return Compiler.Compile(CompileType.Block, Value);
    }
    public override string ToString() => "[Arc Relation]";
}
