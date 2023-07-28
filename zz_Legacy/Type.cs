namespace Arc;

//public class ArcType : IValue
//{
//	//Regular Stuff
//	public IValue blueprint;
//	public ArcType(IValue blueprint)
//	{
//		this.blueprint = blueprint;
//	}
//
//	//Type Finding
//	public bool IsType() => true;
//	public ArcType AsType() => this;
//	public IValue GetCopy() => new ArcType(blueprint);
//	//Contract
//	public IValue ThisConstruct(Block s, Dictionary<string, IValue>? vars) => blueprint.ThisConstruct(s, vars);
//	public static IValue Construct(Block s, Dictionary<string, IValue>? vars)
//	{
//		if (s.Count > 1)
//			throw new Exception("Too many elements given to ArcString");
//		if (s.Count < 0)
//			throw new Exception("Too few elements given to ArcString");
//		if (s.First == null)
//			throw new Exception();
//		if (Compiler.TryGetVariable(vars, s.First.Value, out IValue? n))
//		{
//			if (n == null)
//				throw new Exception();
//		}
//		else
//		{
//			n = (string)s.First.Value switch
//			{
//				"float" => new ArcFloat(0),
//				"int" => new ArcInt(0),
//				"block" => new ArcBlock(new()),
//				"bool" => new ArcBool(false),
//				"string" => new ArcString(""),
//				_ => throw new NotImplementedException(),
//			};
//		}
//		return new ArcType(n);
//	}
//	public static bool TypeContract(IValue v)
//	{
//		if (!v.IsType())
//			return false;
//		ArcType _ = (ArcType)v;
//		return true;
//	}
//	public bool Fulfills(IValue v)
//	{
//		return blueprint.Fulfills(v);
//	}
//	public bool SameTypeAs(IValue v)
//	{
//		return blueprint.SameTypeAs(v);
//	}
//
//	//Code
//	public Walker Call(Walker i, ref List<string> result, Compiler comp)
//	{
//		return comp.Var(i, blueprint.ThisConstruct);
//	}
//}