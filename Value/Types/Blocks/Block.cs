namespace Arc;
public static class CompileList
{
    public static List<ArcBlock> list = new();
    public static int Count => list.Count;
    public static void Add(ArcBlock t)
    {
        list.Add(t);
        if (Compiler.CompileRightAway != 0) t.Compile();
    }
}
public class ArcBlock : IValue, IArcObject
{
    public static bool PastDefineStep = false;
    public string? Compiled;
    public bool ShouldBeCompiled = true;
    public int Id;
    public ArcString? InjectedName;
    public Block Value { get; set; }
    public ArcBlock()
    {
        Value = new();
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
    public Walker Call(Walker i, ref Block result)
    {
        if (i.MoveNext())
        {
            switch (i.Current)
            {
                case "+=!":
                    {
                        i.ForceMoveNext();

                        i = Compiler.GetScope(i, out Block newbv);

                        if (Parser.HasEnclosingBrackets(newbv)) newbv = Compiler.RemoveEnclosingBrackets(newbv);

                        Value.Add(Compile(newbv));

                        Compiled = null;
                    }
                    break;
                case "+=":
                    {
                        i.ForceMoveNext();

                        i = Compiler.GetScope(i, out Block newbv);

                        if (Parser.HasEnclosingBrackets(newbv)) newbv = Compiler.RemoveEnclosingBrackets(newbv);

                        Value.Add(newbv);

                        if (Compiled != null)
                        {
                            string compile = Compile(newbv);
                            Compiled += " ";
                            Compiled += compile;
                        }
                    }
                    break;
                case ":=":
                    {
                        i.ForceMoveNext();

                        i = Compiler.GetScope(i, out Block newbv);

                        if (Parser.HasEnclosingBrackets(newbv)) newbv = Compiler.RemoveEnclosingBrackets(newbv);

                        Value = newbv;

                        Compiled = null;
                    }
                    break;

                default:
                    {
                        i.MoveBack();
                        if (ShouldBeCompiled) result.Add($"__ARC.BLOCK__ = {Id}");
                        else result.Add(Compile());
                    }
                    break;
            }
        }
        else
        {
            if (ShouldBeCompiled) result.Add($"__ARC.BLOCK__ = {Id}");
            else result.Add(Compile());
        }
        return i;
    }
    public virtual string Compile(Block b)
    {
        throw ArcException.Create(b, this);
    }
    public override string ToString()
    {
        return string.Join(' ', Value);
    }
    public string Compile(string wrapper, bool CanBeEmpty = true, bool CanBeSingular = false)
    {
        Block b = new();
        Compile(wrapper, ref b, CanBeEmpty, CanBeSingular);
        return b.ToString();
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
        if (CanBeSingular && Parser.ParseCode(s, "unknown").Count == 1)
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
        throw ArcException.Create(this);
    }
    internal int Count() => Value.Count;

    public IVariable? Get(string indexer) => indexer switch
    {
        "id" => InjectedName ?? new ArcString("unnamed_block"),
        "to_string" => new ArcString(Value),
        _ => throw new Exception()
    };

    public bool CanGet(string indexer) => indexer switch
    {
        "id" => true,
        "to_string" => true,
        _ => false
    };
}