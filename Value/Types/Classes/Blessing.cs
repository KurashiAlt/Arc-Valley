using ArcInstance;
using Pastel;
using System.IO;
using System.Text;

namespace Arc;
public class Blessing : IArcObject
{
    public static readonly Dict<Blessing> Blessings = new();
    public bool IsObject() => true;
    public string Class => "Blessing";
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }
    public ArcBlock Potential { get; set; }
    public ArcBlock Modifier { get; set; }
    public ArcBlock Effect { get; set; }
    public ArcBlock AiWillDo { get; set; }
    public ArcString Id { get; set; }
    public Dict<IValue> KeyValuePairs { get; set; }
    public Blessing(ArcString name, ArcString desc, ArcBlock potential, ArcBlock modifier, ArcBlock effect, ArcBlock aiWillDo, ArcString id)
    {
        Name = name;
        Desc = desc;
        Potential = potential;
        Modifier = modifier;
        Effect = effect;
        AiWillDo = aiWillDo;
        Id = id;
        KeyValuePairs = new Dict<IValue>()
        {
            { "name", Name },
            { "desc", Desc },
            { "potential", Potential },
            { "modifier", Modifier },
            { "effect", Effect },
            { "ai_will_do", AiWillDo },
            { "id", Id },
        };
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);

        Blessing Blessing = new(
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc"),
            args.Get(ArcBlock.Constructor, "potential"),
            args.Get(ArcBlock.Constructor, "modifier"),
            args.Get(ArcBlock.Constructor, "effect"),
            args.Get(ArcBlock.Constructor, "ai_will_do"),
            new(id)
        );

        Blessings.Add(id, Blessing);

        return i;
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref List<string> result, Compiler comp) { result.Add(Id.Value); return i; }
    public static void Transpile()
    {
        StringBuilder sb = new("");
        foreach (Blessing blessing in Blessings.Values())
        {
            sb.Append($"{blessing.Id} = {{ is_blessing = yes potential = {{ {blessing.Potential.Compile()} }} modifier = {{ {blessing.Modifier.Compile()} }} effect = {{ {blessing.Effect.Compile()} }} ai_will_do = {{ {blessing.AiWillDo.Compile()} }} }}");
            Instance.Localisation.Add($"{blessing.Id}", blessing.Name.Value);
            Instance.Localisation.Add($"desc_{blessing.Id}", blessing.Desc.Value);
        }
        Instance.OverwriteFile("target/common/church_aspects/blessings.txt", sb.ToString());
        Console.WriteLine($"Finished Transpiling Blessings".Pastel(ConsoleColor.Cyan));
    }
}