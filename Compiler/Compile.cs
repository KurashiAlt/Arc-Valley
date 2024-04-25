using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Arc;
public static partial class Compiler
{
    public static string Compile(
        CompileType type,
        Block code,
        ArcObject? bound = null
    )
    {
        if (code.Count == 0) return "";

        Block result = new();

        Walker g = new(code);
        do
        {
            //All
            if (g == "new") __new(ref g);
            else if (g == "LOG_CURRENT_COMPILE") __LOG_CURRENT_COMPILE(result);
            else if (g == "DEFINE_MODIFIER") __DEFINE_MODIFIER(ref g);
            else if (g == "write_file") __write_file(ref g, type);
            else if (g == "delete") __delete(ref g);
            else if (g == "when") __when(ref g, ref result, type, bound);
            else if (g == "modifier_to_string") __modifier_to_string(ref g, ref result);
            else if (g == "id_to_name") __id_to_name(ref g, ref result);
            else if (g == "breakpoint") Debugger.Break();
            else if (g == "foreach") __foreach(ref g, ref result, type, bound);
            else if (g == "for") __for(ref g, ref result, type, bound);
            else if (g == "if") __if(ref g, ref result);
            else if (g == "else_if") __else_if(ref g, ref result);
            else if (g == "else") __else(ref g, ref result);
            else if (g == "arc_throw") __arc_throw(ref g, type, bound);
            else if (g == "arc_log") __arc_log(ref g, type, bound);
            else if (g.StartsWith('&')) __variable(ref g, ref result);
            else if (g.EndsWith(',') || g.EndsWith(';')) __multi_scope(ref g, ref result, type, bound);
            else if (TranspiledString(g.Current, '`', out string? newValue, type, bound, g.Current.GetFile()) && newValue != null) result.Add(newValue);
            else if (g.EndsWith('%')) result.Add((double.Parse(g.Current.Value[..^1]) / 100).ToString("0.000"));
            else if (g.EnclosedBy('[', ']')) __quick_limit(ref g, ref result, type, bound);
            else if (g.EnclosedBy('(', ')')) __quick_math(ref g, ref result);
            else if (bound != null && bound.TryGetVariable(g, out IVariable? boundVar) && boundVar != null) g = boundVar.Call(g, ref result);
            else if (TryGetVariable(g.Current, out IVariable? var) && var != null) g = var.Call(g, ref result);
            else if (type != CompileType.Interface && g.Current.Value.Contains(':') && !(g.Current.Value.StartsWith("trigger_value:", "event_target:", "modifier:"))) Console.WriteLine(ArcException.CreateMessage("Unknown word contains ':', meaning it's possibly a mispelled variable.", g, result));
            //Effects
            else if (type == CompileType.Effect && NewFunctions(g, ref result, NewEffects)) continue;
            else if (type == CompileType.Effect && g == "float_random") __float_random(ref g, ref result);
            else if (type == CompileType.Effect && g == "quick_province_modifier") __quick_province_modifier(ref g, ref result);
            else if (type == CompileType.Effect && g == "quick_country_modifier") __quick_country_modifier(ref g, ref result);
            //Triggers
            else if (type == CompileType.Trigger && NewFunctions(g, ref result, NewTriggers)) continue;
            //Modifiers
            else if (type == CompileType.Modifier && NewFunctions(g, ref result, NewModifiers)) continue;
            //None
            else result.Add(g.Current);
        } while (g.MoveNext());

        result.RemoveEnclosingBlock();

        return result.ToString();
    }
}