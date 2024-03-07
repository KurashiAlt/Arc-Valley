
using Pastel;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace Arc;
public class Unit : IArcObject
{
    public static readonly Dict<Unit> Units = new();
    public bool IsObject() => true;
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }
     
    public ArcString Type { get; set; }
    public ArcString? UnitType { get; set; }
     
    public ArcFloat? Manpower { get; set; }
    public ArcTrigger Trigger { get; set; }

    public Dict<ArcCode> Attributes { get; set; } //Used for non implemented Attributes
    public static string[] ImplementedAttributes = new string[]
    {
        "name", "desc", "type", "unit_type", "manpower", "trigger"
    };
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public Unit(
        string id,
        ArcString name,
        ArcString desc,
        ArcString type,
        ArcString? unitType,
        ArcFloat? manpower,
        ArcTrigger trigger,
        Dict<ArcCode> attributes
    ) {
        Id = new(id);
        Name = name; 
        Desc = desc;
        Type = type;
        UnitType = unitType;
        Manpower = manpower;
        Trigger = trigger;
        Attributes = attributes;

        KeyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "desc", Desc },
            { "type", Type },
            { "unit_type", UnitType },
            { "manpower", Manpower },
            { "trigger", Trigger },
            { "attributes", Attributes },
        };

        Units.Add(id, this);
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
    public static Unit Constructor(string id, Args args)
    {
        return new Unit(id,
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc"),
            args.Get(ArcString.Constructor, "type"),
            args.Get(ArcString.Constructor, "unit_type", null),
            args.Get(ArcFloat.Constructor, "manpower", null),
            args.Get(ArcTrigger.Constructor, "trigger", new()),
            args.GetAttributes(ImplementedAttributes)
        );
    }
    private void TranspileSingular()
    {
        Program.Localisation.Add($"{Id}", Name.Value);
        Program.Localisation.Add($"{Id}DESCR", Desc.Value);
        Block b = new()
        {
            { "type", "=", Type },
        };
        Trigger.Compile("trigger", ref b);
        if (Manpower != null) b.Add("manpower", "=", Manpower);
        if (UnitType != null) b.Add("unit_type", "=", UnitType);
        foreach(KeyValuePair<string, ArcCode> attribute in Attributes)
        {
            attribute.Value.Compile(attribute.Key, ref b, true, true);
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/common/units/{Id}.txt", string.Join(' ', b));
    }
    public static string Transpile()
    {
        foreach(Unit unit in Units.Values())
        {
            unit.TranspileSingular();
        }
        return "Units";
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}
