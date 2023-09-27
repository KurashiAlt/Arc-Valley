using ArcInstance;
using Pastel;
using System.ComponentModel;
using System.Text;
using System.Xml.Linq;

namespace Arc;
public class Mission : IArcObject
{
    public static Dict<Mission> Missions = new Dict<Mission>();
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }
    public ArcString Icon { get; set; }
    public ArcInt? Position { get; set; }
    public ArcString? CompletedBy { get; set; }
    public ArcCode Required { get; set; }
    public ArcList<Province> ProvincesToHighlight { get; set; }
    public ArcTrigger Trigger { get; set; }
    public ArcEffect Effect { get; set; }
    public Dict<IVariable?> keyValuePairs { get; set; }
    public Mission(string key, ArcString name, ArcString desc, ArcString icon, ArcInt? position, ArcString? completedBy, ArcCode required, ArcList<Province> provincesToHighlight, ArcTrigger trigger, ArcEffect effect)
    {
        Name = name;
        Desc = desc;
        Icon = icon;
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
    public string Transpile()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"{Id} = {{ icon = {Icon} ");
        if (Position != null) sb.Append($"position = {Position} "); 
        if (CompletedBy != null) sb.Append($"completed_by = {CompletedBy} ");
        sb.Append(Required.Compile("required_missions"));
        if (ProvincesToHighlight.Values.Count > 0)
        {
            sb.Append($"provinces_to_highlight = {{ ");
            foreach (Province? province in ProvincesToHighlight.Values)
            {
                if (province == null) continue;
                sb.Append($"{province.Id} ");
            }
            sb.Append($"}} ");
        }
        sb.Append($"{Trigger.Compile("trigger")} {Effect.Compile("effect")} }}");

        Instance.Localisation.Add($"{Id}_title", Name.Value);
        Instance.Localisation.Add($"{Id}_desc", Desc.Value);
        return sb.ToString();
    }
    public Walker Call(Walker i, ref Block result)
    {
        throw new NotImplementedException();
    }
    public static Mission Constructor(string key, Args args)
    {
        return new Mission(
            key,
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc"),
            args.Get(ArcString.Constructor, "icon"),
            args.Get(ArcInt.Constructor, "position", null),
            args.Get(ArcString.Constructor, "completed_by", null),
            args.Get(ArcCode.Constructor, "required", new()),
            args.Get((Block s) => new ArcList<Province>(s, Province.Provinces), "provinces_to_highlight", new()),
            args.Get(ArcTrigger.Constructor, "trigger", new()),
            args.Get(ArcEffect.Constructor, "effect", new())
        );
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

        string id = i.Current;
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
        StringBuilder sb = new();
        foreach (MissionSeries? MissionSeries in MissionSerieses.Values())
        {
            if (MissionSeries == null) continue;
            sb.Append($"{MissionSeries.Id} = {{ slot = {MissionSeries.Slot} generic = {MissionSeries.Generic} ai = {MissionSeries.Ai} has_country_shield = {MissionSeries.HasCountryShield} {MissionSeries.PotentialOnLoad.Compile("potential_on_load")} {MissionSeries.Potential.Compile("potential")} ");
            foreach(Mission? mission in MissionSeries.Missions.Values())
            {
                sb.Append(mission.Transpile());
            }
            sb.Append($" }} ");
        }

        Instance.OverwriteFile($"{Instance.TranspileTarget}/missions/arc.txt", sb.ToString());
        return "Missions";
    }
    public Walker Call(Walker i, ref Block result)
    {
        throw new NotImplementedException();
    }
}