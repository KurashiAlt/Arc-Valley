using ArcInstance;
using Pastel;
using System.IO;
using System.Linq;
using System.Text;

namespace Arc;
public class Incident : IArcObject
{
    public static readonly Dict<Incident> Incidents = new();
    public bool IsObject() => true;
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public Dict<ArcCode> Attributes { get; set; } 
    public static string[] ImplementedAttributes = new string[]
    {
        "name"
    };
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public Incident(
        string id, 
        ArcString name, 
        Dict<ArcCode> attributes
    ) {
        Id = new(id);
        Name = name;
        Attributes = attributes;
        KeyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "attributes", Attributes },
        };

        Incidents.Add(id, this);
    }

    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);

        Constructor(id, args);

        return i;
    }
    public static Incident Constructor(string id, Args args)
    {
        return new Incident(id,
            args.Get(ArcString.Constructor, "name"),
            args.GetAttributes(ImplementedAttributes)
        );
    }
    public static string Transpile()
    {
        Block file = new();
        foreach (Incident reform in Incidents.Values())
        {
            file.Add(reform.Id, "=", "{");
            foreach (var v in reform.Attributes)
            {
                v.Value.Compile(v.Key, ref file, true, true);
            }
            file.Add("}");

            Instance.Localisation.Add($"{reform.Id}", reform.Name.Value);
        }
        Instance.OverwriteFile($"{Instance.TranspileTarget}/common/imperial_incidents/arc.txt", string.Join(' ', file));
        return "Incidents";
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}
