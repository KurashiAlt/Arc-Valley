using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace Arc;
public static partial class Compiler
{
    public static int QuickEventModifiers = 0;
    public static readonly Dict<IVariable> global = new()
    {
        { "ai_personalities", new Dict<ArcTrigger>() {
            { "human", new("chance", "=", "{", "factor", "=", "0", "}", "icon", "=", "1") },
            { "ai_capitalist", new("chance", "=", "{", "factor", "=", "100", "}", "icon", "=", "2") },
            { "ai_diplomat", new("chance", "=", "{", "factor", "=", "100", "}", "icon", "=", "3") },
            { "ai_militarist", new("chance", "=", "{", "factor", "=", "100", "}", "icon", "=", "4") },
            { "ai_colonialist", new("chance", "=", "{", "factor", "=", "100", "}", "icon", "=", "5") },
            { "ai_balanced", new("chance", "=", "{", "factor", "=", "100", "}", "icon", "=", "6") },
        } },
        { "centers_of_trade", new Dict<ArcCode>()
        {
            { "staple_port", new("level", "=", "1", "type", "=", "coastal", "province_modifiers", "=", "{", "province_trade_power_value", "=", "5", "}") },
            { "entrepot", new("level", "=", "2", "development", "=", "10", "cost", "=", "200", "type", "=", "coastal", "province_modifiers", "=", "{", "province_trade_power_value", "=", "10", "local_development_cost", "=", "-0.05", "local_institution_spread", "=", "0.1", "}") },
            { "world_port", new("level", "=", "3", "development", "=", "25", "cost", "=", "1000", "type", "=", "coastal", "province_modifiers", "=", "{", "province_trade_power_value", "=", "25", "local_institution_spread", "=", "0.3", "}", "state_modifiers", "=", "{", "local_development_cost", "=", "-0.1", "local_sailors_modifier", "=", "1", "allowed_num_of_buildings", "=", "1", "}", "global_modifiers", "=", "{", "navy_tradition_decay", "=", "-0.002", "}") },
            { "emporium", new("level", "=", "1", "type", "=", "inland", "province_modifiers", "=", "{", "province_trade_power_value", "=", "5", "}") },
            { "market_town", new("level", "=", "2", "cost", "=", "200", "development", "=", "10", "type", "=", "inland", "province_modifiers", "=", "{", "province_trade_power_value", "=", "10", "local_development_cost", "=", "-0.05", "}") },
            { "world_trade_center", new("level", "=", "3", "cost", "=", "1000", "development", "=", "25", "type", "=", "inland", "province_modifiers", "=", "{", "province_trade_power_value", "=", "25", "local_institution_spread", "=", "0.3", "}", "state_modifiers", "=", "{", "local_development_cost", "=", "-0.1", "local_manpower_modifier", "=", "0.33", "allowed_num_of_buildings", "=", "1", "}") },

        } },
        { "bi_yearly_events", new ArcBlock() },
        { "on_actions", new Dict<ArcEffect>() {
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
            { "on_heir_disinherited", new() },
            { "on_added_to_trade_company", new() },
            { "on_removed_from_company", new() },
            { "on_company_formed", new() },
            { "on_company_disolved", new() },
            { "on_overrun", new() },
            { "on_qizilbash_raised", new() },
            { "on_mamluks_raised", new() },
        } },
        { "adjacencies", Adjacency.Adjacencies },
        { "advisor_types", AdvisorType.AdvisorTypes },
        { "areas", Area.Areas },
        { "events", Event.Events },
        { "decisions", Decision.Decisions },
        { "blessings", Blessing.Blessings },
        { "bookmarks", Bookmark.Bookmarks },
        { "buildings", Building.Buildings },
        { "building_lines", BuildingLine.BuildingLines },
        { "great_projects", GreatProject.GreatProjects },
        { "church_aspects", ChurchAspect.ChurchAspects },
        { "countries", Country.Countries },
        { "culture_groups", CultureGroup.CultureGroups },
        { "idea_groups", IdeaGroup.IdeaGroups },
        { "cultures", Culture.Cultures },
        { "estates", Estate.Estates },
        { "event_modifiers", EventModifier.EventModifiers },
        { "personal_deitys", PersonalDeity.PersonalDeitys },
        { "mission_trees", MissionTree.MissionTrees },
        { "provinces", Province.Provinces },
        { "regions", Region.Regions },
        { "religions", Religion.Religions },
        { "religious_groups", ReligionGroup.ReligionGroups },
        { "superregions", Superregion.Superregions },
        { "terrains", Terrain.Terrains },
        { "governments", Government.Governments },
        { "government_reforms", GovernmentReform.GovernmentReforms },
        { "trade_goods", TradeGood.TradeGoods },
        { "trade_nodes", TradeNode.TradeNodes },
        { "province_triggered_modifiers", ProvinceTriggeredModifier.ProvinceTriggeredModifiers },
        { "casus_bellies", CasusBelli.CasusBellies },
        { "war_goals", WarGoal.WarGoals },
        { "province_groups", ProvinceGroup.ProvinceGroups },
        { "subject_types", SubjectType.SubjectTypes },
        { "static_modifiers", StaticModifier.StaticModifiers },
        { "default_reform", new ArcCode() },
        { "terrain_declarations", new ArcBlock() },
        { "tree", new ArcBlock() },
        { "args", new ArgList() },
        { "hre_defines", new Dict<IVariable>()
        {
            { "emperor", new ArcString("") }
        } },
        { "interface", new Dict<IValue>() {
            { "church_aspects", new ArcString("") },
            { "countryreligionview", new ArcString("") },
            { "provinceview", new ArcString("") },
            { "macrobuildinterface", new ArcString("") },
            { "buildings", new ArcCode() },
        } },
        { "special_units", new Dict<IVariable>()
        {
            { "galleass", new Dict<IVariable>()
            {
                { "name", new ArcString("\"Galleass\"") },
                { "modifier", new ArcModifier() },
                { "ship", new ArcModifier() },
                { "uses_construction", new ArcInt(1) },
                { "base_cost_modifier", new ArcFloat(1.0) },
                { "sailors_cost_modifier", new ArcFloat(1.0) },
                { "starting_strength", new ArcFloat(1.0) },
                { "starting_morale", new ArcFloat(1.0) },
                { "localisation", new ArcBlock() },
            } },
            { "musketeer", new Dict<IVariable>()
            {
                { "name", new ArcString("\"Musketeer\"") },
                { "modifier", new ArcModifier() },
                { "regiment", new ArcModifier() },
                { "uses_construction", new ArcInt(1) },
                { "base_cost_modifier", new ArcFloat(1.0) },
                { "manpower_cost_modifier", new ArcFloat(1.0) },
                { "prestige_cost", new ArcInt(0) },
                { "absolutism_cost", new ArcInt(0) },
                { "starting_strength", new ArcFloat(1.0) },
                { "starting_morale", new ArcFloat(1.0) },
                { "localisation", new ArcBlock() },
            } },
            { "rajput", new Dict<IVariable>()
            {
                { "name", new ArcString("\"Rajput\"") },
                { "regiment", new ArcModifier() },
                { "uses_construction", new ArcInt(1) },
                { "base_cost_modifier", new ArcFloat(1.0) },
                { "maximum_ratio", new ArcFloat(1.0) },
                { "starting_strength", new ArcFloat(1.0) },
                { "localisation", new ArcBlock() },
            } },
        } },
    };
    public static string GetId(string i)
    {
        return new ArcString(i).Value;
    }
    public static void ObjectDeclare(string file, bool preprocessor = false)
    {
        if (preprocessor)
            file = Parser.Preprocessor(file);

        ObjectDeclare(Parser.ParseCode(file));
    }
    public static Walker Declare(Walker g)
    {
        return (string)g.Current switch
        {
            "province" => Province.Call(g),
            "area" => Area.Call(g),
            "region" => Region.Call(g),
            "superregion" => Superregion.Call(g),
            "tradegood" => TradeGood.Call(g),
            "terrain" => Terrain.Call(g),
            "blessing" => Blessing.Call(g),
            "church_aspect" => ChurchAspect.Call(g),
            "inheritable" => Args.Call(g),
            "country" => Country.Call(g),
            "adjacency" => Adjacency.Call(g),
            "building" => Building.Call(g),
            "bookmark" => Bookmark.Call(g),
            "religion" => Religion.Call(g),
            "religious_group" => ReligionGroup.Call(g),
            "personal_deity" => PersonalDeity.Call(g),
            "advisor_type" => AdvisorType.Call(g),
            "tradenode" => TradeNode.Call(g),
            "idea_group" => IdeaGroup.Call(g),
            "event_modifier" => EventModifier.Call(g),
            "opinion_modifier" => OpinionModifier.Call(g),
            "relation" => Relation.Call(g),
            "culture_group" => CultureGroup.Call(g),
            "culture" => Culture.Call(g),
            "mission_series" => MissionSeries.Call(g),
            "agenda" => EstateAgenda.Call(g),
            "privilege" => EstatePrivilege.Call(g),
            "estate" => Estate.Call(g),
            "government" => Government.Call(g),
            "government_names" => GovernmentNames.Call(g),
            "government_reform" => GovernmentReform.Call(g),
            "country_event" => Event.Call(g, false),
            "province_event" => Event.Call(g, true),
            "incident" => Incident.Call(g),
            "unit" => Unit.Call(g),
            "great_project" => GreatProject.Call(g),
            "localisation" => DefineLoc(g),
            "mercenary_company" => MercenaryCompany.Call(g),
            "advisor" => Advisor.Call(g),
            "age" => Age.Call(g),
            "decision" => Decision.Call(g),
            "building_line" => BuildingLine.Call(g),
            "government_mechanic" => GovernmentMechanic.Call(g),
            "diplomatic_action" => DiplomaticAction.Call(g),
            "effect" => NewEffect.Call(g),
            "trigger" => NewTrigger.Call(g),
            "modifier" => NewModifier.Call(g),
            "mission_tree" => MissionTree.Call(g),
            "holy_order" => HolyOrder.Call(g),
            "province_triggered_modifier" => ProvinceTriggeredModifier.Call(g),
            "casus_belli" => CasusBelli.Call(g),
            "war_goal" => WarGoal.Call(g),
            "expedition" => Expedition.Call(g),
            "province_group" => ProvinceGroup.Call(g),
            "personality_trait" => RulerPersonality.Call(g),
            "policy" => Policy.Call(g),
            "scholarly_research" => ScholarlyResearch(g),
            "subject_type" => SubjectType.Call(g),
            "static_modifier" => StaticModifier.Call(g),
            _ => throw new Exception($"Unknown Object Type {g.Current} in object declaration")
        };
    }
    static Walker ScholarlyResearch(Walker i)
    {
        i.ForceMoveNext();

        string id = GetId(i.Current);

        i = Args.GetArgs(i, out Args args);

        string name = args.Get(ArcString.Constructor, "name").Value;
        ArcTrigger Potential = new(
            "has_country_flag", "=", "coterie_of_organized_scholars", 
            "NOT", "=", "{", 
                "has_country_flag", "=", $"coterie_{id}", 
            "}", 
            "NOT", "=", "{", 
                "has_country_modifier", "=", "coterie_of_organized_scholars_research", 
            "}"
        );
        Potential.Value.Add(args.Get(ArcTrigger.Constructor, "potential", new()).Value);
        ArcTrigger Allow = new(
            "NOT", "=", "{", 
                "has_country_modifier", "=", "coterie_of_organized_scholars_research", 
            "}"
        );
        Allow.Value.Add(args.Get(ArcTrigger.Constructor, "allow", new()).Value);
        ArcEffect Effect = args.Get(ArcEffect.Constructor, "on_start", new());
        Effect.Value.Add(
            "scholarly_research", "=", "{", 
                "id", "=", id, 
                "on_complete", "=", "{"
        );
        foreach(Word w in args.Get(ArcEffect.Constructor, "on_complete", new()).Value)
        {
            Effect.Value.Add(w);
        }
        Effect.Value.Add("}");

        new Decision($"coterie_{id}",
            new ArcString($"Scholarly Research: {name}"),
            args.Get(ArcString.Constructor, "desc", new("")),
            args.Get(ArcBool.Constructor, "major", new(false)),
            args.Get(ArcCode.Constructor, "color", new("0", "108", "255")),
            args.Get(ArcTrigger.Constructor, "provinces_to_highlight", new()),
            Potential,
            Allow,
            Effect,
            args.Get(ArcCode.Constructor, "ai_will_do", new()),
            args.Get(ArcInt.Constructor, "ai_importance", new(400))
        );

        return i;
    }
    static Walker DefineLoc(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();
        ArcString key = new(new Block(i.Current));
        if (!i.MoveNext()) throw new Exception();
        if (i.Current != "=") throw new Exception();
        if (!i.MoveNext()) throw new Exception();
        ArcString value = new(new Block(i.Current));

        if (Program.Localisation.ContainsKey(key.Value))
        {
            if (Program.Localisation[key.Value] != value.Value) throw new Exception($"{key} already exists, and has a different value than the provided input");
        }
        else Program.Localisation.Add(key.Value, value.Value);

        return i;
    }
    public static void ObjectDeclare(Block code)
    {
        if (code.Count == 0)
            return;

        Block result = new();

        Walker g = new(code);
        do
        {
            if (g.Current == "new")
            {
                if (!g.MoveNext()) throw new Exception();
                g = Declare(g);
                continue;
            }
            else if (g.Current == "run_effect")
            {
                g = Args.GetArgs(g, out Args args);
                if (args.block == null) throw new Exception();
                CompileEffect(args.block);
                continue;
            }
            else if (TryGetVariable(g.Current, out IVariable? var))
            {
                Block f = new();
                if (var == null) throw new Exception();
                g = var.Call(g, ref f);
            }
            else if (NewFunctions<NewEffect, ArcEffect>(g, ref result, NewEffects, CompileEffect)) continue;
            else throw new Exception($"Invalid command in Object Declaration: {g.Current}");
        } while (g.MoveNext());
    }
    public static Dict<ArcObject> ModifierLocs = Dict<ArcObject>.Constructor((string id, Args args) =>
    {
        return new ArcObject()
        {
            { "text", new ArcString(args.Get("text")) },
            { "localisation_key", new ArcString(args.Get("localisation_key")) },
            { "multiplier", new ArcFloat(args.Get("multiplier")) },
            { "percent", new ArcBool(args.Get("percent")) },
            { "is_good", new ArcBool(args.Get("is_good")) },
            { "is_bool", new ArcBool(args.Get("is_bool")) },
            { "is_multiplicative", new ArcBool(args.Get("is_multiplicative")) },
            { "precision", new ArcInt(args.Get("precision")) },
        };
    })(Parser.ParseCode(File.ReadAllText($"{Program.directory}modifier_loc.txt")));
    public static bool When(Block code)
    {
        if (code.Count == 0)
            return true;

        Walker g = new(code);
        do
        {
            if (g.Current == "exists")
            {
                g = Args.GetArgs(g, out Args args);
                if (args.block == null) throw new Exception();
                string key = string.Join(' ', args.block);
                if (!TryGetVariable(key, out _)) return false;
            }
            else if (g.Current == "str:contains")
            {
                g = Args.GetArgs(g, out Args args);
                string str = args.Get(ArcString.Constructor, "string").Value;
                string value = args.Get(ArcString.Constructor, "value").Value;
                if (!str.Contains(value))
                {
                    return false;
                }
            }
        } while (g.MoveNext());

        return true;
    }
    public static bool AllCompile(ref Walker g, ref Block result, Func<Block, string> compile)
    {
        if (g.Current == "new")
        {
            if (!g.MoveNext()) throw new Exception();
            g = Declare(g);
            return true;
        }
        if (g.Current == "replace")
        {
            g.ForceMoveNext(); string what = GetId(g.Current);
            g.ForceMoveNext(); g.Asssert("with");
            g.ForceMoveNext(); string with = GetId(g.Current);

            Walker f = new(g);
            do
            {
                f.Current.value = f.Current.value.Replace(what, with);
            } while (f.MoveNext());

            return true;
        }
        if (g.Current == "defineloc") {
            DefineLoc(g);
            return true;
        }
        if (g.Current == "when")
        {
            g.ForceMoveNext();
            string trigger = g.Current.value[1..^1];

            g.ForceMoveNext();
            g = GetScope(g, out Block scope);

            if (Parser.HasEnclosingBrackets(scope)) scope = RemoveEnclosingBrackets(scope);

            if(WhenInterpret(trigger)) result.Add(compile(scope));
            return true;
            bool WhenInterpret(string trigger)
            {
                Block b = Parser.ParseCode(trigger);
                if (Parser.HasEnclosingBrackets(b)) b = RemoveEnclosingBrackets(b);
                return When(b);
            }
        }
        if (g.Current == "modifier_to_string")
        {
            g = Args.GetArgs(g, out Args args);
            if(args.block != null && TryGetVariable(string.Join(' ', args.block), out IVariable? vr))
            {
                if (vr == null) throw new Exception();
                if (vr is not ArcModifier) throw new Exception($"{string.Join(' ', args.block)} is not ArcModifier in modifier_to_string function");
                Block c = new("{");
                foreach(Word w in ((ArcModifier)vr).Value) 
                {
                    c.Add(w);
                }
                c.Add("}");

                args = Args.GetArgs(c);
            }
            if (args.keyValuePairs == null) throw new Exception();
            string str = "";
            foreach(KeyValuePair<string, Block> b in args.keyValuePairs)
            {
                ArcObject modInfo = ModifierLocs[b.Key];
                string text = modInfo.Get<ArcString>("text").Value;
                string key = modInfo.Get<ArcString>("localisation_key").Value;
                if (Program.Localisation.ContainsKey(key)) text = Program.Localisation[key];
                bool percent = modInfo.Get<ArcBool>("percent").Value;
                bool isBool = modInfo.Get<ArcBool>("is_bool").Value;
                bool isGood = modInfo.Get<ArcBool>("is_good").Value;
                int precision = modInfo.Get<ArcInt>("precision").Value;
                double multiplier = modInfo.Get<ArcFloat>("multiplier").Value;

                string value = string.Join(' ', b.Value);

                if (isBool)
                {
                    bool nValue = new ArcBool(value).Value;

                    str += $"{text}: §{(isGood==nValue?'G':'R')}{value}§!";
                }
                else
                {
                    double nValue = new ArcFloat(value).Value * multiplier;

                    str += $"{text}: §{(isGood==nValue>=0?'G':'R')}{nValue.ToString($"F{precision}")}{(percent?"%":"")}§!";
                }

                str += '\n';
            }
            result.Add(str);
            return true;
        }
        if (g.Current == "breakpoint")
        {
            Debugger.Break();
            return true;
        }
        if (g.Current == "foreach")
        {
            g.ForceMoveNext(); string varKey = g.Current;
            g.ForceMoveNext(); g.Asssert("in");
            g.ForceMoveNext(); string dictKey = g.Current;
            g.ForceMoveNext();
            string? whenBlock = null;
            if (g.Current.value.StartsWith("[") && g.Current.value.EndsWith("]"))
            {
                whenBlock = g.Current;
                g.ForceMoveNext();
            }
            GetScope(g, out Block scope);
            if(whenBlock != null)
            {
                scope.Prepend("{");
                scope.Prepend(whenBlock);
                scope.Prepend("when");
                scope.Add("}");
            }
            if (Parser.HasEnclosingBrackets(scope)) RemoveEnclosingBrackets(scope);

            if (TryGetVariable(varKey, out IVariable? _)) throw new Exception($"Variable {varKey} already exists");
            TryGetVariable(dictKey, out IVariable? dictValue);
            if (dictValue == null) throw new Exception($"Variable {dictKey} does not exist");

            if (dictValue is ArgList)
            {
                Arg arg = ArgList.list.First();
                if (arg is ArcObject arcObject)
                {
                    dictValue = arcObject;
                }
                else if (arg is vx Vc)
                {
                    dictValue = Vc.va.Value;
                }
                else throw new Exception();
            }

            if (dictValue is ArcEnumerable arcEnum)
            {
                IEnumerator<IVariable> enume = arcEnum.GetArcEnumerator();
                if (enume.MoveNext())
                {
                    do
                    {
                        global.Add(varKey, enume.Current);
                        result.Add(compile(scope));
                        global.Delete(varKey);
                    } while (enume.MoveNext());
                }
            }
            else throw new Exception();

            return true;
        }
        if (g.Current == "for")
        {
            g.ForceMoveNext(); string varKey = g.Current;
            g.ForceMoveNext(); g.Asssert("as");
            g.ForceMoveNext(); int start = int.Parse(g.Current);
            g.ForceMoveNext(); g.Asssert("to");
            g.ForceMoveNext(); int end = int.Parse(g.Current);
            g.ForceMoveNext(); GetScope(g, out Block scope);
            if (Parser.HasEnclosingBrackets(scope)) RemoveEnclosingBrackets(scope);

            if (TryGetVariable(varKey, out IVariable? _)) throw new Exception($"Variable {varKey} already exists");
            ArcInt varValue = new(start);
            global.Add(varKey, varValue);

            while (varValue.Value != end)
            {
                result.Add(compile(scope));
                if (varValue.Value > end) varValue.Value--;
                else varValue.Value++;
            }

            global.Delete(varKey);

            return true;
        }
        if (g.Current == "if") 
        { 
            result.Add("if", "=");
            return true;
        }
        if (g.Current == "else_if") 
        { 
            result.Add("else_if", "=");
            return true;
        }
        if (g.Current == "else") 
        { 
            result.Add("else", "=");
            return true;
        }
        if (g.Current.value.EndsWith(','))
        {
            List<string> s = new();
            do
            {
                s.Add(g.Current.value[..^1]);
                if (!g.MoveNext()) throw new Exception();
            } while (g.Current.value.EndsWith(','));
            s.Add(g.Current.value);

            if (!g.MoveNext()) throw new Exception();

            if (g.Current.value.StartsWith('[') && g.Current.value.EndsWith(']'))
            {
                string trigger = g.Current.value[1..^1];

                if (!g.MoveNext()) throw new Exception();
                g = GetScope(g, out Block scope);

                if (Parser.HasEnclosingBrackets(scope)) scope = RemoveEnclosingBrackets(scope);

                Block n = new()
                    {
                        "=", "{",
                            "limit", "=", "{",
                                StringCompile(trigger, CompileTrigger),
                            "}",
                            compile(scope),
                        "}"
                    };

                foreach (string k in s)
                {
                    result.Add(StringCompile(k, compile));
                    result.Add(n);
                }
                return true;
            }
            else if (g.Current.value == "=")
            {
                if (!g.MoveNext()) throw new Exception();

                g = GetScope(g, out Block scope);

                string compiled = compile(scope);
                foreach (string k in s)
                {
                    Block n = new()
                        {
                            StringCompile(k, Compile),
                            "=",
                            "{",
                            compiled,
                            "}"
                        };

                    result.Add(n);
                }
                return true;
            }
            else throw new Exception();
        }
        if (TranspiledString(g.Current, '`', out string? newValue, compile) && newValue != null)
        {
            result.Add(newValue);
            return true;
        }
        if (g.Current.value.EndsWith('%'))
        {
            result.Add((double.Parse(g.Current.value[..^1]) / 100).ToString("0.000"));
            return true;
        }
        if (g.Current.value.StartsWith('[') && g.Current.value.EndsWith(']'))
        {
            string trigger = g.Current.value[1..^1];

            if (!g.MoveNext()) throw new Exception();
            g = GetScope(g, out Block scope);

            if (Parser.HasEnclosingBrackets(scope)) scope = RemoveEnclosingBrackets(scope);

            if (result.Last?.Value.value != "=") result.Add("=");

            result.Add(
                "{",
                    "limit", "=", "{",
                        StringCompile(trigger, CompileTrigger),
                    "}",
                    compile(scope),
                "}"
            );
            return true;
        }
        if (g.Current.value.StartsWith('(') && g.Current.value.EndsWith(')'))
        {
            string calc = g.Current.value[1..^1];

            result.Add(Calculator.Calculate(calc));
            return true;
        }
        if (TryGetVariable(g.Current, out IVariable? var))
        {
            if (var == null) throw new Exception();
            g = var.Call(g, ref result);
            return true;
        }
        return false;
    }
    public static bool TranspiledString(string str, char ch, out string? newValue, Func<Block, string> compile)
    {
        if (TryTrimOne(str, ch, out newValue) && newValue != null)
        {
            StringBuilder s = new();
            StringBuilder nc = new();
            int scope = 0;
            foreach (char c in newValue)
            {
                if (c == '{')
                {
                    if (scope > 0) nc.Append(c);
                    scope++;
                    continue;
                }

                if (c == '}')
                {
                    scope--;
                    if (scope > 0) nc.Append(c);
                    if (scope == 0)
                    {
                        s.Append(StringCompile(nc.ToString(), compile));
                        nc = new();
                    }
                    continue;
                }

                if (scope != 0)
                {
                    nc.Append(c);
                    continue;
                }

                s.Append(c);
            }

            newValue = s.ToString();
            return true;
        }
        return false;
    }
    public static bool IsBaseScope(string v) => v == "ROOT" || v == "PREV" || v == "THIS" || v == "FROM";
    public static bool IsLogicalScope(string v) => v == "NOT" || v == "AND" || v == "OR";
    public static bool IsDefaultScope(string v) => v == "REB" || v == "NAT" || v == "PIR";
    public static string StringCompile(string file, Func<Block, string> compiler, bool preprocessor = false)
    {
        if (preprocessor)
            file = Parser.Preprocessor(file);

        return compiler(Parser.ParseCode(file));
    }
    public static List<(string, NewTrigger)> NewTriggers = new();
    public static string CompileTrigger(Block code)
    {
        if (code.Count == 0)
            return "";

        Block result = new();

        Walker g = new(code);
        do
        {
            if (AllCompile(ref g, ref result, CompileTrigger)) continue;

            if (NewFunctions<NewTrigger, ArcTrigger>(g, ref result, NewTriggers, CompileTrigger)) continue;

            Program.Warn($"Unknown Trigger: {g.Current}");
            result.Add(g.Current);
        } while (g.MoveNext());

        if (Parser.HasEnclosingBrackets(result)) result = RemoveEnclosingBrackets(result);

        return string.Join(' ', result);
    }
    public static List<(string, NewEffect)> NewEffects = new();
    public static int NctAmount = 0;
    public static string CompileEffect(Block code)
    {
        if (code.Count == 0)
            return "";

        Block result = new();

        Walker g = new(code);
        do
        {
            if (AllCompile(ref g, ref result, CompileEffect)) continue;
                
            if(NewFunctions<NewEffect, ArcEffect>(g, ref result, NewEffects, CompileEffect)) continue;

            if (g.Current == "quick_province_modifier")
            {
                g = Args.GetArgs(g, out Args args);
                ArcString name = args.Get(ArcString.Constructor, "name");
                ArcBool permanent = args.Get(ArcBool.Constructor, "permanent", new(true));
                ArcInt? years = args.Get(ArcInt.Constructor, "years", null);
                ArcInt duration;
                if (years == null) duration = args.Get(ArcInt.Constructor, "duration", new(-1));
                else duration = args.Get(ArcInt.Constructor, "duration", new(years.Value * 365));
                ArcString desc = args.Get(ArcString.Constructor, "desc", new(""));
                ArcBool hidden = args.Get(ArcBool.Constructor, "hidden", new(false));
                ArcModifier modifier = args.Get(ArcModifier.Constructor, "modifier");

                new EventModifier($"qem_{QuickEventModifiers}", name, modifier);

                if (permanent.Value) result.Add("add_permanent_province_modifier", "=", "{");
                else result.Add("add_province_modifier", "=", "{");
                result.Add("name", "=", $"qem_{QuickEventModifiers}");
                result.Add("duration", "=", duration.Value);
                if (desc.Value.Count() != 0)
                {
                    result.Add("desc", "=", $"qem_{QuickEventModifiers}_desc ");
                    Program.Localisation.Add($"qem_{QuickEventModifiers}_desc", desc.Value);
                }
                if (hidden.Value) result.Add("hidden", "=", "yes");
                result.Add("}");

                QuickEventModifiers++;
                continue;
            }
                 
            if (g.Current == "quick_country_modifier")
            {
                g = Args.GetArgs(g, out Args args);
                ArcString name = args.Get(ArcString.Constructor, "name");
                ArcInt? years = args.Get(ArcInt.Constructor, "years", null);
                ArcInt duration;
                if (years == null) duration = args.Get(ArcInt.Constructor, "duration", new(-1));
                else duration = args.Get(ArcInt.Constructor, "duration", new(years.Value*365));
                ArcString desc = args.Get(ArcString.Constructor, "desc", new(""));
                ArcBool hidden = args.Get(ArcBool.Constructor, "hidden", new(false));
                ArcModifier modifier = args.Get(ArcModifier.Constructor, "modifier");

                new EventModifier($"qem_{QuickEventModifiers}", name, modifier);

                result.Add("add_country_modifier", "=", "{");
                result.Add("name", "=", $"qem_{QuickEventModifiers}");
                result.Add("duration", "=", duration.Value);
                if (desc.Value.Count() != 0)
                {
                    result.Add("desc", "=", $"qem_{QuickEventModifiers}_desc ");
                    Program.Localisation.Add($"qem_{QuickEventModifiers}_desc", desc.Value);
                }
                if (hidden.Value) result.Add("hidden", "=", "yes");
                result.Add("}");

                QuickEventModifiers++;
                continue;
            }
                 
            if (g.Current == "create_flagship")
            {
                g = Args.GetArgs(g, out Args args);

                Block traits = args.Get(ArcCode.Constructor, "traits", new()).Value;

                if (Parser.HasEnclosingBrackets(traits)) traits = RemoveEnclosingBrackets(traits);

                foreach (Word trait in traits)
                {
                    result.Add("set_country_flag", "=", trait);
                }
                if (traits.Any()) result.Add("set_country_flag", "=", "forced_trait");
                result.Add(
                    args.GetFromList(Province.Provinces, "where").Id, "=", "{",
                        "create_flagship", "=", "{",
                            "name", "=", $"\"{args.Get(ArcString.Constructor, "name")}\"",
                            "type", "=", args.Get(ArcString.Constructor, "type"),
                        "}",
                    "}"
                );
                foreach (Word trait in traits)
                {
                    result.Add("clr_country_flag", "=", trait);
                }
                if (traits.Any()) result.Add("clr_country_flag", "=", "forced_trait");

                continue;
            }

            if (g.Current == "new_custom_tooltip")
            {
                string key = $"nct_{NctAmount}";
                result.Add("custom_tooltip", "=", $"nct_{NctAmount}");
                NctAmount++;
                g.ForceMoveNext(); g.Asssert("=");
                g.ForceMoveNext(); string value = g.Current;
                if (TranspiledString(value, '"', out string? nValue, CompileEffect))
                {
                    if (nValue == null) throw new Exception();
                    Program.Localisation.Add(key, nValue);
                }
                else throw new NotImplementedException();
                continue;
            }

            Program.Warn($"Unknown Effect: {g.Current}");
            result.Add(g.Current);
        } while (g.MoveNext());

        if (Parser.HasEnclosingBrackets(result)) result = RemoveEnclosingBrackets(result);

        return string.Join(' ', result);
    }
    public static bool NewFunctions<T, T2>(Walker g, ref Block result, List<(string, T)> newList, Func<Block, string> compile) where T : ArcObject where T2 : ArcBlock
    {
        string key = g.Current;
        IEnumerable<(string, T)> ThisFunctions = from c in newList where c.Item1 == key select c;
        if (ThisFunctions.Any())
        {
            (string, T) LastClickedEffect = ThisFunctions.Last();

            List<string> Errors = new();

            g = Args.GetArgs(g, out Args args);
            foreach ((string, T) effect in ThisFunctions)
            {
                try
                {
                    T b = effect.Item2;
                    Arg a;
                    if (b.Get<vvC>("args") is Dict<Type>) a = ArcObject.FromArgs(args, b);
                    else a = vx.FromArgs(args, b);

                    ArgList.list.AddFirst(a);

                    if(b.CanGet("transpile")) b.Get<T2>("transpile").Compile(ref result);
                    else
                    {
                        if(ArgList.list.First() is ArcObject @object)
                        {
                            result.Add(key, "=", "{");
                            foreach(KeyValuePair<string, IVariable> t in @object)
                            {
                                if(t.Value is ArcBlock)
                                {
                                    result.Add(t.Key, "=", "{", compile(new($"args:{t.Key}")), "}");
                                }
                                else result.Add(t.Key, "=", compile(new($"args:{t.Key}")));
                            }
                            result.Add("}");
                        }
                        else
                        {
                            result.Add(key, "=", compile(new("args")));
                        }
                    }

                    ArgList.list.RemoveFirst();
                    return true;
                }
                catch (Exception e)
                {
                    if (effect == LastClickedEffect)
                    {
                        foreach (string error in Errors)
                        {
                            Console.WriteLine(error);
                        }
                        throw;
                    }
                    else Errors.Add(e.Message);
                }
            }
            return true;
        }
        return false;
    }
    public static List<(string, NewModifier)> NewModifiers = new();
    public static string CompileModifier(Block code)
    {
        if (code.Count == 0)
            return "";

        Block result = new();

        Walker g = new(code);
        do
        {
            if (AllCompile(ref g, ref result, CompileModifier)) continue;

            if (NewFunctions<NewModifier, ArcModifier>(g, ref result, NewModifiers, CompileModifier)) continue;

            result.Add(g.Current);
        } while (g.MoveNext());

        if (Parser.HasEnclosingBrackets(result)) result = RemoveEnclosingBrackets(result);

        return string.Join(' ', result);
    }
    public static string Compile(Block code)
    {
        if (code.Count == 0)
            return "";

        Block result = new();

        Walker g = new(code);
        do
        {
            if (AllCompile(ref g, ref result, Compile)) continue;
                
            result.Add(g.Current);
        } while (g.MoveNext());

        if (Parser.HasEnclosingBrackets(result)) result = RemoveEnclosingBrackets(result);

        return string.Join(' ', result);
    }
    public static string FactorCompile(Block code)
    {
        if (code.Count == 0)
            return "";

        Block result = new();

        Walker g = new(code);
        do
        {
            if (AllCompile(ref g, ref result, FactorCompile)) continue;
            if (g.Current == "modifier")
            {
                g.ForceMoveNext(); g.Asssert("=");
                g.ForceMoveNext(); g = GetScope(g, out Block scope);
                result.Add("modifier", "=", "{", CompileTrigger(scope), "}");
                continue;
            }
                
            result.Add(g.Current);
        } while (g.MoveNext());

        if (Parser.HasEnclosingBrackets(result)) result = RemoveEnclosingBrackets(result);

        return string.Join(' ', result);
    }
    [GeneratedRegex("{([^}]+)}", RegexOptions.Compiled)]
    public static partial Regex TranspiledString();
}