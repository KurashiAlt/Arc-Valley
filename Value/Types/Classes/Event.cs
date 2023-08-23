﻿using ArcInstance;
using Pastel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arc;

public class Option : IArcObject
{
    public string Class => "Option";
    ArcString Name { get; set; }
    ArcBlock AiChance { get; set; }
    ArcBool Highlight { get; set; }
    Province? Goto { get; set; }
    ArcBlock? Trigger { get; set; }
    ArcBlock Effect { get; set; }
    public Dict<IVariable?> keyValuePairs { get; set; }
    public Option(ArcString name, ArcBlock aiChance, ArcBool highlight, Province? @goto, ArcBlock? trigger, ArcBlock effect)
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
    public Walker Call(Walker i, ref Block result, Compiler comp)
    {
        throw new NotImplementedException();
    }
    public static Option Constructor(Block block)
    {
        Walker i = new(block);

        i = Args.GetArgs(i, out Args args, 2);

        return new Option(
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcBlock.Constructor, "ai_chance", new("factor", "=", "1")),
            args.Get(ArcBool.Constructor, "highlight", new(false)),
            args.GetFromListNullable(Province.Provinces, "goto"),
            args.Get(ArcBlock.Constructor, "trigger", null),
            args.Get(ArcBlock.Constructor, "effect", new())
        );
    }
}

public class Event : IArcObject
{
    public static Dict<Event> Events = new();
    ArcString Id { get; set; }
    ArcBool ProvinceEvent { get; set; }
    ArcString Title { get; set; }
    ArcString Desc { get; set; }
    ArcString Picture { get; set; }
    ArcBool Major { get; set; }
    ArcBlock MajorTrigger { get; set; }
    ArcBool FireOnlyOnce { get; set; }
    ArcBool Hidden { get; set; }
    ArcBlock Trigger { get; set; }
    ArcBlock Immediate { get; set; }
    ArcBlock After { get; set; }
    ArcBlock MeanTimeToHappen { get; set; }
    ArcBool IsTriggeredOnly { get; set; }
    ArcList<Option> Options { get; set; }
    public Dict<IVariable> keyValuePairs { get; set; }
    public Event(
        string id,
        ArcBool provinceEvent,
        ArcString title,
        ArcString desc,
        ArcString picture,
        ArcBool major,
        ArcBlock majorTrigger,
        ArcBool fireOnlyOnce,
        ArcBool hidden,
        ArcBlock trigger,
        ArcBlock immediate,
        ArcBlock after,
        ArcBlock meanTimeToHappen,
        ArcBool isTriggeredOnly,
        ArcList<Option> options
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

        Events.Add(id, this);
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
            args.Get(ArcBlock.Constructor, "major_trigger", new()),
            args.Get(ArcBool.Constructor, "fire_only_once", new(false)),
            args.Get(ArcBool.Constructor, "hidden", new(false)),
            args.Get(ArcBlock.Constructor, "trigger", new()),
            args.Get(ArcBlock.Constructor, "immediate", new()),
            args.Get(ArcBlock.Constructor, "after", new()),
            args.Get(ArcBlock.Constructor, "mean_time_to_happen", new()),
            args.Get(ArcBool.Constructor, "is_triggered_only", new(true)),
            args.Get((Block s) => new ArcList<Option>(s, Option.Constructor), "options")
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

        Instance.OverwriteFile("target/events/arc.txt", string.Join(' ', b));
        return "Events";
    }
    public Walker Call(Walker i, ref Block result, Compiler comp)
    {
        throw new NotImplementedException();
    }
}