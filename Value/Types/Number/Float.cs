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
        Word value = b.toWord();
        if (value.EndsWith('%')) value = new Word((double.Parse(value.Value[..^1]) / 100).ToString("0.000"), value);
        Value = Calculator.Calculate(value);
    }
    public void Set(Block b)
    {
        Word value = b.toWord();
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

    public Walker Call(Walker i, ref Block result)
    {
        if (i.MoveNext())
        {
            switch (i.Current)
            {
                case "+=":
                    {
                        i.ForceMoveNext();

                        string k = i.Current;

                        Value += double.Parse(k);
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