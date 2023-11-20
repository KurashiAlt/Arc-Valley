
using Pastel;
using System.ComponentModel;
using System.Text;
using System.Xml.Linq;

namespace Arc;
public class Idea : IArcObject
{
    public string Class => "Idea";
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }
    public ArcModifier Modifier { get; set; }
    public ArcEffect Effect { get; set; }
    public ArcEffect RemovedEffect { get; set; }
    public Dict<IVariable> keyValuePairs { get; set; }
    public Idea(
        string key, 
        ArcString name, 
        ArcString desc, 
        ArcModifier modifier,
        ArcEffect effect,
        ArcEffect removedEffect
    ) {
        Name = name;
        Desc = desc;
        Modifier = modifier;
        Effect = effect;
        RemovedEffect = removedEffect;
        keyValuePairs = new()
        {
            { "name", Name },
            { "desc", Desc },
            { "modifier", Modifier },
            { "effect", Effect },
            { "removed_effect", RemovedEffect },
        };

        Id = new(key);
    }

    public bool CanGet(string indexer) => keyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => keyValuePairs.Get(indexer);
    public override string ToString() => Name.Value;
    public string Transpile(IdeaGroup ideaGroup)
    {
        Block sb = new()
        {
            { 
                Id, "=", "{", 
                    Modifier.Compile(), 
                    Effect.Compile("effect"),
                    RemovedEffect.Compile("removed_effect"),
                "}" 
            }
        };

        Program.Localisation.Add($"{Id}", Name.Value);
        if (Id.Value.EndsWith("7"))
        {
            bool addedSpacing = false;
            StringBuilder desc = new(Desc.Value.Trim('"'));

            IEnumerable<string> a = from idea 
                in IdeaGroup.IdeaGroups 
                where string.Join(' ', idea.Value.Trigger.Value).Contains(ideaGroup.Id.Value) 
                orderby idea.Value.Name.Value 
                select idea.Value.Name.Value.Trim('"');
            if (a.Any())
            {
                addedSpacing = true;
                desc.Append("\\n");
                if(a.Count() == 1)
                {
                    desc.Append($"\\nUnlocks §O{a.First()}§! Ideas");
                }
                else
                {
                    desc.Append($"\\nUnlocks §O{string.Join(", ", from c in a where c != a.Last() select c)}, and {a.Last()}§! Ideas");
                }
            }

            IEnumerable<string> b = from building 
                in Building.Buildings where
                building.Value.GetNullable<ArcInt>("unlock_tier") != null && 
                building.Value.GetNullable<ArcInt>("unlock_tier").Value == 1 && 
                building.Value.GetNullable<ArcList<IdeaGroup>>("idea_group_unlocks") != null && 
                building.Value.GetNullable<ArcList<IdeaGroup>>("idea_group_unlocks").Values.Contains(ideaGroup) 
                orderby building.Value.Get<ArcString>("name").Value 
                select building.Key[..1].ToUpper() + building.Key[1..^2];
            if (b.Any())
            {
                if (!addedSpacing) desc.Append("\\n");
                if(b.Count() == 1)
                {
                    desc.Append($"\\nUnlocks the next tier of §O{b.First()}§! Buildings");
                }
                else
                {
                    desc.Append($"\\nUnlocks the next tier of §O{string.Join(", ", from c in b where c != b.Last() select c)}, and {b.Last()}§! Buildings");
                }
            }

            Program.Localisation.Add($"{Id}_desc", desc.ToString());
        }
        else
        {
            Program.Localisation.Add($"{Id}_desc", Desc.Value);
        }
        return string.Join(' ', sb);
    }
    public Walker Call(Walker i, ref Block result)
    {
        throw new NotImplementedException();
    }
    public static Idea Constructor(Block block, string key, int num)
    {
        Walker i = new(block);

        i = Args.GetArgs(i, out Args args, 2);

        return new Idea(
            $"{key}_{num}",
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc"),
            args.Get(ArcModifier.Constructor, "modifier", new()),
            args.Get(ArcEffect.Constructor, "effect", new()),
            args.Get(ArcEffect.Constructor, "removed_effect", new())
        );
    }
}
public class IdeaGroup : IArcObject
{
    public static Dict<IdeaGroup> IdeaGroups = new();
    public bool IsObject() => true;
    public ArcString Id { get; set; }
    public ArcInt Priority { get; set; }
    public ArcString Name { get; set; }
    public ArcString Category { get; set; }
    public ArcList<Idea> Ideas { get; set; }
    public ArcModifier Start { get; set; } 
    public ArcModifier Bonus { get; set; } 
    public ArcTrigger Trigger { get; set; }
    public ArcCode AiWillDo { get; set; }
    public Dict<IVariable> keyValuePairs { get; set; }
    public IdeaGroup(
        string id, 
        ArcInt priority, 
        ArcString name, 
        ArcString category, 
        ArcList<Idea> ideas, 
        ArcModifier start, 
        ArcModifier bonus, 
        ArcTrigger trigger, 
        ArcCode aiWillDo
    ) {
        Id = new(id);
        Priority = priority;
        Name = name;
        Category = category;
        Ideas = ideas;
        Start = start;
        Bonus = bonus;
        Trigger = trigger;
        AiWillDo = aiWillDo;
        keyValuePairs = new()
        {
            { "name", Name },
            { "priority", Priority },
            { "category", Category },
            { "start", Start },
            { "bonus", Bonus },
            { "ideas", Ideas },
            { "trigger", Trigger },
            { "ai_will_do", AiWillDo },
        };
    }

