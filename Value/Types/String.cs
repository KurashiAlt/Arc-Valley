using System.Text.RegularExpressions;

namespace Arc;

public class ArcString : IValue
{
    //Regular Stuff
    public string Value { get; set; }
    public string TrimOneQuote(string value)
    {
        if (value.StartsWith('"') && value.EndsWith('"')) return value[1..^1];
        return value;
    }
    public ArcString(string value)
    {
        Value = TrimOneQuote(value);
    }
    public ArcString(Block b)
    {
        if (Compiler.TranspiledString(string.Join(' ', b), '`', out string? newValue, Compiler.Compile) && newValue != null)
        {
            Value = newValue;
        }
        else if (Compiler.TranspiledString(string.Join(' ', b), '"', out string? nw2, Compiler.Compile) && nw2 != null)
        {
            Value = nw2;
        }
        else 
        {
            Value = TrimOneQuote(string.Join(' ', b));
        }
    }
    public void Set(Block value)
    {
        Value = string.Join(' ', value);
    }
    public static ArcString Constructor(Block b)
    {
        return new(b);
    }
    //Type Finding
    public bool IsString() => true;

    public override string ToString()
    {
        return Value.ToString();
    }

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
                        result.Add(string.Join(' ', Value));
                    }
                    break;
            }
        }
        else result.Add(string.Join(' ', Value));
        return i;
    }
}