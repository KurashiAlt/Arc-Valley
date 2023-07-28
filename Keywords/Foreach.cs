namespace Arc;

public partial class Compiler
{
    //public Walker Foreach(Walker i, ref List<string> result)
    //{
    //	i.MoveNext(); //Previous is the inherit word
    //
    //	(Dictionary<string, IValue> dict, string key) = GetNewVariable(i.Current);
    //
    //	i.MoveNext();
    //
    //	if (i.Current != "in")
    //		throw new Exception();
    //
    //	i.MoveNext();
    //
    //	if (!TryGetVariable(i.Current, out IValue? enumerableObject))
    //		throw new Exception();
    //	if (enumerableObject == null)
    //		throw new Exception();
    //
    //	i.MoveNext();
    //
    //	if(!Parser.equal.IsMatch(i.Current))
    //		throw new Exception();
    //
    //	i.MoveNext();
    //
    //	GetValue(i, out Block codeblock);
    //
    //	if (Parser.HasEnclosingBrackets(codeblock))
    //		RemoveEnclosingBrackets(codeblock);
    //
    //	if (codeblock.First == null)
    //		throw new Exception();
    //
    //	Block? whereBlock = null;
    //	if (codeblock.First.Value == "where")
    //	{
    //		whereBlock = codeblock.ExtractFirstSubBlock("where");
    //	}
    //
    //	if(enumerableObject.IsObject())
    //	{
    //		foreach (KeyValuePair<string, IValue> kvp in enumerableObject.AsObject())
    //		{
    //			IValue NewValue = new IArcObject(new Dictionary<string, IValue>()
    //			{
    //				{ "key", new ArcString(kvp.Key) },
    //				{ "value", kvp.Value }
    //			});
    //			dict[key] = new ArcPointer(ref NewValue);
    //			
    //			if(whereBlock == null)
    //			{
    //				result.Add(Compile(codeblock));
    //			}
    //			else
    //			{
    //				if (Logic(whereBlock))
    //				{
    //					result.Add(Compile(codeblock));
    //				}
    //			}
    //
    //		}
    //		dict.Remove(key);
    //	}
    //	else if(enumerableObject.IsList())
    //	{
    //		ArcList lst = enumerableObject.AsList();
    //		foreach(ArcPointer val in lst)
    //		{
    //			dict[key] = new ArcPointer(ref val.Value);
    //	
    //			result.Add(Compile(codeblock));
    //	
    //		}
    //		dict.Remove(key);
    //	}
    //
    //	return i;
    //}
}
