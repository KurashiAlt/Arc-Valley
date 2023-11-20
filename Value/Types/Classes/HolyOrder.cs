using Arc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class HolyOrder : ArcObject
{
    public static readonly Dict<HolyOrder> HolyOrders = new();
    public HolyOrder(string id)
    {
        HolyOrders.Add(id, this);
    }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static HolyOrder Constructor(string id, Args args) => new(id)
    {
        { "id", new ArcString(id) },
        { "name", args.Get(ArcString.Constructor, "name") },
        { "desc", args.Get(ArcString.Constructor, "desc", new("")) },
        { "icon", args.Get(ArcTrigger.Constructor, "icon") },
        { "trigger", args.Get(ArcTrigger.Constructor, "trigger") },
        { "color", args.Get(ArcCode.Constructor, "color") },
        { "cost", args.Get(ArcInt.Constructor, "cost", new(50)) },
        { "cost_type", args.Get(ArcString.Constructor, "cost_type") },
        { "on_apply", args.Get(ArcEffect.Constructor, "on_apply", new()) },
        { "on_remove", args.Get(ArcEffect.Constructor, "on_remove", new()) },
        { "modifier", args.Get(ArcModifier.Constructor, "modifier", new()) },
        { "ai_priority", args.Get(ArcCode.Constructor, "ai_priority", new("factor", "=", "2")) },
        { "localization", args.Get(ArcString.Constructor, "localization", new("holy_order")) },
    };
    public override string ToString() => Get("id").ToString();
    public void Transpile(ref Block b)
    {
        string id = Get("id").ToString();
        Program.Localisation.Add($"{id}", Get("name").ToString());
        Program.Localisation.Add($"{id}_desc", Get("desc").ToString());

        b.Add(
            id, "=", "{",
                "icon", "=", $"GFX_holy_order_{Get("icon")}",
                Get<ArcTrigger>("trigger").Compile("trigger"),
                Get<ArcCode>("color").Compile("color"),
                "cost", "=", Get("cost"),
                "cost_type", "=", Get("cost_type"),
                Get<ArcEffect>("on_apply").Compile("per_province_effect"),
                Get<ArcEffect>("on_remove").Compile("per_province_abandon_effect"),
                Get<ArcModifier>("modifier").Compile("modifier"),
                Get<ArcCode>("ai_priority").Compile("ai_priority"),
                "localization", "=", Get("localization"),
            "}"
        );
    }
    public static string Transpile()
    {
        Block b = new();
        foreach (KeyValuePair<string, HolyOrder> HolyOrder in HolyOrders)
        {
            HolyOrder.Value.Transpile(ref b);
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/common/holy_orders/arc.txt", string.Join(' ', b));
        return "Holy Orders";
    }
    public override Walker Call(Walker i, ref Block result) 
    {
        result.Add(ToString());
        return i;
    }
}
