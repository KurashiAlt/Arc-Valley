using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arc;

public class SubjectType : ArcObject
{
    public static readonly Dict<SubjectType> SubjectTypes = new();
    public SubjectType(string key) { SubjectTypes.Add(key, this); }
    public static Walker Call(Walker i) => Call(i, Constructor);
    public static SubjectType Constructor(string id, Args args) => new(id)
    {
        { "id", new ArcString(id) },
        { "copy_from", args.Get(ArcString.Constructor, "copy_from", null) },
        { "count", args.Get(ArcString.Constructor, "count", null) },
        { "sprite", args.Get(ArcString.Constructor, "sprite", null) },
        { "diplomacy_overlord_sprite", args.Get(ArcString.Constructor, "diplomacy_overlord_sprite", null) },
        { "diplomacy_subject_sprite", args.Get(ArcString.Constructor, "diplomacy_subject_sprite", null) },
        { "is_potential_overlord", args.Get(ArcTrigger.Constructor, "is_potential_overlord", null) },
        { "has_overlords_ruler", args.Get(ArcBool.Constructor, "has_overlords_ruler", null) },
        { "can_fight_independence_war", args.Get(ArcBool.Constructor, "can_fight_independence_war", null) },
        { "is_voluntary", args.Get(ArcBool.Constructor, "is_voluntary", null) },
        { "transfer_trade_power", args.Get(ArcBool.Constructor, "transfer_trade_power", null) },
        { "transfer_trade_if_merchant_republic", args.Get(ArcBool.Constructor, "transfer_trade_if_merchant_republic", null) },
        { "joins_overlords_wars", args.Get(ArcBool.Constructor, "joins_overlords_wars", null) },
        { "can_be_co_belligerented", args.Get(ArcBool.Constructor, "can_be_co_belligerented", null) },
        { "must_accept_cta_from_overlord", args.Get(ArcBool.Constructor, "must_accept_cta_from_overlord", null) },
        { "favors_cost_to_join_offensive_wars", args.Get(ArcInt.Constructor, "favors_cost_to_join_offensive_wars", null) },
        { "favors_cost_to_join_defensive_wars", args.Get(ArcInt.Constructor, "favors_cost_to_join_defensive_wars", null) },
        { "opinion_cost_to_join_offensive_wars", args.Get(ArcInt.Constructor, "opinion_cost_to_join_offensive_wars", null) },
        { "opinion_cost_to_join_defensive_wars", args.Get(ArcInt.Constructor, "opinion_cost_to_join_defensive_wars", null) },
        { "opinion_cost_instead_of_favors_cost", args.Get(ArcBool.Constructor, "opinion_cost_instead_of_favors_cost", null) },
        { "opinion_on_subject_integration", args.Get(ArcInt.Constructor, "opinion_on_subject_integration", null) },
        { "joins_colonial_wars", args.Get(ArcBool.Constructor, "joins_colonial_wars", null) },
        { "can_be_integrated", args.Get(ArcBool.Constructor, "can_be_integrated", null) },
        { "can_release_and_play", args.Get(ArcBool.Constructor, "can_release_and_play", null) },
        { "uses_tariffs", args.Get(ArcBool.Constructor, "uses_tariffs", null) },
        { "dynamically_created_during_history", args.Get(ArcBool.Constructor, "dynamically_created_during_history", null) },
        { "eats_overlords_colonies", args.Get(ArcBool.Constructor, "eats_overlords_colonies", null) },
        { "has_colonial_parent", args.Get(ArcBool.Constructor, "has_colonial_parent", null) },
        { "overlord_can_attack", args.Get(ArcBool.Constructor, "overlord_can_attack", null) },
        { "overlord_can_be_subject", args.Get(ArcBool.Constructor, "overlord_can_be_subject", null) },
        { "can_have_subjects_of_other_types", args.Get(ArcBool.Constructor, "can_have_subjects_of_other_types", null) },
        { "can_be_annexed", args.Get(ArcBool.Constructor, "can_be_annexed", null) },
        { "takes_diplo_slot", args.Get(ArcBool.Constructor, "takes_diplo_slot", null) },
        { "has_power_projection", args.Get(ArcBool.Constructor, "has_power_projection", null) },
        { "can_release_in_peace", args.Get(ArcBool.Constructor, "can_release_in_peace", null) },
        { "uses_military_focus", args.Get(ArcBool.Constructor, "uses_military_focus", null) },
        { "overlord_protects_external", args.Get(ArcBool.Constructor, "overlord_protects_external", null) },
        { "counts_for_borders", args.Get(ArcBool.Constructor, "counts_for_borders", null) },
        { "overlord_enforce_peace_attacking", args.Get(ArcBool.Constructor, "overlord_enforce_peace_attacking", null) },
        { "can_use_claims", args.Get(ArcBool.Constructor, "can_use_claims", null) },
        { "gives_daimyo_bonuses", args.Get(ArcBool.Constructor, "gives_daimyo_bonuses", null) },
        { "gets_help_with_rebels", args.Get(ArcBool.Constructor, "gets_help_with_rebels", null) },
        { "share_rebel_popup", args.Get(ArcBool.Constructor, "share_rebel_popup", null) },
        { "separatists_become_subjects", args.Get(ArcBool.Constructor, "separatists_become_subjects", null) },
        { "allows_taking_land_without_independence", args.Get(ArcBool.Constructor, "allows_taking_land_without_independence", null) },
        { "can_transfer_in_peace", args.Get(ArcBool.Constructor, "can_transfer_in_peace", null) },
        { "can_set_mil_focus", args.Get(ArcBool.Constructor, "can_set_mil_focus", null) },
        { "can_send_missionary_to_subject", args.Get(ArcBool.Constructor, "can_send_missionary_to_subject", null) },
        { "can_union_break", args.Get(ArcBool.Constructor, "can_union_break", null) },
        { "overlord_can_fabricate_for", args.Get(ArcBool.Constructor, "overlord_can_fabricate_for", null) },
        { "does_overlord_size_count_for_warscore_cost", args.Get(ArcBool.Constructor, "does_overlord_size_count_for_warscore_cost", null) },
        { "is_colony_subtype", args.Get(ArcBool.Constructor, "is_colony_subtype", null) },
        { "is_march", args.Get(ArcBool.Constructor, "is_march", null) },
        { "forms_trade_companies", args.Get(ArcBool.Constructor, "forms_trade_companies", null) },
        { "can_concentrate_development", args.Get(ArcBool.Constructor, "can_concentrate_development", null) },
        { "can_have_great_projects_moved_by_overlord", args.Get(ArcBool.Constructor, "can_have_great_projects_moved_by_overlord", null) },
        { "extend_trading_range", args.Get(ArcBool.Constructor, "extend_trading_range", null) },
        { "max_government_rank", args.Get(ArcInt.Constructor, "max_government_rank", null) },
        { "cities_required_for_bonuses", args.Get(ArcInt.Constructor, "cities_required_for_bonuses", null) },
        { "trust_on_start", args.Get(ArcInt.Constructor, "trust_on_start", null) },
        { "base_liberty_desire", args.Get(ArcFloat.Constructor, "base_liberty_desire", null) },
        { "liberty_desire_negative_prestige", args.Get(ArcFloat.Constructor, "liberty_desire_negative_prestige", null) },
        { "liberty_desire_development_ratio", args.Get(ArcFloat.Constructor, "liberty_desire_development_ratio", null) },
        { "liberty_desire_same_dynasty", args.Get(ArcFloat.Constructor, "liberty_desire_same_dynasty", null) },
        { "liberty_desire_revolution", args.Get(ArcFloat.Constructor, "liberty_desire_revolution", null) },
        { "pays_overlord", args.Get(ArcFloat.Constructor, "pays_overlord", null) },
        { "forcelimit_to_overlord", args.Get(ArcFloat.Constructor, "forcelimit_to_overlord", null) },
        { "naval_forcelimit_to_overlord", args.Get(ArcFloat.Constructor, "naval_forcelimit_to_overlord", null) },
        { "manpower_to_overlord", args.Get(ArcFloat.Constructor, "manpower_to_overlord", null) },
        { "sailors_to_overlord", args.Get(ArcFloat.Constructor, "sailors_to_overlord", null) },
        { "military_focus", args.Get(ArcFloat.Constructor, "military_focus", null) },
        { "annex_cost_per_development", args.Get(ArcFloat.Constructor, "annex_cost_per_development", null) },
        { "relative_power_class", args.Get(ArcInt.Constructor, "relative_power_class", null) },
        { "should_quit_wars_on_activation", args.Get(ArcBool.Constructor, "should_quit_wars_on_activation", null) },
        { "diplomacy_view_class", args.Get(ArcInt.Constructor, "diplomacy_view_class", null) },
        { "allow_settlement_growth", args.Get(ArcBool.Constructor, "allow_settlement_growth", null) },
        { "block_settlement_growth", args.Get(ArcBool.Constructor, "block_settlement_growth", null) },
        { "can_fight", args.Get(ArcCode.Constructor, "can_fight", null) },
        { "can_rival", args.Get(ArcCode.Constructor, "can_rival", null) },
        { "can_ally", args.Get(ArcCode.Constructor, "can_ally", null) },
        { "can_marry", args.Get(ArcCode.Constructor, "can_marry", null) },
        { "embargo_rivals", args.Get(ArcBool.Constructor, "embargo_rivals", null) },
        { "support_loyalists", args.Get(ArcBool.Constructor, "support_loyalists", null) },
        { "subsidize_armies", args.Get(ArcBool.Constructor, "subsidize_armies", null) },
        { "scutage", args.Get(ArcBool.Constructor, "scutage", null) },
        { "send_officers", args.Get(ArcBool.Constructor, "send_officers", null) },
        { "divert_trade", args.Get(ArcBool.Constructor, "divert_trade", null) },
        { "placate_rulers", args.Get(ArcBool.Constructor, "placate_rulers", null) },
        { "place_relative_on_throne", args.Get(ArcBool.Constructor, "place_relative_on_throne", null) },
        { "enforce_religion", args.Get(ArcBool.Constructor, "enforce_religion", null) },
        { "customize_subject", args.Get(ArcBool.Constructor, "customize_subject", null) },
        { "replace_governor", args.Get(ArcBool.Constructor, "replace_governor", null) },
        { "grant_province", args.Get(ArcBool.Constructor, "grant_province", null) },
        { "enforce_culture", args.Get(ArcBool.Constructor, "enforce_culture", null) },
        { "siphon_income", args.Get(ArcBool.Constructor, "siphon_income", null) },
        { "fortify_march", args.Get(ArcBool.Constructor, "fortify_march", null) },
        { "seize_territory", args.Get(ArcBool.Constructor, "seize_territory", null) },
        { "start_colonial_war", args.Get(ArcBool.Constructor, "start_colonial_war", null) },
        { "grant_core_claim", args.Get(ArcBool.Constructor, "grant_core_claim", null) },
        { "sacrifice_ruler", args.Get(ArcBool.Constructor, "sacrifice_ruler", null) },
        { "sacrifice_heir", args.Get(ArcBool.Constructor, "sacrifice_heir", null) },
        { "increase_tariffs", args.Get(ArcBool.Constructor, "increase_tariffs", null) },
        { "decrease_tariffs", args.Get(ArcBool.Constructor, "decrease_tariffs", null) },
        { "takeondebt", args.Get(ArcBool.Constructor, "takeondebt", null) },
        { "bestow_gifts", args.Get(ArcBool.Constructor, "bestow_gifts", null) },
        { "send_additional_troops", args.Get(ArcBool.Constructor, "send_additional_troops", null) },
        { "demand_artifacts", args.Get(ArcBool.Constructor, "demand_artifacts", null) },
        { "demand_additional_tribute", args.Get(ArcBool.Constructor, "demand_additional_tribute", null) },
        { "force_seppuku", args.Get(ArcBool.Constructor, "force_seppuku", null) },
        { "press_sailors", args.Get(ArcBool.Constructor, "press_sailors", null) },
        { "contribute_to_capital", args.Get(ArcBool.Constructor, "contribute_to_capital", null) },
        { "force_isolation", args.Get(ArcBool.Constructor, "force_isolation", null) },
        { "return_land", args.Get(ArcBool.Constructor, "return_land", null) },
        { "conscript_general", args.Get(ArcBool.Constructor, "conscript_general", null) },
        { "knowledge_sharing", args.Get(ArcBool.Constructor, "knowledge_sharing", null) },
        { "change_colonial_type", args.Get(ArcBool.Constructor, "change_colonial_type", null) },
        { "upgrade_subject_type", args.Get(ArcBool.Constructor, "upgrade_subject_type", null) },
        { "seize_court_resources", args.Get(ArcBool.Constructor, "seize_court_resources", null) },
        { "request_extra_levies", args.Get(ArcBool.Constructor, "request_extra_levies", null) },
        { "grant_administrative_autonomy", args.Get(ArcBool.Constructor, "grant_administrative_autonomy", null) },
        { "disable_inheritance", args.Get(ArcBool.Constructor, "disable_inheritance", null) },
        { "sword_hunt", args.Get(ArcBool.Constructor, "sword_hunt", null) },
        { "sankin_kotai", args.Get(ArcBool.Constructor, "sankin_kotai", null) },
        { "expel_ronin", args.Get(ArcBool.Constructor, "expel_ronin", null) },
        { "clear_subject_modifier", args.Get(ArcBool.Constructor, "clear_subject_modifier", null) },
        { "subject_modifiers", args.Get(ArcList<ArcCode>.GetConstructor(ArcCode.Constructor), "subject_modifiers", new()) },
        { "clear_overlord_modifier", args.Get(ArcBool.Constructor, "clear_overlord_modifier", null) },
        { "overlord_modifiers", args.Get(ArcList<ArcCode>.GetConstructor(ArcCode.Constructor), "overlord_modifiers", new()) },
        { "overlord_opinion_modifier", args.Get(ArcString.Constructor, "overlord_opinion_modifier", null) },
        { "subject_opinion_modifier", args.Get(ArcString.Constructor, "subject_opinion_modifier", null) },
        { "name", args.Get(ArcString.Constructor, "name", null) },
        { "desc", args.Get(ArcString.Constructor, "desc", null) },
        { "title", args.Get(ArcString.Constructor, "title", null) },
        { "plural", args.Get(ArcString.Constructor, "plural", null) },
        { "overlord", args.Get(ArcString.Constructor, "overlord", null) },
        { "subject", args.Get(ArcString.Constructor, "subject", null) },
        { "is_our", args.Get(ArcString.Constructor, "is_our", null) },
    };
    public override string ToString() => Get("id").ToString();
    public override Walker Call(Walker i, ref Block result) { result.Add(ToString()); return i; }
    public void Transpile(ref Block s)
    {
        string id = ToString();
        s.Add(id, "=", "{");
        foreach(KeyValuePair<string, DictPointer<IVariable?>> Kvp in Kvps)
        {
            switch (Kvp.Key)
            {
                case "id": break;
                case "name": { if (Kvp.Value.Value is ArcString value && value != null) Program.Localisation.Add($"{id}", value.Value); } break;
                case "desc": { if (Kvp.Value.Value is ArcString value && value != null) Program.Localisation.Add($"{id}_long_desc", value.Value); }; break;
                case "title": { if (Kvp.Value.Value is ArcString value && value != null) Program.Localisation.Add($"{id}_title", value.Value); } break;
                case "plural": { if (Kvp.Value.Value is ArcString value && value != null) Program.Localisation.Add($"{id}_title_plural", value.Value); } break;
                case "overlord": { if (Kvp.Value.Value is ArcString value && value != null) Program.Localisation.Add($"{id}_overlord", value.Value); } break;
                case "subject": { if (Kvp.Value.Value is ArcString value && value != null) Program.Localisation.Add($"{id}_subject", value.Value); } break;
                case "is_our": { if (Kvp.Value.Value is ArcString value && value != null) Program.Localisation.Add($"{id}_is_our", value.Value); } break;
                case "clear_subject_modifier": { if (Kvp.Value.Value is ArcBool value && value == true) s.Add("modifier_subject = clear"); } break;
                case "clear_overlord_modifier": { if (Kvp.Value.Value is ArcBool value && value == true) s.Add("modifier_overlord = clear"); } break;
                case "subject_modifiers":
                case "overlord_modifiers":
                    {
                        if (Kvp.Value.Value is ArcList<ArcCode> list)
                        {
                            foreach (ArcCode? value in list.Values)
                            {
                                if (value is null) continue;
                                value.Compile(Kvp.Key switch
                                {
                                    "subject_modifiers" => "modifier_subject",
                                    "overlord_modifiers" => "modifier_overlord",
                                    _ => throw new Exception()
                                }, ref s);
                            }
                        }
                        else throw ArcException.Create($"Arc-Valley is not supported in space or another high radiation location.");
                    }
                    break;
                default:
                    {
                        if (Kvp.Value.Value is not null)
                        {
                            if (Kvp.Value.Value is ArcBlock value) { value.Compile(Kvp.Key, ref s, false); }
                            else s.Add(Kvp.Key, "=", Kvp.Value.Value.ToString());
                        }
                    }
                break;
            }
        }
        s.Add("}");
    }
    public static string Transpile()
    {
        Block s = new();
        foreach (KeyValuePair<string, SubjectType> SubjectType in SubjectTypes)
        {
            if (SubjectType.Key == "default") continue;
            s.Add(SubjectType.Value.ToString(), "=", "{", "}");
        }
        foreach (KeyValuePair<string, SubjectType> SubjectType in SubjectTypes)
        {
            SubjectType.Value.Transpile(ref s);
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/common/subject_types/arc.txt", string.Join(' ', s));
        return "Subject Types";
    }
}