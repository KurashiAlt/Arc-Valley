using System.Security.Cryptography;

namespace Arc;

public class Args
{
    public static readonly Dictionary<string, Args> Inheritables = new Dictionary<string, Args>();
    public Dictionary<string, Block>? keyValuePairs;
    public Block? block;
    public Block Get(string key, Block defaultValue)
    {
        if (keyValuePairs == null) throw new Exception($"Non object type arguments; Trying to get: {key}");
        if (!keyValuePairs.ContainsKey(key)) return defaultValue;
        return keyValuePairs[key];
    }
    public Block Get(string key)
    {
        if (keyValuePairs == null) throw new Exception($"Non object type arguments; Trying to get: {key}");
        if (!keyValuePairs.ContainsKey(key)) throw new Exception($"Arguments do not include {key}");
        return keyValuePairs[key];
    }
    public T Get<T>(Func<Block,T> Constructor, string key) where T : IVariable
    {
        if (keyValuePairs == null) throw new Exception($"Non object type arguments; Trying to get: {key}");
        if (!keyValuePairs.ContainsKey(key)) throw new Exception($"Arguments do not include {key}");
        return Constructor(keyValuePairs[key]);
    }
    public T Get<T>(Func<Block,T> Constructor, string key, T defaultValue) where T : IVariable? => GetDefault(Constructor, key, defaultValue);
    public T GetDefault<T>(Func<Block, T> Constructor, string key, T defaultValue) where T : IVariable?
    {
        if (keyValuePairs == null) throw new Exception();
        if (keyValuePairs.ContainsKey(key))
            return Constructor(keyValuePairs[key]);
        return defaultValue;
    }
    public T GetFromList<T>(Dict<T> List, string key) where T : IVariable
    {
        T? Item = GetFromListNullable(List, key);
        if (Item == null) throw new Exception();
        return Item;
    }
    public T? GetFromListNullable<T>(Dict<T> List, string key) where T : IVariable
    {
        if (keyValuePairs == null) throw new Exception("Arguments for call were not of type [ArcObject]");
        if (!keyValuePairs.ContainsKey(key)) return default;

        string s = string.Join(' ', keyValuePairs[key]);
        if (!List.CanGet(s)) throw new Exception($"Could not get {s} from {List}");
        T? Item = (T?)List.Get(s);
        return Item;
    }

    public Block this[string indexer]
    {
        get
        {
            if (keyValuePairs == null)
                throw new Exception();
            return keyValuePairs[indexer];
        }
        set
        {
            if (keyValuePairs == null)
                throw new Exception();
            keyValuePairs[indexer] = value;
        }
    }
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = GetArgs(i, out Args args);

        Inheritables.Add(id, args);

        return i;
    }
    public Dict<ArcBlock> GetAttributes(string[] ImplementedAttributes) => GetAttributes(this, ImplementedAttributes);
    public static Dict<ArcBlock> GetAttributes(Args args, string[] ImplementedAttributes)
    {
        Dict<ArcBlock> dict = new();
        IEnumerable<(string Key, ArcBlock)> c = from a in args.keyValuePairs where !ImplementedAttributes.Contains(a.Key) select (a.Key, new ArcBlock(a.Value, true));
        foreach (var kv in c)
        {
            dict.Add(kv);
        }
        return dict;
    }
    public static Walker GetArgs(Walker i, out Args args, int StartOffset = 0, bool noInherit = false, bool multiKey = false)
    {
        /*EXAMPLE
        western_sea_of_thule = {
	        name = "Western Sea of Thule"
	        color = { 43 32 47 }
	        history = { }
            sea = yes
        } 
         */
        args = new();

        if(StartOffset < 1)
        {
            if (!i.MoveNext()) throw new Exception(); //western_sea_of_thule
        }

        if (StartOffset < 2)
        {
            if (i.Current != "=") throw new Exception();
            if (!i.MoveNext()) throw new Exception(); //=
        }

        i = Compiler.GetScope(i, out Block scope); /*
                                                    {
	                                                    name = "Western Sea of Thule"
	                                                    color = { 43 32 47 }
	                                                    history = { }
                                                        sea = yes
                                                    }
                                                    */

        if (Parser.HasEnclosingBrackets(scope))
            scope = Compiler.RemoveEnclosingBrackets(scope);

        if (scope.Count == 1)
        {
            args.block = scope;
            return i;
        }

        args.keyValuePairs = new();

        int Inherits = 0;

        Walker q = new(scope);
        do //name = "Western Sea of Thule"
        {
            string Key = q.Current; //name
            if (!q.MoveNext()) throw new Exception(); //name

            if (q.Current != "=") throw new Exception(Key);
            if (!q.MoveNext()) throw new Exception(); //=

            if(Key == "inherit")
            {
                if (noInherit)
                {
                    Key = $"{Key}~{Inherits}";

                    q = Compiler.GetScope(q, out Block Value);
                    if (args.keyValuePairs.ContainsKey(Key))
                        args.keyValuePairs[Key] = Value;
                    else
                        args.keyValuePairs.Add(Key, Value);

                    Inherits++;
                    continue;
                }
                Args inherit = Inheritables[q.Current];
                if (inherit.keyValuePairs == null)
                    throw new Exception();
                foreach(KeyValuePair<string, Block> kvp in inherit.keyValuePairs)
                {
                    args.keyValuePairs.Add(kvp.Key, kvp.Value);
                }
            }
            else if (multiKey)
            {
                q = Compiler.GetScope(q, out Block Value);
                args.keyValuePairs.Add($"{Key}_{(from a in args.keyValuePairs where a.Key.StartsWith(Key) select a).Count()}", Value);
            }
            else
            {
                q = Compiler.GetScope(q, out Block Value);
                if(args.keyValuePairs.ContainsKey(Key))
                    args.keyValuePairs[Key] = Value;
                else
                    args.keyValuePairs.Add(Key, Value);
            }
        } while (q.MoveNext());

        return i;
    }
}