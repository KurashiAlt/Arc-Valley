using ArcInstance;
using Pastel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arc;
public class Expedition : ArcObject
{
    public static Dict<Expedition> Expeditions = new();
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public Expedition(string id, Args args)
    {
        ArcString? picture = args.Get(ArcString.Constructor, "picture", null);
        int length = args.Get(ArcInt.Constructor, "length").Value;
        Province prov = Province.Provinces[id];
        prov.History.Value.Add(
            "1.1.1", "=", "{", 
                "add_permanent_province_modifier", "=", "{", 
                    "name", "=", "expedition_target", 
                    "duration", "=", "-1", 
                "}", 
                "set_variable", "=", "{", 
                    "which", "=", "expedition_start_length", 
                    "value", "=", length, 
                "}", 
                "set_variable", "=", "{", 
                    "which", "=", "expedition_length", 
                    "value", "=", length, 
                "}", 
            "}"
        );
        Event.Events["expedition.3"].Options.Add(
            new(
                new("Great"),
                new("factor", "=", "1"),
                new(false),
                prov,
                new($"`province_id = {prov.Id}`"),
                args.Get(ArcEffect.Constructor, "on_complete")
            )
        );
        if(picture != null)
        {
            Event.Events["expedition.1"].Pictures.Add(picture.Value, new($"`province_id = {prov.Id}`"));
            Event.Events["expedition.2"].Pictures.Add(picture.Value, new($"`province_id = {prov.Id}`"));
            Event.Events["expedition.3"].Pictures.Add(picture.Value, new($"`province_id = {prov.Id}`"));
        }

        Expeditions.Add(id, this);
    }
    public static Expedition Constructor(string id, Args args) => new(id, args);
}

public class Option : IArcObject
{
    public string Class => "Option";
    ArcString Name { get; set; }
    ArcCode AiChance { get; set; }
    ArcBool Highlight { get; set; }
    Province? Goto { get; set; }
    ArcTrigger? Trigger { get; set; }
    ArcEffect Effect { get; set; }
    public Dict<IVariable?> keyValuePairs { get; set; }
    public Option(ArcString name, ArcCode aiChance, ArcBool highlight, Province? @goto, ArcTrigger? trigger, ArcEffect effect)
    {
        Name = name;
        AiChance = aiChance;
        Highlight = highlight;
        Goto = @goto;
        Trigger = trigger;
        Effect = effect;
        keyValuePairs = new()
        {
            { "name", Name },
            { "ai_chance", AiChance },
            { "highlight", Highlight },
            { "goto", Goto },
            { "trigger", Trigger },
            { "effect", Effect },
        };
    }

    public bool CanGet(string indexer) => keyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => keyValuePairs.Get(indexer);
    public override string ToString() => Name.Value;
    public void Transpile(ref Block sb, string key)
    {
        Instance.Localisation.Add(key, Name.Value);
        sb.Add("option", "=", "{",
            "name", "=", key);
        if (Goto != null) sb.Add("goto", "=", Goto.Id);
        if (Highlight) sb.Add("highlight", "=", "yes");
        AiChance.Compile("ai_chance", ref sb);
        if(Trigger != null) Trigger.Compile("trigger", ref sb);
        Effect.Compile(ref sb);
        sb.Add("}");
    }
    public Walker Call(Walker i, ref Block result)
    {
        throw new NotImplementedException();
    }
    public static Option Constructor(Block block)
    {
        Walker i = new(block);

        i = Args.GetArgs(i, out Args args, 2);

        return new Option(
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcCode.Constructor, "ai_chance", new("factor", "=", "1")),
            args.Get(ArcBool.Constructor, "highlight", new(false)),
            args.GetFromListNullable(Province.Provinces, "goto"),
            args.Get(ArcTrigger.Constructor, "trigger", null),
            args.Get(ArcEffect.Constructor, "effect", new())
        );
    }
}

