using System;
using System.Security.Principal;
using System.Text;

namespace Arc;
public class InterfaceNode : IArcObject
{
    public ArcString type;
    public ArcCode block;
    public List<InterfaceNode> children;
    public static Dict<InterfaceNode> Files = new();
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
            foreach(InterfaceNode child in children)
            {
                b.Add(child.ToString());
            }
            b.Add("}");
        }
        return b.ToString();
    }
    public InterfaceNode(ArcCode block, List<InterfaceNode> children, ArcString type)
    {
        this.block = block;
        this.children = children;
        this.type = type;
    }
    public bool CanGet(string indexer)
    {
        if(int.TryParse(indexer, out var i))
        {
            return children.Count > i;
        }
        foreach (InterfaceNode child in children)
        {
            string name = GetName(child);
            if (name == null) continue;
            if (name == $"\"{indexer}\"") return true;
        }
        return false;
    }
    public static string? GetName(InterfaceNode node)
    {
        foreach (InterfaceNode child in node.children)
        {
            if (child.type.Value != "name") continue;
            return child.block.ToString();
        }
        return null;
    }
    public IVariable? Get(string indexer)
    {
        if (int.TryParse(indexer, out var i))
        {
            return children[i];
        }
        foreach (InterfaceNode child in children)
        {
            string name = GetName(child);
            if (name == null) continue;
            if (name == $"\"{indexer}\"") return child;
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

        Files.Add(id, Constructor("guiTypes", scope));

        return i;
    }
    public static InterfaceNode Constructor(string id, Block b)
    {
        if (Parser.HasEnclosingBrackets(b)) Compiler.RemoveEnclosingBrackets(b);

        List<InterfaceNode> children = new();
        
        if(b.Count > 1)
        {
            AddChildren(b, ref children);
        }

        return new InterfaceNode(new(b), children, new(id));
    }
    public static void AddChildren(Block b, ref List<InterfaceNode> children)
    {
        Walker i = new(b);
        do
        {
            string nid = i.Current;
            i.ForceMoveNext(); 
            i.Asssert("=");
            i.ForceMoveNext(); 
            i = Compiler.GetScope(i, out Block nScope);
            children.Add(Constructor(nid, Parser.ParseCode(new ArcCode(nScope).Compile(), i.Current.GetFile())));
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
        }
        return i;
    }
    public static string Transpile()
    {
        foreach(var kvp in Files)
        {
            string file = kvp.Value.ToString();
            Program.OverwriteFile($"{Program.TranspileTarget}/interface/{kvp.Key}.gui", file);
        }

        return "Interfaces";
    }
}
