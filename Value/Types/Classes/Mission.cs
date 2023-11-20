
using Pastel;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Text;
using System.Xml.Linq;

namespace Arc;
public class Mission : IArcObject
{
    public static Dict<Mission> Missions = new();
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }
    public ArcString Icon { get; set; }
    public ArcInt? Position { get; set; }
    public ArcString? CompletedBy { get; set; }
    public ArcCode Required { get; set; }
    public ArcTrigger ProvincesToHighlight { get; set; }
    public ArcTrigger Trigger { get; set; }
    public ArcEffect Effect { get; set; }
    public Dict<IVariable?> keyValuePairs { get; set; }
    public Mission(string key, ArcString name, ArcString desc, ArcString icon, ArcInt? position, ArcString? completedBy, ArcCode required, ArcTrigger provincesToHighlight, ArcTrigger trigger, ArcEffect effect)
    {
        Name = name;
        Desc = desc;
        Icon = icon;
        position.Value += 1;
        Position = position;
        CompletedBy = completedBy;
        Required = required;
        ProvincesToHighlight = provincesToHighlight;
        Trigger = trigger;
        Effect = effect;
        keyValuePairs = new()
        {
            { "name", Name },
            { "desc", Desc },
            { "icon", Icon },
            { "position", Position },
            { "completed_by", CompletedBy },
            { "required", Required },
            { "provinces_to_highlight", ProvincesToHighlight },
            { "trigger", Trigger },
            { "effect", Effect },
        };

        Id = new(key);
        Missions.Add(key, this);
    }

    public bool CanGet(string indexer) => keyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => keyValuePairs.Get(indexer);
    public override string ToString() => Name.Value;
    public void Transpile(ref Block b)
    {
        b.Add(
            Id, "=", "{", 
                "icon", "=", Icon
        );
        if (Position != null) b.Add("position", "=", Position); 
        if (CompletedBy != null) b.Add("completed_by", "=", CompletedBy);
        Required.Compile("required_missions", ref b);
        ProvincesToHighlight.Compile("provinces_to_highlight", ref b);
        Trigger.Compile("trigger", ref b);
        Effect.Compile("effect", ref b);
        b.Add("}");

        Program.Localisation.Add($"{Id}_title", Name.Value);
        Program.Localisation.Add($"{Id}_desc", Desc.Value);
    }
    public Walker Call(Walker i, ref Block result)
    {
        throw new NotImplementedException();
    }
    public static Mission Constructor(string key, Args args) => new(
        key,
        args.Get(ArcString.Constructor, "name"),
        args.Get(ArcString.Constructor, "desc", new("")),
        args.Get(ArcString.Constructor, "icon", new("mission_unknown_mission")),
        args.Get(ArcInt.Constructor, "position", null),
        args.Get(ArcString.Constructor, "completed_by", null),
        args.Get(ArcCode.Constructor, "required", new()),
        args.Get(ArcTrigger.Constructor, "provinces_to_highlight", new()),
        args.Get(ArcTrigger.Constructor, "trigger", new()),
        args.Get(ArcEffect.Constructor, "effect", new())
    );
}
public class MissionTree : ArcObject
{
    public static Dict<MissionTree> MissionTrees = new();
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public MissionSeries?[] Serieses { get; set; }
    string Id { get; set; }
    ArcBool Generic { get; set; }
    ArcBool Ai { get; set; }
    ArcBool HasCountryShield { get; set; }
    ArcTrigger PotentialOnLoad { get; set; }
    ArcTrigger Potential { get; set; }
    public MissionTree(string id, Args args)
    {
        Id = id;
        Generic = args.Get(ArcBool.Constructor, "generic", new(false));
        Ai = args.Get(ArcBool.Constructor, "ai", new(true));
        HasCountryShield = args.Get(ArcBool.Constructor, "has_country_shield", new(false));
        PotentialOnLoad = args.Get(ArcTrigger.Constructor, "potential_on_load", new());
        Potential = args.Get(ArcTrigger.Constructor, "potential", new());

        Serieses = new MissionSeries?[]{
            null, null, null, null, null,
            null, null, null, null, null
        };

        AddFromArgs(args);

        MissionTrees.Add(id, this);
    }
    public void AddFromArgs(Args args)
    {
        string[] v = { "generic", "ai", "has_country_shield", "potential_on_load", "potential" };
        if (args.keyValuePairs == null) throw new Exception();
        foreach (KeyValuePair<string, Block> mis in from m in args.keyValuePairs where !v.Contains(m.Key) select m)
        {
            Args a = Args.GetArgs(mis.Value);
            Mission mission = new(
                $"{Id}_{mis.Key}",
                a.Get(ArcString.Constructor, "name"),
                a.Get(ArcString.Constructor, "desc", new("")),
                a.Get(ArcString.Constructor, "icon", new("mission_unknown_mission")),
                a.Get(ArcInt.Constructor, "y", null),
                a.Get(ArcString.Constructor, "completed_by", null),
                new ArcCode(from b in a.Get(ArcCode.Constructor, "required", new()).Value select $"{Id}_{b.value}"),
                a.Get(ArcTrigger.Constructor, "provinces_to_highlight", new()),
                a.Get(ArcTrigger.Constructor, "trigger", new()),
                a.Get(ArcEffect.Constructor, "effect", new())
            );
            int x = a.Get(ArcInt.Constructor, "x").Value;
            if (Serieses[x] == null) Serieses[x] = new(
                $"{Id}_{x}",
                new(x),
                Generic,
                Ai,
                HasCountryShield,
                PotentialOnLoad,
                Potential,
                new()
            );
            Serieses[x].Missions.Add(mis.Key, mission);
        }
    }
    public static MissionTree Constructor(string id, Args args) => new(id, args);
    public override Walker Call(Walker i, ref Block result)
    {
        i.ForceMoveNext();
        if (i.Current != "+=") throw new Exception();
        i.ForceMoveNext();
        i = Args.GetArgs(i, out Args args, 2);
        AddFromArgs(args);
        return i;
    }
}
public class MissionSeries : IArcObject
{
    public static Dict<MissionSeries> MissionSerieses = new();
    public string Class => "Idea";
    public ArcString Id { get; set; }
    public ArcInt? Slot { get; set; }
    public ArcBool Generic { get; set; }
    public ArcBool Ai { get; set; }
    public ArcBool HasCountryShield { get; set; }
    public ArcTrigger PotentialOnLoad { get; set; }
    public ArcTrigger Potential { get; set; }
    public Dict<Mission> Missions { get; set; }
    public Dict<IVariable?> keyValuePairs { get; set; }
    public MissionSeries(string key, ArcInt? slot, ArcBool generic, ArcBool ai, ArcBool hasCountryShield, ArcTrigger potentialOnLoad, ArcTrigger potential, Dict<Mission> missions)
    {
        Id = new($"{key}_series");
        slot.Value += 1;
        Slot = slot;
        Generic = generic;
        Ai = ai;
        HasCountryShield = hasCountryShield;
        PotentialOnLoad = potentialOnLoad;
        Potential = potential;
        Missions = missions;
        keyValuePairs = new()
        {
            { "id", Id },
            { "slot", Slot },
            { "generic", Generic },
            { "ai", Ai },
            { "has_country_shield", HasCountryShield },
            { "potential_on_load", PotentialOnLoad },
            { "potential", Potential },
            { "missions", Missions }
        };

        MissionSerieses.Add(key, this);
    }

