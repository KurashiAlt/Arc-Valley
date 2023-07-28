namespace Valley;
//public static class Valley
//{
//	public static Dictionary<string, string> loc = new();
//	public static Dictionary<string, string> mod = new();
//	public static void Save(string directory)
//	{
//		StringBuilder sb = new StringBuilder();
//		sb.Append("l_english:\n");
//		foreach (KeyValuePair<string, string> kvp in loc)
//		{
//			sb.Append($" {kvp.Key}: {kvp.Value}\n");
//		}
//		File.WriteAllText(directory + "/target/localisation/english/arc5_l_english.yml", Parser.ConvertStringToUtf8Bom(sb.ToString()));
//		
//		StringBuilder sa = new StringBuilder();
//		foreach(KeyValuePair<string, string> kvp in mod)
//		{
//			sa.Append($"{kvp.Key} = {{ {kvp.Value} }}");
//		}
//
//		File.WriteAllText(directory + "target/common/event_modifiers/arc5.txt", sa.ToString());
//	}
//	public static Walker DefineMod(Walker i, Compiler comp)
//	{
//		Dictionary<string, IValue> ARGS = new Dictionary<string, IValue>();
//		i = Compiler.Var(ARGS, comp.variables, i, ArcBlock.Construct);
//
//		foreach(KeyValuePair<string, IValue> s in ARGS)
//		{
//			mod.Add(s.Key, s.Value.AsBlock().ToString());
//		}
//
//		return i;
//	}
//	public static Walker DefineLoc(Walker i, Compiler comp)
//	{
//		Dictionary<string, IValue> ARGS = new Dictionary<string, IValue>();
//		i = Compiler.Var(ARGS, comp.variables, i, ArcString.Construct);
//
//		foreach(KeyValuePair<string, IValue> s in ARGS)
//		{
//			loc.Add(s.Key, s.Value.AsString().Value);
//		}
//
//		return i;
//	}
//	public static Walker SaveAsProvinces(Walker i, Compiler comp, string directory)
//	{
//		i = Compiler.TryGetKeyValue(i, out string _, out Block? value, out bool Copy);
//
//		if (value == null)
//			throw new Exception();
//
//		IArcObject Right;
//
//		if (value.First == null)
//			throw new Exception();
//
//		if (value.Count == 1 && comp.TryGetVariable(value.First.Value, out IValue? NewValue))
//		{
//			if (NewValue == null)
//				throw new Exception();
//			Right = NewValue.AsObject();
//		}
//		else
//			Right = IArcObject.Construct(value, null).AsObject();
//
//		StringBuilder loc = new();
//		StringBuilder def = new();
//		StringBuilder imp = new();
//		StringBuilder sea = new();
//		StringBuilder lake = new();
//		loc.Append("l_english:\n");
//		foreach (KeyValuePair<string, IValue> province in Right.Properties)
//		{
//			IArcObject ArcObject = province.Value.AsObject();
//			int id = ArcObject.Properties["id"].AsInt().Value;
//			string name = ArcObject.Properties["name"].AsString().Value;
//			Block color = ArcObject.Properties["color"].AsBlock().Value;
//			Block history = ArcObject.Properties["history"].AsBlock().Value;
//			loc.Append($" PROV{id}: \"{name.Trim('"')}\"\n");
//			def.Append($"{id};{string.Join(';', color)};;x\n");
//
//			if (ArcObject.Properties.ContainsKey("impassible"))
//			{
//				bool impassible = ArcObject.Properties["impassible"].AsBool().Value;
//
//				if (impassible)
//				{
//					imp.Append($"{id} ");
//				}
//			}
//			if (ArcObject.Properties.ContainsKey("sea"))
//			{
//				bool issea = ArcObject.Properties["sea"].AsBool().Value;
//
//				if (issea)
//				{
//					sea.Append($"{id} ");
//				}
//			}
//			if (ArcObject.Properties.ContainsKey("lake"))
//			{
//				bool islake = ArcObject.Properties["lake"].AsBool().Value;
//
//				if (islake)
//				{
//					lake.Append($"{id} ");
//				}
//			}
//
//			string result = "";
//			if (history.Count > 0) result = comp.Compile(history);
//
//			File.WriteAllText($"{directory}/target/history/provinces/{id} - ARC.txt", result);
//		}
//
//		File.WriteAllText(directory + "/target/localisation/replace/es_provinces_l_english.yml", Parser.ConvertStringToUtf8Bom(loc.ToString()));
//		File.WriteAllText(directory + "/target/map/definition.csv", def.ToString());
//		File.WriteAllText(directory + "/target/map/climate.txt", $@"
//tropical = {{
//    
//}}
//
//arid = {{
//    
//}}
//
//arctic = {{
//	
//}}
//
//mild_winter = {{
//    
//}}
//
//
//normal_winter = {{
//    
//}}
//
//severe_winter = {{
//    
//}}
//
//impassable = {{
//	{imp}
//}}
//
//mild_monsoon = {{
//    
//}}
//
//normal_monsoon = {{
//    
//}}
//
//severe_monsoon = {{
//    
//}}
//
//equator_y_on_province_image = 224");
//		File.WriteAllText(directory + "/target/map/default.map", $@"
//width = 4096
//height = 2816
//
//max_provinces = {Right.Properties.Count + 1}
//sea_starts = {{ 
//	{sea}
//}}
//
//only_used_for_random = {{
//}}
//
//lakes = {{
//	{lake}
//}}
//
//force_coastal = {{
//}}
//
//definitions = ""definition.csv""
//provinces = ""provinces.bmp""
//positions = ""positions.txt""
//terrain = ""terrain.bmp""
//rivers = ""rivers.bmp""
//terrain_definition = ""terrain.txt""
//heightmap = ""heightmap.bmp""
//tree_definition = ""trees.bmp""
//continent = ""continent.txt""
//adjacencies = ""adjacencies.csv""
//climate = ""climate.txt""
//region = ""region.txt""
//superregion = ""superregion.txt""
//area = ""area.txt""
//provincegroup = ""provincegroup.txt""
//ambient_object = ""ambient_object.txt""
//seasons = ""seasons.txt""
//trade_winds = ""trade_winds.txt""
//
//# Define which indices in trees.bmp palette which should count as trees for automatic terrain assignment
//tree = {{ 3 4 7 10 }}
//");
//
//		return i;
//	}
//}
