using Arc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class Policy : ArcObject
{
    public static readonly Dict<Policy> Policies = new();
    public Policy(string id) { Policies.Add(id, this); }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static Policy Constructor(string id, Args args) {
        Policy policy = new(id)
        {
            { "id", new ArcString($"{id}") },
            { "name", args.Get(ArcString.Constructor, "name", null) },
            { "desc", args.Get(ArcString.Constructor, "desc", null) },
            { "monarch_power", args.Get(ArcString.Constructor, "monarch_power") },
            { "potential", args.Get(ArcTrigger.Constructor, "potential", new()) },
            { "allow", args.Get(ArcTrigger.Constructor, "allow", new()) },
            { "modifier", args.Get(ArcModifier.Constructor, "modifier", new()) },
            { "effect", args.Get(ArcEffect.Constructor, "effect", new()) },
            { "removed_effect", args.Get(ArcEffect.Constructor, "removed_effect", new()) },
            { "ai_will_do", args.Get(ArcTrigger.Constructor, "ai_will_do", new("factor", "=", "1")) },
        };

        policy.Add("group_1", args.Get(ArcType.Types["idea_group"].ThisConstructor, "group_1"));
        policy.Add("group_2", args.Get(ArcType.Types["idea_group"].ThisConstructor, "group_2"));

        return policy;
    } 
    public override string ToString() => Get("id").ToString();
    public override Walker Call(Walker i, ref Block result) { result.Add(ToString()); return i; }
    public void Transpile(ref Block s)
    {
        string id = ToString();
        if (CanGet("name")) Program.Localisation.Add($"{id}", Get("name").ToString());
        if (CanGet("desc")) Program.Localisation.Add($"desc_{id}", Get("desc").ToString());

        IVariable i1 = Get<ArcObject>("group_1").Get("id");
        IVariable i2 = Get<ArcObject>("group_2").Get("id");

        s.Add(
            id, "=", "{",
                "monarch_power", "=", Get("monarch_power"),
                "potential", "=", "{",
                    Get<ArcTrigger>("potential").Compile());

        s.Add(
            "has_idea_group", "=", i1,
            "has_idea_group", "=", i2
        );

        s.Add(
                "}",
                "allow", "=", "{",
                    Get<ArcTrigger>("allow").Compile()
        );

        s.Add(
            "full_idea_group", "=", i1,
            "full_idea_group", "=", i2
        );

        s.Add(
                "}",
                Get<ArcModifier>("modifier").Compile(),
                Get<ArcEffect>("effect").Compile("effect"),
                Get<ArcEffect>("removed_effect").Compile("removed_effect"),
                Get<ArcTrigger>("ai_will_do").Compile("ai_will_do"),
            "}"
        );
    }
    public static string Transpile()
    {
        Block s = new();
        foreach (KeyValuePair<string, Policy> Policy in Policies)
        {
            Policy.Value.Transpile(ref s);
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/common/policies/arc.txt", string.Join(' ', s));
        return "Policies";
    }
}