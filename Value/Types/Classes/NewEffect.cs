using Arc;
using ArcInstance;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
public class ArgList : IArcObject, IArcNumber
{
    public LinkedList<Arg> list = new LinkedList<Arg>();
    public bool CanGet(string indexer)
    {
        Arg arg = list.First();
        if (arg is IArcObject @object) return @object.CanGet(indexer);
        throw new Exception("args isn't of type [Arc Object]");
    }
    public IVariable? Get(string indexer)
    {
        Arg arg = list.First();
        if (arg is IArcObject @object) return @object.Get(indexer);
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
        Type typ = b.Get<Type>("args");
        if (args.block == null) throw new Exception();
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
    public Type(
        Func<string, IVariable?> get
    ) {
        ThisConstructor = (Block b) => get(string.Join(' ', b));
    }
    public Type(
        Func<Block, IVariable> constructor
    ) {
        ThisConstructor = constructor;
    }
    public static Type Constructor(Block b)
    {
        if(b.Count != 1) throw new NotImplementedException();
        string key = b.First.Value;

        return key switch
        {
            "effect" => new(ArcEffect.Constructor),
            "modifier" => new(ArcModifier.Constructor),
            "trigger" => new(ArcTrigger.Constructor),
            "bool" => new(ArcBool.Constructor),
            "string" => new(ArcString.Constructor),
            "float" => new(ArcFloat.Constructor),
            "int" => new(ArcInt.Constructor),

            "base_scope" => new((Block b) =>
            {
                string tag = string.Join(' ', b);
                if (Compiler.IsBaseScope(tag)) return ArcString.Constructor(b);
                throw new Exception($"{tag} is not a base_scope");
            }),
            "country_scope" => new((Block b) =>
            {
                string tag = string.Join(' ', b);
                if (Compiler.IsDefaultScope(tag) || Compiler.IsBaseScope(tag)) return ArcString.Constructor(b);
                return Country.Countries.Get(tag);
            }),

            "province" => new(Province.Provinces.Get),
            "area" => new(Area.Areas.Get),
            "region" => new(Region.Regions.Get),
            "superregion" => new(Superregion.Superregions.Get),
            "tradegood" => new(TradeGood.TradeGoods.Get),
            "terrain" => new(Terrain.Terrains.Get),
            "blessing" => new(Blessing.Blessings.Get),
            "church_aspect" => new(ChurchAspect.ChurchAspects.Get),
            "inheritable" => throw new NotImplementedException(),
            "country" => new(Country.Countries.Get),
            "adjacency" => new(Adjacency.Adjacencies.Get),
            "building" => new(Building.Buildings.Get),
            "bookmark" => new(Bookmark.Bookmarks.Get),
            "religion" => new(Religion.Religions.Get),
            "religious_group" => new(ReligionGroup.ReligionGroups.Get),
            "personal_deity" => new(PersonalDeity.PersonalDeitys.Get),
            "advisor_type" => new(AdvisorType.AdvisorTypes.Get),
            "tradenode" => new(TradeNode.TradeNodes.Get),
            "idea_group" => new(IdeaGroup.IdeaGroups.Get),
            "event_modifier" => new(EventModifier.EventModifiers.Get),
            "opinion_modifier" => new(OpinionModifier.OpinionModifiers.Get),
            "relation" => new(Relation.Relations.Get),
            "culture_group" => new(CultureGroup.CultureGroups.Get),
            "culture" => new(Culture.Cultures.Get),
            "missions" => new(Mission.Missions.Get),
            "mission_series" => new(MissionSeries.MissionSerieses.Get),
            "agenda" => new(EstateAgenda.EstateAgendas.Get),
            "privilege" => new(EstatePrivilege.EstatePrivileges.Get),
            "estate" => new(Estate.Estates.Get),
            "government" => new(Government.Governments.Get),
            "government_names" => new(GovernmentNames.GovernmentNameDict.Get),
            "government_reform" => new(GovernmentReform.GovernmentReforms.Get),
            "event" => new(Event.Events.Get),
            "country_event" => new((Block b) =>
            {
                string id = string.Join(' ', b);
                Event c = (Event)Event.Events.Get(id);
                if (c.ProvinceEvent.Value) throw new Exception($"{id} is not a country_event");
                return c;
            }),
            "province_event" => new((Block b) =>
            {
                string id = string.Join(' ', b);
                Event c = (Event)Event.Events.Get(id);
                if (!c.ProvinceEvent.Value) throw new Exception($"{id} is not a province_event");
                return c;
            }),
            "incident" => new(Incident.Incidents.Get),
            "unit" => new(Unit.Units.Get),
            "great_project" => new(GreatProject.GreatProjects.Get),
            "localisation" => throw new NotImplementedException(),
            "mercenary_company" => new(MercenaryCompany.Companies.Get),
            "advisor" => new(Advisor.Advisors.Get),
            "age" => new(Age.Ages.Get),
            "decision" => new(Decision.Decisions.Get),
            "building_line" => new(BuildingLine.BuildingLines.Get),
            "government_mechanic" => new(GovernmentMechanic.GovernmentMechanics.Get),
            "diplomatic_action" => new(DiplomaticAction.DiplomaticActions.Get),
            _ => throw new NotImplementedException($"Unknown type {string.Join(' ', b)}")
        };
    }
}
public class NewEffect : ArcObject
{
    public NewEffect(string id, Args args)
    {
        Add("args", args.Get(vvC.Constructor, "args"));

        ArcEffect? transpile = args.Get(ArcEffect.Constructor, "transpile");
        if (transpile != null)
        {
            Add("transpile", transpile);
            Compiler.NewEffects.Add((id, this));
        }
    }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static NewEffect Constructor(string id, Args args) => new(id, args);
}
public class NewTrigger : ArcObject
{
    public NewTrigger(string id, Args args)
    {
        Add("args", args.Get(vvC.Constructor, "args"));

        ArcTrigger? transpile = args.Get(ArcTrigger.Constructor, "transpile");
        if (transpile != null)
        {
            Add("transpile", transpile);
            Compiler.NewTriggers.Add((id, this));
        }
    }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static NewTrigger Constructor(string id, Args args) => new(id, args);
}
public class NewModifier : ArcObject
{
    public NewModifier(string id, Args args)
    {
        Add("args", args.Get(vvC.Constructor, "args"));

        ArcModifier? transpile = args.Get(ArcModifier.Constructor, "transpile");
        if (transpile != null)
        {
            Add("transpile", transpile);
            Compiler.NewModifiers.Add((id, this));
        }
    }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static NewTrigger Constructor(string id, Args args) => new(id, args);
}