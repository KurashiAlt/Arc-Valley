using Arc;
using System;
using System.Diagnostics;
using System.Numerics;

public class ArgList : IArcObject, IArcNumber
{
    public static Dictionary<string, LinkedList<IVariable>> lists = new();
    public LinkedList<IVariable> list;
    public static void Add(string list, IVariable v)
    {
        lists[list].AddFirst(v);
    }
    public static void Drop(string list)
    {
        lists[list].RemoveFirst();
    }
    public ArgList(string listName, LinkedList<IVariable> list)
    {
        this.list = list;
        lists.Add(listName, list);
    }
    public bool CanGet(string indexer)
    {
        if (list.Count == 0) return false;
        IVariable arg = list.First();
        if (arg is IArcObject @object) return @object.CanGet(indexer);
        if (arg is IVariable bc)
        {
            if(bc is IArcObject @object2)
            {
                return @object2.CanGet(indexer);
            }
        }
        throw ArcException.Create(indexer, arg, "args isn't of type [Arc Object]");
    }
    public IVariable? Get(string indexer)
    {
        IVariable arg = list.First();
        if (arg is IArcObject @object) return @object.Get(indexer);
        if (arg is IVariable bc)
        {
            if (bc is IArcObject @object2)
            {
                return @object2.Get(indexer);
            }
        }
        throw ArcException.Create(indexer, arg, "args isn't of type [Arc Object]");
    }
    public IVariable Get() => list.First();
    public bool LogicalCall(ref Walker i) => list.First().LogicalCall(ref i);
    public Walker Call(Walker w, ref Block result)
    {
        IVariable arg = list.First();
        if (arg is IVariable @object) return @object.Call(w, ref result);
        if (arg is IArcObject @objec) return @objec.Call(w, ref result);
        throw ArcException.Create(w, result, arg, "args isn't of type [Arc Single]");
    }
    public override string ToString()
    {
        return list.First().ToString() ?? "";
    }
    public double GetNum()
    {
        IVariable arg = list.First();
        if (arg is IVariable @object) return ((IArcNumber)@object).GetNum();
        throw ArcException.Create(arg, "args isn't of type [IArcNumber]");
    }
}
public class ArcType : IValue
{
    public static Dictionary<string, ArcType> Types = new Dictionary<string, ArcType>()
    {
        { "effect", new(ArcEffect.NamelessConstructor) },
        { "modifier", new(ArcModifier.NamelessConstructor) },
        { "trigger", new(ArcTrigger.NamelessConstructor) },
        { "block", new(ArcCode.NamelessConstructor) },
        { "named_effect", new(ArcEffect.Constructor) },
        { "named_modifier", new(ArcModifier.Constructor) },
        { "named_trigger", new(ArcTrigger.Constructor) },
        { "named_block", new(ArcCode.Constructor) },
        { "bool", new(ArcBool.Constructor) },
        { "string", new(ArcString.Constructor) },
        { "float", new(ArcFloat.Constructor) },
        { "int", new(ArcInt.Constructor) },
        { "text", new((string s) => {
            ArcString c = new(s);
            c.Value = $"{s}";
            return c;
        })},
        { "base_scope", new((Block b) => {
            Word tag = b.ToWord();
            if (Compiler.IsBaseScope(tag) || tag.StartsWith("event_target")) return ArcString.Constructor(b);
            throw ArcException.Create(b, $"{tag} is not a base_scope");
        }) },
        { "country_scope", new((Block b) => {
            Word tag = b.ToWord();
            if (Compiler.IsDefaultScope(tag) || Compiler.IsBaseScope(tag) || tag.StartsWith("event_target") || tag == "emperor") return ArcString.Constructor(b);
            return Country.Countries.Get(tag) ?? throw ArcException.Create(b);
        })},
        { "province_scope", new((Block b) => {
            Word tag = b.ToWord();
            if (Compiler.IsDefaultScope(tag) || Compiler.IsBaseScope(tag) || tag.StartsWith("event_target") || tag == "emperor") return ArcString.Constructor(b);
            return Province.Provinces.Get(tag) ?? throw ArcException.Create(b);
        })},
        { "unknown", new(Compiler.global.Get) },
        { "idea_list", new((Block s) => new ArcList<Idea>(s, (Block s, int num) => Idea.Constructor(s, num + 1))) },
        { "province", new(Province.Provinces.Get) },
        { "area", new(Area.Areas.Get) },
        { "region", new(Region.Regions.Get) },
        { "superregion", new(Superregion.Superregions.Get) },
        { "terrain", new(Terrain.Terrains.Get) },
        { "blessing", new(Blessing.Blessings.Get) },
        { "church_aspect", new(ChurchAspect.ChurchAspects.Get) },
        { "personality_trait", new(RulerPersonality.RulerPersonalities.Get) },
        { "country", new(Country.Countries.Get) },
        { "building", new(Building.Buildings.Get) },
        { "bookmark", new(Bookmark.Bookmarks.Get) },
        { "religion", new(Religion.Religions.Get) },
        { "religious_group", new(ReligionGroup.ReligionGroups.Get) },
        { "personal_deity", new(PersonalDeity.PersonalDeitys.Get) },
        { "tradenode", new(TradeNode.TradeNodes.Get) },
        { "static_modifier", new(StaticModifier.StaticModifiers.Get) },
        { "opinion_modifier", new(OpinionModifier.OpinionModifiers.Get) },
        { "relation", new(Relation.Relations.Get) },
        { "culture_group", new(CultureGroup.CultureGroups.Get) },
        { "culture", new(Culture.Cultures.Get) },
        { "mission", new(Mission.Missions.Get) },
        { "mission_series", new(MissionSeries.MissionSerieses.Get) },
        { "agenda", new(EstateAgenda.EstateAgendas.Get) },
        { "privilege", new(EstatePrivilege.EstatePrivileges.Get) },
        { "policy", new(Policy.Policies.Get) },
        { "estate", new(Estate.Estates.Get) },
        { "government", new(Government.Governments.Get) },
        { "government_names", new(GovernmentNames.GovernmentNameDict.Get) },
        { "government_reform", new(GovernmentReform.GovernmentReforms.Get) },
        { "event", new(Event.Events.Get) },
        { "incident", new(Incident.Incidents.Get) },
        { "decision", new(Decision.Decisions.Get) },
        { "building_line", new(BuildingLine.BuildingLines.Get) },
        { "government_mechanic", new(GovernmentMechanic.GovernmentMechanics.Get) },
        { "diplomatic_action", new(DiplomaticAction.DiplomaticActions.Get) },
        { "holy_order", new(HolyOrder.HolyOrders.Get) },
        { "casus_belli", new(CasusBelli.CasusBellies.Get) },
        { "province_triggered_modifier", new(ProvinceTriggeredModifier.ProvinceTriggeredModifiers.Get) },
        { "war_goal", new(WarGoal.WarGoals.Get) },
        { "province_group", new(ProvinceGroup.ProvinceGroups.Get) },
        { "subject_type", new(SubjectType.SubjectTypes.Get) },
        { "estate_privilege", new((Block b) => {
            string id = Compiler.GetId(b.ToString());
            string[] parts = id.Split(':');
            Estate est = Estate.Estates[parts[0]];
            if (est.Privileges.dict == null) throw new Exception();
            EstatePrivilege pr = est.Privileges.dict[parts[1]];
            return pr;
        }) },
        { "country_event", new((Block b) => {
            string id = Compiler.GetId(b.ToString());
            Event c = Event.Events[id];
            if (c.ProvinceEvent.Value) throw ArcException.Create(b, id, $"{id} is not a country_event");
            return c;
        }) },
        { "province_event", new((Block b) => {
            string id = Compiler.GetId(b.ToString());
            Event c = Event.Events[id];
            if (!c.ProvinceEvent.Value) throw ArcException.Create(b, id, $"{id} is not a country_event");
            return c;
        }) },
        { "ui_node", new((Block b) => {
            string id = Compiler.GetId(b.ToString());
            InterfaceNode c = Compiler.GetVariable<InterfaceNode>(new(id, b.ToWord()));
            return c;
        }) }
    };
    public Func<Block, IVariable> InstanceConstructor { get; set; }
    public bool Nullable { get; set; }


#pragma warning disable CS8618
    public ArcType() { } // Required for types that inherit from this type.
#pragma warning restore CS8618

