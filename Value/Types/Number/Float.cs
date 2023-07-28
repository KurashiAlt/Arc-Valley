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

    public Walker Call(Walker i, ref List<string> result, Compiler comp)
    {
        throw new NotImplementedException();
    }
}