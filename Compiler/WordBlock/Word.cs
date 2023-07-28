namespace Arc;
public class Word
{
    public string value;
    public Word(string value)
    {
        this.value = value;
    }
    public override string ToString() => value;
    public static implicit operator string(Word w)
    {
        if (w == null)
            return "";
        return w.ToString();
    }
    public static implicit operator Word(string w) => new(w);
}