namespace Arc;
public class Word
{
    public string Value;
    public int Line;
    public int File;
    public static List<string> Files = new();
    public string GetFile()
    {
        return Files[File];
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