    public ArcType(Func<string, IVariable?> get, bool nullable = false)
    {
        Nullable = nullable;
        InstanceConstructor = (Block b) => get(b.ToWord());
    }
    public ArcType(Func<Block, IVariable> constructor, bool nullable = false)
    {
        Nullable = nullable;
        InstanceConstructor = constructor;
    }
    public static ArcType Constructor(Block b)
    {
        if(b.Count != 1)
        {
            Args args = Args.GetArgs(b);
            return new ArcStruct(null, args);
        }

        if(b.First == null) throw new Exception();
        string key = b.First.Value;

        bool nullable = false;
        if (key.EndsWith('?'))
        {
            key = key[..^1];
            nullable = true;
        }

        if(key.StartsWith("list<") && key.EndsWith(">"))
        {
            string t = key[5..^1];

            return new((Block b) =>
            {
                ArcType sub = Regulat(t, false);

                return ArcList<IVariable>.GetConstructor(sub.InstanceConstructor, va: false)(b);
            }, nullable);
        }
        else if (key.StartsWith("dict<") && key.EndsWith(">"))
        {
            string t = key[5..^1];

            return new((Block b) =>
            {
                ArcClass sub = ArcClass.Classes[t];

                return Dict<IVariable>.Constructor(sub)(b);
            }, nullable);
        }
        else
        {
            return Regulat(key, nullable);
        }
    }
    public static ArcType Regulat(string key, bool nullable)
    {
        ArcType c = Types[key].CreateCopy();
        c.Nullable = nullable;
        return c;
    }

