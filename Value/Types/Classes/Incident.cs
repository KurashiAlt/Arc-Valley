using ArcInstance;
using Pastel;
using System.IO;
using System.Linq;
using System.Text;

namespace Arc;
public class Incident : ArcObject
{
    public static readonly Dict<Incident> Incidents = new();
    public static new Walker Call(Walker i) => Call(i, Constructor);
    Incident(string key)
    {
        Incidents.Add(key, this);
    }
    public static Incident Constructor(string id, Args args) => new(id)
    {
        { "id", new ArcString(id) },
        { "name", args.Get(ArcString.Constructor, "name") },
        { "immediate", args.Get(ArcEffect.Constructor, "immediate", new()) },
        { "after", args.Get(ArcEffect.Constructor, "after", new()) },
        { "can_stop", args.Get(ArcTrigger.Constructor, "can_stop", new()) },
        { "options", args.Get((Block s) => new ArcList<Option>(s, Option.Constructor), "options") }
    };
    public void Transpile(ref Block file, int i)
    {
        string id = Get<ArcString>("id").Value;
        string name = Get<ArcString>("name").Value;
        file.Add(id, "=", "{");
        file.Add("event", "=", $"incidents.{i}");
        file.Add("default_option", "=", "0");
        Get<ArcTrigger>("can_stop").Compile("can_stop", ref file);
        file.Add("0", "=", "{", "factor", "=", "1", "}");
        file.Add("1", "=", "{", "factor", "=", "1", "}");
        file.Add("2", "=", "{", "factor", "=", "1", "}");
        file.Add("}");

        _ = new Event(
            $"incidents.{i}",
            new(false),
            new(name),
            new(""),
            new("DEATH_OF_HEIR_eventPicture"),
            new(false),
            new(),
            new(true),
            new(false),
            new(),
            Get<ArcEffect>("immediate"),
            Get<ArcEffect>("after"),
            new(),
            new(true),
            Get<ArcList<Option>>("options"),
            null
        );

        Instance.Localisation.Add($"{id}", name);
    }
    public static string Transpile()
    {
        Block file = new();
        int i = 0;
        foreach (Incident reform in Incidents.Values())
        {
            reform.Transpile(ref file, i);
            i++;
        }
        Instance.OverwriteFile($"{Instance.TranspileTarget}/common/imperial_incidents/arc.txt", string.Join(' ', file));
        return "Incidents";
    }
    public override string ToString() => Get<ArcString>("id").Value;
    public Walker Call(Walker i, ref Block result) { result.Add(ToString()); return i; }
}
