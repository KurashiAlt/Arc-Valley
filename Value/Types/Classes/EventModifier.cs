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
            Program.Localisation.Add(mod.Id.Value, mod.Name.Value);
        }

        Program.OverwriteFile($"{Program.TranspileTarget}/common/event_modifiers/arc.txt", sb.ToString());
        return "Event Modifiers";
    }
}

public class OpinionModifier : ArcObject
{
    public static readonly Dict<OpinionModifier> OpinionModifiers = new();
    OpinionModifier(string key)
    {
        OpinionModifiers.Add(key, this);
    }
    public static OpinionModifier Constructor(string id, Args args) => new(id)
    {
        { "id", new ArcString(id) },
        { "name", args.Get(ArcString.Constructor, "name") },
        { "opinion", args.Get(ArcInt.Constructor, "opinion") },
        { "min", args.Get(ArcInt.Constructor, "min", null) },
        { "max", args.Get(ArcInt.Constructor, "max", null) },
        { "max_vassal", args.Get(ArcInt.Constructor, "max_vassal", null) },
        { "max_in_other_direction", args.Get(ArcInt.Constructor, "max_in_other_direction", null) },
        { "yearly_decay", args.Get(ArcInt.Constructor, "yearly_decay", null) },
        { "months", args.Get(ArcInt.Constructor, "months", null) }
    };
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public override Walker Call(Walker i, ref Block result) 
    {
        result.Add(Get<ArcString>("id").Value);
        return i;
    }
    void TranspileThis(ref Block a)
    {
        string id = Get<ArcString>("id").Value;
        Program.Localisation.Add(id, Get<ArcString>("name").Value);

        a.Add(id, "=", "{");
        a.Add("opinion", "=", Get<ArcInt>("opinion"));

        ArcInt? min = GetNullable<ArcInt>("min");
        if (min != null) a.Add("min", "=", min);
        ArcInt? max = GetNullable<ArcInt>("max");
        if (max != null) a.Add("max", "=", max);
        ArcInt? max_vassal = GetNullable<ArcInt>("max_vassal");
        if (max_vassal != null) a.Add("max_vassal", "=", max_vassal);
        ArcInt? max_in_other_direction = GetNullable<ArcInt>("max_in_other_direction");
        if (max_in_other_direction != null) a.Add("max_in_other_direction", "=", max_in_other_direction);
        ArcInt? yearly_decay = GetNullable<ArcInt>("yearly_decay");
        if (yearly_decay != null) a.Add("yearly_decay", "=", yearly_decay);
        ArcInt? months = GetNullable<ArcInt>("months");
        if (months != null) a.Add("months", "=", months);

        a.Add("}");
    }
    public static string Transpile()
    {
        Block a = new();

        foreach(KeyValuePair<string, OpinionModifier> mod in OpinionModifiers)
        {
            mod.Value.TranspileThis(ref a);
        }

        Program.OverwriteFile($"{Program.TranspileTarget}/common/opinion_modifiers/arc.txt", string.Join(' ', a));
        return "Opinion Modifiers";
    }
}
public class ProvinceTriggeredModifier : ArcObject
{
    public static readonly Dict<ProvinceTriggeredModifier> ProvinceTriggeredModifiers = new();
    public ProvinceTriggeredModifier(string key) { ProvinceTriggeredModifiers.Add(key, this); }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static ProvinceTriggeredModifier Constructor(string id, Args args) => new(id)
    {
        { "id", new ArcString(id) },
        { "name", args.Get(ArcString.Constructor, "name") },
        { "potential", args.Get(ArcTrigger.Constructor, "potential", new()) },
        { "trigger", args.Get(ArcTrigger.Constructor, "trigger", new()) },
        { "modifier", args.Get(ArcModifier.Constructor, "modifier", new()) },
        { "on_apply", args.Get(ArcEffect.Constructor, "on_apply", new()) },
        { "on_remove", args.Get(ArcEffect.Constructor, "on_remove", new()) },
    };
    public override string ToString() => Get("id").ToString();
    public override Walker Call(Walker i, ref Block result) { result.Add(ToString()); return i; }
    public void Transpile(ref Block s)
    {
        string id = Get("id").ToString();
        Program.Localisation.Add(id, Get("name").ToString());
        s.Add(
            id, "=", "{",
                Get<ArcBlock>("modifier").Compile(),
                Get<ArcBlock>("potential").Compile("potential"),
                Get<ArcBlock>("trigger").Compile("trigger"),
                Get<ArcBlock>("on_apply").Compile("on_activation"),
                Get<ArcBlock>("on_remove").Compile("on_deactivation"),
            "}"
        );
    }
    public static string Transpile()
    {
        Block s = new();
        foreach (KeyValuePair<string, ProvinceTriggeredModifier> ProvinceTriggeredModifier in ProvinceTriggeredModifiers)
        {
            ProvinceTriggeredModifier.Value.Transpile(ref s);
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/common/province_triggered_modifiers/arc.txt", string.Join(' ', s));
        return "Province Triggered Modifiers";
    }
}