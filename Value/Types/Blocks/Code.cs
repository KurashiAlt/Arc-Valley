using Arc;

public class ArcCode : ArcBlock
{
    public ArcCode()
    {
        Value = new();
        Id = CompileList.Count;
        CompileList.Add(this);
    }
    public ArcCode(params string[] s)
    {
        Value = new()
        {
            s
        };
        Id = CompileList.Count;
        CompileList.Add(this);
    }
    public ArcCode(Block value, bool t = false)
    {
        if (!t)
            if (Parser.HasEnclosingBrackets(value))
                value = Compiler.RemoveEnclosingBrackets(value);
        Value = value;
        Id = CompileList.Count;
        CompileList.Add(this);
    }
    public override string Compile()
    {
        if (!ShouldBeCompiled) return Compiler.Compile(BlockType.Block, Value);
        Compiled ??= Compiler.Compile(BlockType.Block, Value);
        return Compiled;
    }
    public override string Compile(Block b)
    {
        return Compiler.Compile(BlockType.Block, b);
    }
    internal static ArcCode Constructor(Block block) => new(block);
    internal static ArcCode NamelessConstructor(Block block) => new(block) { ShouldBeCompiled = false };
}
