namespace Arc;

public partial class Compiler
{
    //public Walker Save(Walker i)
    //{
    //	if (!i.MoveNext())
    //		throw new Exception();
    //
    //	i = TryGetKeyValue(i, out string file, out Block? value, out bool Copy);
    //
    //	if (value == null)
    //		throw new Exception();
    //
    //	ArcBlock Right;
    //
    //	if (value.First == null)
    //		throw new Exception();
    //
    //	if (TryTrimOne(file, '`', out string? newValue))
    //	{
    //		if (newValue == null)
    //			throw new Exception();
    //
    //		Regex Replace = new("{([^}]+)}", RegexOptions.Compiled);
    //		newValue = Replace.Replace(newValue, delegate (Match m) {
    //			return Compile(m.Groups[1].Value).Trim();
    //		});
    //
    //		file = newValue;
    //	}
    //
    //	if (value.Count == 1 && TryGetVariable(value.First.Value, out IValue? NewValue))
    //	{
    //		if (NewValue == null)
    //			throw new Exception();
    //		Right = NewValue.AsBlock();
    //	}
    //	else
    //		Right = ArcBlock.Construct(value, null).AsBlock();
    //
    //	File.WriteAllText(Path.Combine(directory, file), Compile(Right.Value));
    //
    //	return i;
    //}
}