    public void Set(Block b) => throw new NotImplementedException();
    public ArcType CreateCopy()
    {
        return new ArcType(InstanceConstructor, Nullable);
    }
}
public enum CompileType
{
    Effect,
    Trigger,
    Modifier,
    Block,
    Interface
}
public class CommandCall : IVariable
{
    public NewCommand Command;
    public ArcObject This;
    public CommandCall(NewCommand command, ArcObject @this)
    {
        Command = command;
        This = @this;
    }
    public Walker Call(Walker i, ref Block result)
    {
        i = Args.GetArgs(i, out Args args);
        ArgList.Add("this", This);
        Command.Call(args, ref result);
        ArgList.Drop("this");
        return i;
    }
}
public class NewCommand : ArcObject
{
    static IVariable QFromArgs<T>(Args args, T b) where T : ArcObject
    {
        if (args.block == null) throw new Exception();
        try
        {
            if (Compiler.TryGetVariable(args.block.ToWord(), out IVariable? var))
            {
                if (var == null) throw new Exception();
                return var;
            }
        }
        catch (Exception) { }
        ArcType typ = b.Get<ArcType>("args");
        try
        {
            return typ.InstanceConstructor(args.block);
        }
        catch (Exception e)
        {
            throw ArcException.Create(args, b, e);
        }
    }
    public void Call(Args args, ref Block result)
    {
        args.Inherit(Get<ArgsObject>("default").args);

        IVariable a;
        if (Get("args") is Dict<ArcType>) a = FromArgs(args, this);
        else a = QFromArgs(args, this);

        ArgList.Add("args", a);
        Compiler.CompileRightAway++;

        Get<ArcBlock>("transpile").Compile(ref result);

        Compiler.CompileRightAway--;
        ArgList.Drop("args");
    }
    public override Walker Call(Walker i, ref Block result)
    {
        i = Args.GetArgs(i, out Args args);
        Call(args, ref result);
        return i;
    }
    public CompileType CommandType;
    public NewCommand(string id, Args args, CompileType commandType)
    {
        ArgsObject def = args.Get(ArgsObject.Constructor, "default", new(new()));
        ArcType type = args.Get(ArcType.Constructor, "args");
        Add("args", type);
        Add("default", def);

        Block? block = args.GetNullable("transpile");
        ArcBlock transpile = commandType switch
        {
            CompileType.Effect => new ArcEffect(),
            CompileType.Trigger => new ArcTrigger(),
            CompileType.Modifier => new ArcModifier(),
            CompileType.Block => new ArcCode(),
            _ => throw ArcException.Create(id, args, commandType, type),
        };
        transpile.ShouldBeCompiled = false;

        if (block != null)
        {
            transpile.Value = block;
        }
        else {
            Word FirstWord = args.block.First.Value;
            Block cArgs = args.Get("args");
            if (cArgs.Count == 1)
            {
                transpile.Value.Add(new Word($"`{id}`", FirstWord));
                transpile.Value.Add(new Word("=", FirstWord));
                transpile.Value.Add(new Word("args", FirstWord));
            }
            else
            {
                transpile.Value.Add(new Word($"`{id}`", FirstWord));
                transpile.Value.Add(new Word("=", FirstWord));
                transpile.Value.Add(new Word("{", FirstWord));
                Args nArgs = Args.GetArgs(args.Get("args"));
                foreach (KeyValuePair<string, Block> v in nArgs.keyValuePairs ?? throw ArcException.Create(id, args, FirstWord, cArgs))
                {
                    transpile.Value.Add(new Word("when", FirstWord));
                    transpile.Value.Add(new Word($"[exists = args:{v.Key}]", FirstWord));

                    switch (v.Value.ToString())
                    {
                        case "effect":
                        case "trigger":
                        case "modifier":
                            transpile.Value.Add(new Word("{", FirstWord));
                            transpile.Value.Add(new Word($"`{v.Key} =`", FirstWord));
                            transpile.Value.Add(new Word("{", FirstWord));
                            transpile.Value.Add(new Word($"args:{v.Key}", FirstWord));
                            transpile.Value.Add(new Word("}", FirstWord));
                            transpile.Value.Add(new Word("}", FirstWord));
                            break;
                        default:
                            transpile.Value.Add(new Word($"`{v.Key} = {{args:{v.Key}}}`", FirstWord));
                            break;
                    }
                }
                transpile.Value.Add(new Word("}", FirstWord));
            }
        }

        Add("transpile", transpile);
        CommandType = commandType;
    }
    public static Walker CallEffect(Walker i) => Call(i, ConstructorEffect);
    public static Walker CallTrigger(Walker i) => Call(i, ConstructorTrigger);
    public static Walker CallModifier(Walker i) => Call(i, ConstructorModifier);
    public static NewCommand Constructor(string id, Args args, CompileType commandType, ref List<(string, NewCommand)> list)
    {
        NewCommand command;
        command = new(id, args, commandType);
        list.Add((id, command));
        return command;
    }
    public static NewCommand ConstructorEffect(string id, Args args) => Constructor(id, args, CompileType.Effect, ref Compiler.NewEffects);
    public static NewCommand ConstructorTrigger(string id, Args args) => Constructor(id, args, CompileType.Trigger, ref Compiler.NewTriggers);
    public static NewCommand ConstructorModifier(string id, Args args) => Constructor(id, args, CompileType.Modifier, ref Compiler.NewModifiers);
}