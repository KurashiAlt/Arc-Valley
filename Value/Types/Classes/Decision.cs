
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
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }
    public ArcBool Major { get; set; }
    public ArcCode? Color { get; set; }
    public ArcTrigger ProvincesToHighlight { get; set; }
    public ArcTrigger Potential { get; set; }
    public ArcTrigger Allow { get; set; }
    public ArcEffect Effect { get; set; }
    public ArcCode AiWillDo { get; set; }
    public ArcInt AiImportance { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public Decision(
        string key,
        ArcString name,
        ArcString desc,
        ArcBool major,
        ArcCode? color,
        ArcTrigger provincesToHighlight,
        ArcTrigger potential,
        ArcTrigger allow,
        ArcEffect effect,
        ArcCode aiWillDo,
        ArcInt aiImportance
    )
    {
        Decisions.Add(key, this);
        Id = new(key);
        Name = name;
        Desc = desc;
        Major = major;
        Color = color;
        ProvincesToHighlight = provincesToHighlight;
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
            { "provinces_to_highlight", ProvincesToHighlight },
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

        string id = Compiler.GetId(i.Current);

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
            args.Get(ArcCode.Constructor, "color", null),
            args.Get(ArcTrigger.Constructor, "provinces_to_highlight", new()),
            args.Get(ArcTrigger.Constructor, "potential", new()),
            args.Get(ArcTrigger.Constructor, "allow", new()),
            args.Get(ArcEffect.Constructor, "effect", new()),
            args.Get(ArcCode.Constructor, "ai_will_do", new()),
            args.Get(ArcInt.Constructor, "ai_importance", new(400))
        );
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value); return i; }
    public void Transpile(ref Block s)
    {
        Program.Localisation.Add($"{Id}_title", Name.Value);
        Program.Localisation.Add($"{Id}_desc", Desc.Value);
        s.Add(
            Id, "=", "{",
                "major", "=", Major
        );
        Color?.Compile("color", ref s);
        ProvincesToHighlight.Compile("provinces_to_highlight", ref s);
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
        Program.OverwriteFile($"{Program.TranspileTarget}/decisions/arc.txt", string.Join(' ', s));
        return "Decisions";
    }
}
