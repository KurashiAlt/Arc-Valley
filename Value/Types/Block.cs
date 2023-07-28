namespace Arc;
public class ArcBlock : IValue
{
    public Block Value { get; set; }
    public ArcBlock()
    {
        Value = new();
    }
    public ArcBlock(string s)
    {
        Value = Parser.ParseCode(s);
    }
    public ArcBlock(Block value)
    {
        if (Parser.HasEnclosingBrackets(value))
            value = Compiler.RemoveEnclosingBrackets(value);
        Value = value;
    }
    public void Set(Block value)
    {
        if (Parser.HasEnclosingBrackets(value))
            value = Compiler.RemoveEnclosingBrackets(value);
        Value = value;
    }
    public bool IsEmpty() => Value.Count == 0;
    public bool IsBlock() => true;
    public Walker Call(Walker i, ref List<string> result, Compiler comp)
    {
        if (i.MoveNext())
        {
            switch (i.Current)
            {
                case "+=":
                    {
                        if (!i.MoveNext())
                            throw new Exception();

                        i = Compiler.GetScope(i, out Block newbv);

                        if (Parser.HasEnclosingBrackets(newbv)) newbv = Compiler.RemoveEnclosingBrackets(newbv);

                        foreach (string s in newbv)
                        {
                            Value.Add(s);
                        }
                    }
                    break;
                case ":=":
                    {
                        if (!i.MoveNext())
                            throw new Exception();

                        i = Compiler.GetScope(i, out Block newbv);

                        if (Parser.HasEnclosingBrackets(newbv)) newbv = Compiler.RemoveEnclosingBrackets(newbv);

                        Value = newbv;
                    }
                    break;

                default:
                    {
                        i.MoveBack();
                        result.Add(comp.Compile(Value));
                    }
                    break;
            }
        }
        else result.Add(comp.Compile(Value));
        return i;
    }
    public override string ToString()
    {
        return string.Join(' ', Value);
    }
    public string Compile(string wrapper)
    {
        string s = Compile();
        if (s == "") return "";
        return $"{wrapper} = {{ {s} }}";
    }
    public string Compile()
    {
        return new Compiler().Compile(Value);
    }
    internal static ArcBlock Constructor(Block block) => new ArcBlock(block);
}