using ArcInstance;
using Pastel;
using System.Text;

namespace Arc;
public class PersonalDeity : IArcObject
{
    public static readonly Dict<PersonalDeity> PersonalDeitys = new();
    public bool IsObject() => true;
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }
    public ArcInt Sprite { get; set; }
    public ArcModifier Modifiers { get; set; }
    public ArcTrigger Potential { get; set; }
    public ArcTrigger Trigger { get; set; }
    public ArcEffect Effect { get; set; }
    public ArcEffect RemovedEffect { get; set; }
    public ArcCode AiWillDo { get; set; }

    public Dict<IVariable> KeyValuePairs { get; set; }
    public PersonalDeity(ArcString id, ArcString name, ArcString desc, ArcInt sprite, ArcModifier modifiers, ArcTrigger potential, ArcTrigger trigger, ArcEffect effect, ArcEffect removedEffect, ArcCode aiWillDo) 
    {
        Id = id; 
        Name = name; 
        Desc = desc; 
        Sprite = sprite; 
        Modifiers = modifiers; 
        Potential = potential;
        Trigger = trigger; 
        Effect = effect; 
        RemovedEffect = removedEffect; 
        AiWillDo = aiWillDo;
        KeyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "desc", Desc },
            { "sprite", Sprite },
            { "modifiers", Modifiers },
            { "potential", Potential },
            { "trigger", Trigger },
            { "effect", Effect },
            { "removed_effect", RemovedEffect },
            { "ai_will_do", AiWillDo },
        };
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);

        PersonalDeity PersonalDeity = new(
            new(id),
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc"),
            args.Get(ArcInt.Constructor, "sprite"),
            args.Get(ArcModifier.Constructor, "modifiers"),
            args.Get(ArcTrigger.Constructor, "potential", new()),
            args.Get(ArcTrigger.Constructor, "trigger", new()),
            args.Get(ArcEffect.Constructor, "effect", new()),
            args.Get(ArcEffect.Constructor, "removed_effect", new()),
            args.Get(ArcCode.Constructor, "ai_will_do", new("factor = 1"))
        );

        PersonalDeitys.Add(id, PersonalDeity);

        return i;
    }
    public static string Transpile()
    {
        StringBuilder sb = new();
        foreach (PersonalDeity PersonalDeity in PersonalDeitys.Values())
        {
            Program.Localisation.Add($"{PersonalDeity.Id}", PersonalDeity.Name.Value);
            Program.Localisation.Add($"{PersonalDeity.Id}_desc", PersonalDeity.Desc.Value);

            sb.Append($"{PersonalDeity.Id} = {{ sprite = {PersonalDeity.Sprite} {PersonalDeity.AiWillDo.Compile("ai_will_do")} ");
            if (!PersonalDeity.Potential.IsEmpty()) sb.Append(PersonalDeity.Potential.Compile("potential"));
            if (!PersonalDeity.Trigger.IsEmpty()) sb.Append(PersonalDeity.Trigger.Compile("trigger"));
            if (!PersonalDeity.Effect.IsEmpty()) sb.Append(PersonalDeity.Effect.Compile("effect"));
            if (!PersonalDeity.RemovedEffect.IsEmpty()) sb.Append(PersonalDeity.RemovedEffect.Compile("removed_effect"));
            sb.Append($"{PersonalDeity.Modifiers.Compile()} }} ");
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/common/personal_deities/arc.txt", sb.ToString());
        return "Personal Deitys";
    }
    public override string ToString() => Id.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value); return i; }
}