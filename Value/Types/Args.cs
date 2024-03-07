using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Arc;

public class Args
{
    public static readonly Dictionary<string, Args> Inheritables = new Dictionary<string, Args>();
    public Dictionary<string, Block>? keyValuePairs;
    public Block block;
    public Block Get()
    {
        if (block == null) throw ArcException.Create(this);
        return block;
    }
    public Block Get(string key)
    {
        if (keyValuePairs == null) throw ArcException.Create($"Non object type arguments; Trying to get: {key}", key, this);
        if (!keyValuePairs.ContainsKey(key)) throw ArcException.Create($"Arguments do not include {key}", key, this);
        return keyValuePairs[key];
    }
    public Block Get(string key, Block defaultValue)
    {
        if (keyValuePairs == null) throw ArcException.Create($"Non object type arguments; Trying to get: {key}", key, this);
        if (!keyValuePairs.ContainsKey(key)) return defaultValue;
        return keyValuePairs[key];
    }
    public T Get<T>(Func<Block,T> Constructor, string key) where T : IVariable
    {
        if (keyValuePairs == null) throw ArcException.Create($"Non object type arguments; Trying to get: {key}", key, this);
        if (!keyValuePairs.ContainsKey(key)) throw ArcException.Create($"Arguments do not include {key}", key, this);
        if (Compiler.TryGetVariable(string.Join(' ', keyValuePairs[key]), out var value))
        {
            if(value is T @val) return @val;
            else if (value is ArgList aList)
            {
                if (ArgList.list.First.Value is vx v && v.va.Value is T @val2) return @val2;
            }
        }
        return Constructor(keyValuePairs[key]);
    }
    public T Get<T>(Func<Block,T> Constructor, string key, T defaultValue) where T : IVariable? => GetDefault(Constructor, key, defaultValue);
    public T GetDefault<T>(Func<Block, T> Constructor, string key, T defaultValue) where T : IVariable?
    {
        if (keyValuePairs == null) throw ArcException.Create(this);
        if (keyValuePairs.ContainsKey(key)) return Constructor(keyValuePairs[key]);
        return defaultValue;
    }
    public LazyPointer<T> GetLazyFromList<T>(Dict<T> List, string key) where T : IVariable
    {
        LazyPointer<T>? Item = GetLazyFromListNullable(List, key);
        if (Item == null) throw ArcException.Create(this);
        return Item;
    }
    public LazyPointer<T>? GetLazyFromListNullable<T>(Dict<T> List, string key) where T : IVariable
    {
        if (keyValuePairs == null) throw ArcException.Create("Arguments for call were not of type [ArcObject]", key, this, List);
        if (!keyValuePairs.ContainsKey(key)) return null;

        string s = string.Join(' ', keyValuePairs[key]);

        return new LazyPointer<T>(List, s);
    }
    public T GetFromList<T>(Dict<T> List, string key) where T : IVariable
    {
        T? Item = GetFromListNullable(List, key);
        if (Item == null) throw ArcException.Create(List, key, this);
        return Item;
    }
    public T? GetFromListNullable<T>(Dict<T> List, string key) where T : IVariable
    {
        if (keyValuePairs == null) throw ArcException.Create("Arguments for call were not of type [ArcObject]", List, key, this);
        if (!keyValuePairs.ContainsKey(key)) return default;

        if (Compiler.TryGetVariable(string.Join(' ', keyValuePairs[key]), out var value))
        {
            if (value is T @val) return @val;
            else if (value is ArgList aList)
            {
                if (ArgList.list.First.Value is vx v && v.va.Value is T @val2) return @val2;
            }
        }

        string s = string.Join(' ', keyValuePairs[key]);
        if (!List.CanGet(s)) throw ArcException.Create($"Could not get {s} from {List}", key, List, this);
        T? Item = (T?)List.Get(s);
        return Item;
    }
    public static Walker Call(Walker i)
    {
        i.ForceMoveNext();

        string id = Compiler.GetId(i.Current);

        i = GetArgs(i, out Args args);

        Inheritables.Add(id, args);

        return i;
    }
    public Dict<ArcCode> GetAttributes(string[] ImplementedAttributes) => GetAttributes(this, ImplementedAttributes);
    public static Dict<ArcCode> GetAttributes(Args args, string[] ImplementedAttributes)
    {
        Dict<ArcCode> dict = new();
        IEnumerable<(string Key, ArcCode)> c = from a in args.keyValuePairs where !ImplementedAttributes.Contains(a.Key) select (a.Key, new ArcCode(a.Value, true));
        foreach (var kv in c)
        {
            dict.Add(kv);
        }
        return dict;
    }
    public static Args GetArgs(Block b, Args? inherit = null)
    {
        Walker i = new(b);
        i = GetArgs(i, out Args args, 2, globalInherit: inherit);
        return args;
    }
    public static Args GetArgsFromFile(string filename)
    {
        Block va = Parser.ParseFile(Path.Combine(Program.directory, filename));
        va.Prepend("{");
        va.Add("}");
        return GetArgs(va);
    }
    public static Walker GetArgs(Walker i, out Args args, int StartOffset = 0, Args? globalInherit = null, bool hasInherit = true)
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
            i.ForceMoveNext(); //western_sea_of_thule
        }

        if (StartOffset < 2)
        {
            i.Asssert("=");
            i.ForceMoveNext(); //=
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

        args.block = scope;
        if (scope.Count == 1)
        {
            return i;
        }

        args.keyValuePairs = new();

        int Inherits = 0;

        try
        {
            Walker q = new(scope);
            do //name = "Western Sea of Thule"
            {
                string Key = q.Current; //name
                q.ForceMoveNext();

                switch (q.Current)
                {
                    case "=":
                        {
                            q.ForceMoveNext();

                            if (Key == "inherit" && hasInherit)
                            {
                                Args inherit = Inheritables[q.Current];
                                if (inherit.keyValuePairs == null) throw ArcException.Create(q, Key, hasInherit, inherit);
                                foreach (KeyValuePair<string, Block> kvp in inherit.keyValuePairs)
                                {
                                    args.keyValuePairs.Add(kvp.Key, kvp.Value);
                                }
                            }
                            else
                            {
                                q = Compiler.GetScope(q, out Block Value);
                                if (args.keyValuePairs.ContainsKey(Key))
                                    args.keyValuePairs[Key] = Value;
                                else
                                    args.keyValuePairs.Add(Key, Value);
                            }
                        }
                        break;
                    case "=*":
                        {
                            q.ForceMoveNext();

                            if (Key == "inherit" && hasInherit)
                            {
                                Args inherit = Inheritables[q.Current];
                                if (inherit.keyValuePairs == null) throw ArcException.Create(Key, q, hasInherit, inherit);
                                foreach (KeyValuePair<string, Block> kvp in inherit.keyValuePairs)
                                {
                                    args.keyValuePairs.Add(kvp.Key, kvp.Value);
                                }
                            }
                            else
                            {
                                q = Compiler.GetScope(q, out Block Value);
                                Regex regex = new Regex("\\*([^*]+)\\*");
                                Block nBlock = new();
                                foreach(Word w in Value)
                                {
                                    nBlock.Add(regex.Replace(w.Value, (Match m) => {
                                        return Compiler.GetVariable<IVariable>(m.Groups[1].Value).ToString();
                                    }));
                                }
                                if (args.keyValuePairs.ContainsKey(Key))
                                    args.keyValuePairs[Key] = nBlock;
                                else
                                    args.keyValuePairs.Add(Key, nBlock);
                            }
                        }
                        break;
                    case "+=":
                        {
                            q.ForceMoveNext();

                            q = Compiler.GetScope(q, out Block Value);

                            if (args.keyValuePairs.ContainsKey(Key))
                            {
                                Block original = args.keyValuePairs[Key];
                                if (Parser.HasEnclosingBrackets(Value)) Value = Compiler.RemoveEnclosingBrackets(Value);
                                if (Parser.HasEnclosingBrackets(original)) original = Compiler.RemoveEnclosingBrackets(original);

                                args.keyValuePairs[Key] = new Block("{") + original + Value + new Block("}");
                            } 
                            else
                                args.keyValuePairs.Add(Key, Value);
                        }
                        break;
                    default: throw ArcException.Create(q);
                }
            } while (q.MoveNext());

            if (globalInherit != null)
            {
                if (globalInherit.keyValuePairs == null) throw ArcException.Create(q);
                foreach (KeyValuePair<string, Block> kvp in globalInherit.keyValuePairs)
                {
                    if (args.keyValuePairs.ContainsKey(kvp.Key)) continue;
                    args.keyValuePairs.Add(kvp.Key, kvp.Value);
                }
            }
        }
        catch (Exception)
        {
            args.block = scope;
        }

        return i;
    }
}