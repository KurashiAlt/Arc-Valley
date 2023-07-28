﻿namespace Arc;

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

    public Walker Call(Walker i, ref List<string> result, Compiler comp)
    {
        throw new NotImplementedException();
    }
}