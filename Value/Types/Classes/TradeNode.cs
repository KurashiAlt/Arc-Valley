using ArcInstance;
using Pastel;
using System.ComponentModel;
using System.Text;

namespace Arc;
public class Outgoing : IArcObject
{
    public TradeNode Node { get; set; }
    public ArcList<Province> Path { get; set; }
    public ArcBlock Control { get; set; }

    public Dict<IVariable> keyValuePairs { get; set; }
    public bool CanGet(string key) => keyValuePairs.CanGet(key);
    public IVariable? Get(string key) => keyValuePairs.Get(key);

    public Outgoing(TradeNode node, ArcList<Province> path, ArcBlock control) 
    {
        Node = node;
        Path = path;
        Control = control;
        keyValuePairs = new Dict<IVariable>()
        {
            { "node", Node },
            { "path", Path },
            { "control", Control }
        };
    }

    public string Transpile()
    {
        StringBuilder sb = new();
        sb.Append("outgoing = { name = ");
        sb.Append(Node.Id);
        sb.Append(" path = { ");
        foreach(Province? prov in Path.Values)
        {
            if(prov == null) continue;
            sb.Append(prov.Id);
            sb.Append(" ");
        }
        sb.Append("}");
        sb.Append(" control = { ");
        sb.Append(Control.Compile());
        sb.Append(" }");
        sb.Append(" } ");
        return sb.ToString();
    }

    public Walker Call(Walker i, ref Block result)
    {
        throw new NotImplementedException();
    }

    public static Outgoing Constructor(Block block)
    {
        Walker i = new(block);

        i = Args.GetArgs(i, out Args args, 2);

        return new Outgoing(
            args.GetFromList(TradeNode.TradeNodes, "node"),
            args.Get((Block s) => new ArcList<Province>(s, Province.Provinces), "path", new()),
            args.Get(ArcCode.Constructor, "control", new())
        );
    }
}
public class TradeNode : IArcObject
{
    public static readonly Dict<TradeNode> TradeNodes = new();
    public string Class => "TradeNode";
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcList<Area> Areas { get; set; }
    public ArcList<Outgoing> Outgoings { get; set; }
    public Province Location { get; set; }
    public ArcBool AiWillPropagateThroughTrade { get; set; }

    public Dict<IVariable> keyValuePairs { get; set; }
    public TradeNode(ArcString id, ArcString name, ArcList<Area> areas, ArcList<Outgoing> outgoings, Province location, ArcBool aiWillPropogateTrade)
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
            args.Get(ArcList<Area>.GetConstructor(Area.Areas), "areas"),
            args.Get(ArcList<Outgoing>.GetConstructor(Outgoing.Constructor), "outgoings", new()),
            args.GetFromList(Province.Provinces, "location"),
            args.Get(ArcBool.Constructor, "ai_will_propagate_through_trade", new(false))
        );

        TradeNodes.Add(id, TradeNode);

        return i;
    }

    public override string ToString() => Name.Value;
    public static string Transpile()
    {
        StringBuilder sb = new();
        
        foreach(TradeNode node in TradeNodes.Values().Reverse())
        {
            sb.Append($"{node.Id} = {{ ");
            sb.Append($"location = {node.Location.Id} ");
            if (node.AiWillPropagateThroughTrade) sb.Append("ai_will_propagate_through_trade = yes ");
            foreach(Outgoing? outgoing in node.Outgoings.Values)
            {
                if (outgoing == null) continue;
                sb.Append(outgoing.Transpile());
            }
            sb.Append("members = { ");
            foreach(Province prov in from provi in Province.Provinces where node.Areas.Values.Contains(provi.Value.Area) select provi.Value)
            {
                sb.Append($"{prov.Id} ");
            }
            sb.Append("} ");
            if(node.Outgoings.Values.Count == 0)
            {
                sb.Append("end = yes ");
            }
            sb.Append("} ");
            Program.Localisation.Add(node.Id.Value, node.Name.Value);
        }

        Program.OverwriteFile($"{Program.TranspileTarget}/common/tradenodes/arc.txt", sb.ToString());
        return "Trade Nodes";
    }
    public Walker Call(Walker i, ref Block result)
    {
        throw new NotImplementedException();
    }
}