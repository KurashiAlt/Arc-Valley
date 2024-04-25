namespace Arc;
public class ArcFloat : IArcNumber, IValue
{
    public double Value { get; set; }
    public ArcFloat(double value)
    {
        Value = value;
    }
    public ArcFloat(Word value)
    {
        if (value.EndsWith('%')) value = new Word((double.Parse(value.Value[..^1]) / 100).ToString("0.000"), value);
        Value = Calculator.Calculate(value);
    }
    public ArcFloat(Block b)
    {
        Word value = b.ToWord();
        if (value.EndsWith('%')) value = new Word((double.Parse(value.Value[..^1]) / 100).ToString("0.000"), value);
        Value = Calculator.Calculate(value);
    }
    public void Set(Block b)
    {
        Word value = b.ToWord();
        if (value.EndsWith('%')) value = new Word((double.Parse(value.Value[..^1]) / 100).ToString("0.000"), value);
        Value = Calculator.Calculate(value);
    }
    public static ArcFloat Constructor(Block b) => new ArcFloat(b);
    public double GetNum() => Value;
    public bool IsFloat() => true;

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
            ArcFloat right = new(w);
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

                        string k = i.Current;

                        Value = double.Parse(k);
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