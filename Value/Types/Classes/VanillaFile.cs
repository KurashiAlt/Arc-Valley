using System;
using System.Security.Principal;
using System.Text;

namespace Arc;
public class ArcVanillaBlock : ArcBlock
{
    public ArcVanillaBlock()
    {
        Value = new();
        Id = CompileList.Count;
        CompileList.Add(this);
    }
    public ArcVanillaBlock(params string[] s)
    {
        Value = new()
        {
            s
        };
        Id = CompileList.Count;
        CompileList.Add(this);
    }
    public ArcVanillaBlock(Block value, bool t = false)
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
        if (!ShouldBeCompiled) return Compiler.Compile(CompileType.Interface, Value);
        Compiled ??= Compiler.Compile(CompileType.Interface, Value);
        return Compiled;
    }
    public override string Compile(Block b)
    {
        return Compiler.Compile(CompileType.Interface, b);
    }
    internal static ArcVanillaBlock Constructor(Block block) => new(block);
    internal static ArcVanillaBlock NamelessConstructor(Block block) => new(block) { ShouldBeCompiled = false };
}

public class VanillaNode : IArcObject, ArcEnumerable
{
    public ArcString type;
    public ArcVanillaBlock block;
    public List<VanillaNode> children;
    public static Dict<VanillaNode> Files = new();
    public override string ToString()
    {
        Block b = new()
        {
            type, "="
        };
        if (children.Count == 0) b.Add(block);
        else
        {
            b.Add("{");
            foreach (VanillaNode child in children)
            {
                b.Add(child.ToString());
            }
            b.Add("}");
        }
        return b.ToString();
    }
    public VanillaNode(ArcVanillaBlock block, List<VanillaNode> children, ArcString type)
    {
        this.block = block;
        this.children = children;
        this.type = type;
    }
    public bool CanGet(string indexer)
    {
        if (int.TryParse(indexer, out var i))
        {
            return children.Count > i;
        }
        if (indexer == "count") return true;
        if (indexer == "first") return true;
        if (indexer == "last") return true;
        if (indexer == "type") return true;
        if (indexer == "value") return true;
        foreach (VanillaNode child in children)
        {
            string name = GetName(child);
            if (name == null) continue;
            if (name == $"{indexer}") return true;
        }
        return false;
    }
    public static string GetName(VanillaNode node)
    {
        return node.type.Value;
    }
    public IVariable? Get(string indexer)
    {
        if (int.TryParse(indexer, out var i))
        {
            return children[i];
        }
        if (indexer == "count") return new ArcInt(children.Count);
        if (indexer == "first") return children[0];
        if (indexer == "last") return children[^1];
        if (indexer == "type") return type;
        if (indexer == "value") return block;
        foreach (VanillaNode child in children)
        {
            string name = GetName(child);
            if (name == null) continue;
            if (name == $"{indexer}") return child;
        }
        throw new Exception();
    }
    public static Walker Call(Walker i)
    {
        i.ForceMoveNext();

        string id = Compiler.GetId(i.Current);
        i.ForceMoveNext();
        i.Asssert("=");
        i.ForceMoveNext();
        i = Compiler.GetScope(i, out Block scope);

        Files.Add(
            id, 
            Constructor("file", 
                Parser.ParseFile(
                    ArcString.Constructor(scope).Value
                )
            )
        );

        return i;
    }
    public static VanillaNode Constructor(string id, Block b)
    {
        if (Parser.HasEnclosingBrackets(b)) Compiler.RemoveEnclosingBrackets(b);

        List<VanillaNode> children = new();

        if (b.Count > 1)
        {
            AddChildren(b, ref children);
        }

        return new VanillaNode(new(b), children, new(id));
    }
    public static void AddChildren(Block b, ref List<VanillaNode> children)
    {
        Walker i = new(b);
        do
        {
            string nid = i.Current;
            i.ForceMoveNext();
            i.Asssert("=");
            i.ForceMoveNext();
            i = Compiler.GetScope(i, out Block nScope);
            children.Add(Constructor(nid, Parser.ParseCode(Compiler.Compile(CompileType.Interface, nScope), i.Current.GetFile())));
        } while (i.MoveNext());
    }
    public Walker Call(Walker i, ref Block result)
    {
        if (i.MoveNext())
        {
            if (i.Current == "+=")
            {
                i.ForceMoveNext();
                i = Compiler.GetScope(i, out Block scope);
                if (Parser.HasEnclosingBrackets(scope)) Compiler.RemoveEnclosingBrackets(scope);
                AddChildren(scope, ref children);
            }
            else
            {
                i.MoveBack();
                result.Add(block);
            }
        }
        else
        {
            result.Add(block);
        }
        return i;
    }
    public static string Transpile()
    {
        foreach (var kvp in Files)
        {
            string file = kvp.Value.ToString();
            Program.OverwriteFile($"{Program.TranspileTarget}/interface/{kvp.Key}.gui", file);
        }

        return "Interfaces";
    }

    public IEnumerator<IVariable> GetArcEnumerator()
    {
        return children.GetEnumerator();
    }
}
