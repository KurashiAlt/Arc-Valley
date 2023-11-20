using Arc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class WarSide : ArcObject
{
    public static WarSide Constructor(Block b)
    {
        Args.GetArgs(new(b), out Args args, 2);
        return Constructor(args);
    }
    public static WarSide Constructor(Args args) => new()
    {
        { "badboy_factor", args.Get(ArcFloat.Constructor, "badboy_factor", new(1)) },
        { "prestige_factor", args.Get(ArcFloat.Constructor, "prestige_factor", new(1)) },
        { "peace_cost_factor", args.Get(ArcFloat.Constructor, "peace_cost_factor", new(1)) },
        { "required_treaty_to_take_provinces", args.Get(ArcTrigger.Constructor, "required_treaty_to_take_provinces", new()) },
        { "allowed_provinces", args.Get(ArcTrigger.Constructor, "allowed_provinces", new()) },
        { "allowed_tribal_provinces", args.Get(ArcTrigger.Constructor, "allowed_tribal_provinces", new()) },
        { "allowed_provinces_are_eligible", args.Get(ArcBool.Constructor, "allowed_provinces_are_eligible", new(false)) },
        { "allow_annex", args.Get(ArcBool.Constructor, "allow_annex", new(false)) },
        { "deny_annex", args.Get(ArcBool.Constructor, "deny_annex", new(false)) },
        { "peace_options", args.Get(ArcCode.Constructor, "peace_options", new()) },
        { "transfer_trade_cost_factor", args.Get(ArcFloat.Constructor, "transfer_trade_cost_factor", null) },
        { "is_excommunication", args.Get(ArcBool.Constructor, "is_excommunication", new(false)) },
        { "country_desc", args.Get(ArcString.Constructor, "country_desc", null) },
        { "prov_desc", args.Get(ArcString.Constructor, "prov_desc", null) },
    };
    public override string ToString() => Get("id").ToString();
    public override Walker Call(Walker i, ref Block result) { result.Add(ToString()); return i; }
    public void Transpile(ref Block s, string cb, string side)
    {
        s.Add(
            side, "=", "{",
                "badboy_factor", "=", Get("badboy_factor").ToString(),
                "prestige_factor", "=", Get("prestige_factor").ToString(),
                "peace_cost_factor", "=", Get("peace_cost_factor").ToString(),
                Get<ArcBlock>("required_treaty_to_take_provinces").Compile("required_treaty_to_take_provinces"),
                Get<ArcBlock>("allowed_provinces").Compile("allowed_provinces"),
                Get<ArcBlock>("allowed_tribal_provinces").Compile("allowed_tribal_provinces"),
                Get<ArcBlock>("peace_options").Compile("peace_options")
        );
        if (Get<ArcBool>("allow_annex").Value == true) s.Add("allow_annex", "=", "yes");
        if (Get<ArcBool>("deny_annex").Value == true) s.Add("deny_annex", "=", "yes");
        if (GetNullable("transfer_trade_cost_factor") != null) s.Add("transfer_trade_cost_factor", "=", Get("transfer_trade_cost_factor").ToString());
        if (Get<ArcBool>("is_excommunication").Value == true) s.Add("is_excommunication", "=", "yes");
        if (GetNullable("country_desc") != null)
        {
            s.Add("country_desc", "=", $"{cb}_country_desc");
            Program.Localisation.Add($"{cb}_country_desc", Get("country_desc").ToString());
        }
        if (GetNullable("prov_desc") != null)
        {
            s.Add("prov_desc", "=", $"{cb}_{side}_prov_desc");
            Program.Localisation.Add($"{cb}_{side}_prov_desc", Get("prov_desc").ToString());
        }
        s.Add("}");
    }
}
public class WarGoal : ArcObject
{
    public static readonly Dict<WarGoal> WarGoals = new();
    public WarGoal(string id) { WarGoals.Add(id, this); }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static WarGoal Constructor(string id, Args args) => new(id)
    {
        { "id", new ArcString(id) },
        { "type", args.Get(ArcString.Constructor, "type") },
        { "title", args.Get(ArcString.Constructor, "title") },
        { "war_name", args.Get(ArcString.Constructor, "war_name") },
        { "attacker", args.Get(WarSide.Constructor, "attacker") },
        { "defender", args.Get(WarSide.Constructor, "defender") },
    };
    public override string ToString() => Get("id").ToString();
    public override Walker Call(Walker i, ref Block result) { result.Add(ToString()); return i; }
    public void Transpile(ref Block s)
    {
        string id = Get("id").ToString();
        Program.Localisation.Add($"{id}_title", Get("title").ToString());
        Program.Localisation.Add($"{id.ToUpper()}_WAR_NAME", Get("war_name").ToString());
        s.Add(
            id, "=", "{",
                "type", "=", Get("type").ToString(),
                "war_name", "=", $"{id.ToUpper()}_WAR_NAME"
        );
        Get<WarSide>("attacker").Transpile(ref s, id, "attacker");
        Get<WarSide>("defender").Transpile(ref s, id, "defender");
        s.Add("}");
    }
    public static string Transpile()
    {
        Block s = new();
        foreach (KeyValuePair<string, WarGoal> Advisor in WarGoals)
        {
            Advisor.Value.Transpile(ref s);
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/common/wargoal_types/arc.txt", string.Join(' ', s));
        return "War Goals";
    }
}
public class CasusBelli : ArcObject
{
    public static readonly Dict<CasusBelli> CasusBellies = new();
    public CasusBelli(string id) { CasusBellies.Add(id, this); }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static CasusBelli Constructor(string id, Args args) => new(id)
    {
        { "id", new ArcString(id) },
        { "name", args.Get(ArcString.Constructor, "name") },
        { "desc", args.Get(ArcString.Constructor, "desc", new("")) },
        { "valid_for_subject", args.Get(ArcBool.Constructor, "valid_for_subject", new(false)) },
        { "is_triggered_only", args.Get(ArcBool.Constructor, "is_triggered_only", new(false)) },
        { "months", args.Get(ArcInt.Constructor, "months", null) },
        { "prerequisites_self", args.Get(ArcTrigger.Constructor, "prerequisites_self", new()) },
        { "prerequisites", args.Get(ArcTrigger.Constructor, "prerequisites", new()) },
        { "war_goal", args.GetFromList(WarGoal.WarGoals, "war_goal") }
    };
    public override string ToString() => Get("id").ToString();
    public override Walker Call(Walker i, ref Block result) { result.Add(ToString()); return i; }
    public void Transpile(ref Block s)
    {
        string id = Get("id").ToString();
        Program.Localisation.Add($"{id}", Get("name").ToString());
        Program.Localisation.Add($"{id}_desc", Get("desc").ToString());
        s.Add(
            id, "=", "{",
                "valid_for_subject", "=", Get("valid_for_subject")
        );
        if (Get<ArcBool>("is_triggered_only").Value == true) s.Add("is_triggered_only", "=", "yes");
        if (GetNullable("months") != null) s.Add("months", "=", Get("months").ToString());
        s.Add(
                Get<ArcBlock>("prerequisites_self").Compile("prerequisites_self"),
                Get<ArcBlock>("prerequisites").Compile("prerequisites"),
                "war_goal", "=", Get("war_goal").ToString(),
            "}"
        );
    }
    public static string Transpile()
    {
        Block s = new();
        foreach (KeyValuePair<string, CasusBelli> Advisor in CasusBellies)
        {
            Advisor.Value.Transpile(ref s);
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/common/cb_types/arc.txt", string.Join(' ', s));
        return "Casus Bellies";
    }
}