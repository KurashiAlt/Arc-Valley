using System.Text.RegularExpressions;
namespace Arc;
public static partial class Utils
{
    public static bool StartsWith(this string str, params string[] strs)
    {
        foreach(string str2 in strs)
        {
            if(str.StartsWith(str2)) return true;
        }
        return false;
    }
    public static string Prepend(this string sa, int amount, char c)
    {
        string s = "".PadLeft(amount, c);
        return s + sa;
    }
    public static string RegRep(this string sa, string regex, string replace)
    {
        return Regex.Replace(sa, regex, replace);
    }
    public static bool EnclosedBy(this string sa, string c)
    {
        return sa.StartsWith(c) && sa.EndsWith(c);
    }
    public static bool EnclosedBy(this string sa, char c)
    {
        if (sa.Length == 0) return false;
        return sa[0] == c && sa[^1] == c;
    }
}
