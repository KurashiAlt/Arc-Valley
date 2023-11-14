namespace Arc;
public class ArcTrigger : ArcBlock
{
    public ArcTrigger()
    {
        Value = new();
    }
    public ArcTrigger(Block value, bool t = false)
    {
        if (!t)
            if (Parser.HasEnclosingBrackets(value))
                value = Compiler.RemoveEnclosingBrackets(value);
        Value = value;
    }
    public ArcTrigger(params string[] s)
    {
        Value = new()
        {
            s
        };
    }
    public override string Compile()
    {
        return Compiler.CompileTrigger(Value);
    }
    public override string Compile(Block b)
    {
        return Compiler.CompileTrigger(b);
    }
    internal static ArcTrigger Constructor(Block block) => new(block);
}
public class ArcEffect : ArcBlock
{
    public ArcEffect()
    {
        Value = new();
    }
    public ArcEffect(Block value, bool t = false)
    {
        if (!t)
            if (Parser.HasEnclosingBrackets(value))
                value = Compiler.RemoveEnclosingBrackets(value);
        Value = value;
    }
    public ArcEffect(params string[] s)
    {
        Value = new()
        {
            s
        };
    }
    public override string Compile()
    {
        return Compiler.CompileEffect(Value);
    }
    public override string Compile(Block b)
    {
        return Compiler.CompileEffect(b);
    }
    internal static ArcEffect Constructor(Block block) => new(block);
}
public class ArcModifier : ArcBlock
{
    public ArcModifier()
    {
        Value = new();
    }
    public ArcModifier(Block value, bool t = false)
    {
        if (!t)
            if (Parser.HasEnclosingBrackets(value))
                value = Compiler.RemoveEnclosingBrackets(value);
        Value = value;
    }
    public ArcModifier(params string[] s)
    {
        Value = new()
        {
            s
        };
    }
    public override string Compile()
    {
        return Compiler.CompileModifier(Value);
    }
    public override string Compile(Block b)
    {
        return Compiler.CompileModifier(b);
    }
    internal static ArcModifier Constructor(Block block) => new(block);
}
public class ArcCode : ArcBlock
{
    public ArcCode()
    {
        Value = new();
    }
    public ArcCode(IEnumerable<string> enumerable)
    {
        Value = new();

        foreach (string s in enumerable)
        {
            Value.Add(s);
        }
    }
    public ArcCode(params string[] s)
    {
        Value = new()
        {
            s
        };
    }
    public ArcCode(Block value, bool t = false)
    {
        if (!t)
            if (Parser.HasEnclosingBrackets(value))
                value = Compiler.RemoveEnclosingBrackets(value);
        Value = value;
    }
    public override string Compile()
    {
        return Compiler.Compile(Value);
    }
    public override string Compile(Block b)
    {
        return Compiler.Compile(b);
    }
    internal static ArcCode Constructor(Block block) => new(block);
}
public class ArcFactor : ArcBlock
{
    public ArcFactor()
    {
        Value = new();
    }
    public ArcFactor(IEnumerable<string> enumerable)
    {
        Value = new();

        foreach (string s in enumerable)
        {
            Value.Add(s);
        }
    }
    public ArcFactor(params string[] s)
    {
        Value = new()
        {
            s
        };
    }
    public ArcFactor(Block value, bool t = false)
    {
        if (!t)
            if (Parser.HasEnclosingBrackets(value))
                value = Compiler.RemoveEnclosingBrackets(value);
        Value = value;
    }
    public override string Compile()
    {
        return Compiler.FactorCompile(Value);
    }
    public override string Compile(Block b)
    {
        return Compiler.FactorCompile(b);
    }
    internal static ArcFactor Constructor(Block block) => new(block);
}