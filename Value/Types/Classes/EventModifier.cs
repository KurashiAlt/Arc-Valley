using ArcInstance;
using Pastel;
using System.Text;

namespace Arc;
public class EventModifier : IArcObject
{
    public static readonly Dict<EventModifier> EventModifiers = new();
    public bool IsObject() => true;
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcModifier Modifier { get; set; }
    public Dict<IVariable> KeyValuePairs { get; set; }
    public EventModifier(string key, ArcString name, ArcModifier modifier)
    {
        Id = new(key);
        Name = name;
        Modifier = modifier;
        KeyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "modifier", Modifier }
        };

        EventModifiers.Add(key, this);
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);
        EventModifier mod = new(
            id,
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcModifier.Constructor, "modifier")
        );

        return i;
    }
    public Walker Call(Walker i, ref Block result) => throw new Exception();
    public static string Transpile()
    {
        StringBuilder sb = new();

        foreach (EventModifier mod in EventModifiers.Values())
        {
            sb.Append($"{mod.Id} = {{ {mod.Modifier.Compile()} }}");
            Instance.Localisation.Add(mod.Id.Value, mod.Name.Value);
        }

        Instance.OverwriteFile("target/common/event_modifiers/arc.txt", sb.ToString());
        return "Event Modifiers";
    }
}
