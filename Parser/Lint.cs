using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcInstance;
namespace Arc;
public interface ILintCommand
{
    public bool get(out LintEdit? b, out LintAdd? c);
    public string Lint(string key);
    public string GetFile();
}
public class LintEdit : ILintCommand
{
    public static int Edits = 0;
    public bool get(out LintEdit? b, out LintAdd? c)
    {
        b = this;
        c = null;
        return true;
    }
    public string File;
    public Block Code;
    public string GetFile() => File;
    public LintEdit(string file, Block code)
    {
        File = Path.GetRelativePath(Instance.directory, file);
        Code = code;
    }
    public string Lint(string key) => string.Join(' ', Code);
}
public class LintAdd : ILintCommand
{
    public bool get(out LintEdit? b, out LintAdd? c)
    {
        b = null;
        c = this;
        return false;
    }
    public string File;
    public Args Args;
    public string GetFile() => File;
    public LintAdd(string file, Args args)
    {
        File = Path.GetRelativePath(Instance.directory, file);
        Args = args;
    }
    public string Lint(string key)
    {
        StringBuilder sb = new();
        string[] v = key.Split('~');
        sb.Append($"new {v[0]} {v[1]} = {{ ");
        if (Args.keyValuePairs == null) throw new Exception();
        foreach(KeyValuePair<string, Block> arg in Args.keyValuePairs)
        {
            sb.Append($"{arg.Key.Split('~')[0]} = {string.Join(' ', arg.Value)} ");
        }
        sb.Append($"}}");
        return sb.ToString();
    }
}