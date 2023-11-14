using ArcInstance;
using Pastel;
using System.IO;
using System.Text;

namespace Arc;
public class ChurchAspect : IArcObject
{
    public static readonly Dict<ChurchAspect> ChurchAspects = new();
    public bool IsObject() => true;
    public string Class => "ChurchAspect";
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }
    public ArcInt Cost { get; set; }
    public ArcTrigger Allow { get; set; }
    public ArcTrigger Trigger { get; set; }
    public ArcTrigger Potential { get; set; }
    public ArcModifier Modifier { get; set; }
    public ArcEffect Effect { get; set; }
    public ArcCode AiWillDo { get; set; }
    public ArcString Id { get; set; }
    public Dict<IValue> KeyValuePairs { get; set; }
    public ChurchAspect(ArcString name, ArcString desc, ArcTrigger potential, ArcTrigger allow, ArcModifier modifier, ArcEffect effect, ArcCode aiWillDo, ArcString id, ArcInt cost, ArcTrigger trigger)
    {
        Name = name;
        Desc = desc;
        Potential = potential;
        Allow = allow;
        Modifier = modifier;
        Effect = effect;
        AiWillDo = aiWillDo;
        Id = id;
        Cost = cost;
        Trigger = trigger;
        KeyValuePairs = new()
        {
            { "name", Name },
            { "desc", Desc },
            { "potential", Potential },
            { "allow", Allow },
            { "modifier", Modifier },
            { "effect", Effect },
            { "ai_will_do", AiWillDo },
            { "id", Id },
            { "cost", Cost },
            { "trigger", Trigger },
        };
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);

        ChurchAspect ChurchAspect = new(
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc"),
            args.Get(ArcTrigger.Constructor, "potential", new()),
            args.Get(ArcTrigger.Constructor, "allow", new()),
            args.Get(ArcModifier.Constructor, "modifier", new()),
            args.Get(ArcEffect.Constructor, "effect", new()),
            args.Get(ArcCode.Constructor, "ai_will_do", new("factor = 1")),
            new(id),
            args.Get(ArcInt.Constructor, "cost", new(100)),
            args.Get(ArcTrigger.Constructor, "trigger", new())
        );

        ChurchAspects.Add(id, ChurchAspect);

        return i;
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value); return i; }
    public void TranspileThis(ref Block b)
    {
        b.Add(
            Id.Value, "=", "{",
                "cost", "=", Cost,
                Potential.Compile("potential"),
                Allow.Compile("allow"),
                Trigger.Compile("trigger"),
                Modifier.Compile("modifier"),
                Effect.Compile("effect"),
                AiWillDo.Compile("ai_will_do"),
            "}"
        );

        Program.Localisation.Add($"{Id}", Name.Value);
        Program.Localisation.Add($"desc_{Id}", Desc.Value);
    }
    public static string Transpile()
    {
        Block b = new();
        foreach (ChurchAspect ChurchAspect in ChurchAspects.Values())
        {
            ChurchAspect.TranspileThis(ref b);
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/common/church_aspects/ChurchAspects.txt", string.Join(' ', b));
        return "Church Aspects";
    }
}