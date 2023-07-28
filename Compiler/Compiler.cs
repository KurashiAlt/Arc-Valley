using ArcInstance;
using System.Text;
using System.Text.RegularExpressions;

namespace Arc
{
    public partial class Compiler
    {
        public static readonly Dictionary<string, IVariable> global = new()
        {
            { "on_actions", new Dict<ArcBlock>() {
                { "on_startup", new() },
                { "on_religion_change", new() },
                { "on_secondary_religion_change", new() },
                { "on_enforce_rebel_demands", new() },
                { "on_colonial_liberation", new() },
                { "on_colonial_reintegration", new() },
                { "on_peace_actor", new() },
                { "on_peace_recipient", new() },
                { "on_war_won", new() },
                { "on_main_war_won", new() },
                { "on_separate_war_won", new() },
                { "on_war_lost", new() },
                { "on_main_war_lost", new() },
                { "on_separate_war_lost", new() },
                { "on_battle_won_country", new() },
                { "on_battle_lost_country", new() },
                { "on_battle_won_province", new() },
                { "on_battle_won_unit", new() },
                { "on_battle_lost_unit", new() },
                { "on_battle_lost_province", new() },
                { "on_siege_won_country", new() },
                { "on_siege_lost_country", new() },
                { "on_siege_won_province", new() },
                { "on_siege_lost_province", new() },
                { "on_abandon_colony", new() },
                { "on_great_project_constructed", new() },
                { "on_new_monarch", new() },
                { "on_new_consort", new() },
                { "on_monarch_death", new() },
                { "on_regent", new() },
                { "on_extend_regency", new() },
                { "on_new_term_election", new() },
                { "on_death_election", new() },
                { "on_death_foreign_slave_ruler", new() },
                { "on_replace_governor", new() },
                { "on_bankruptcy", new() },
                { "on_diplomatic_annex", new() },
                { "on_heir_death", new() },
                { "on_queen_death", new() },
                { "on_new_heir", new() },
                { "on_weak_heir_claim", new() },
                { "on_heir_needed_theocracy", new() },
                { "on_successive_emperor", new() },
                { "on_emperor_elected", new() },
                { "on_released_hre_member", new() },
                { "on_hre_member_false_religion", new() },
                { "on_hre_member_true_religion", new() },
                { "on_hre_wins_defensive_war", new() },
                { "on_hre_member_annexed", new() },
                { "on_hre_released_vassal", new() },
                { "on_hre_transfered_vassal", new() },
                { "on_hre_defense", new() },
                { "on_hre_non_defense", new() },
                { "on_hre_province_reconquest", new() },
                { "on_lock_hre_religion", new() },
                { "on_change_hre_religion", new() },
                { "on_hre_religion_white_peace", new() },
                { "on_government_change", new() },
                { "on_native_change_government", new() },
                { "on_integrate", new() },
                { "on_annexed", new() },
                { "on_chinese_empire_dismantled", new() },
                { "on_mandate_of_heaven_gained", new() },
                { "on_mandate_of_heaven_lost", new() },
                { "on_embrace_revolution", new() },
                { "on_dismantle_revolution", new() },
                { "on_adm_development", new() },
                { "on_dip_development", new() },
                { "on_mil_development", new() },
                { "on_overextension_pulse", new() },
                { "on_colonial_pulse", new() },
                { "on_siberian_pulse", new() },
                { "on_monthly_pulse", new() },
                { "on_yearly_pulse", new() },
                { "on_yearly_pulse_2", new() },
                { "on_yearly_pulse_3", new() },
                { "on_yearly_pulse_4", new() },
                { "on_yearly_pulse_5", new() },
                { "on_bi_yearly_pulse", new() },
                { "on_bi_yearly_pulse_2", new() },
                { "on_bi_yearly_pulse_3", new() },
                { "on_bi_yearly_pulse_4", new() },
                { "on_bi_yearly_pulse_5", new() },
                { "on_thri_yearly_pulse", new() },
                { "on_thri_yearly_pulse_2", new() },
                { "on_thri_yearly_pulse_3", new() },
                { "on_thri_yearly_pulse_4", new() },
                { "on_four_year_pulse", new() },
                { "on_four_year_pulse_2", new() },
                { "on_four_year_pulse_3", new() },
                { "on_four_year_pulse_4", new() },
                { "on_five_year_pulse", new() },
                { "on_five_year_pulse_2", new() },
                { "on_five_year_pulse_3", new() },
                { "on_five_year_pulse_4", new() },
                { "on_explore_coast", new() },
                { "on_conquistador_empty", new() },
                { "on_conquistador_native", new() },
                { "on_buy_religious_reform", new() },
                { "on_circumnavigation", new() },
                { "on_become_free_city", new() },
                { "on_remove_free_city", new() },
                { "on_revoke_estate_land_ai", new() },
                { "on_revoke_estate_land", new() },
                { "on_revoke_estate_land_ai_post", new() },
                { "on_revoke_estate_land_post", new() },
                { "on_grant_estate_land", new() },
                { "on_death_has_harem", new() },
                { "on_select_heir_from_harem", new() },
                { "on_fetishist_cult_change", new() },
                { "on_gain_great_power_status", new() },
                { "on_lose_great_power_status", new() },
                { "on_province_religion_converted", new() },
                { "on_province_religion_changed", new() },
                { "on_province_culture_converted", new() },
                { "on_province_culture_changed", new() },
                { "on_convert_by_trade_policy", new() },
                { "on_province_owner_change", new() },
                { "on_parliament_debate_reset", new() },
                { "on_parliament_seat_given", new() },
                { "on_parliament_seat_lost", new() },
                { "on_parliament_debate_failed", new() },
                { "on_parliament_debate_succeeded", new() },
                { "on_harmonized_pagan", new() },
                { "on_harmonized_christian", new() },
                { "on_harmonized_muslim", new() },
                { "on_harmonized_jewish_group", new() },
                { "on_harmonized_zoroastrian_group", new() },
                { "on_harmonized_dharmic", new() },
                { "on_harmonized_mahayana", new() },
                { "on_harmonized_buddhism", new() },
                { "on_harmonized_shinto", new() },
                { "on_harmonized_vajrayana", new() },
                { "on_consecrate_patriarch", new() },
                { "on_accept_tribute", new() },
                { "on_refuse_tribute", new() },
                { "on_leader_recruited", new() },
                { "on_general_recruited", new() },
                { "on_admiral_recruited", new() },
                { "on_conquistador_recruited", new() },
                { "on_explorer_recruited", new() },
                { "on_regiment_recruited", new() },
                { "on_mercenary_recruited", new() },
                { "on_add_pasha", new() },
                { "on_remove_pasha", new() },
                { "on_janissaries_raised", new() },
                { "on_cawa_raised", new() },
                { "on_carolean_raised", new() },
                { "on_hussars_raised", new() },
                { "on_revolutionary_guard_raised", new() },
                { "on_banner_raised", new() },
                { "on_cossack_raised", new() },
                { "on_marine_raised", new() },
                { "on_rajput_raised", new() },
                { "on_streltsy_raised", new() },
                { "on_tercio_raised", new() },
                { "on_musketeer_raised", new() },
                { "on_samurai_raised", new() },
                { "on_geobukseon_raised", new() },
                { "on_man_of_war_raised", new() },
                { "on_galleon_raised", new() },
                { "on_galleass_raised", new() },
                { "on_caravel_raised", new() },
                { "on_voc_indiamen_raised", new() },
                { "on_hre_reform_passed", new() },
                { "on_mandate_reform_passed", new() },
                { "on_reform_enacted", new() },
                { "on_reform_changed", new() },
                { "on_trade_company_investment", new() },
                { "on_center_of_trade_upgrade", new() },
                { "on_center_of_trade_downgrade", new() },
                { "on_culture_promoted", new() },
                { "on_culture_demoted", new() },
                { "on_primary_culture_changed", new() },
                { "on_company_chartered", new() },
                { "on_dependency_gained", new() },
                { "on_dependency_lost", new() },
                { "on_create_vassal", new() },
                { "on_holy_order_established", new() },
                { "on_minority_expelled", new() },
                { "on_raid_coast", new() },
                { "on_flagship_captured", new() },
                { "on_flagship_destroyed", new() },
                { "on_country_released", new() },
                { "on_trade_good_changed", new() },
                { "on_loan_repaid", new() },
                { "on_rebels_break_country", new() },
                { "on_pre_adm_advisor_fired", new() },
                { "on_pre_dip_advisor_fired", new() },
                { "on_pre_mil_advisor_fired", new() },
                { "on_post_adm_advisor_fired", new() },
                { "on_post_dip_advisor_fired", new() },
                { "on_post_mil_advisor_fired", new() },
                { "on_create_client_state", new() },
                { "on_change_revolution_target", new() },
                { "on_golden_bull_enacted", new() },
                { "on_conquest", new() },
                { "on_country_creation", new() },
                { "on_federation_leader_change", new() },
                { "on_pillaged_capital", new() },
                { "on_transfer_development", new() },
                { "on_colonial_type_change", new() },
                { "on_estate_led_regency", new() },
                { "on_estate_led_regency_surpassed", new() },
                { "on_extended_regency", new() },
                { "on_estate_removed", new() },
                { "on_colony_established", new() },
                { "on_colonial_nation_established", new() },
                { "on_colonist_boosting_colony", new() },
                { "on_force_conversion", new() },
                { "on_institution_embracement", new() },
                { "on_national_focus_change", new() },
                { "on_capital_moved", new() },
                { "on_expanded_infrastructure", new() },
                { "on_centralized_state", new() },
                { "monarch_on_shipwreck", new() },
                { "heir_on_shipwreck", new() },
                { "consort_on_shipwreck", new() },
                { "on_defender_of_faith_loss", new() },
                { "on_defender_of_faith_claim", new() },
                { "on_hre_dismantled", new() },
                { "on_new_age", new() },
                { "on_adm_exploited", new() },
                { "on_dip_exploited", new() },
                { "on_mil_exploited", new() },
                { "on_raze", new() },
                { "on_concentrate_development", new() },
                { "on_slacken_start", new() },
                { "on_slacken_stop", new() },
                { "on_colony_finished", new() },
                { "on_advisor_hired", new() },
                { "on_adm_advisor_hired", new() },
                { "on_dip_advisor_hired", new() },
                { "on_mil_advisor_hired", new() },
                { "on_core", new() },
                { "on_tech_taken", new() },
                { "on_adm_tech_taken", new() },
                { "on_dip_tech_taken", new() },
                { "on_mil_tech_taken", new() },
                { "on_barrage", new() },
                { "on_naval_barrage", new() },
                { "on_scorch_earth", new() },
                { "on_war_ended", new() },
                { "on_alliance_broken", new() },
                { "on_royal_marriage_broken", new() },
                { "on_alliance_created", new() },
                { "on_royal_marriage", new() },
                { "on_heir_disinherited", new() }
            } },
            { "provinces", Province.Provinces },
            { "areas", Area.Areas },
            { "regions", Region.Regions },
            { "superregions", Superregion.Superregions },
            { "tradegoods", TradeGood.TradeGoods },
            { "terrains", Terrain.Terrains },
            { "blessings", Blessing.Blessings },
            { "terrain_declarations", new ArcBlock() },
            { "tree", new ArcBlock() },
            { "interface", new Dict<IValue>() {
                { "church_aspects", new ArcString("") },
                { "countryreligionview", new ArcString("") }
            } }
        };

#pragma warning disable CS8618 // Fields are assigned by Arc5.cs
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static string directory;
        public static Instance owner;
#pragma warning restore CA2211
#pragma warning restore CS8618

