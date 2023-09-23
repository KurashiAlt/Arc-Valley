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
    public ArcBlock(params string[] s)
    {
        Value = new()
        {
            s
        };
    }
    public ArcBlock(Block value, bool t = false)
    {
        if (!t)
            if (Parser.HasEnclosingBrackets(value))
                value = Compiler.RemoveEnclosingBrackets(value);
        Value = value;
    }
    public ArcBlock RemoveEnclosingBrackets()
    {
        if(Parser.HasEnclosingBrackets(Value)) Value = Compiler.RemoveEnclosingBrackets(Value);

        return this;
    }
    public void Set(Block value)
    {
        if (Parser.HasEnclosingBrackets(value))
            value = Compiler.RemoveEnclosingBrackets(value);
        Value = value;
    }
    public bool IsEmpty() => Value.Count == 0;
    public bool IsBlock() => true;
    public Walker Call(Walker i, ref Block result)
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
                        result.Add(Compiler.Compile(Value));
                    }
                    break;
            }
        }
        else result.Add(Compiler.Compile(Value));
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
    public void Compile(ref Block b)
    {
        string s = Compile();
        if (s == "") return;
        b.Add(s);
    }
    public void Compile(string wrapper, ref Block b, bool CanBeEmpty = true, bool CanBeSingular = false)
    {
        string s = Compile();
        if (CanBeEmpty && s == "") return;
        b.Add(wrapper);
        b.Add("=");
        if (CanBeSingular && Parser.ParseCode(s).Count == 1)
        {
            b.Add(s);
        }
        else
        {
            b.Add("{");
            b.Add(s);
            b.Add("}");
        }
    }

    public virtual string Compile()
    {
        return Compiler.Compile(Value);
    }
    internal int Count() => Value.Count;
}