﻿using System.Text.RegularExpressions;

namespace Arc;

public class ArcString : IValue
{
    //Regular Stuff
    public string Value { get; set; }
    public string TrimOneQuote(string value)
    {
        if (value.StartsWith('"') && value.EndsWith('"')) return value[1..^1];
        return value;
    }
    public ArcString(string value)
    {
        Value = TrimOneQuote(value);
    }
    public ArcString(Block b)
    {
        string val = TrimOneQuote(string.Join(' ', b));

        if (Compiler.TryTrimOne(val, '`', out string? newValue))
        {
            if (newValue == null)
                throw new Exception();

            Regex Replace = Compiler.TranspiledString();
            newValue = Replace.Replace(newValue, delegate (Match m)
            {
                return new Compiler().Compile(m.Groups[1].Value).Trim();
            });

            val = newValue;
        }

        Value = val;
    }
    public void Set(Block value)
    {
        Value = string.Join(' ', value);
    }
    public static ArcString Constructor(Block b)
    {
        return new(b);
    }
    //Type Finding
    public bool IsString() => true;

    public override string ToString()
    {
        return Value.ToString();
    }

    public Walker Call(Walker i, ref Block result, Compiler comp)
    {
        if (i.MoveNext())
        {
            switch (i.Current)
            {
                case "+=":
                    {
                        if (!i.MoveNext())
                            throw new Exception();

                        i = Compiler.GetScope(i, out Block newbv);

                        if (Parser.HasEnclosingBrackets(newbv)) newbv = Compiler.RemoveEnclosingBrackets(newbv);

                        Value += string.Join(' ', newbv);
                    }
                    break;
                case ":=":
                    {
                        if (!i.MoveNext())
                            throw new Exception();

                        i = Compiler.GetScope(i, out Block newbv);

                        if (Parser.HasEnclosingBrackets(newbv)) newbv = Compiler.RemoveEnclosingBrackets(newbv);

                        Value = string.Join(' ', newbv);
                    }
                    break;

                default:
                    {
                        i.MoveBack();
                        result.Add(comp.Compile(Value));
                    }
                    break;
            }
        }
        else result.Add(comp.Compile(Value));
        return i;
    }
}