using Arc;

public class ArgList : IArcObject, IArcNumber
{
    public static LinkedList<Arg> list = new LinkedList<Arg>();
    public bool CanGet(string indexer)
    {
        Arg arg = list.First();
        if (arg is IArcObject @object) return @object.CanGet(indexer);
        if (arg is vx bc)
        {
            if(bc.va.Value is IArcObject @object2)
            {
                return @object2.CanGet(indexer);
            }
        }
        throw ArcException.Create(indexer, arg, "args isn't of type [Arc Object]");
    }
    public IVariable? Get(string indexer)
    {
        Arg arg = list.First();
        if (arg is IArcObject @object) return @object.Get(indexer);
        if (arg is vx bc)
        {
            if (bc.va.Value is IArcObject @object2)
            {
                return @object2.Get(indexer);
            }
        }
        throw ArcException.Create(indexer, arg, "args isn't of type [Arc Object]");
    }
    public Walker Call(Walker w, ref Block result)
    {
        Arg arg = list.First();
        if (arg is vx @object) return @object.va.Value.Call(w, ref result);
        if (arg is IArcObject @objec) return @objec.Call(w, ref result);
        throw ArcException.Create(w, result, arg, "args isn't of type [Arc Single]");
    }
    public override string ToString()
    {
        return list.First().ToString();
    }
    public double GetNum()
    {
        Arg arg = list.First();
        if (arg is vx @object) return ((IArcNumber)@object.va.Value).GetNum();
        throw ArcException.Create(arg, "args isn't of type [IArcNumber]");
    }
}
public interface Arg 
{

}
public class vx : Arg
{
    public DictPointer<IVariable> va { get; set; }
    public static Arg FromArgs<T>(Args args, T b) where T : ArcObject
    {
        if (args.block == null) throw new Exception();
        try
        {
            if(Compiler.TryGetVariable(string.Join(' ', args.block), out IVariable? var))
            {
                if (var == null) throw new Exception();
                return new vx(var);
            }
        }
        catch(Exception) { }
        ArcType typ = b.Get<ArcType>("args");
        return new vx(typ.ThisConstructor(args.block));
    }
    public override string ToString()
    {
        return va.Value.ToString();
    }
    public vx(IVariable v)
    {
        va = new(v);
    }
}
public interface vvC : IVariable
{
    public static vvC Constructor(Block b)
    {
        if (b.Count == 1)
        {
            return ArcType.Constructor(b);
        }
        else
        {
            return Dict<ArcType>.Constructor(ArcType.Constructor)(b);
        }
    }
}
public class ArcType : IValue, vvC
{
    public static Dictionary<string, ArcType> Types = new Dictionary<string, ArcType>()
    {
        { "effect", new(ArcEffect.Constructor) },
        { "modifier", new(ArcModifier.Constructor) },
        { "trigger", new(ArcTrigger.Constructor) },
        { "block", new(ArcCode.Constructor) },
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
            Word tag = b.toWord();
            if (Compiler.IsBaseScope(tag) || tag.StartsWith("event_target")) return ArcString.Constructor(b);
            throw ArcException.Create(b, $"{tag} is not a base_scope");
        }) },
        { "country_scope", new((Block b) => {
            Word tag = b.toWord();
            if (Compiler.IsDefaultScope(tag) || Compiler.IsBaseScope(tag) || tag.StartsWith("event_target") || tag == "emperor") return ArcString.Constructor(b);
            return Country.Countries.Get(tag) ?? throw ArcException.Create(b);
        })},
        { "province_scope", new((Block b) => {
            Word tag = b.toWord();
            if (Compiler.IsDefaultScope(tag) || Compiler.IsBaseScope(tag) || tag.StartsWith("event_target") || tag == "emperor") return ArcString.Constructor(b);
            return Province.Provinces.Get(tag) ?? throw ArcException.Create(b);
        })},
        { "province", new(Province.Provinces.Get) },
        { "area", new(Area.Areas.Get) },
        { "region", new(Region.Regions.Get) },
        { "superregion", new(Superregion.Superregions.Get) },
        { "tradegood", new(TradeGood.TradeGoods.Get) },
        { "terrain", new(Terrain.Terrains.Get) },
        { "blessing", new(Blessing.Blessings.Get) },
        { "church_aspect", new(ChurchAspect.ChurchAspects.Get) },
        { "country", new(Country.Countries.Get) },
        { "adjacency", new(Adjacency.Adjacencies.Get) },
        { "building", new(Building.Buildings.Get) },
        { "bookmark", new(Bookmark.Bookmarks.Get) },
        { "religion", new(Religion.Religions.Get) },
        { "religious_group", new(ReligionGroup.ReligionGroups.Get) },
        { "personal_deity", new(PersonalDeity.PersonalDeitys.Get) },
        { "advisor_type", new(AdvisorType.AdvisorTypes.Get) },
        { "tradenode", new(TradeNode.TradeNodes.Get) },
        { "idea_group", new(IdeaGroup.IdeaGroups.Get) },
        { "static_modifier", new(StaticModifier.StaticModifiers.Get) },
        { "event_modifier", new(EventModifier.EventModifiers.Get) },
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
        { "unit", new(Unit.Units.Get) },
        { "great_project", new(GreatProject.GreatProjects.Get) },
        { "mercenary_company", new(MercenaryCompany.Companies.Get) },
        { "advisor", new(Advisor.Advisors.Get) },
        { "age", new(Age.Ages.Get) },
        { "decision", new(Decision.Decisions.Get) },
        { "building_line", new(BuildingLine.BuildingLines.Get) },
        { "government_mechanic", new(GovernmentMechanic.GovernmentMechanics.Get) },
        { "diplomatic_action", new(DiplomaticAction.DiplomaticActions.Get) },
        { "holy_order", new(HolyOrder.HolyOrders.Get) },
        { "casus_belli", new(CasusBelli.CasusBellies.Get) },
        { "province_triggered_modifier", new(ProvinceTriggeredModifier.ProvinceTriggeredModifiers.Get) },
        { "war_goal", new(WarGoal.WarGoals.Get) },
        { "expedition", new(Expedition.Expeditions.Get) },
        { "province_group", new(ProvinceGroup.ProvinceGroups.Get) },
        { "subject_type", new(SubjectType.SubjectTypes.Get) },
        { "estate_privilege", new((Block b) => {
            string id = Compiler.GetId(b.ToString());
            string[] parts = id.Split(':');
            Estate est = Estate.Estates[parts[0]];
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
    };
    public void Set(Block b) => throw new NotImplementedException();
    public Func<Block, IVariable> ThisConstructor { get; set; }
    public bool Nullable { get; set; }
    public ArcType CreateCopy()
    {
        return new ArcType(ThisConstructor, Nullable);
    }
    public ArcType(Func<string, IVariable?> get, bool nullable = false)
    {
        Nullable = nullable;
        ThisConstructor = (Block b) => get(string.Join(' ', b));
    }
    public ArcType(Func<Block, IVariable> constructor, bool nullable = false)
    {
        Nullable = nullable;
        ThisConstructor = constructor;
    }
    public static ArcType Constructor(Block b)
    {
        if(b.Count != 1)
        {
            Args args = Args.GetArgs(b);
            Dictionary<string, ArcType> Structure = new();

            foreach (KeyValuePair<string, Block> pair in args.keyValuePairs ?? throw ArcException.Create(b))
            {
                Structure.Add(pair.Key, Constructor(pair.Value));
            }

            ArcType Struct = new((Block c) => {
                if (!Parser.HasEnclosingBrackets(c))
                {
                    c.AddFirst(new Word("{", c.First.Value));
                    c.AddLast(new Word("}", c.Last.Value));
                }
                Args nArgs = Args.GetArgs(c);
                ArcObject obj = new();
                foreach (KeyValuePair<string, ArcType> pair in Structure)
                {
                    try
                    {
                        obj.Add(pair.Key, nArgs.Get(pair.Value.ThisConstructor, pair.Key));
                    }
                    catch
                    {
                        if (!pair.Value.Nullable) throw;
                    }
                }
                return obj;
            });

            return Struct;
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

                return ArcList<IVariable>.GetConstructor(sub.ThisConstructor)(b);
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
}
public class NewEffect : ArcObject
{
    public NewEffect(string id, Args args)
    {
        ArcType type = args.Get(ArcType.Constructor, "args");
        Add("args", type);

        ArcEffect? transpile = args.Get(ArcEffect.Constructor, "transpile", null);

        if (transpile == null)
        {
            Word FirstWord = args.block.First.Value;
            transpile = new();
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
        Compiler.NewEffects.Add((id, this));
    }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static NewEffect Constructor(string id, Args args) => new(id, args);
}
public class NewTrigger : ArcObject
{
    public NewTrigger(string id, Args args)
    {
        Add("args", args.Get(ArcType.Constructor, "args"));

        ArcTrigger? transpile = args.Get(ArcTrigger.Constructor, "transpile", null);

        if (transpile == null)
        {
            Word FirstWord = args.block.First.Value;
            transpile = new();
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
        Compiler.NewTriggers.Add((id, this));
    }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static NewTrigger Constructor(string id, Args args) => new(id, args);
}
public class NewModifier : ArcObject
{
    public NewModifier(string id, Args args)
    {
        Add("args", args.Get(ArcType.Constructor, "args"));

        ArcModifier? transpile = args.Get(ArcModifier.Constructor, "transpile", null);

        if (transpile == null)
        {
            Word FirstWord = args.block.First.Value;
            transpile = new();
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
        Compiler.NewModifiers.Add((id, this));
    }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static NewTrigger Constructor(string id, Args args) => new(id, args);
}