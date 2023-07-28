namespace Arc;
public class ArcList<T> : IVariable where T : IVariable
{
    public List<T?> Values { get; set; }
    public ArcList()
    {
        Values = new();
    }
    public ArcList(Block value, Dict<T> Dictionary)
    {
        if(Parser.HasEnclosingBrackets(value)) Compiler.RemoveEnclosingBrackets(value);

        Values = new();

        if (value.Count == 0) return;

        Walker i = new(value);
        do
        {
            if (!Dictionary.CanGet(i.Current)) throw new Exception();
            Values.Add((T?)Dictionary.Get(i.Current));
        } while (i.MoveNext());
    }
    public Walker Call(Walker i, ref List<string> result, Compiler comp)
    {
        throw new Exception();
    }
    public override string ToString()
    {
        return string.Join(' ', from value in Values select value.ToString());
    }
}