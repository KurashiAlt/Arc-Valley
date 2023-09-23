using System.Collections;
namespace Arc;
public class Dict<Type> : IArcObject, IEnumerable<KeyValuePair<string, Type>> where Type : IVariable?
{
    //(Block s) => new Dict<Mission>(s, Mission.Constructor)
    public static Func<Block, Dict<Type>> Constructor(Func<string, Args, Type> constructor)
    {
        return (Block s) =>
        {
            if (Parser.HasEnclosingBrackets(s)) s = Compiler.RemoveEnclosingBrackets(s);

            Dict<Type> dict = new();

            if (s.Count == 0) return dict;

            Walker i = new(s);
            do
            {
                string key = i.Current;
                i = Args.GetArgs(i, out Args args);
                dict.Add(key, constructor(key, args));
            } while (i.MoveNext());

            return dict;
        };
    }
    private readonly Dictionary<string, DictPointer<Type>> Kvps;
    public Dictionary<string, Type>.ValueCollection Values() => Kvps.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Value).Values;
    public Dict() { Kvps = new(); }
    public Dict(Block s, Func<string, Args, Type> constructor) 
    {
        Kvps = new();
        if (Parser.HasEnclosingBrackets(s)) Compiler.RemoveEnclosingBrackets(s);

        if (s.Count == 0) return;

        Walker i = new(s);
        do
        {
            string key = i.Current;
            i = Args.GetArgs(i, out Args args);
            Add(key, constructor(key, args));
        } while (i.MoveNext());
    }
    public bool IsObject() => true;
    public IVariable? Get(string indexer) => Kvps[indexer].Value;
    public bool CanGet(string indexer) => Kvps.ContainsKey(indexer);
    public Type this[string indexer] => Kvps[indexer].Value;
    public int Count => Kvps.Count;
    public void Add(string indexer, Type Value)
    {
        Kvps.Add(indexer, new DictPointer<Type>(Value));
    }
    public void Add((string indexer, Type Value) v)
    {
        Kvps.Add(v.indexer, new DictPointer<Type>(v.Value));
    }

    public static Walker Call(Walker i)
    {
        throw new Exception();
    }

    public override string ToString() => "Arc Dict";

    public Walker Call(Walker i, ref Block result)
    {
        Console.Write(i.Current);
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<string, Type>> GetEnumerator() => new DictEnumerator(Kvps.GetEnumerator());

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public class DictEnumerator : IEnumerator<KeyValuePair<string, Type>>
    {
        private Dictionary<string, DictPointer<Type>>.Enumerator Enum;
        public DictEnumerator(Dictionary<string, DictPointer<Type>>.Enumerator Enum) => this.Enum = Enum;
        public KeyValuePair<string, Type> Current => new(Enum.Current.Key, Enum.Current.Value.Value);
        object IEnumerator.Current => throw new NotImplementedException();
        public void Dispose()
        {
            Enum.Dispose();
            GC.SuppressFinalize(this);
        }
        public bool MoveNext() => Enum.MoveNext();
        public void Reset() => throw new Exception();
    }
}
public class DictPointer<Type>
{
    public Type Value;
    public DictPointer(Type value) { Value = value; }
}