public class Event : IArcObject
{
    public static Dict<Event> Events = new()
    {
        { "expedition.1", new(
            "expedition.1",
            new(true),
            new("Send an Expedition"),
            new(""),
            new("TOMB_eventPicture"),
            new(false),
            new(),
            new(false),
            new(false),
            new(),
            new(),
            new(),
            new(),
            new(true),
            new()
            {
                new Option(
                    new("Send an Expedition"),
                    new("factor", "=", "1"),
                    new(false),
                    null,
                    null,
                    new(@"
owner = {
    add_treasury = -50
}
set_province_flag = expedition_in_progress
change_variable = {
    which = expedition_length
    value = -1
}
if = {
    limit = {
        check_variable = {
            which = expedition_length
            value = 1
        }
    }
    province_event = {
        id = expedition.2
        days = 730 
    }
} 
else = { 
    province_event = { 
        id = expedition.3 
        days = 730 
    } 
}")
                ),
                new Option(
                    new("Nevermind"),
                    new("factor", "=", "1"),
                    new(false),
                    null,
                    null,
                    new()
                )
            },
            new(),
            false
        ) },
        { "expedition.2", new(
            "expedition.2",
            new(true),
            new("End of Expedition"),
            new(""),
            new("TOMB_eventPicture"),
            new(false),
            new(),
            new(false),
            new(false),
            new(),
            new("clr_province_flag = expedition_in_progress"),
            new(),
            new(),
            new(true),
            new()
            {
                new Option(
                    new("I see"),
                    new("factor", "=", "1"),
                    new(false),
                    null,
                    null,
                    new()
                )
            },
            new(),
            false
        ) },
        { "expedition.3", new(
            "expedition.3",
            new(true),
            new("A find"),
            new(""),
            new("TOMB_eventPicture"),
            new(false),
            new(),
            new(false),
            new(false),
            new(),
            new("clr_province_flag = expedition_in_progress"),
            new("remove_province_modifier = expedition_target"),
            new(),
            new(true),
            new(),
            new(),
            false
        ) },
    };
    public ArcString Id { get; set; }
    public ArcBool ProvinceEvent { get; set; }
    public ArcString Title { get; set; }
    public ArcString Desc { get; set; }
    public ArcString Picture { get; set; }
    public Dict<ArcTrigger>? Pictures { get; set; }
    public ArcBool Major { get; set; }
    public ArcTrigger MajorTrigger { get; set; }
    public ArcBool FireOnlyOnce { get; set; }
    public ArcBool Hidden { get; set; }
    public ArcTrigger Trigger { get; set; }
    public ArcEffect Immediate { get; set; }
    public ArcEffect After { get; set; }
    public ArcFactor MeanTimeToHappen { get; set; }
    public ArcBool IsTriggeredOnly { get; set; }
    public ArcList<Option> Options { get; set; }
    public Dict<IVariable> keyValuePairs { get; set; }
    public Event(
        string id,
        ArcBool provinceEvent,
        ArcString title,
        ArcString desc,
        ArcString picture,
        ArcBool major,
        ArcTrigger majorTrigger,
        ArcBool fireOnlyOnce,
        ArcBool hidden,
        ArcTrigger trigger,
        ArcEffect immediate,
        ArcEffect after,
        ArcFactor meanTimeToHappen,
        ArcBool isTriggeredOnly,
        ArcList<Option> options,
        Dict<ArcTrigger>? pictures,
        bool addToList = true
    ) {
        Id = new(id);
        ProvinceEvent = provinceEvent;
        Title = title;
        Desc = desc;
        Picture = picture;
        Major = major;
        MajorTrigger = majorTrigger;
        FireOnlyOnce = fireOnlyOnce;
        Hidden = hidden;
        Trigger = trigger;
        Immediate = immediate;
        After = after;
        MeanTimeToHappen = meanTimeToHappen;
        IsTriggeredOnly = isTriggeredOnly;
        Options = options;
        Pictures = pictures;
        keyValuePairs = new()
        {
            { "id", Id },
            { "province_event", ProvinceEvent },
            { "title", Title },
            { "desc", Desc },
            { "picture", Picture },
            { "major", Major },
            { "major_trigger", MajorTrigger },
            { "fire_only_once", FireOnlyOnce },
            { "hidden", Hidden },
            { "trigger", Trigger },
            { "immediate", Immediate },
            { "after", After },
            { "mean_time_to_happen", MeanTimeToHappen },
            { "is_triggered_only", IsTriggeredOnly },
            { "options", Options },
        };

        if(addToList) Events.Add(id, this);
    }

    public bool CanGet(string indexer) => keyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => keyValuePairs.Get(indexer);
    public static Walker Call(Walker i, bool eventType)
    {
        if (!i.MoveNext()) throw new Exception();

        string key = i.Current;
        i = Args.GetArgs(i, out Args args);

        new Event(
            key,
            new(eventType),
            args.Get(ArcString.Constructor, "title"),
            args.Get(ArcString.Constructor, "desc"),
            args.Get(ArcString.Constructor, "picture"),
            args.Get(ArcBool.Constructor, "major", new(false)),
            args.Get(ArcTrigger.Constructor, "major_trigger", new()),
            args.Get(ArcBool.Constructor, "fire_only_once", new(false)),
            args.Get(ArcBool.Constructor, "hidden", new(false)),
            args.Get(ArcTrigger.Constructor, "trigger", new()),
            args.Get(ArcEffect.Constructor, "immediate", new()),
            args.Get(ArcEffect.Constructor, "after", new()),
            args.Get(ArcFactor.Constructor, "mean_time_to_happen", new()),
            args.Get(ArcBool.Constructor, "is_triggered_only", new(true)),
            args.Get((Block s) => new ArcList<Option>(s, Option.Constructor), "options"),
            args.Get(Dict<ArcTrigger>.Constructor(ArcTrigger.Constructor), "pictures", null)
        );
        return i;
    }

    public override string ToString() => Id.Value;
    public void Transpile(ref Block b)
    {
        Instance.Localisation.Add($"{Id}.title", Title.Value);
        Instance.Localisation.Add($"{Id}.desc", Desc.Value);

        if (ProvinceEvent) b.Add("province_event", "=", "{");
        else b.Add("country_event", "=", "{");

        b.Add("id", "=", Id);
        b.Add("title", "=", $"{Id}.title");
        b.Add("desc", "=", $"{Id}.desc");
        if(Pictures != null) 
        {
            foreach(KeyValuePair<string, ArcTrigger> pic in Pictures)
            {
                b.Add("picture", "=", "{");
                pic.Value.Compile("trigger", ref b);
                b.Add("picture", "=", pic.Key, "}");
            }
        }
        b.Add("picture", "=", Picture);
        if (Major) b.Add("major", "=", "yes");
        MajorTrigger.Compile("major_trigger", ref b);
        if (FireOnlyOnce) b.Add("fire_only_once", "=", "yes");
        if (Hidden) b.Add("hidden", "=", "yes");
        Trigger.Compile("trigger", ref b);
        Immediate.Compile("immediate", ref b);
        After.Compile("after", ref b);
        MeanTimeToHappen.Compile("mean_time_to_happen", ref b);
        if (IsTriggeredOnly) b.Add("is_triggered_only", "=", "yes");

        int i = 1;
        foreach(Option? opt in Options.Values)
        {
            if (opt == null) continue;
            opt.Transpile(ref b, $"{Id}.{i}");
            i++;
        }
        b.Add("}");
    }
    public static string Transpile()
    {
        Block b = new();

        string currentNamespace = "";
        foreach (Event ev in from eve in Events.Values() orderby eve.Id.Value select eve)
        {
            if (ev.Id.Value.Contains('.'))
            {
                string newNamespace = ev.Id.Value.Split('.')[0];
                if (currentNamespace != newNamespace)
                {
                    currentNamespace = newNamespace;
                    b.Add("namespace", "=", currentNamespace);
                }
            }
            else
            {
                currentNamespace = "";
            }

            ev.Transpile(ref b);
        }

        Instance.OverwriteFile($"{Instance.TranspileTarget}/events/arc.txt", string.Join(' ', b));
        return "Events";
    }
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}