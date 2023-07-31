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
    public ArcBlock Modifiers { get; set; }
    public ArcBlock Potential { get; set; }
    public ArcBlock Trigger { get; set; }
    public ArcBlock Effect { get; set; }
    public ArcBlock RemovedEffect { get; set; }
    public ArcBlock AiWillDo { get; set; }

    public Dict<IVariable> KeyValuePairs { get; set; }
    public PersonalDeity(ArcString id, ArcString name, ArcString desc, ArcInt sprite, ArcBlock modifiers, ArcBlock potential, ArcBlock trigger, ArcBlock effect, ArcBlock removedEffect, ArcBlock aiWillDo) 
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
            args.Get(ArcBlock.Constructor, "modifiers"),
            args.Get(ArcBlock.Constructor, "potential", new()),
            args.Get(ArcBlock.Constructor, "trigger", new()),
            args.Get(ArcBlock.Constructor, "effect", new()),
            args.Get(ArcBlock.Constructor, "removed_effect", new()),
            args.Get(ArcBlock.Constructor, "ai_will_do", new("factor = 1"))
        );

        PersonalDeitys.Add(id, PersonalDeity);

        return i;
    }
    public static void Transpile()
    {
        StringBuilder sb = new();
        foreach (PersonalDeity PersonalDeity in PersonalDeitys.Values())
        {
            Instance.Localisation.Add($"{PersonalDeity.Id}", PersonalDeity.Name.Value);
            Instance.Localisation.Add($"{PersonalDeity.Id}_desc", PersonalDeity.Desc.Value);

            sb.Append($"{PersonalDeity.Id} = {{ sprite = {PersonalDeity.Sprite} {PersonalDeity.AiWillDo.Compile("ai_will_do")} ");
            if (!PersonalDeity.Potential.IsEmpty()) sb.Append($"potential = {{ {PersonalDeity.Potential} }} ");
            if (!PersonalDeity.Trigger.IsEmpty()) sb.Append($"trigger = {{ {PersonalDeity.Trigger} }} ");
            if (!PersonalDeity.Effect.IsEmpty()) sb.Append($"effect = {{ {PersonalDeity.Effect} }} ");
            if (!PersonalDeity.RemovedEffect.IsEmpty()) sb.Append($"removed_effect = {{ {PersonalDeity.RemovedEffect} }} ");
            sb.Append($"{PersonalDeity.Modifiers} }} ");
        }
        Instance.OverwriteFile("target/common/personal_deities/arc.txt", sb.ToString());
        Console.WriteLine($"Finished Transpiling Personal Deitys".Pastel(ConsoleColor.Cyan));
    }
    public override string ToString() => Id.Value;
    public Walker Call(Walker i, ref List<string> result, Compiler comp) { result.Add(Id.Value); return i; }
}