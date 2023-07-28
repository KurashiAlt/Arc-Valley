namespace Arc;

//public class ArcList : IArcEnumerable
//{
//	//Regular Stuff
//	public LinkedList<IValue> List { get; set; }
//	public ArcType Type { get; set; }
//#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
//	private ArcList() { }
//#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
//	public ArcList(LinkedList<IValue> value, ArcType type)
//	{
//		List = value;
//		Type = type;
//	}
//	//Type Finding
//	public bool IsList() => true;
//	public ArcList AsList() => this;
//	public IValue GetCopy() => new ArcList(new(List), new(Type));
//	//Contract
//	public IValue ThisConstruct(Block code, Dictionary<string, IValue>? vars) => Construct(code, vars, Type);
//	public static IValue Construct(Block code, Dictionary<string, IValue>? vars)
//	{
//		if (code.First == null)
//			throw new Exception();
//
//		if (!code.First.Value.value.EndsWith("[]"))
//			throw new Exception();
//
//		ArcType Type = ArcType.Construct(new(code.First.Value.value[..^2]), vars).AsType();
//		code.RemoveFirst();
//
//		if(code.First.Value == null)
//			throw new Exception();
//
//		return Construct(code, vars, Type);
//	}
//	public static IValue Construct(Block code, Dictionary<string, IValue>? vars, ArcType type)
//	{
//		ArcList NewList = new()
//		{
//			List = new(),
//			Type = type
//		};
//
//		if (Parser.HasEnclosingBrackets(code))
//			code = Compiler.RemoveEnclosingBrackets(code);
//
//		if (code.Count == 0) return NewList;
//
//		Walker i = new(code);
//
//		do
//		{
//			i = Compiler.GetValue(i, out Block words);
//			IValue newValue = type.ThisConstruct(words, vars);
//			NewList.List.AddLast(new ArcPointer(ref newValue));
//		} while (i.MoveNext());
//
//		return NewList;
//	}
//	public bool Fulfills(IValue v)
//	{
//		if (!v.IsString())
//			return false;
//		ArcList a = v.AsList();
//		return List == a.List;
//	}
//	public bool SameTypeAs(IValue v)
//	{
//		return v.IsList();
//	}
//	//Code
//	public Walker Call(Walker i, ref List<string> result, Compiler comp)
//	{
//		if (i.MoveNext())
//		{
//			switch (i.Current)
//			{
//				case "+=":
//					{
//						//if (!i.MoveNext())
//						//throw new Exception();
//						//
//						//Value += int.Parse(i.Current);
//						if (!i.MoveNext())
//							throw new Exception();
//
//						i = Compiler.GetScope(i, out Block scope);
//
//						if (scope.First == null)
//							throw new Exception();
//
//						if(scope.Count == 1)
//						{
//							if(comp.TryGetVariable(scope.First.Value, out var variable))
//							{
//								if(variable == null)
//									throw new Exception();
//
//								if (!Type.Fulfills(variable))
//									throw new Exception();
//								
//								List.AddLast(new ArcPointer(ref variable));
//							}
//							else
//							{
//								List.AddLast(Type.ThisConstruct(scope, comp.variables));
//							}
//						}
//						else
//						{
//							List.AddLast(Type.ThisConstruct(scope, comp.variables));
//						}
//					}
//					break;
//				default:
//					throw new Exception();
//			}
//		}
//		else
//		{
//			throw new Exception();
//		}
//		return i;
//	}
//	public override string ToString()
//	{
//		return string.Join(' ', List);
//	}
//	public IEnumerator GetEnumerator()
//	{
//		return List.GetEnumerator();
//	}
//}