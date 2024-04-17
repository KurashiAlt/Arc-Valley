using Arc;

public class ArcModifier : ArcBlock
{
    public ArcModifier()
    {
        Value = new();
        Id = CompileList.Count;
        CompileList.Add(this);
    }
    public ArcModifier(Block value, bool t = false)
    {
        if (!t)
            if (Parser.HasEnclosingBrackets(value))
                value = Compiler.RemoveEnclosingBrackets(value);
        Value = value;
        Id = CompileList.Count;
        CompileList.Add(this);
    }
    public ArcModifier(params string[] s)
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
        if (!ShouldBeCompiled) return Compiler.Compile(BlockType.Modifier, Value);
        Compiled ??= Compiler.Compile(BlockType.Modifier, Value);
        return Compiled;
    }
    public override string Compile(Block b)
    {
        return Compiler.Compile(BlockType.Modifier, b);
    }
    internal static ArcModifier Constructor(Block block) => new(block);
    internal static ArcModifier NamelessConstructor(Block block) => new(block) { ShouldBeCompiled = false };
}
