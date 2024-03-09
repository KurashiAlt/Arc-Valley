using Arc;
using System.Text.RegularExpressions;

public static partial class Calculator
{
    static List<string> GetSteps(string s)
    {
        s = s.Replace("--", "+");
        List<string> steps = new();

        string current = s[0].ToString();
        foreach (char c in s[1..])
        {
            switch (c)
            {
                case '+':
                case '-':
                case '/':
                case '*':
                case '^':
                    steps.Add(current);
                    current = "";
                    steps.Add(c.ToString());
                    break;
                default:
                    current += c;
                    break;
            }
        }
        steps.Add(current);

        return steps;
    }
    public static double Calculate(Word s)
    {
        try
        {
            s.Value = GetParentheses().Replace(s, delegate (Match m)
            {
                return Calculate(
                    new Word(m.Groups[1].Value, s)
                ).ToString();
            });

            s.ReplaceSelf(',', '.');
            List<string> steps = GetSteps(s);

            Operations(ref steps, "^");
            Operations(ref steps, "/", "*");
            Operations(ref steps, "+", "-");

            if (steps.Count != 1) throw ArcException.Create(string.Join(' ', steps), s);

            return GetNum(steps[0]);
        }
        catch (Exception)
        {
            Console.WriteLine(ArcException.CreateMessage(s));
            throw;
        }
    }
    static void Operations(ref List<string> steps, params string[] operators)
    {
        for (int i = 0; i < steps.Count; i++)
        {
            if (i < 0) continue;

            if (operators.Contains(steps[i]))
            {
                double left = GetNum(steps[i - 1]);
                string oper = steps[i];
                double right = GetNum(steps[i + 1]);

                steps.RemoveRange(i - 1, 3);
                i -= 3;

                double value = oper switch
                {
                    "+" => left + right,
                    "-" => left - right,
                    "/" => left / right,
                    "*" => left * right,
                    "^" => double.Pow(left, right),
                    _ => throw ArcException.Create("Unknown Operator", left, oper, right, steps, operators)
                };

                steps.Insert(i + 2, value.ToString());
            }
        }
    }
    static double GetNum(string s)
    {
        if(Compiler.TryGetVariable(new Word(s, 0, "unknown"), out IVariable? var))
        {
            if(var is IArcNumber)
            {
                return ((IArcNumber)var).GetNum();
            }
        }
        
        if (double.TryParse(s, out var num))
        {
            return num;
        }

        throw ArcException.Create(s);
    }

    [GeneratedRegex("\\((.+)\\)")]
    private static partial Regex GetParentheses();
}