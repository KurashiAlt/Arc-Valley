using Arc;
using ArcInstance;
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
        IdeaGroup? onlyIdeaGroup = args.GetFromListNullable(IdeaGroup.IdeaGroups, "group");
        
        Policy policy = new(id)
        {
            { "id", new ArcString($"{id}_policy") },
            { "name", args.Get(ArcString.Constructor, "name") },
            { "desc", args.Get(ArcString.Constructor, "desc", new("")) },
            { "monarch_power", args.Get(ArcString.Constructor, "monarch_power") },
            { "potential", args.Get(ArcTrigger.Constructor, "potential", new()) },
            { "allow", args.Get(ArcTrigger.Constructor, "allow", new()) },
            { "modifier", args.Get(ArcModifier.Constructor, "modifier", new()) },
            { "effect", args.Get(ArcEffect.Constructor, "effect", new()) },
            { "removed_effect", args.Get(ArcEffect.Constructor, "removed_effect", new()) },
            { "ai_will_do", args.Get(ArcTrigger.Constructor, "ai_will_do", new("factor", "=", "1")) },
        };

        if(onlyIdeaGroup == null)
        {
            policy.Add("group_1", args.GetFromList(IdeaGroup.IdeaGroups, "group_1"));
            policy.Add("group_2", args.GetFromList(IdeaGroup.IdeaGroups, "group_2"));
        }
        else
        {
            policy.Add("group", onlyIdeaGroup);
        }

        return policy;
    } 
    public override string ToString() => Get("id").ToString();
    public override Walker Call(Walker i, ref Block result) { result.Add(ToString()); return i; }
    public void Transpile(ref Block s)
    {
        string id = ToString();
        Program.Localisation.Add($"{id}", Get("name").ToString());
        Program.Localisation.Add($"desc_{id}", Get("desc").ToString());

        IdeaGroup? i0 = GetNullable<IdeaGroup>("group");
        IdeaGroup? i1 = GetNullable<IdeaGroup>("group_1");
        IdeaGroup? i2 = GetNullable<IdeaGroup>("group_2");

        s.Add(
            id, "=", "{",
                "monarch_power", "=", Get("monarch_power"),
                "potential", "=", "{",
                    Get<ArcTrigger>("potential").Compile());
        if(i0 == null)
        {
            s.Add(
                "has_idea_group", "=", i1,
                "has_idea_group", "=", i2
            );
        }
        else
        {
            s.Add(
                "has_idea_group", "=", i0,
                "hidden_trigger", "=", "{",
                    "OR", "=", "{",
                        "has_idea_group", "=", i0,
                        "has_idea_group", "=", "only_ideas",
                    "}",
                "}"
            );
        }
        s.Add(
                "}",
                "allow", "=", "{",
                    Get<ArcTrigger>("allow").Compile()
        );
        if (i0 == null)
        {
            s.Add(
                "full_idea_group", "=", i1,
                "full_idea_group", "=", i2
            );
        }
        else
        {
            s.Add(
                "full_idea_group", "=", i0,
                "hidden_trigger", "=", "{",
                    "OR", "=", "{",
                        "full_idea_group", "=", i0,
                        "full_idea_group", "=", "only_ideas",
                    "}",
                "}"
            );
        }
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