namespace Arc;
public class Word
{
    public string Value;
    public int Line;
    public int File;
    public static List<string> Files = new()
    {
        "unknown"
    };
    public bool StartsWith(char text) => Value.StartsWith(text);
    public bool StartsWith(string text) => Value.StartsWith(text);
    public bool EndsWith(char text) => Value.EndsWith(text);
    public bool EndsWith(string text) => Value.EndsWith(text);
    public Word ReplaceSelf(char oldValue, char newValue)
    {
        Value = Value.Replace(oldValue, newValue);
        return this;
    }
    public Word ReplaceSelf(string oldValue, string newValue)
    {
        Value = Value.Replace(oldValue, newValue);
        return this;
    }
    public string GetFile()
    {
        return Files[File];
    }
    public Word(string value)
    {
        Value = value;
        Line = 0;
        File = 0;
    }
    public Word(string value, Walker w)
    {
        Value = value;
        Line = w.Current.Line;
        File = w.Current.File;
    }
    public Word(string value, Word w)
    {
        Value = value;
        Line = w.Line;
        File = w.File;
    }
    public Word(
        string value,
        int line,
        string file
    ) {
        Value = value;
        Line = line;
        file = Path.GetRelativePath(Program.directory, file);
        if (Files.Contains(file))
        {
            File = Files.IndexOf(file);
        }
        else
        {
            Files.Add(file);
            File = Files.Count - 1;
        }
    }
    public override string ToString() => Value;
    public static implicit operator string(Word w)
    {
        if (w == null)
            return "";
        return w.ToString();
    }
}