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
    public ArcBlock Allow { get; set; }
    public ArcBlock Potential { get; set; }
    public ArcBlock Modifier { get; set; }
    public ArcBlock Effect { get; set; }
    public ArcBlock AiWillDo { get; set; }
    public ArcString Id { get; set; }
    public Dict<IValue> KeyValuePairs { get; set; }
    public ChurchAspect(ArcString name, ArcString desc, ArcBlock potential, ArcBlock allow, ArcBlock modifier, ArcBlock effect, ArcBlock aiWillDo, ArcString id, ArcInt cost)
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
            args.Get(ArcBlock.Constructor, "potential", new()),
            args.Get(ArcBlock.Constructor, "allow", new()),
            args.Get(ArcBlock.Constructor, "modifier", new()),
            args.Get(ArcBlock.Constructor, "effect", new()),
            args.Get(ArcBlock.Constructor, "ai_will_do", new("factor = 1")),
            new(id),
            args.Get(ArcInt.Constructor, "cost", new(100))
        );

        ChurchAspects.Add(id, ChurchAspect);

        return i;
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result, Compiler comp) { result.Add(Id.Value); return i; }
    public static string Transpile()
    {
        StringBuilder sb = new("");
        foreach (ChurchAspect ChurchAspect in ChurchAspects.Values())
        {
            sb.Append($"{ChurchAspect.Id} = {{ cost = {ChurchAspect.Cost} {ChurchAspect.Potential.Compile("potential")} {ChurchAspect.Allow.Compile("allow")} {ChurchAspect.Modifier.Compile("modifier")} {ChurchAspect.Effect.Compile("effect")} {ChurchAspect.AiWillDo.Compile("ai_will_do")} }}");
            Instance.Localisation.Add($"{ChurchAspect.Id}", ChurchAspect.Name.Value);
            Instance.Localisation.Add($"desc_{ChurchAspect.Id}", ChurchAspect.Desc.Value);
        }
        Instance.OverwriteFile("target/common/church_aspects/ChurchAspects.txt", sb.ToString());
        return "Church Aspects";
    }
}