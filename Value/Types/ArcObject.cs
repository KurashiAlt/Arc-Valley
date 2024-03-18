using Arc;

public class ArcObject : Dict<IVariable?>
{
    public Dictionary<string, NewCommand>? functions;
    public override IVariable? Get(string indexer)
    {
        if (indexer == "first") return Kvps.Values.First().Value;
        if (indexer == "last") return Kvps.Values.Last().Value;
        if (functions != null && functions.TryGetValue(indexer, out NewCommand? value)) return new CommandCall(value, this);
        return Kvps[indexer].Value;
    }
    public override T Get<T>(string indexer)
    {
        IVariable? v = Get(indexer);
        if (v is T t) return t;
        else throw ArcException.Create($"{indexer} is of wrong type", v);
    }
    public override bool CanGet(string indexer)
    {
        if (indexer == "first") return true;
        if (indexer == "last") return true;
        if (functions != null && functions.ContainsKey(indexer)) return true;
        return Kvps.ContainsKey(indexer);
    }
    protected static Walker Call<T>(Walker i, Func<string, Args, T> func)
    {
        i.ForceMoveNext();

        string id = Compiler.GetId(i.Current);

        i = Args.GetArgs(i, out Args args);

        func(id, args);

        return i;
    }
    public virtual void Initialize(Args args) => throw ArcException.Create(this, args);
    public override Walker Call(Walker i, ref Block result)
    {
        try
        {
            result.Add(Get<ArcString>("id").Value); 
        }
        catch (Exception)
        {
            Console.WriteLine(ArcException.CreateMessage(i, result, this));
            throw;
        }
        return i; 
    }
    public static IVariable FromArgs<T>(Args args, T b) where T : ArcObject
    {
        ArcObject a = new ArcObject();

        foreach (KeyValuePair<string, ArcType> kvp in b.Get<Dict<ArcType>>("args"))
        {
            if (kvp.Value.Nullable && !args.keyValuePairs.ContainsKey(kvp.Key)) continue;

            ArcString d = args.Get(ArcString.Constructor, kvp.Key);
            if (Compiler.TryGetVariable(new Word(d.Value), out IVariable? var))
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
