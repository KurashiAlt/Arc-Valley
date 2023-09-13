﻿using ArcInstance;
using Pastel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace Arc;
public class Decision : IArcObject
{
    public static readonly Dict<Decision> Decisions = new();
    public bool IsObject() => true;
    ArcString Id { get; set; }
    ArcString Name { get; set; }
    ArcString Desc { get; set; }
    ArcBool Major { get; set; }
    ArcBlock? Color { get; set; }
    ArcTrigger Potential { get; set; }
    ArcTrigger Allow { get; set; }
    ArcBlock Effect { get; set; }
    ArcBlock AiWillDo { get; set; }
    ArcInt AiImportance { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public Decision(
        string key,
        ArcString name,
        ArcString desc,
        ArcBool major,
        ArcBlock? color,
        ArcTrigger potential,
        ArcTrigger allow,
        ArcBlock effect,
        ArcBlock aiWillDo,
        ArcInt aiImportance
    )
    {
        Decisions.Add(key, this);
        Id = new(key);
        Name = name;
        Desc = desc;
        Major = major;
        Color = color;
        Potential = potential;
        Allow = allow;
        Effect = effect;
        AiWillDo = aiWillDo;
        AiImportance = aiImportance;
        KeyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "desc", Desc },
            { "major", Major },
            { "color", Color },
            { "potential", Potential },
            { "allow", Allow },
            { "effect", Effect },
            { "ai_will_do", AiWillDo },
            { "ai_importance", AiImportance },
        };
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        i.ForceMoveNext();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);

        Constructor(id, args);

        return i;
    }
    public static Decision Constructor(string id, Args args)
    {
        return new Decision(id,
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc", new("")),
            args.Get(ArcBool.Constructor, "major", new(false)),
            args.Get(ArcBlock.Constructor, "color", null),
            args.Get(ArcTrigger.Constructor, "potential", new()),
            args.Get(ArcTrigger.Constructor, "allow", new()),
            args.Get(ArcBlock.Constructor, "effect", new()),
            args.Get(ArcBlock.Constructor, "ai_will_do", new()),
            args.Get(ArcInt.Constructor, "ai_importance", new(400))
        );
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result, Compiler comp) { result.Add(Id.Value); return i; }
    public void Transpile(ref Block s)
    {
        Instance.Localisation.Add($"{Id}_title", Name.Value);
        Instance.Localisation.Add($"{Id}_desc", Desc.Value);
        s.Add(
            Id, "=", "{",
                "major", "=", Major
        );
        Color?.Compile("color", ref s);
        Potential.Compile("potential", ref s);
        Allow.Compile("allow", ref s);
        Effect.Compile("effect", ref s);
        AiWillDo.Compile("ai_will_do", ref s);
        s.Add(
                "ai_importance", "=", AiImportance,
            "}"
        );
    }
    public static string Transpile()
    {
        Block s = new("country_decisions", "=", "{");
        foreach (Decision Decision in Decisions.Values())
        {
            Decision.Transpile(ref s);
        }
        s.Add("}");
        Instance.OverwriteFile("target/decisions/arc.txt", string.Join(' ', s));
        return "Decisions";
    }
}
