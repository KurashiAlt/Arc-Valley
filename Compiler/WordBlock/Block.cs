using System.Text;

namespace Arc;

public class Block : LinkedList<Word>
{
    public Block(params string[] s)
    {
        foreach(string s2 in s)
        {
            AddLast(s2);
        }
    }
    public Block(string s)
    {
        AddLast(s);
    }
    public void Prepend(string s)
    {
        AddFirst(s);
    }
    public Block()
    {

    }
    //public Block ExtractFirstSubBlock(string? key = null)
    //{
    //	if(key != null)
    //	{
    //		Dictionary<string, IValue> ret = new();
    //		Compiler.Var(ret, null, new(this), ArcBlock.Construct, false, key);
    //		Block ret2 = ret[key].AsBlock().Value;
    //		for(int i = 0; i < (ret2.Count + 4); i++)
    //		{
    //			RemoveFirst();
    //		}
    //		return ret2;
    //	}
    //	else
    //	{
    //		Compiler.GetScope(new(this), out Block scope);
    //		for (int i = 0; i < scope.Count; i++)
    //		{
    //			RemoveFirst();
    //		}
    //		return scope;
    //	}
    //}
    public override string ToString()
    {
        Walker i = new(this);
        StringBuilder sb = new();
        do
        {
            sb.Append($"{i.Current} ");
        } while (i.MoveNext());
        return sb.ToString();
    }
    public void Add(string id, ArcBool value)
    {
        if (value.Value) Add(id, "=", "yes");
    }
    public void Add(params object[] s)
    {
        foreach(object v in s)
        {
            string? c = v.ToString();
            if (c == null) throw new Exception();
            AddLast(c);
        }
    }
    public void Add(params string[] s)
    {
        foreach(string v in s)
        {
            AddLast(v);
        }
    }
    public void Add(string s) => AddLast(s);
    public void Add(ArcString s) => AddLast(s.Value);
    public void Add(Block s)
    {
        foreach (Word w in s)
        {
            Add(w);
        }
    }
}
public class Walker
{
    private LinkedListNode<Word> node;

    public Walker(LinkedListNode<Word> node)
    {
        this.node = node;
    }
    public Walker(Block code)
    {
        if (code.First == null)
            throw new Exception();
        node = code.First;
    }
    public void ForceMoveNext()
    {
        if (!MoveNext()) throw new Exception();
    }
    public void ForceMoveBack()
    {
        if (!MoveBack()) throw new Exception();
    }
    public bool MoveNext()
    {
        if (node.Next == null)
            return false;
        node = node.Next;
        return true;
    }
    public bool MoveBack()
    {
        if (node.Previous == null)
            return false;
        node = node.Previous;
        return true;
    }
    public Word Current => node.Value;
}