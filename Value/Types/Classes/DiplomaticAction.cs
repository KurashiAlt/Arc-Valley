using Arc;
using ArcInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DiplomaticAction : IArcObject
{
    public static readonly Dict<DiplomaticAction> DiplomaticActions = new();
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcString Title { get; set; }
    public ArcString Desc { get; set; }
    public ArcString Tooltip { get; set; }
    public ArcString Category { get; set; }
    public ArcInt? AlertIndex { get; set; }
    public ArcString? AlertTooltip { get; set; }
    public ArcBool RequireAcceptance { get; set; }
    public ArcTrigger Potential { get; set; }
    public ArcTrigger Trigger { get; set; }
    public ArcEffect OnAccept { get; set; }
    public ArcEffect? OnDecline { get; set; }
    public ArcCode? AiAcceptance { get; set; }
    public ArcCode AiWillDo { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public DiplomaticAction(string id,
        ArcString name,
        ArcString title,
        ArcString desc,
        ArcString tooltip,
        ArcString category,
        ArcInt? alertIndex,
        ArcString? alertTooltip,
        ArcBool requireAcceptance,
        ArcTrigger potential,
        ArcTrigger trigger,
        ArcEffect onAccept,
        ArcEffect? onDecline,
        ArcCode? aiAcceptance,
        ArcCode aiWillDo
    ) {
        Id = new(id);
        Name = name;
        Title = title;
        Desc = desc;
        Tooltip = tooltip;
        Category = category;
        AlertIndex = alertIndex;
        AlertTooltip = alertTooltip;
        RequireAcceptance = requireAcceptance;
        Potential = potential;
        Trigger = trigger;
        OnAccept = onAccept;
        OnDecline = onDecline;
        AiAcceptance = aiAcceptance;
        AiWillDo = aiWillDo;

        KeyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "title", Title },
            { "desc", Desc },
            { "tooltip", Tooltip },
            { "category", Category },
            { "alert_index", AlertIndex },
            { "alert_tooltip", AlertTooltip },
            { "require_acceptance", RequireAcceptance },
            { "potential", Potential },
            { "trigger", Trigger },
            { "on_accept", OnAccept },
            { "on_decline", OnDecline },
            { "ai_acceptance", AiAcceptance },
            { "ai_will_do", AiWillDo },
        };

        DiplomaticActions.Add(id, this);
    }

    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        i.ForceMoveNext();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);

        Constructor(id, args);

        return i;
    }
    public static DiplomaticAction Constructor(string id, Args args) => new(id,
        args.Get(ArcString.Constructor, "name"),
        args.Get(ArcString.Constructor, "title"),
        args.Get(ArcString.Constructor, "desc"),
        args.Get(ArcString.Constructor, "tooltip"),
        args.Get(ArcString.Constructor, "category"),
        args.Get(ArcInt.Constructor, "alert_index", null),
        args.Get(ArcString.Constructor, "alert_tooltip", null),
        args.Get(ArcBool.Constructor, "require_acceptance"),
        args.Get(ArcTrigger.Constructor, "potential"),
        args.Get(ArcTrigger.Constructor, "trigger"),
        args.Get(ArcEffect.Constructor, "on_accept"),
        args.Get(ArcEffect.Constructor, "on_decline", null),
        args.Get(ArcCode.Constructor, "ai_acceptance", null),
        args.Get(ArcCode.Constructor, "ai_will_do")
    );
    public void Transpile(ref Block b)
    {
        Instance.Localisation.Add($"{Id}", Name.Value);
        Instance.Localisation.Add($"{Id}_title", Title.Value);
        Instance.Localisation.Add($"{Id}_desc", Desc.Value);
        Instance.Localisation.Add($"{Id}_tooltip", Tooltip.Value);
        if(AlertTooltip != null) Instance.Localisation.Add($"{Id}_alert_tooltip", AlertTooltip.Value);

        b.Add(
            Id, "=", "{",
                "category", "=", Category
        );

        if(AlertIndex != null && AlertTooltip != null)
        {
            b.Add(
                "alert_index", "=", AlertIndex,
                "alert_tooltip", "=", AlertTooltip
            );
        }

        b.Add(
                "require_acceptance", "=", RequireAcceptance,
                Potential.Compile("is_visible"),
                Trigger.Compile("is_allowed"),
                OnAccept.Compile("on_accept"),
                OnDecline?.Compile("on_decline"),
                AiAcceptance?.Compile("ai_acceptance"),
                AiWillDo.Compile("ai_will_do"),
            "}"
        );

    }
    public static string Transpile()
    {
        Block b = new("static_actions", "=", "{", "royal_marriage", "=", "{", "alert_index", "=", "1", "alert_tooltip", "=", "ICON_RM", "}", "requestpeace", "=", "{", "alert_index", "=", "2", "alert_tooltip", "=", "ICON_PEACE", "}", "allianceaction", "=", "{", "alert_index", "=", "3", "alert_tooltip", "=", "ICON_ALLIANCE", "}", "integrationaction", "=", "{", "alert_index", "=", "5", "alert_tooltip", "=", "ICON_ANNEX", "}", "annexationaction", "=", "{", "alert_index", "=", "5", "alert_tooltip", "=", "ICON_ANNEX", "}", "vassalaction", "=", "{", "alert_index", "=", "6", "alert_tooltip", "=", "ICON_VASSAL", "}", "milaccess", "=", "{", "alert_index", "=", "7", "alert_tooltip", "=", "ICON_ASKMIL", "}", "fleet_access", "=", "{", "alert_index", "=", "8", "alert_tooltip", "=", "ICON_ASKFLEET", "}", "offermilaccess", "=", "{", "alert_index", "=", "9", "alert_tooltip", "=", "ICON_OFFACC", "}", "callaction", "=", "{", "alert_index", "=", "10", "alert_tooltip", "=", "ICON_CALLALLY", "}", "offerloan", "=", "{", "alert_index", "=", "11", "alert_tooltip", "=", "ICON_OFFERLOAN", "}", "sellprov", "=", "{", "alert_index", "=", "12", "alert_tooltip", "=", "ICON_PROVSALE", "}", "request_to_join_federation", "=", "{", "alert_index", "=", "14", "alert_tooltip", "=", "ICON_FEDERATIONREQUEST", "}", "invite_to_federation", "=", "{", "alert_index", "=", "15", "alert_tooltip", "=", "ICON_FEDERATIONINVITE", "}", "transfer_trade_power", "=", "{", "alert_index", "=", "16", "alert_tooltip", "=", "TRANSFER_TRADE_POWER", "}", "religious_unity_action", "=", "{", "alert_index", "=", "17", "alert_tooltip", "=", "ICON_RELIGIOUS_UNITY", "}", "form_coalition", "=", "{", "alert_index", "=", "19", "alert_tooltip", "=", "ICON_FORM_COALITION", "}", "enforce_peace", "=", "{", "alert_index", "=", "20", "alert_tooltip", "=", "ICON_ENFORCE_PEACE", "}", "grant_electorate", "=", "{", "alert_index", "=", "21", "alert_tooltip", "=", "ICON_GRANT_ELECTORATE", "}", "demand_unlawful_territory_action", "=", "{", "alert_index", "=", "22", "alert_tooltip", "=", "ICON_DEMAND_UNLAWFUL_TERRITORY", "}", "support_independence_action", "=", "{", "alert_index", "=", "28", "alert_tooltip", "=", "support_independence", "}", "steer_trade", "=", "{", "alert_index", "=", "29", "alert_tooltip", "=", "ICON_STEER_TRADE", "}", "ask_for_march", "=", "{", "alert_index", "=", "30", "alert_tooltip", "=", "ICON_MARCH", "}", "sell_ships_action", "=", "{", "alert_index", "=", "31", "alert_tooltip", "=", "ICON_FLEETSALE", "}", "grant_freecity", "=", "{", "alert_index", "=", "32", "alert_tooltip", "=", "ICON_GRANT_FREECITY", "}", "invite_to_trade_league", "=", "{", "alert_index", "=", "34", "alert_tooltip", "=", "ICON_TRADELEAGUEINVITE", "}", "request_to_join_trade_league", "=", "{", "alert_index", "=", "35", "alert_tooltip", "=", "ICON_TRADELEAGUEREQUEST", "}", "sharemap", "=", "{", "alert_index", "=", "36", "alert_tooltip", "=", "ICON_REQUESTSHAREMAP", "}", "condottieri_action", "=", "{", "alert_index", "=", "37", "alert_tooltip", "=", "ICON_CONDOTTIERI", "}", "offer_fleet_access", "=", "{", "alert_index", "=", "38", "alert_tooltip", "=", "ICON_OFFERFLEET", "}", "break_alliance", "=", "{", "alert_index", "=", "39", "alert_tooltip", "=", "ICON_BREAK_ALLIANCE", "}", "tributary_state_action", "=", "{", "alert_index", "=", "40", "alert_tooltip", "=", "ICON_TRIBUTARY", "}", "ask_for_tributary_state_action", "=", "{", "alert_index", "=", "41", "alert_tooltip", "=", "ICON_ASK_TRIBUTARY", "}", "knowledge_sharing", "=", "{", "alert_index", "=", "42", "alert_tooltip", "=", "ICON_KNOWLEDGESHARE", "}", "charter_company", "=", "{", "alert_index", "=", "44", "alert_tooltip", "=", "ICON_CHARTERCOMPANY", "}", "reduce_relations_favor", "=", "{", "alert_index", "=", "45", "alert_tooltip", "=", "ICON_TRADEFAVORSFORREDUCERELATIONS", "}", "return_core_favor", "=", "{", "alert_index", "=", "46", "alert_tooltip", "=", "ICON_TRADEFAVORSFORRETURNCORE", "}", "break_alliance_favor", "=", "{", "alert_index", "=", "47", "alert_tooltip", "=", "ICON_TRADEFAVORSFORBREAKALLIANCE", "}", "trade_favors_for_trust", "=", "{", "alert_index", "=", "48", "alert_tooltip", "=", "ICON_TRADEFAVORSFORTRUST", "}", "trade_favors_for_men", "=", "{", "alert_index", "=", "49", "alert_tooltip", "=", "ICON_TRADEFAVORSFORMEN", "}", "trade_favors_for_sailors", "=", "{", "alert_index", "=", "50", "alert_tooltip", "=", "ICON_TRADEFAVORSFORSAILORS", "}", "trade_favors_for_heir", "=", "{", "alert_index", "=", "51", "alert_tooltip", "=", "ICON_TRADEFAVORSFORHEIR", "}", "trade_favors_for_war_prep", "=", "{", "alert_index", "=", "52", "alert_tooltip", "=", "ICON_TRADEFAVORSFORWARPREP", "}", "trade_favors_for_gold", "=", "{", "alert_index", "=", "53", "alert_tooltip", "=", "ICON_TRADEFAVORSFORGOLD", "}", "ask_knowledge_sharing", "=", "{", "alert_index", "=", "55", "alert_tooltip", "=", "ICON_ASKKNOWLEDGESHARE", "}", "}");
        
        foreach (DiplomaticAction action in DiplomaticActions.Values())
        {
            action.Transpile(ref b);
        }

        Instance.OverwriteFile($"{Instance.TranspileTarget}/common/new_diplomatic_actions/arc.txt", string.Join(' ', b));
        return "Government Mechanics";
    }
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}
