namespace Arc;
//public class ArcInterface : IArcEnumerable
//{
//	//Regular Stuff
//	public Dictionary<string, IValue> Properties;
//	public ArcInterface()
//	{
//		Properties = new();
//	}
//	public ArcInterface(Dictionary<string, IValue> value)
//	{
//		Properties = value;
//	}
//	//Type Finding
//	public bool IsInterface() => true;
//	public ArcInterface AsInterface() => this;
//	public IValue GetCopy() => new ArcInterface(Properties);
//	//Contract
//	public IValue ThisConstruct(Block code, Dictionary<string, IValue>? vars) {
//		IArcObject t = new(Construct(code, Contract(), vars).Properties);
//		foreach(KeyValuePair<string, IValue> pair in Properties)
//		{
//			if(!t.Properties.ContainsKey(pair.Key))
//			{
//				if (pair.Value.IsType())
//					throw new Exception($"Object is expecting {pair.Key} to be defined, it is not optional. Object was \n{Parser.FormatCode(code.ToString())}");
//				t.Properties.Add(pair.Key, pair.Value.GetCopy());
//			}
//		}
//		return t;
//	}
//	public static IValue Construct(Block code, Dictionary<string, IValue>? vars)
//	{
//		return Construct(code, new(), vars);
//	}
//	public static ArcInterface Construct(Block code, Dictionary<string, Func<Block, Dictionary<string, IValue>?, IValue>> Types, Dictionary<string, IValue>? vars)
//	{
//		ArcInterface newValue = new()
//		{
//			Properties = new()
//		};
//
//		if (Parser.HasEnclosingBrackets(code))
//			code = Compiler.RemoveEnclosingBrackets(code);
//
//		if (code.Count == 0) return newValue;
//
//		Dictionary<string, Func<Walker, Walker>> keywords = new()
//		{
//			{ "string", (Walker i) => Compiler.Var(newValue.Properties, vars, i, ArcString.Construct) },
//			{ "bool", (Walker i) => Compiler.Var(newValue.Properties, vars, i, ArcBool.Construct) },
//			{ "float", (Walker i) => Compiler.Var(newValue.Properties, vars, i, ArcFloat.Construct) },
//			{ "int", (Walker i) => Compiler.Var(newValue.Properties, vars, i, ArcInt.Construct) },
//			{ "object", (Walker i) => Compiler.Var(newValue.Properties, vars, i, ArcInterface.Construct) },
//			{ "block", (Walker i) => Compiler.Var(newValue.Properties, vars, i, ArcBlock.Construct) },
//			{ "list", (Walker i) => Compiler.Var(newValue.Properties, vars, i, ArcList.Construct) },
//			{ "type", (Walker i) => Compiler.Var(newValue.Properties, vars, i, ArcType.Construct) }
//		};
//
//		Walker i = new(code);
//
//		do
//		{
//			if (keywords.ContainsKey(i.Current))
//			{
//				i = keywords[i.Current](i);
//				continue;
//			}
//			else if (Types.ContainsKey(i.Current))
//			{
//				i = Compiler.Var(newValue.Properties, vars, i, Types[i.Current], false);
//				continue;
//			}
//			else if(Compiler.TryGetVariable(vars, i.Current, out IValue? var))
//			{
//				if (var == null)
//					throw new Exception();
//
//				i = Compiler.Var(newValue.Properties, vars, i, var.ThisConstruct);
//				continue;
//			}
//			else
//			{
//				throw new Exception($"Unknown Datatype for {i.Current}");
//			}
//		} while (i.MoveNext());
//
//		return newValue;
//	}
//	public bool Fulfills(IValue v)
//	{
//		if (!v.IsObject())
//			return false;
//		IArcObject a = v.AsObject();
//		foreach (KeyValuePair<string, IValue> s in Properties)
//		{
//			if (a.Properties.ContainsKey(s.Key))
//			{
//				IValue l = s.Value.GetOrigin();
//				IValue r = a.Properties[s.Key].GetOrigin();
//				if (!l.SameTypeAs(r))
//				{
//					return false;
//				}
//			}
//			else
//			{
//				return false;
//			}
//		}
//		return true;
//	}
//	public bool SameTypeAs(IValue v)
//	{
//		return v.IsInterface();
//	}
//	//Code
//	public Dictionary<string, Func<Block, Dictionary<string, IValue>?, IValue>> Contract()
//	{
//		Dictionary<string, Func<Block, Dictionary<string, IValue>?, IValue>> contract = new();
//		foreach(KeyValuePair<string, IValue> i in Properties)
//		{
//			contract.Add(i.Key, i.Value.ThisConstruct);
//		}
//		return contract;
//	}
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
//						ArcInterface addition = Construct(scope, new() { { "key", ArcString.Construct } }, new()).AsInterface();
//
//						Properties.Add(addition.Properties["key"].AsString().Value, addition.Properties["value"]);
//					}
//					break;
//				default:
//					i.MoveBack();
//					i = comp.Var(i, ThisConstruct);
//					break;
//			}
//		}
//		else
//		{
//			i = comp.Var(i, ThisConstruct);
//		}
//		return i;
//	}
//	public override string ToString()
//	{
//		return string.Join(' ', Properties);
//	}
//	public IEnumerator GetEnumerator()
//	{
//		return Properties.GetEnumerator();
//	}
//}
//{
//	public new bool IsObject() => false;
//	public new ArcInterface AsObject() { throw new Exception(); }
//
//	public new IValue ThisConstruct(Block code, Dictionary<string, IValue>? vars) => (ArcInterface)base.ThisConstruct(code, vars);
//	public new static IValue Construct(Block code, Dictionary<string, IValue>? vars) => (ArcInterface)ArcInterface.Construct(code, vars);
//	public new static IValue Construct(Block code, Dictionary<string, Func<Block, Dictionary<string, IValue>?, IValue>> Types, Dictionary<string, IValue>? //vars) => (ArcInterface)ArcInterface.Construct(code, Types, vars);
//	public new bool Fulfills(IValue v)
//	{
//		if (!v.IsObject())
//			return false;
//		ArcInterface a = v.AsObject();
//		foreach(KeyValuePair<string, IValue> s in Properties)
//		{
//			if (a.Properties.ContainsKey(s.Key))
//			{
//				if (!s.Value.Fulfills(a.Properties[s.Key]))
//				{
//					return false;
//				}
//			}
//			else
//			{
//				return false;
//			}
//		}
//		return true;
//	}
//}