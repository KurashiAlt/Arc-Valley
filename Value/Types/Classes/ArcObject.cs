using Arc;

public class ArcObject : Dict<IVariable>
{
    protected static Walker Call<T>(Walker i, Func<string, Args, T> func)
    {
        i.ForceMoveNext();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);

        func(id, args);

        return i;
    }
    public override Walker Call(Walker i, ref Block result) { result.Add(Get<ArcString>("id").Value); return i; }
}
