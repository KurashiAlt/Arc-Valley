using System.ComponentModel.Design;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Arc;

public class Block : LinkedList<Word>
{
    public Word ToWord()
    {
        if (First != null) return new(ToString(), (First).Value);
        return new Word(ToString());
    }
    public Block(params string[] s)
    {
        foreach(string s2 in s)
        {
            AddLast(new Word(s2, 0, "unknown"));
        }
    }
    public Block(Block b)
    {
        foreach (Word w in b)
        {
            AddLast(new Word(w.Value, w));
        }
    }
    public Block(string s)
    {
        AddLast(new Word(s, 0, "unknown"));
    }
    public void Prepend(string s)
    {
        AddFirst(new Word(s, 0, "unknown"));
    }
    public Block()
    {

    }
    public Block RemoveEnclosingBlock()
    {
        if (First == null || Last == null) return this;
        if (First.Value == "{" && Last.Value == "}")
        {
            RemoveFirst();
            RemoveLast();
        }
        return this;
    }
    public override string ToString()
    {
        return string.Join(' ', this);
    }
    public void Add(string id, ArcBool value)
    {
        if (value.Value) Add(id, "=", "yes");
    }
    public void Add(params object?[] s)
    {
        foreach(object? v in s)
        {
            if (v == null) continue;
            string c = v.ToString() ?? throw new Exception();
            AddLast(new Word(c, 0, "unknown"));
        }
    }
    public void Add(params string[] s)
    {
        foreach(string v in s)
        {
            AddLast(new Word(v, 0, "unknown"));
        }
    }
    public void Add(string s) => AddLast(new Word(s, 0, "unknown"));
    public void Add(ArcString s) => AddLast(new Word(s.ToString(), 0, "unknown"));
    public void Add(Block s)
    {
        foreach (Word w in s)
        {
            Add(w);
        }
    }
    public static Block operator +(Block a, Block b)
    {
        return new Block()
        {
            a,
            b
        };
    }
}
public class Walker
{
    private LinkedListNode<Word> node;
    public static implicit operator string(Walker w) => w.Current;
    public bool StartsWith(string w) => Current.StartsWith(w);
    public bool StartsWith(char w) => Current.StartsWith(w);
    public bool EndsWith(string w) => Current.EndsWith(w);
    public bool EndsWith(char w) => Current.EndsWith(w);
    public bool EnclosedBy(string left, char right) => Current.StartsWith(left) && Current.EndsWith(right);
    public bool EnclosedBy(char left, char right) => Current.StartsWith(left) && Current.EndsWith(right);
    public Walker(Block code)
    {
        if (code.First == null)
            throw new Exception();
        node = code.First;
    }
    public void Asssert(string s)
    {
        if (Current != s) throw ArcException.Create(s, this);
    }
    public void ForceMoveNext()
    {
        if (!MoveNext()) throw ArcException.Create(this);
    }
    public void ForceMoveBack()
    {
        if (!MoveBack()) throw ArcException.Create(this);
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