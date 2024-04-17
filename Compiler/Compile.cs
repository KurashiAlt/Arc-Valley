namespace Arc;
public enum BlockType
{
    Effect,
    Trigger,
    Modifier,
    Block,
}
public static partial class Compiler
{
    public static string Compile(Block code)
    {
        if (code.Count == 0)
            return "";

        Block result = new();

        Walker g = new(code);
        do
        {
            if (AllCompile(ref g, ref result, Compile)) continue;

            result.Add(g.Current);
        } while (g.MoveNext());

        if (Parser.HasEnclosingBrackets(result)) result = RemoveEnclosingBrackets(result);

        return string.Join(' ', result);
    }
}