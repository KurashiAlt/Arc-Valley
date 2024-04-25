namespace Arc;

public class ArcInt : IArcNumber, IValue
{
    public int Value { get; set; }
    public ArcInt(int value)
    {
        Value = value;
    }
    public ArcInt(Word value)
    {
        Value = (int)Calculator.Calculate(value);
    }
    public ArcInt(Block b)
    {
        Value = (int)Calculator.Calculate(b.ToWord());
    }
    public void Set(Block value)
    {
        Value = (int)Calculator.Calculate(value.ToWord());
    }
    public static ArcInt Constructor(Block b) => new ArcInt(b);
    public double GetNum() => Value;
    public bool IsInt() => true;

    public override string ToString()
    {
        return Value.ToString();
    }
    public bool LogicalCall(ref Walker i)
    {
        i.ForceMoveNext();
        string oper = i;

        i.ForceMoveNext();
        Word w = i.Current;

        if (Compiler.TryGetVariable(w, out var v) && v != null)
        {
            if (v is IArcNumber vi) return oper switch
            {
                "==" => Value == vi.GetNum(),
                "!=" => Value != vi.GetNum(),
                "<=" => Value <= vi.GetNum(),
                "<" => Value < vi.GetNum(),
                ">=" => Value >= vi.GetNum(),
                ">" => Value > vi.GetNum(),
                _ => throw new Exception(oper)
            };
            return false;
        }
        else
        {
            ArcInt right = new(w);
            return oper switch
            {
                "==" => Value == right.GetNum(),
                "!=" => Value != right.GetNum(),
                "<=" => Value <= right.GetNum(),
                "<" => Value < right.GetNum(),
                ">=" => Value >= right.GetNum(),
                ">" => Value > right.GetNum(),
                _ => throw new Exception(oper)
            };
        }
    }
    public Walker Call(Walker i, ref Block result)
    {
        if (i.MoveNext())
        {
            switch (i.Current)
            {
                case "+=":
                    {
                        i.ForceMoveNext();

                        Block k = i.GetScope();

                        Value += Constructor(k).Value;
                    }
                    break;
                case "-=":
                    {
                        i.ForceMoveNext();

                        Block k = i.GetScope();

                        Value -= Constructor(k).Value;
                    }
                    break;
                case ":=":
                    {
                        i.ForceMoveNext();

                        i = Compiler.GetScope(i, out Block k);

                        Value = Constructor(k).Value;
                    }
                    break;

                default:
                    {
                        i.MoveBack();
                        result.Add(Value.ToString());
                    }
                    break;
            }
        }
        else result.Add(Value.ToString());
        return i;
    }
}