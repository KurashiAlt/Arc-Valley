using ArcInstance;
using Pastel;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace Arc;
public class Objective : IArcObject
{
    public bool IsObject() => true;
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }
    public ArcTrigger Trigger { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public Objective(
        string id,
        ArcString name,
        ArcString desc,
        ArcTrigger trigger
    ) {
        Id = new(id);
        Name = name;
        Desc = desc;
        Trigger = trigger;

        KeyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "desc", Desc },
            { "trigger", Trigger }
        };
    }
    public static Objective Constructor(string id, Args args)
    {
        return new Objective(id,
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc"),
            args.Get(ArcTrigger.Constructor, "trigger")
        );
    }
    public void TranspileSingular(ref Block s)
    {
        Program.Localisation.Add($"{Id}", Name.Value);
        Program.Localisation.Add($"{Id}_desc", Desc.Value);
        s.Add(
            Id, "=", "{",
                Trigger.Compile(),
            "}"
        );
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}
public class Ability : IArcObject
{
    public bool IsObject() => true;
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcTrigger Allow { get; set; }
    public ArcEffect Effect { get; set; }
    public ArcModifier Modifier { get; set; }
    public ArcCode Rule { get; set; }
    public ArcCode AiWillDo { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public Ability(
        string id,
        ArcString name,
        ArcTrigger allow,
        ArcEffect effect,
        ArcModifier modifier,
        ArcCode rule,
        ArcCode aiWillDo
    ) {
        Id = new(id);
        Name = name;
        Allow = allow;
        Effect = effect;
        Modifier = modifier;
        Rule = rule;
        AiWillDo = aiWillDo;

        KeyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "allow", Allow },
            { "effect", Effect },
            { "modifier", Modifier },
            { "rule", Rule },
            { "ai_will_do", AiWillDo },
        };
    }
    public static Ability Constructor(string id, Args args)
    {
        return new Ability(id,
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcTrigger.Constructor, "allow", new()),
            args.Get(ArcEffect.Constructor, "effect", new()),
            args.Get(ArcModifier.Constructor, "modifier", new()),
            args.Get(ArcCode.Constructor, "rule", new()),
            args.Get(ArcCode.Constructor, "ai_will_do", new())
        );
    }
    public void TranspileSingular(ref Block s)
    {
        Program.Localisation.Add(Id.Value, Name.Value);
        s.Add(
            Id, "=", "{"
        );
        Allow.Compile("allow", ref s);
        Effect.Compile("effect", ref s);
        Modifier.Compile("modifier", ref s);
        Rule.Compile("rule", ref s);
        AiWillDo.Compile("ai_will_do", ref s);
        s.Add(
            "}"
        );
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}
public class Age : IArcObject
{
    public static readonly Dict<Age> Ages = new();
    public bool IsObject() => true;
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }
    public ArcInt Start { get; set; }
    public ArcTrigger CanStart { get; set; }
    public ArcBool ReligiousConflicts { get; set; }
    public ArcFloat Papacy { get; set; }
    public ArcCode Absolutism { get; set; }
    public Dict<Objective> Objectives { get; set; }
    public Dict<Ability> Abilities { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public Age(
        string id,
        ArcString name,
        ArcString desc,
        ArcInt start,
        ArcTrigger canStart,
        ArcBool religiousConflicts,
        ArcFloat papacy,
        ArcCode absolutism,
        Dict<Objective> objectives,
        Dict<Ability> abilities
    )
    {
        Id = new(id);
        Name = name;
        Desc = desc;
        Start = start;
        CanStart = canStart;
        ReligiousConflicts = religiousConflicts;
        Papacy = papacy;
        Absolutism = absolutism;
        Objectives = objectives;
        Abilities = abilities;

        KeyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "desc", Desc },
            { "start", Start },
            { "can_start", CanStart },
            { "religious_conflicts", ReligiousConflicts },
            { "papacy", Papacy },
            { "absolutism", Absolutism },
            { "objectives", Objectives },
            { "abilities", Abilities },
        };

        Ages.Add(id, this);
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
    public static Age Constructor(string id, Args args)
    {
        return new Age(id,
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc"),
            args.Get(ArcInt.Constructor, "start"),
            args.Get(ArcTrigger.Constructor, "can_start"),
            args.Get(ArcBool.Constructor, "religious_conflicts", new(false)),
            args.Get(ArcFloat.Constructor, "papacy", new(1.0)),
            args.Get(ArcCode.Constructor, "absolutism", new("harsh_treatment = 1 stability = 1 decrease_autonomy_command = 1 strengthen_government = 1  increase_autonomy_command = -1 debase_currency = -1 execute_rebel_acceptance_command = -10 seat_in_parliament = -5 war_exhaustion = -1")),
            args.Get(Dict<Objective>.Constructor(Objective.Constructor), "objectives", new()),
            args.Get(Dict<Ability>.Constructor(Ability.Constructor), "abilities", new())
        );
    }
    private void TranspileSingular(ref Block s)
    {
        Program.Localisation.Add($"{Id}", Name.Value);
        Program.Localisation.Add($"{Id}_desc", Desc.Value);
        s.Add(
            Id, "=", "{",
                "start", "=", Start,
                CanStart.Compile("can_start"),
                Absolutism.Compile("absolutism"),
                "religious_conflicts", "=", ReligiousConflicts,
                "papacy", "=", Papacy,
                "objectives", "=", "{"
        );
        foreach (Objective obj in Objectives.Values())
        {
            obj.TranspileSingular(ref s);
        }
        s.Add(
                "}",
                "abilities", "=", "{"
        );
        foreach (Ability ab in Abilities.Values())
        {
            ab.TranspileSingular(ref s);
        }
        s.Add(
                "}",
            "}"
        );
    }
    public static string Transpile()
    {
        Block s = new Block();
        foreach (Age Age in Ages.Values())
        {
            Age.TranspileSingular(ref s);
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/common/ages/ages.txt", string.Join(' ', s));
        return "Ages";
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}
