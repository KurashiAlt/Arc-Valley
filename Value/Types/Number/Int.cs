namespace Arc;

public class ArcInt : IArcNumber
{
    public int Value { get; set; }
    public ArcInt(int value)
    {
        Value = value;
    }
    public ArcInt(string value)
    {
        Value = int.Parse(value);
    }
    public ArcInt(Block b)
    {
        Value = int.Parse(string.Join(' ', b));
    }
    public void Set(Block value)
    {
        Value = int.Parse(string.Join(' ', value));
    }
    public static ArcInt Constructor(Block b) => new ArcInt(b);
    public double GetNum() => Value;
    public bool IsInt() => true;

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

                        Value += int.Parse(k);
                    }
                    break;
                case ":=":
                    {
                        if (!i.MoveNext())
                            throw new Exception();

                        string k = i.Current;

                        Value = int.Parse(k);
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