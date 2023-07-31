using ArcInstance;
using Pastel;
using System.ComponentModel;
using System.Text;

namespace Arc;
public class TradeNode : IArcObject
{
    public static readonly Dict<TradeNode> TradeNodes = new();
    public string Class => "TradeNode";
    ArcString Id { get; set; }
    ArcString Name { get; set; }
    ArcList<Area> Areas { get; set; }
    ArcList<ArcBlock> Outgoings { get; set; }
    Province Location { get; set; }
    ArcBool AiWillPropagateThroughTrade { get; set; }

    public Dict<IVariable> keyValuePairs { get; set; }
    public TradeNode(ArcString id, ArcString name, ArcList<Area> areas, ArcList<ArcBlock> outgoings, Province location, ArcBool aiWillPropogateTrade)
    {
        Id = id;
        Name = name;
        Areas = areas;
        Outgoings = outgoings;
        Location = location;
        AiWillPropagateThroughTrade = aiWillPropogateTrade;
        keyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "areas", Areas },
            { "outgoings", Outgoings },
            { "location", Location },
            { "ai_will_propagate_through_trade", AiWillPropagateThroughTrade },
        };
    }

    public bool CanGet(string indexer) => keyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => keyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);

        TradeNode TradeNode = new(
            new($"{id}_tn"),
            args.Get(ArcString.Constructor, "name"),
            args.Get((Block s) => new ArcList<Area>(s, Area.Areas), "areas"),
            args.Get((Block s) => new ArcList<ArcBlock>(s, ArcBlock.Constructor), "outgoings", new()),
            args.GetFromList(Province.Provinces, "location"),
            args.Get(ArcBool.Constructor, "ai_will_propagate_through_trade", new(false))
        );

        TradeNodes.Add(id, TradeNode);

        return i;
    }

    public override string ToString() => Name.Value;
    public static void Transpile()
    {
        StringBuilder sb = new();
        
        foreach(TradeNode node in TradeNodes.Values())
        {
            sb.Append($"{node.Id} = {{ ");
            sb.Append($"location = {node.Location.Id} ");
            if (node.AiWillPropagateThroughTrade) sb.Append("ai_will_propagate_through_trade = yes ");
            foreach(ArcBlock? outgoing in node.Outgoings.Values)
            {
                if (outgoing == null) continue;
                sb.Append($"outgoing = {{ {outgoing.Compile()} }} ");
            }
            sb.Append("members = { ");
            foreach(Province prov in from provi in Province.Provinces where node.Areas.Values.Contains(provi.Value.Area) select provi.Value)
            {
                sb.Append($"{prov.Id} ");
            }
            sb.Append("} }");
            Instance.Localisation.Add(node.Id.Value, node.Name.Value);
        }

        Instance.OverwriteFile("target/common/tradenodes/arc.txt", sb.ToString());
        Console.WriteLine($"Finished Transpiling Trade Nodes".Pastel(ConsoleColor.Cyan));
    }
    public Walker Call(Walker i, ref List<string> result, Compiler comp)
    {
        throw new NotImplementedException();
    }
}