    public bool CanGet(string indexer) => keyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => keyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = Compiler.GetId(i.Current);
        try
        {
            i = Args.GetArgs(i, out Args args);

            IdeaGroups.Add(id, new(
                id,
                args.Get(ArcInt.Constructor, "priority", new(100)),
                args.Get(ArcString.Constructor, "name"),
                args.Get(ArcString.Constructor, "category"),
                args.Get((Block s) => new ArcList<Idea>(s, (Block s, int num) => Idea.Constructor(s, id, num + 1)), "ideas"),
                args.Get(ArcModifier.Constructor, "start", new()),
                args.Get(ArcModifier.Constructor, "bonus"),
                args.Get(ArcTrigger.Constructor, "trigger", new()),
                args.Get(ArcCode.Constructor, "ai_will_do", new())
            ));
            return i;
        }   
        catch(Exception e)
        {
            throw;
        }
    }

    public override string ToString() => Id.Value;
    public static string Transpile()
    {
        StringBuilder sb = new();
        foreach(IdeaGroup ideaGroup in from c in IdeaGroups orderby c.Value.Priority.Value descending select c.Value)
        {
            sb.Append($"{ideaGroup.Id} = {{ ");

            if (ideaGroup.Category.Value == "national")
            {
                Program.Localisation.Add($"{ideaGroup.Id}_start", $"{ideaGroup.Name.Value.Trim('"')} Traditions");
                sb.Append($"free = yes ");
            }
            else
            {
                sb.Append($"category = {ideaGroup.Category} ");
            }

            sb.Append(ideaGroup.Trigger.Compile("trigger"));
            sb.Append(' ');
            sb.Append(ideaGroup.Start.Compile("start"));
            sb.Append(' ');

            if (ideaGroup.Ideas.Values.Count != 7) throw new Exception($"Cannot create an idea group with more or less than 7 ideas, {ideaGroup.Id}");
            foreach(Idea? idea in ideaGroup.Ideas.Values)
            {
                if (idea == null) continue;

                sb.Append(idea.Transpile(ideaGroup));
                sb.Append(' ');
            }

            sb.Append(ideaGroup.Bonus.Compile("bonus"));
            sb.Append(' ');
            sb.Append(ideaGroup.AiWillDo.Compile("ai_will_do"));

            sb.Append(" } ");

            Program.Localisation.Add($"{ideaGroup.Id}", $"{ideaGroup.Name.Value.Trim('"')} Ideas");
            Program.Localisation.Add($"{ideaGroup.Id}_bonus", $"{ideaGroup.Name.Value.Trim('"')} Ambitions");
        }

        Program.OverwriteFile($"{Program.TranspileTarget}/common/ideas/arc.txt", sb.ToString());
        return "Idea Groups";
    }
    public Walker Call(Walker i, ref Block result)
    {
        throw new NotImplementedException();
    }
}