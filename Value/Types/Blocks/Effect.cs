using Arc;

public class ArcEffect : ArcBlock
{
    public ArcEffect()
    {
        Value = new();
        Id = CompileList.Count;
        CompileList.Add(this);
    }
    public ArcEffect(Block value, bool t = false)
    {
        if (!t)
            if (Parser.HasEnclosingBrackets(value))
                value = Compiler.RemoveEnclosingBrackets(value);
        Value = value;
        Id = CompileList.Count;
        CompileList.Add(this);
    }
    public ArcEffect(params string[] s)
    {
        Value = new()
        {
            s
        };
        Id = CompileList.Count;
        CompileList.Add(this);
    }
    public override string Compile()
    {
        if (!ShouldBeCompiled) return Compiler.Compile(CompileType.Effect, Value);
        Compiled ??= Compiler.Compile(CompileType.Effect, Value);
        return Compiled;
    }
    public override string Compile(Block b)
    {
        return Compiler.Compile(CompileType.Effect, b);
    }
    public override string ToString()
    {
        return Compile();
    }
    internal static ArcEffect Constructor(Block block) => new(block);
    internal static ArcEffect NamelessConstructor(Block block) => new(block) { ShouldBeCompiled = false };
}