    public bool CanGet(string indexer) => keyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => keyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = Compiler.GetId(i.Current);
        try
        {
            i = Args.GetArgs(i, out Args args);

             new MissionSeries(
                id,
                args.Get(ArcInt.Constructor, "slot", null),
                args.Get(ArcBool.Constructor, "generic", new(false)),
                args.Get(ArcBool.Constructor, "ai", new(true)),
                args.Get(ArcBool.Constructor, "has_country_shield", new(false)),
                args.Get(ArcTrigger.Constructor, "potential_on_load", new()),
                args.Get(ArcTrigger.Constructor, "potential", new()),
                args.Get((Block s) => new Dict<Mission>(s, Mission.Constructor), "missions")
            );
            return i;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public override string ToString() => Id.Value;
    public static string Transpile()
    {
        Block b = new();
        foreach (MissionSeries? MissionSeries in MissionSerieses.Values())
        {
            if (MissionSeries == null) continue;
            b.Add(
                MissionSeries.Id, "=", "{", 
                    "slot", "=", MissionSeries.Slot, 
                    "generic", "=", MissionSeries.Generic, 
                    "ai", "=", MissionSeries.Ai ,
                    "has_country_shield", "=", MissionSeries.HasCountryShield
            );
            MissionSeries.PotentialOnLoad.Compile("potential_on_load", ref b);
            MissionSeries.Potential.Compile("potential", ref b);
            foreach(Mission? mission in from c in MissionSeries.Missions orderby c.Value.Position.Value select c.Value)
            {
                mission.Transpile(ref b);
            }
            b.Add("}");
        }

        Program.OverwriteFile($"{Program.TranspileTarget}/missions/arc.txt", string.Join(' ', b));
        return "Missions";
    }
    public Walker Call(Walker i, ref Block result)
    {
        throw new NotImplementedException();
    }
}