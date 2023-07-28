namespace Arc;
//public class ArcArray : IValue
//{
//	public ValueTypeCode TypeCode => ValueTypeCode.Array;
//	public LinkedList<ArcPointer> List { get; set; }
//	public ArcArray()
//	{
//		List = new LinkedList<ArcPointer>();
//	}
//	public ArcArray(LinkedList<ArcPointer> value)
//	{
//		List = value;
//	}
//	public ArcArray(Block code)
//	{
//		List = new LinkedList<ArcPointer>();
//
//		if (!Parser.HasEnclosingBrackets(code, "[", "]"))
//			throw new Exception("Object without enclosing brackets");
//		code = Compiler.RemoveEnclosingBrackets(code);
//
//		ArcList ob = new(code);
//		List = ob.List;
//	}
//	public Walker Call(Walker i, ref List<string> result, Compiler comp)
//	{
//		i.MoveNext();
//		return Call(i, comp.variables);
//	}
//	public Walker Call(Walker i, Dictionary<string, ArcPointer> vars)
//	{
//		Walker g = Compiler.Var(vars, i, (Block s) => new ArcList(s, List), false);
//		return g;
//	}
//	public bool Fulfills(IValue v)
//	{
//		if (v.TypeCode != TypeCode)
//			return false;
//		LinkedList<ArcPointer> vlist = ((ArcList)v).List;
//		if (vlist.Count != List.Count)
//			return false;
//		LinkedList<ArcPointer>.Enumerator a = List.GetEnumerator();
//		LinkedList<ArcPointer>.Enumerator b = vlist.GetEnumerator();
//		while (a.MoveNext() && b.MoveNext())
//		{
//			if (!a.Current.Value.Fulfills(b.Current.Value))
//				return false;
//		}
//		return true;
//	}
//	public override string ToString()
//	{
//		return "[Arc Array]";
//	}
//}