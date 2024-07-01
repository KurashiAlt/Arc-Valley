using System.ComponentModel.DataAnnotations;
using System.Drawing.Printing;
using System.Text.RegularExpressions;

namespace Arc;
public class ArcPredicate : IVariable
{
    public Func<Walker, bool> Predicate;
    public ArcPredicate(Func<Walker, bool> predicate)
    {
        Predicate = predicate;
    }
    public ArcPredicate(Func<IVariable, bool> predicate, Func<Block, IVariable> constructor)
    {
        Predicate = (Walker i) => {
            i.ForceMoveNext();
            i.Asssert("=");

            i.ForceMoveNext();
            Block w = i.GetScope();

            if (Compiler.TryGetVariable(w.ToWord(), out var v) && v != null)
            {
                if (v is IVariable vi) return predicate(vi);
                return false;
            }
            else
            {
                IVariable right = constructor(w);
                return predicate(right);
            }
        };
    }
    public bool LogicalCall(ref Walker i)
    {
        return Predicate(i);
    }
}
public class ArcString : IValue, IArcObject
{
    public string Value { get; set; }

    public static string TrimOneQuote(string value)
    {
        if (value.StartsWith('"') && value.EndsWith('"')) return value[1..^1];
        return value;
    }
    public ArcString(string value)
    {
        if (Compiler.TranspiledString(value, '`', out string? newValue, CompileType.Block, null, "unknown") && newValue != null)
        {
            Value = newValue;
        }
        else if (Compiler.TranspiledString(value, '"', out string? nw2, CompileType.Block, null, "unknown") && nw2 != null)
        {
            Value = nw2;
        }
        else
        {
            Value = Compiler.TranspiledString(value, CompileType.Block, null, "unknown");
        }
    }
    public ArcString(Block b)
    {
        string value = string.Join(' ', b);
        if (Compiler.TranspiledString(value, '`', out string? newValue, CompileType.Block, null, "unknown") && newValue != null)
        {
            Value = newValue;
        }
        else if (Compiler.TranspiledString(value, '"', out string? nw2, CompileType.Block, null, "unknown") && nw2 != null)
        {
            Value = nw2;
        }
        else 
        {
            Value = TrimOneQuote(value);
        }
    }
    public void Set(Block value)
    {
        Value = string.Join(' ', value);
    }
    public static ArcString Constructor(Block b)
    {
        return new(b);
    }
    public override string ToString()
    {
        return Value.ToString();
    }
    public bool LogicalCall(ref Walker i)
    {
        i.ForceMoveNext();
        string oper = i;

        i.ForceMoveNext();
        Word w = i.Current;

        if (Compiler.TryGetVariable(w, out var v) && v != null)
        {
            if (v is IVariable vi) return oper switch
            {
                "==" => Value == vi.ToString(),
                "!=" => Value != vi.ToString(),
                _ => throw new Exception(oper)
            };
            return false;
        }
        else
        {
            ArcString right = new(w);
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
                case "+=":
                    {
                        i.ForceMoveNext();

                        i = Compiler.GetScope(i, out Block newbv);

                        if (Parser.HasEnclosingBrackets(newbv)) newbv = Compiler.RemoveEnclosingBrackets(newbv);

                        Value += new ArcString(newbv).Value;
                    }
                    break;
                case ":=":
                    {
                        i.ForceMoveNext();

                        i = Compiler.GetScope(i, out Block newbv);

                        if (Parser.HasEnclosingBrackets(newbv)) newbv = Compiler.RemoveEnclosingBrackets(newbv);

                        Value = string.Join(' ', newbv);
                    }
                    break;

                default:
                    {
                        i.MoveBack();
                        result.Add(string.Join(' ', Value));
                    }
                    break;
            }
        }
        else result.Add(string.Join(' ', Value));
        return i;
    }
    public IVariable? Get(string indexer)
    {
        return indexer switch
        {
            "contains" => new ArcPredicate((IVariable right) => {
                return Value.Contains(right.ToString());
            }, ArcString.Constructor),
            "to_lower" => new ArcString(Value.ToLower()),
            _ => throw new Exception()
        };;;
    }

    public bool CanGet(string indexer)
    {
        return indexer switch
        {
            "contains" => true,
            "to_lower" => true,
            _ => false
        };
    }
}