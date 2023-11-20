using System.Collections;

namespace Arc;
public class ArcList<T> : IArcObject, IEnumerable, ArcEnumerable where T : IVariable
{
    public List<T?> Values { get; set; }
    public Dict<T>? dict { get; set; }
    public Func<string, Args, T>? constructor { get; set; }
    public Func<Block, T>? tConstructor { get; set; }
    public bool IsObject() => true;
    public ArcList()
    {
        Values = new();
    }
    public ArcList(Func<Block, T> Constructor)
    {
        Values = new();
        tConstructor = Constructor;
    }
    public ArcList(Dict<T> _dict)
    {
        Values = new();
        dict = _dict;
    }
    public ArcList(Block value, Func<Block, int, T> Constructor)
    {
        if (Parser.HasEnclosingBrackets(value)) Compiler.RemoveEnclosingBrackets(value);

        Values = new();

        if (value.Count == 0) return;

        Walker i = new(value);
        do
        {
            i = Compiler.GetScope(i, out Block f);
            Values.Add(Constructor(f, Values.Count));
        } while (i.MoveNext());
    }
    public ArcList(Block value, Func<string, Args, T> Constructor)
    {
        if (Parser.HasEnclosingBrackets(value)) Compiler.RemoveEnclosingBrackets(value);

        constructor = Constructor;
        Values = new();

        if (value.Count == 0) return;

        Walker i = new(value);
        do
        {
            string key = i.Current;
            i = Args.GetArgs(i, out Args args);

            Values.Add(Constructor(key, args));
        } while (i.MoveNext());
    }
    public ArcList(Block value, Func<Block, T> Constructor)
    {
        if (Parser.HasEnclosingBrackets(value)) Compiler.RemoveEnclosingBrackets(value);
        if (value.Count != 0 && !Parser.HasEnclosingBrackets(value))
        {
            value.Prepend("{");
            value.Add("}");
        }

        tConstructor = Constructor;
        Values = new();

        if (value.Count == 0) return;

        Walker i = new(value);
        do
        {
            i = Compiler.GetScope(i, out Block f);
            Values.Add(Constructor(f));
        } while (i.MoveNext());
    }
    public ArcList(Block value, Dict<T> Dictionary)
    {
        if(Parser.HasEnclosingBrackets(value)) Compiler.RemoveEnclosingBrackets(value);

        dict = Dictionary;
        Values = new();

        if (value.Count == 0) return;

        Walker i = new(value);
        do
        {
            if (!Dictionary.CanGet(i.Current)) throw new Exception($"{i.Current} does not exist within dictionary while creating a list");
            Values.Add((T?)Dictionary.Get(i.Current));
        } while (i.MoveNext());
    }
    public Walker Call(Walker i, ref Block result)
    {
        i.ForceMoveNext();
        if (i.Current != "+=") throw new Exception();
        i.ForceMoveNext();
        if(i.Current == "new")
        {
            if (constructor == null) throw new Exception();
            i.ForceMoveNext();

            string id = Compiler.GetId(i.Current);

            i = Args.GetArgs(i, out Args args);

            Values.Add(constructor(id, args));
        }
        else if (i.Current == "{")
        {
            if (tConstructor == null) throw new Exception();
            i.ForceMoveNext();

            i = Compiler.GetScope(i, out Block scope);

            Values.Add(tConstructor(scope));
            i.ForceMoveNext();
            i.Asssert("}");
        }
        else
        {
            if (dict == null) throw new Exception();
            Values.Add(dict[i.Current]);
        }
        return i;
    }
    public override string ToString()
    {
        return string.Join(' ', from value in Values select value.ToString());
    }
    public static Func<Block, ArcList<T>> GetConstructor(Dict<T> dict)
    {
        return (Block s) => new ArcList<T>(s, dict);
    }
    public static Func<Block, ArcList<T>> GetConstructor(Func<Block, T> func)
    {
        return (Block s) => new ArcList<T>(s, func);
    }
    public static Func<Block, ArcList<T>> GetConstructor(Func<string, Args, T> func)
    {
        return (Block s) => new ArcList<T>(s, func);
    }
    public virtual IVariable? Get(string indexer)
    {
        if(int.TryParse(indexer, out int res))
        {
            res -= 1;
            return Values[res];
        }
        throw new Exception();
    }

    public virtual bool CanGet(string indexer)
    {
        if (int.TryParse(indexer, out int res))
        {
            res -= 1;
            if(res < 0) return false;
            if(res >= Values.Count) return false;
            return true;
        }
        return false;
    }
    public void Add(T value)
    {
        Values.Add(value);
    }
    public IEnumerator GetEnumerator() => Values.GetEnumerator();

    public IEnumerator<IVariable> GetArcEnumerator()
    {
        return (from v in Values select v as IVariable).GetEnumerator();
    }
}