using System.Diagnostics;

namespace Arc;
public enum LogicalScope
{
    NOT,
    AND,
    OR
}
public static partial class Compiler
{
    public static bool When(Block code, LogicalScope scope = LogicalScope.AND)
    {
        code.RemoveEnclosingBlock();
        if (code.Count == 0)
            return true;

        int Questions = 0;
        int Hits = 0;

        Walker g = new(code);
        do
        {
            Questions++;
            if (g.Current == "NOT")
            {
                g = Args.GetArgs(g, out Args args);
                args.block.RemoveEnclosingBlock();

                if (When(args.block, LogicalScope.NOT)) Hits++;
            }
            else if (g.Current == "OR")
            {
                g = Args.GetArgs(g, out Args args);
                args.block.RemoveEnclosingBlock();

                if (When(args.block, LogicalScope.OR)) Hits++;
            }
            else if (g.Current == "AND")
            {
                g = Args.GetArgs(g, out Args args);
                args.block.RemoveEnclosingBlock();

                if (When(args.block, LogicalScope.AND)) Hits++;
            }
            else if (g.Current == "exists")
            {
                g = Args.GetArgs(g, out Args args);
                Word key = args.block.ToWord();

                if (TryGetVariable(key, out _)) Hits++;
            }
            else if (TryGetVariable(g, out IVariable? var) && var != null)
            {
                if (var.LogicalCall(ref g)) Hits++;
            }
            else throw ArcException.Create(code, scope, Questions, Hits, g);
        } while (g.MoveNext());

        return scope switch
        {
            LogicalScope.OR => Hits > 0,
            LogicalScope.NOT => Hits == 0,
            _ => Questions == Hits,
        };
    }
}