        public Compiler() { }
        public string Compile(string file, bool preprocessor = false)
        {
            if (preprocessor)
                file = Parser.Preprocessor(file);

            return Compile(Parser.ParseCode(file));
        }
        public string Compile(Block code)
        {
            if (code.Count == 0)
                return "";

            List<string> result = new();
            Dictionary<string, Func<Walker, Walker>> keywords = new()
            {
                { "new", (Walker i)=>{
                    if(!i.MoveNext()) throw new Exception();
                    return (string)i.Current switch
                    {
                        "province" => Province.Call(i),
                        "area" => Area.Call(i),
                        "region" => Region.Call(i),
                        "superregion" => Superregion.Call(i),
                        "tradegood" => TradeGood.Call(i),
                        "terrain" => Terrain.Call(i),
                        "blessing" => Blessing.Call(i),
                        "church_aspect" => ChurchAspect.Call(i),
                        "inheritable" => Args.Call(i),
                        "country" => Country.Call(i),
                        "adjacency" => Adjacency.Call(i),
                        "building" => Building.Call(i),
                        "bookmark" => Bookmark.Call(i),
                        "religion" => Religion.Call(i),
                        "religious_group" => ReligionGroup.Call(i),
                        "personal_deity" => PersonalDeity.Call(i),
                        _ => throw new Exception()
                    };
                } },
            };

            Walker g = new(code);
            do
            {
                if (keywords.ContainsKey(g.Current))
                {
                    g = keywords[g.Current].Invoke(g);
                    continue;
                }
                else if (TryGetVariable(g.Current, out IVariable? var))
                {
                    if (var == null) throw new Exception();
                    g = var.Call(g, ref result, this);
                }
                else if (TryTrimOne(g.Current, '`', out string? newValue))
                {
                    if (newValue == null)
                        throw new Exception();

                    Regex Replace = TranspiledString();
                    newValue = Replace.Replace(newValue, delegate (Match m)
                    {
                        return Compile(m.Groups[1].Value).Trim();
                    });

                    result.Add(newValue);
                    continue;
                }
                else if (g.Current.value.StartsWith("p@"))
                {
                    result.Add(Province.Provinces[g.Current.value[2..]].Id.Value.ToString());
                    continue;
                }
                else if (g.Current == "\\n")
                {
                    result.Add(Environment.NewLine);
                }
                else result.Add(g.Current);
            } while (g.MoveNext());

            StringBuilder res = new();
            foreach (string s in result)
            {
                res.Append($"{s} ");
            }

            return res.ToString();
        }

        [GeneratedRegex("{([^}]+)}", RegexOptions.Compiled)]
        private static partial Regex TranspiledString();
    }
}
