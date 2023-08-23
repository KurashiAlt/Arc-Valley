namespace Arc;
public class ArcFloat : IArcNumber
{
    public double Value { get; set; }
    public ArcFloat(double value)
    {
        Value = value;
    }
    public ArcFloat(string value)
    {
        Value = double.Parse(value);
    }
    public ArcFloat(Block b)
    {
        Value = double.Parse(string.Join(' ', b));
    }
    public void Set(Block value)
    {
        Value = double.Parse(string.Join(' ', value));
    }
    public static ArcFloat Constructor(Block b) => new ArcFloat(b);
    public double GetNum() => Value;
    public bool IsFloat() => true;

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

                        string k = i.Current;

                        Value += double.Parse(k);
                    }
                    break;
                case ":=":
                    {
                        if (!i.MoveNext())
                            throw new Exception();

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