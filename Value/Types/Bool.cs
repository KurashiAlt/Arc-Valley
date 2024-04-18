namespace Arc;
public class ArcBool : IValue
{
    public bool Value { get; set; }

    public ArcBool(bool value)
    {
        Value = value;
    }
    public ArcBool(string value)
    {
        value = value.Replace("yes", "true");
        value = value.Replace("no", "false");
        Value = bool.Parse(value);
    }
    public static ArcBool Constructor(Block b) => new(b);
    public ArcBool(Block value)
    {
        if (value.Count > 1)
            throw ArcException.Create("Too many elements given to ArcBool", value);
        if (value.Count < 0)
            throw ArcException.Create("Too few elements given to ArcBool", value);
        if (value.First == null)
            throw ArcException.Create(value);
        Value = new ArcBool(value.First.Value).Value;
    }
    public void Set(Block value)
    {
        if (value.Count > 1)
            throw ArcException.Create("Too many elements given to ArcBool", value);
        if (value.Count < 0)
            throw ArcException.Create("Too few elements given to ArcBool", value);
        if (value.First == null)
            throw ArcException.Create(value);
        Value = new ArcBool(value.First.Value).Value;
    }
    public bool IsBool() => true;
    public override string ToString()
    {
        return Value?"yes":"no";
    }
    public static implicit operator bool(ArcBool b) => b.Value;
    public bool LogicalCall(ref Walker i)
    {
        if (!i.MoveNext()) return Value;
        string oper = i;

        i.ForceMoveNext();
        Word w = i.Current;

        if (Compiler.TryGetVariable(w, out var v) && v != null)
        {
            if (v is ArcBool vi) return oper switch
            {
                "==" => Value == vi.Value,
                "!=" => Value != vi.Value,
                _ => throw new Exception(oper)
            };
            return false;
        }
        else
        {
            ArcBool right = new(w);
            return oper switch
            {
                "==" => Value == right.Value,
                "!=" => Value != right.Value,
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
                case ":=":
                    {
                        i.ForceMoveNext();

                        string k = i.Current;
                        k = k.Replace("yes", "true");
                        k = k.Replace("no", "false");
                        Value = bool.Parse(k);
                    }
                    break;

                default:
                    {
                        i.MoveBack();
                        result.Add(ToString());
                    }
                    break;
            }
        }
        else result.Add(ToString());
        return i;
    }
}