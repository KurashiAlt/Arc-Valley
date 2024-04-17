namespace Arc;
public class ArcTrigger : ArcBlock
{
    public ArcTrigger()
    {
        Value = new();
        Id = CompileList.Count;
        CompileList.Add(this);
    }
    public ArcTrigger(Block value, bool t = false)
    {
        if (!t)
            if (Parser.HasEnclosingBrackets(value))
                value = Compiler.RemoveEnclosingBrackets(value);
        Value = value;
        Id = CompileList.Count;
        CompileList.Add(this);
    }
    public ArcTrigger(params string[] s)
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
        if (!ShouldBeCompiled) return Compiler.Compile(BlockType.Trigger, Value);
        Compiled ??= Compiler.Compile(BlockType.Trigger, Value);
        return Compiled;
    }
    public override string Compile(Block b)
    {
        return Compiler.Compile(BlockType.Trigger, b);
    }
    internal static ArcTrigger Constructor(Block block) => new(block);
    internal static ArcTrigger NamelessConstructor(Block block) => new(block) { ShouldBeCompiled = false };
}