namespace Arc;

public partial class Compiler
{
    //public Walker Var<T>(Walker i, Func<Block, Dictionary<string, IValue>?, T> Constructor, bool move = true, string? keyOverride = null) where T : IValue
    //{
    //	return Var(variables, variables, i, Constructor, move, keyOverride);
    //}
    //public static Walker Var<T>(Dictionary<string, IValue> props, Dictionary<string, IValue>? vars, Walker i, Func<Block,Dictionary<string, IValue>?, T> Constructor, bool move /=/ true, string? keyOverride = null) where T : IValue
    //{
    //	if(move)
    //		i.MoveNext(); //The previous spot is the datatype
    //
    //	i = GetKeyValue(i, out string key, out Block? value, out bool Copy);
    //
    //	if (!Parser.IsVariableKey(key))
    //		throw new Exception("Invalid Variable Key");
    //
    //	if (value == null || value.First == null)
    //		throw new Exception();
    //
    //	if(value.Count == 1 && value.First != null && vars != null)
    //	{
    //		if (Compiler.TryTrimOne(value.First.Value, '`', out string? newValue))
    //		{
    //			if (newValue == null)
    //				throw new Exception();
    //
    //			Regex Replace = new("{([^}]+)}", RegexOptions.Compiled);
    //			newValue = Replace.Replace(newValue, delegate (Match m) {
    //				if(TryGetVariable(vars, m.Groups[1].Value, out IValue? newV))
    //				{
    //					if (newV == null)
    //						throw new Exception();
    //
    //					return newV.ToString();
    //				}
    //				else
    //				{
    //					return m.Groups[1].Value;
    //				}
    //			});
    //
    //			value.First.Value = newValue;
    //		}
    //	}
    //
    //	IValue Value;
    //	if (value.Count == 1 && vars != null && TryGetVariable(vars, value.First.Value, out IValue? NewValue))
    //	{
    //		if (NewValue == null)
    //			throw new Exception();
    //		if (Copy)
    //		{
    //			Value = NewValue.GetCopy();
    //		}
    //		else
    //		{
    //			Value = NewValue;
    //		}
    //	}
    //	else {
    //		if (value == null)
    //			throw new Exception();
    //
    //		Value = Constructor(value, vars);
    //	}
    //
    //	if (keyOverride != null)
    //		key = keyOverride;
    //
    //	(Dictionary<string, IValue> dict, string key) Val = GetNewVariable(props, key); //This will start as null
    //
    //	Val.dict[Val.key] = new ArcPointer(ref Value);
    //
    //	return i;
    //}
}
