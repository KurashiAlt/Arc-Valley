﻿namespace Arc;
public class ArcList<T> : IArcObject where T : IVariable
{
    public List<T?> Values { get; set; }
    public bool IsObject() => true;
    public ArcList()
    {
        Values = new();
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
    public ArcList(Block value, Func<string, Args, T> Constructor, Dict<T> Dictionary, Func<string, string>? KeyMorpher = null)
    {
        if (Parser.HasEnclosingBrackets(value)) Compiler.RemoveEnclosingBrackets(value);

        Values = new();

        if (value.Count == 0) return;

        Walker i = new(value);
        do
        {
            if(i.Current == "new")
            {
                if (!i.MoveNext()) throw new Exception();
                string key = i.Current;
                i = Args.GetArgs(i, out Args args);

                if (KeyMorpher != null) key = KeyMorpher(key);

                Values.Add(Constructor(key, args));
            }
            else
                Values.Add((T?)Dictionary.Get(i.Current));
        } while (i.MoveNext());
    }
    public ArcList(Block value, Func<Block, T> Constructor)
    {
        if (Parser.HasEnclosingBrackets(value)) Compiler.RemoveEnclosingBrackets(value);

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
        throw new Exception();
    }
    public override string ToString()
    {
        return string.Join(' ', from value in Values select value.ToString());
    }
    public static Func<Block, ArcList<T>> GetConstructor(Dict<T> dict)
    {
        return (Block s) => new ArcList<T>(s, dict);
    }
    public virtual IVariable? Get(string indexer)
    {
        if(int.TryParse(indexer, out int res))
        {
            return Values[res];
        }
        throw new Exception();
    }

    public virtual bool CanGet(string indexer)
    {
        if (int.TryParse(indexer, out int res))
        {
            if(res < 0) return false;
            if(res >= Values.Count) return false;
            return true;
        }
        return false;
    }
}