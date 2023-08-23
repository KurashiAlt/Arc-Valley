namespace Arc;

public class ArcString : IValue
{
    //Regular Stuff
    public string Value { get; set; }
    public ArcString(string value)
    {
        Value = value;
    }
    public ArcString(Block b)
    {
        Value = string.Join(' ', b);
    }
    public void Set(Block value)
    {
        Value = string.Join(' ', value);
    }
    public static ArcString Constructor(Block b) => new(b);
    //Type Finding
    public bool IsString() => true;

    public override string ToString()
    {
        return Value.ToString();
    }

    public Walker Call(Walker i, ref Block result, Compiler comp)
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

                        Value += string.Join(' ', newbv);
                    }
                    break;
                case ":=":
                    {
                        if (!i.MoveNext())
                            throw new Exception();

                        i = Compiler.GetScope(i, out Block newbv);

                        if (Parser.HasEnclosingBrackets(newbv)) newbv = Compiler.RemoveEnclosingBrackets(newbv);

                        Value = string.Join(' ', newbv);
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
}