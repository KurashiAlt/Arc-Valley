using Arc;

public class ArcObject : Dict<IVariable?>, Arg
{
    protected static Walker Call<T>(Walker i, Func<string, Args, T> func)
    {
        i.ForceMoveNext();

        string id = Compiler.GetId(i.Current);

        i = Args.GetArgs(i, out Args args);

        func(id, args);

        return i;
    }
    public virtual void Initialize(Args args) => throw ArcException.Create(this, args);
    public override Walker Call(Walker i, ref Block result) { result.Add(Get<ArcString>("id").Value); return i; }
    public static Arg FromArgs<T>(Args args, T b) where T : ArcObject
    {
        ArcObject a = new ArcObject();

        foreach (KeyValuePair<string, Type> kvp in b.Get<Dict<Type>>("args"))
        {
            if (kvp.Value.Nullable && !args.keyValuePairs.ContainsKey(kvp.Key)) continue;

            ArcString d = args.Get(ArcString.Constructor, kvp.Key);
            if (Compiler.TryGetVariable(d.Value, out IVariable? var))
            {
                a.Add(kvp.Key, var);
            }
            else
            {
                IVariable c = args.Get(kvp.Value.ThisConstructor, kvp.Key);
                a.Add(kvp.Key, c);
            }
        }

        return a;
    }
}
