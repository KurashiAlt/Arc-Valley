using Arc;
using ArcInstance;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
        throw new Exception("args isn't of type [Arc Object]");
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
        throw new Exception("args isn't of type [Arc Object]");
    }
    public Walker Call(Walker w, ref Block result)
    {
        Arg arg = list.First();
        if (arg is vx @object) return @object.va.Value.Call(w, ref result);
        if (arg is IArcObject @objec) return @objec.Call(w, ref result);
        throw new Exception("args isn't of type [Arc Single]");
    }
    public double GetNum()
    {
        Arg arg = list.First();
        if (arg is vx @object) return ((IArcNumber)@object.va.Value).GetNum();
        throw new Exception("args isn't of type [IArcNumber]");
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
        Type typ = b.Get<Type>("args");
        return new vx(typ.ThisConstructor(args.block));
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
            return Type.Constructor(b);
        }
        else
        {
            return Dict<Type>.Constructor(Type.Constructor)(b);
        }
    }
}
public class Type : IValue, vvC
{
    public void Set(Block b) => throw new NotImplementedException();
    public Func<Block, IVariable> ThisConstructor { get; set; }
    public bool Nullable { get; set; }
    public Type(
        Func<string, IVariable?> get,
        bool nullable
    ) {
        Nullable = nullable;
        ThisConstructor = (Block b) => get(string.Join(' ', b));
    }
    public Type(
        Func<Block, IVariable> constructor,
        bool nullable
    ) {
        Nullable = nullable;
        ThisConstructor = constructor;
    }
    public static Type Constructor(Block b)
    {
        if(b.Count != 1) throw new NotImplementedException();
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
                Type sub = Regulat(t, false);

                return ArcList<IVariable>.GetConstructor(sub.ThisConstructor)(b);
            }, nullable);
        }
        else
        {
            return Regulat(key, nullable);
        }
    }
    public static Type Regulat(string key, bool nullable)
    {
        Type ConstC(Func<Block, IVariable> constructor) => new(constructor, nullable);
        Type ConstD(Func<string, IVariable?> get) => new(get, nullable);
        return key switch
        {
            "effect" => ConstC(ArcEffect.Constructor),
            "modifier" => ConstC(ArcModifier.Constructor),
            "trigger" => ConstC(ArcTrigger.Constructor),
            "block" => ConstC(ArcCode.Constructor),
            "bool" => ConstC(ArcBool.Constructor),
            "string" => ConstC(ArcString.Constructor),
            "text" => ConstD((string s) => {
                ArcString c = new ArcString(s);
                c.Value = $"{s}";
                return c;
            }),
            "float" => ConstC(ArcFloat.Constructor),
            "int" => ConstC(ArcInt.Constructor),

            "base_scope" => ConstC((Block b) =>
            {
                string tag = string.Join(' ', b);
                if (Compiler.IsBaseScope(tag) || tag.StartsWith("event_target")) return ArcString.Constructor(b);
                throw new Exception($"{tag} is not a base_scope");
            }),
            "country_scope" => ConstC((Block b) =>
            {
                string tag = string.Join(' ', b);
                if (Compiler.IsDefaultScope(tag) || Compiler.IsBaseScope(tag) || tag.StartsWith("event_target") || tag == "emperor") return ArcString.Constructor(b);
                return Country.Countries.Get(tag);
            }),

            "province" => ConstD(Province.Provinces.Get),
            "area" => ConstD(Area.Areas.Get),
            "region" => ConstD(Region.Regions.Get),
            "superregion" => ConstD(Superregion.Superregions.Get),
            "tradegood" => ConstC((Block b) =>
            {
                string tag = string.Join(' ', b);
                if (tag == "unknown") return ArcString.Constructor(b);
                return TradeGood.TradeGoods.Get(tag);
            }),
            "terrain" => ConstD(Terrain.Terrains.Get),
            "blessing" => ConstD(Blessing.Blessings.Get),
            "church_aspect" => ConstD(ChurchAspect.ChurchAspects.Get),
            "inheritable" => throw new NotImplementedException(),
            "country" => ConstD(Country.Countries.Get),
            "adjacency" => ConstD(Adjacency.Adjacencies.Get),
            "building" => ConstD(Building.Buildings.Get),
            "bookmark" => ConstD(Bookmark.Bookmarks.Get),
            "religion" => ConstD(Religion.Religions.Get),
            "religious_group" => ConstD(ReligionGroup.ReligionGroups.Get),
            "personal_deity" => ConstD(PersonalDeity.PersonalDeitys.Get),
            "advisor_type" => ConstD(AdvisorType.AdvisorTypes.Get),
            "tradenode" => ConstD(TradeNode.TradeNodes.Get),
            "idea_group" => ConstD(IdeaGroup.IdeaGroups.Get),
            "event_modifier" => ConstD(EventModifier.EventModifiers.Get),
            "opinion_modifier" => ConstD(OpinionModifier.OpinionModifiers.Get),
            "relation" => ConstD(Relation.Relations.Get),
            "culture_group" => ConstD(CultureGroup.CultureGroups.Get),
            "culture" => ConstD(Culture.Cultures.Get),
            "mission" => ConstD(Mission.Missions.Get),
            "mission_series" => ConstD(MissionSeries.MissionSerieses.Get),
            "agenda" => ConstD(EstateAgenda.EstateAgendas.Get),
            "privilege" => ConstD(EstatePrivilege.EstatePrivileges.Get),
            "estate" => ConstD(Estate.Estates.Get),
            "estate_privilege" => ConstC((Block b) =>
            {
                string id = string.Join(' ', b);
                string[] parts = id.Split(':');
                Estate est = Estate.Estates[parts[0]];
                EstatePrivilege pr = est.Privileges.dict[parts[1]];
                return pr;
            }),
            "government" => ConstD(Government.Governments.Get),
            "government_names" => ConstD(GovernmentNames.GovernmentNameDict.Get),
            "government_reform" => ConstD(GovernmentReform.GovernmentReforms.Get),
            "event" => ConstD(Event.Events.Get),
            "country_event" => ConstC((Block b) =>
            {
                string id = string.Join(' ', b);
                Event c = (Event)Event.Events.Get(id);
                if (c.ProvinceEvent.Value) throw new Exception($"{id} is not a country_event");
                return c;
            }),
            "province_event" => ConstC((Block b) =>
            {
                string id = string.Join(' ', b);
                Event c = (Event)Event.Events.Get(id);
                if (!c.ProvinceEvent.Value) throw new Exception($"{id} is not a province_event");
                return c;
            }),
            "incident" => ConstD(Incident.Incidents.Get),
            "unit" => ConstD(Unit.Units.Get),
            "great_project" => ConstD(GreatProject.GreatProjects.Get),
            "localisation" => throw new NotImplementedException(),
            "mercenary_company" => ConstD(MercenaryCompany.Companies.Get),
            "advisor" => ConstD(Advisor.Advisors.Get),
            "age" => ConstD(Age.Ages.Get),
            "decision" => ConstD(Decision.Decisions.Get),
            "building_line" => ConstD(BuildingLine.BuildingLines.Get),
            "government_mechanic" => ConstD(GovernmentMechanic.GovernmentMechanics.Get),
            "diplomatic_action" => ConstD(DiplomaticAction.DiplomaticActions.Get),
            "holy_order" => ConstD(HolyOrder.HolyOrders.Get),
            "casus_belli" => ConstD(CasusBelli.CasusBellies.Get),
            "province_triggered_modifier" => ConstD(ProvinceTriggeredModifier.ProvinceTriggeredModifiers.Get),
            "war_goal" => ConstD(WarGoal.WarGoals.Get),
            "expedition" => ConstD(Expedition.Expeditions.Get),
            "province_group" => ConstD(ProvinceGroup.ProvinceGroups.Get),
            _ => throw new NotImplementedException($"Unknown type {key}")
        };
    }
}
public class NewEffect : ArcObject
{
    public NewEffect(string id, Args args)
    {
        Add("args", args.Get(vvC.Constructor, "args"));

        ArcEffect? transpile = args.Get(ArcEffect.Constructor, "transpile", null);
        if (transpile != null) Add("transpile", transpile);
        Compiler.NewEffects.Add((id, this));
    }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static NewEffect Constructor(string id, Args args) => new(id, args);
}
public class NewTrigger : ArcObject
{
    public NewTrigger(string id, Args args)
    {
        Add("args", args.Get(vvC.Constructor, "args"));

        ArcTrigger? transpile = args.Get(ArcTrigger.Constructor, "transpile", null);
        if (transpile != null) Add("transpile", transpile);
        Compiler.NewTriggers.Add((id, this));
    }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static NewTrigger Constructor(string id, Args args) => new(id, args);
}
public class NewModifier : ArcObject
{
    public NewModifier(string id, Args args)
    {
        Add("args", args.Get(vvC.Constructor, "args"));

        ArcModifier? transpile = args.Get(ArcModifier.Constructor, "transpile", null);

        if(transpile != null) Add("transpile", transpile);
        Compiler.NewModifiers.Add((id, this));
    }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static NewTrigger Constructor(string id, Args args) => new(id, args);
}