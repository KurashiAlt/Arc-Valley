﻿using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Numerics;
using Pastel;

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
        { "bi_yearly_events", new ArcBlock() { ShouldBeCompiled = false } },
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
        { "custom_buttons", CustomButton.CustomButtons },
        { "custom_icons", CustomIcon.CustomIcons },
        { "interface_files", InterfaceNode.Files },
        { "units", Unit.Units },
        { "defines", new ArcObject() },
        { "default_reform", new ArcCode() },
        { "terrain_declarations", new ArcBlock() },
        { "tree", new ArcBlock() },
        { "args", new ArgList("args", new()) },
        { "this", new ArgList("this", new()) },
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
    public static void ObjectDeclare(string file, string fileName, bool preprocessor = false)
    {
        if (preprocessor)
            file = Parser.Preprocessor(file);

        ObjectDeclare(Parser.ParseCode(file, fileName));
    }
    public static Walker Declare(Walker g)
    {
        return (string)g.Current switch {
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
            "advisor" => Advisor.Call(g),
            "age" => Age.Call(g),
            "decision" => Decision.Call(g),
            "building_line" => BuildingLine.Call(g),
            "government_mechanic" => GovernmentMechanic.Call(g),
            "diplomatic_action" => DiplomaticAction.Call(g),
            "effect" => NewCommand.CallEffect(g),
            "trigger" => NewCommand.CallTrigger(g),
            "modifier" => NewCommand.CallModifier(g),
            "mission_tree" => MissionTree.Call(g),
            "holy_order" => HolyOrder.Call(g),
            "province_triggered_modifier" => ProvinceTriggeredModifier.Call(g),
            "casus_belli" => CasusBelli.Call(g),
            "war_goal" => WarGoal.Call(g),
            "province_group" => ProvinceGroup.Call(g),
            "personality_trait" => RulerPersonality.Call(g),
            "policy" => Policy.Call(g),
            "subject_type" => SubjectType.Call(g),
            "static_modifier" => StaticModifier.Call(g),
            "define" => CallDefine(g),
            "named_effect" => NamedBlock(g, ArcEffect.Constructor),
            "named_trigger" => NamedBlock(g, ArcTrigger.Constructor),
            "named_modifier" => NamedBlock(g, ArcModifier.Constructor),
            "named_block" => NamedBlock(g, ArcCode.Constructor),
            "custom_button" => CustomButton.Call(g),
            "custom_icon" => CustomIcon.Call(g),
            "interface_file" => InterfaceNode.Call(g),
            "class" => ArcClass.Call(g),
            "struct" => ArcStruct.Call(g),
            _ => DynamicClass(g)
        };
    }
    static Walker DynamicClass(Walker i)
    {
        foreach (KeyValuePair<string, ArcClass> cls in ArcClass.Classes)
        {
            if (i.Current == cls.Key)
            {
                i.ForceMoveNext();

                string id = GetId(i.Current);

                i = Args.GetArgs(i, out Args args);

                cls.Value.Define(id);
                cls.Value.Init(args, id);

                return i;
            }
        }

        throw ArcException.Create($"Unknown Object Type {i.Current} in object declaration", i);
    }
    static Walker NamedBlock(Walker i, Func<Block, IVariable> constructor)
    {
        i.ForceMoveNext();

        string id = GetId(i.Current);

        i = Args.GetArgs(i, out Args args);

        global.Add(id, constructor(args.block));

        return i;
    }
    static Walker CallDefine(Walker i)
    {
        i.ForceMoveNext();

        string id = GetId(i.Current);

        i = Args.GetArgs(i, out Args args);

        global.Get<ArcObject>("defines").Add(id, new ArcString(args.block));

        return i;
    }
    static Walker DefineLoc(Walker i)
    {
        i.ForceMoveNext();
        ArcString key = new(new Block(i.Current));
        i.ForceMoveNext();
        i.Asssert("=");
        i.ForceMoveNext();
        ArcString value = new(new Block(i.Current));

        if (Program.Localisation.ContainsKey(key.Value))
        {
            if (Program.Localisation[key.Value] != value.Value) throw ArcException.Create($"{key} already exists, and has a different value than the provided input", i);
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
            if (g.Current == "breakpoint")
            {
                Debugger.Break();
                continue;
            }
            if (g.Current == "new")
            {
                g.ForceMoveNext();
                g = Declare(g);
                continue;
            }
            else if (g.Current == "run_effect")
            {
                g = Args.GetArgs(g, out Args args);
                if (args.block == null) throw ArcException.Create("Unknown Error: Null Reference", g, args);
                Compile(CompileType.Effect, args.block);
                continue;
            }
            if (g.Current == "write_file")
            {
                g.ForceMoveNext();
                string file = GetId(g.Current);
                g = Args.GetArgs(g, out Args args);
                ArcCode blo = ArcCode.NamelessConstructor(args.block);
                Program.OverwriteFile($"{Program.TranspileTarget}/{file}", blo.Compile());
            }
            else if (TryGetVariable(g.Current, out IVariable? var))
            {
                Block f = new();
                if (var == null) throw ArcException.Create("Unknown Error: Null Reference", g);
                g = var.Call(g, ref f);
            }
            else if (NewFunctions(g, ref result, NewEffects)) continue;
            else throw ArcException.Create($"Invalid command in Object Declaration: {g.Current}", g, result, code);
        } while (g.MoveNext());
    }
    public static Dict<ArcObject> ModifierLocs = Dict<ArcObject>.Constructor((string id, Args args) => new ArcObject() {
        { "text", new ArcString(args.Get("text")) },
        { "localisation_key", new ArcString(args.Get("localisation_key")) },
        { "multiplier", new ArcFloat(args.Get("multiplier")) },
        { "percent", new ArcBool(args.Get("percent")) },
        { "is_good", new ArcBool(args.Get("is_good")) },
        { "is_bool", new ArcBool(args.Get("is_bool")) },
        { "is_multiplicative", new ArcBool(args.Get("is_multiplicative")) },
        { "precision", new ArcInt(args.Get("precision")) },
    })(Parser.ParseFile($"{Program.directory}modifier_loc.txt"));
    public static ArcObject ModifierLocConstructor(string id, Args args)
    {
        return new ArcObject()
        {
            { "text", args.Get(ArcString.Constructor, "text") },
            { "localisation_key", args.Get(ArcString.Constructor, "localisation_key") },
            { "multiplier", args.Get(ArcFloat.Constructor, "multiplier") },
            { "percent", args.Get(ArcBool.Constructor, "percent") },
            { "is_good", args.Get(ArcBool.Constructor, "is_good") },
            { "is_bool", args.Get(ArcBool.Constructor, "is_bool") },
            { "is_multiplicative", args.Get(ArcBool.Constructor, "is_multiplicative") },
            { "precision", args.Get(ArcInt.Constructor, "precision") },
        };
    }
    public static void __new(ref Walker g)
    {
        g.ForceMoveNext();
        g = Declare(g);
    }
    public static void __LOG_CURRENT_COMPILE(Block result)
    {
        Console.WriteLine(Parser.FormatCode(result.ToString()));
    }
    public static void __write_file(ref Walker g)
    {
        g.ForceMoveNext();
        string file = GetId(g.Current);
        g = Args.GetArgs(g, out Args args);
        ArcCode blo = ArcCode.NamelessConstructor(args.block);
        Program.OverwriteFile($"{Program.TranspileTarget}/{file}", blo.Compile());
    }
    public static void __delete(ref Walker g)
    {
        g = Args.GetArgs(g, out Args args);

        ArcString key = ArcString.Constructor(args.block);
        global.Delete(key.Value);
    }
    public static void __when(ref Walker g, ref Block result, CompileType type, ArcObject? bound, bool previous = false)
    {
        g.ForceMoveNext();
        string trigger = g.Current.Value[1..^1];
        string file = g.Current.GetFile();

        g.ForceMoveNext();
        Block scope = g.GetScope();
        scope.RemoveEnclosingBlock();

        bool Click = WhenInterpret(trigger);
        if (Click && !previous) result.Add(Compile(type, scope, bound));

        if (!g.MoveNext()) return;
        if (g == "when_not_if")
        {
            __when(ref g, ref result, type, bound, Click);
        }
        else if (g == "when_not")
        {
            g.ForceMoveNext();
            scope = g.GetScope();
            scope.RemoveEnclosingBlock();

            if (!Click) result.Add(Compile(type, scope, bound));
        }
        else
        {
            g.ForceMoveBack();
            return;
        }

        bool WhenInterpret(string trigger)
        {
            Block b = Parser.ParseCode(trigger, file);
            if (Parser.HasEnclosingBrackets(b)) b = RemoveEnclosingBrackets(b);
            return When(b);
        }
    }
    public static void __DEFINE_MODIFIER(ref Walker g)
    {
        g = Args.GetArgs(g, out Args args);

        ArcString key = args.Get(ArcString.Constructor, "key");

        ModifierLocs.Add(key.Value, ModifierLocConstructor(key.Value, args));
    }
    public static void __modifier_to_string(ref Walker g, ref Block result)
    {
        g = Args.GetArgs(g, out Args args);
        if (args.block != null && TryGetVariable(args.block.ToWord(), out IVariable? vr))
        {
            if (vr == null) throw ArcException.Create("Unknown Error: Null Reference", g);
            if (vr is not ArcModifier) throw ArcException.Create($"{string.Join(' ', args.block)} is not ArcModifier in modifier_to_string function", g);
            Block c = new("{");
            foreach (Word w in Parser.ParseCode(((ArcModifier)vr).Compile(), g.Current.GetFile()))
            {
                c.Add(w);
            }
            c.Add("}");

            args = Args.GetArgs(c);
        }
        if (args.keyValuePairs == null) throw ArcException.Create("Unknown Error: Null Reference", g);
        string str = "";
        foreach (KeyValuePair<string, Block> b in args.keyValuePairs)
        {
            ArcObject modInfo = ModifierLocs[b.Key];
            string text = modInfo.Get<ArcString>("text").Value;
            string key = modInfo.Get<ArcString>("localisation_key").Value;
            if (Program.Localisation.TryGetValue(key, out string? v)) text = v;
            bool percent = modInfo.Get<ArcBool>("percent").Value;
            bool isBool = modInfo.Get<ArcBool>("is_bool").Value;
            bool isGood = modInfo.Get<ArcBool>("is_good").Value;
            int precision = modInfo.Get<ArcInt>("precision").Value;
            double multiplier = modInfo.Get<ArcFloat>("multiplier").Value;

            Word value = b.Value.ToWord();

            if (isBool)
            {
                bool nValue = new ArcBool(value).Value;

                str += $"{text}: §{(isGood == nValue ? 'G' : 'R')}{value}§!";
            }
            else
            {
                double nValue = new ArcFloat(value).Value * multiplier;

                str += $"{text}: §{(isGood == nValue >= 0 ? 'G' : 'R')}{nValue.ToString($"F{precision}")}{(percent ? "%" : "")}§!";
            }

            str += '\n';
        }
        result.Add(str.Trim());
    }
    public static void __id_to_name(ref Walker g, ref Block result)
    {
        g = Args.GetArgs(g, out Args args);
        IVariable i0 = GetVariable<IVariable>(args.block.ToWord());
        string i1 = i0.ToString() ?? "";
        string i2 = i1.Replace('_', ' ');
        Block i3 = Parser.ParseCode(i2, g.Current.GetFile());
        IEnumerable<string> i4 = from iz in i3 select ToUpperFirstLetter(iz);
        result.Add(string.Join(' ', i4));

        string ToUpperFirstLetter(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            // convert to char array of the string
            char[] letters = source.ToCharArray();
            // upper case the first char
            letters[0] = char.ToUpper(letters[0]);
            // return the array made of the new char array
            return new string(letters);
        }
    }
    public static void __foreach(ref Walker g, ref Block result, CompileType type, ArcObject? bound)
    {
        g.ForceMoveNext(); Word varKey = g.Current;
        g.ForceMoveNext(); g.Asssert("in");
        g.ForceMoveNext(); Word dictKey = g.Current;
        g.ForceMoveNext();
        string? whenBlock = null;
        if (g.Current.Value.StartsWith("[") && g.Current.Value.EndsWith("]"))
        {
            whenBlock = g.Current;
            g.ForceMoveNext();
        }
        GetScope(g, out Block scope);
        if (whenBlock != null)
        {
            scope.Prepend("{");
            scope.Prepend(whenBlock);
            scope.Prepend("when");
            scope.Add("}");
        }
        if (Parser.HasEnclosingBrackets(scope)) RemoveEnclosingBrackets(scope);

        if (TryGetVariable(varKey, out IVariable? _)) throw ArcException.Create($"Variable {varKey} already exists", g);
        TryGetVariable(dictKey, out IVariable? dictValue);
        if (dictValue == null) throw ArcException.Create($"Variable {dictKey} does not exist", g);

        if (dictValue is ArgList lst)
        {
            IVariable arg = lst.list.First();
            if (arg is ArcObject arcObject)
            {
                dictValue = arcObject;
            }
            else if (arg is IVariable Vc)
            {
                dictValue = Vc;
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
                    result.Add(Compile(type, scope, bound));
                    global.Delete(varKey);
                } while (enume.MoveNext());
            }
        }
        else throw new Exception();
    }
    public static void __for(ref Walker g, ref Block result, CompileType type, ArcObject? bound)
    {
        g.ForceMoveNext(); Word varKey = g.Current;
        g.ForceMoveNext(); g.Asssert("as");
        g.ForceMoveNext(); int start = new ArcInt(g.Current).Value;
        g.ForceMoveNext(); g.Asssert("to");
        g.ForceMoveNext(); int end = new ArcInt(g.Current).Value;
        g.ForceMoveNext(); GetScope(g, out Block scope);
        if (Parser.HasEnclosingBrackets(scope)) RemoveEnclosingBrackets(scope);

        if (TryGetVariable(varKey, out IVariable? _)) throw ArcException.Create($"Variable {varKey} already exists", g);
        ArcInt varValue = new(start);
        global.Add(varKey, varValue);

        while (varValue.Value != end)
        {
            result.Add(Compile(type, scope, bound));
            if (varValue.Value > end) varValue.Value--;
            else varValue.Value++;
        }

        global.Delete(varKey);
    }
    public static void __if(ref Walker g, ref Block result)
    {
        if (g.MoveNext())
        {
            if (g.Current != "=") g.MoveBack();
        }
        result.Add("if", "=");
    }
    public static void __else_if(ref Walker g, ref Block result)
    {
        if (g.MoveNext())
        {
            if (g.Current != "=") g.MoveBack();
        }
        result.Add("else_if", "=");
    }
    public static void __else(ref Walker g, ref Block result)
    {
        if (g.MoveNext())
        {
            if (g.Current != "=") g.MoveBack();
        }
        result.Add("else", "=");
    }
    public static void __arc_throw(ref Walker g, CompileType type, ArcObject? bound)
    {
        g.ForceMoveNext();
        g.Asssert("=");
        g.ForceMoveNext();
        string value = g.Current;
        if (TranspiledString(value, '"', out string? nValue, type, bound, g.Current.GetFile()) && nValue != null)
        {
            throw ArcException.Create(nValue, g);
        }
        else throw ArcException.Create(value, g);
    }
    public static void __arc_log(ref Walker g, CompileType type, ArcObject? bound)
    {
        g.ForceMoveNext();
        g.Asssert("=");
        g.ForceMoveNext();
        string value = g.Current;
        if (TranspiledString(value, '"', out string? nValue, type, bound, g.Current.GetFile()))
        {
            Console.WriteLine($"{g.Current.GetFile()} line {g.Current.File}: {nValue}");
        }
        else Console.WriteLine($"{g.Current.GetFile()} line {g.Current.File}: {value}");
    }
    public static void __variable(ref Walker g, ref Block result)
    {
        string left = g.Current.Value[1..];
        if (!VariableExists(left)) Console.WriteLine(ArcException.CreateMessage($"Variable Definition not found for {left}", g));

        g.ForceMoveNext();
        Word Operator = g.Current;
        g.ForceMoveNext();
        g = GetScope(g, out Block rightBlock);
        Args args = Args.GetArgs(rightBlock);
        switch (Operator)
        {
            case ":=": VariableOperator("set_variable", ref result); break;
            case "+=": VariableOperator("change_variable", ref result); break;
            case "-=": VariableOperator("subtract_variable", ref result); break;
            case "*=": VariableOperator("multiply_variable", ref result); break;
            case "/=": VariableOperator("divide_variable", ref result); break;
            case "&=": VariableOperator("export_to_variable", ref result, BothAreValue: true); break;
            case ">=": VariableOperator("check_variable", ref result); break;
            case ">": VariableOperator("check_variable", ref result, CheckNotEqual: true); break;
            case "<=": VariableOperator("check_variable", ref result, CheckOrEqual: true, Invert: true); break;
            case "<": VariableOperator("check_variable", ref result, Invert: true); break;
            case "==": VariableOperator("is_variable_equal", ref result); break;
            case "!=": VariableOperator("is_variable_equal", ref result, Invert: true); break;
            default: throw ArcException.Create($"While performing a quick variable operation '&' syntax, operator {Operator} was not recognized", left, Operator, args, result);
        }

        void VariableOperator(string command, ref Block result, bool Invert = false, bool CheckOrEqual = false, bool CheckNotEqual = false, bool BothAreValue = false)
        {
            if (CheckNotEqual) result.Add("AND", "=", "{");
            if (CheckOrEqual) result.Add("OR", "=", "{");
            if (Invert) result.Add("NOT", "=", "{");
            try
            {
                result.Add(
                    command, "=", "{",
                        "which", "=", left,
                        "value", "=", new ArcFloat(args.block),
                    "}"
                );
                if (CheckNotEqual) result.Add(
                    "NOT", "=", "{",
                        "is_variable_equal", "=", "{",
                            "which", "=", left,
                            "value", "=", new ArcFloat(args.block),
                        "}",
                    "}"
                );
                if (Invert) result.Add("}");
                if (CheckOrEqual) result.Add(
                    "is_variable_equal", "=", "{",
                        "which", "=", left,
                        "value", "=", new ArcFloat(args.block),
                    "}"
                );
            }
            catch
            {
                result.Add(
                    command, "=", "{",
                        "which", "=", left,
                        BothAreValue ? "value" : "which", "=", args.block,
                    "}"
                );
                if (CheckNotEqual) result.Add(
                    "NOT", "=", "{",
                        "is_variable_equal", "=", "{",
                            "which", "=", left,
                            "which", "=", args.block,
                        "}",
                    "}"
                );
                if (Invert) result.Add("}");
                if (CheckOrEqual) result.Add(
                    "is_variable_equal", "=", "{",
                        "which", "=", left,
                        "which", "=", args.block,
                    "}"
                );
                if (!VariableExists(left)) Console.WriteLine(ArcException.CreateMessage($"Variable Definition not found for {left}", left, Operator, args, command, result));
            }
            if (CheckOrEqual) result.Add("}");
            if (CheckNotEqual) result.Add("}");
        }
        bool VariableExists(string id)
        {
            if (global.CanGet("variables"))
                if (global.Get<IArcObject>("variables").CanGet(left))
                    return true;
            return false;
        }
    }
    public static void __multi_scope(ref Walker g, ref Block result, CompileType type, ArcObject? bound)
    {
        string pre = g.Current.Value.Split(':')[0];
        List<string> s = new() { g.Current.Value[..^1] };
        while (g.Current.Value.EndsWith(',') || g.Current.Value.EndsWith(';'))
        {
            if (g.Current.Value.EndsWith(';'))
            {
                g.ForceMoveNext();
                if (g.Current.Value.EndsWith(',') || g.Current.Value.EndsWith(';')) s.Add($"{pre}:{g.Current.Value[..^1]}");
                else s.Add($"{pre}:{g.Current.Value}"); ;
            }
            else
            {
                g.ForceMoveNext();
                if (g.Current.Value.EndsWith(',') || g.Current.Value.EndsWith(';')) s.Add(g.Current.Value[..^1]);
                else s.Add(g.Current.Value);
            }
        }

        g.ForceMoveNext();

        if (g.Current.Value.StartsWith('[') && g.Current.Value.EndsWith(']'))
        {
            string trigger = g.Current.Value[1..^1];

            g.ForceMoveNext();
            string file = g.Current.GetFile();
            g = GetScope(g, out Block scope);

            if (Parser.HasEnclosingBrackets(scope)) scope = RemoveEnclosingBrackets(scope);

            Block n = new()
                    {
                        "=", "{",
                            "limit", "=", "{",
                                StringCompile(trigger, file, CompileType.Trigger, bound),
                            "}",
                            Compile(type, scope, bound),
                        "}"
                    };

            foreach (string k in s)
            {
                result.Add(StringCompile(k, file, type, bound));
                result.Add(n);
            }
        }
        else if (g.Current.Value == "=")
        {
            g.ForceMoveNext();

            string file = g.Current.GetFile();
            g = GetScope(g, out Block scope);

            string compiled = Compile(type, scope, bound);
            foreach (string k in s)
            {
                Block n = new()
                {
                    StringCompile(k, file, type, bound), "=", "{",
                        compiled,
                    "}"
                };

                result.Add(n);
            }
        }
        else throw ArcException.Create("Unknown Error", g);
    }
    public static void __quick_limit(ref Walker g, ref Block result, CompileType type, ArcObject? bound)
    {
        string trigger = g.Current.Value[1..^1];
        string file = g.Current.GetFile();

        g.ForceMoveNext();
        g = GetScope(g, out Block scope);

        if (Parser.HasEnclosingBrackets(scope)) scope = RemoveEnclosingBrackets(scope);

        if (result.Last?.Value.Value != "=") result.Add("=");

        result.Add(
            "{",
                "limit", "=", "{",
                    StringCompile(trigger, file, CompileType.Trigger, bound),
                "}",
                Compile(type, scope, bound),
            "}"
        );
    }
    public static void __quick_math(ref Walker g, ref Block result)
    {
        string calc = g.Current.Value[1..^1];

        result.Add(Calculator.Calculate(new Word(calc, g)));
    }
    public static void __float_random(ref Walker g, ref Block result)
    {
        g = Args.GetArgs(g, out Args args);

        double Chance = args.Get(ArcFloat.Constructor, "chance").Value;
        string Effect = args.Get(ArcEffect.Constructor, "effect").Compile();

        if (Chance >= 1)
        {
            result.Add(
                "random_list", "=", "{",
                    (int)Chance, "=", "{",
                        Effect,
                    "}",
                    100 - (int)Chance, "=", "{",
                        "random", "=", "{",
                            "chance", "=", "1",
                            "random_list", "=", "{",
                                (int)(Chance % 1 * 100), "=", "{",
                                    Effect,
                                "}",
                                100 - (int)(Chance % 1 * 100), "=", "{", "}",
                            "}",
                        "}",
                    "}",
                "}"
            );
        }
        else
        {
            result.Add(
                "random", "=", "{",
                    "chance", "=", "1",
                    "random_list", "=", "{",
                        (int)(Chance % 1 * 100), "=", "{",
                            Effect,
                        "}",
                        100 - (int)(Chance % 1 * 100), "=", "{", "}",
                    "}",
                "}"
            );
        }
    }
    public static void __quick_province_modifier(ref Walker g, ref Block result)
    {
        g = Args.GetArgs(g, out Args args);

        ArcString id = args.Get(ArcString.Constructor, "id", new(""));
        if (id.Value == "")
        {
            id.Value = $"qem_{QuickEventModifiers}";
            QuickEventModifiers++;
        }

        ArcString name = args.Get(ArcString.Constructor, "name");
        ArcBool permanent = args.Get(ArcBool.Constructor, "permanent", new(true));
        ArcInt? years = args.Get(ArcInt.Constructor, "years", null);
        ArcInt duration;
        if (years == null) duration = args.Get(ArcInt.Constructor, "duration", new(-1));
        else duration = args.Get(ArcInt.Constructor, "duration", new(years.Value * 365));
        ArcString desc = args.Get(ArcString.Constructor, "desc", new(""));
        ArcBool hidden = args.Get(ArcBool.Constructor, "hidden", new(false));
        ArcModifier modifier = args.Get(ArcModifier.Constructor, "modifier");

        GetVariable<Dict<IVariable>>(new Word("event_modifiers")).Add(
            id.Value,
            new ArcObject()
            {
                        { "id", id },
                        { "name", name },
                        { "modifier", modifier }
            }
        );

        if (permanent.Value) result.Add("add_permanent_province_modifier", "=", "{");
        else result.Add("add_province_modifier", "=", "{");
        result.Add("name", "=", $"{id}");
        result.Add("duration", "=", duration.Value);
        if (desc.Value.Count() != 0)
        {
            result.Add("desc", "=", $"{id}_desc ");
            Program.Localisation.Add($"{id}_desc", desc.Value);
        }
        if (hidden.Value) result.Add("hidden", "=", "yes");
        result.Add("}");
    }
    public static void __quick_country_modifier(ref Walker g, ref Block result)
    {
        g = Args.GetArgs(g, out Args args);

        ArcString id = args.Get(ArcString.Constructor, "id", new(""));
        if (id.Value == "")
        {
            id.Value = $"qem_{QuickEventModifiers}";
            QuickEventModifiers++;
        }

        ArcString name = args.Get(ArcString.Constructor, "name");
        ArcInt? years = args.Get(ArcInt.Constructor, "years", null);
        ArcInt duration;
        if (years == null) duration = args.Get(ArcInt.Constructor, "duration", new(-1));
        else duration = args.Get(ArcInt.Constructor, "duration", new(years.Value * 365));
        ArcString desc = args.Get(ArcString.Constructor, "desc", new(""));
        ArcBool hidden = args.Get(ArcBool.Constructor, "hidden", new(false));
        ArcModifier modifier = args.Get(ArcModifier.Constructor, "modifier");

        GetVariable<Dict<IVariable>>(new Word("event_modifiers")).Add(
            id.Value,
            new ArcObject()
            {
                        { "id", id },
                        { "name", name },
                        { "modifier", modifier }
            }
        );

        result.Add("add_country_modifier", "=", "{");
        result.Add("name", "=", $"{id}");
        result.Add("duration", "=", duration.Value);
        if (desc.Value.Count() != 0)
        {
            result.Add("desc", "=", $"{id}_desc ");
            Program.Localisation.Add($"{id}_desc", desc.Value);
        }
        if (hidden.Value) result.Add("hidden", "=", "yes");
        result.Add("}");

        QuickEventModifiers++;
    }
    public static string TranspiledString(string newValue, CompileType type, ArcObject? bound, string fileName)
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
                    string tv = StringCompile(nc.ToString(), fileName, type, bound);
                    string[] tvc = tv.Split('\n');
                    string tc = s.ToString();
                    int indent = 0;
                    if (tc.Contains('\n'))
                    {
                        int ti = tc.LastIndexOf('\n');
                        while (true)
                        {
                            ti++;

                            if (ti < tc.Length && tc[ti] == '\t') indent++;
                            else break;
                        }
                    }
                    s.Append(tvc[0]);
                    for (int i = 1; i < tvc.Length; i++)
                    {
                        s.Append("\\n");
                        s.Append(new string('\t', indent));
                        s.Append(tvc[i]);
                    }
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

        return s.ToString();
    }
    public static bool TranspiledString(string str, char ch, out string? newValue, CompileType type, ArcObject? bound, string fileName)
    {
        if (TryTrimOne(str, ch, out newValue) && newValue != null)
        {
            newValue = TranspiledString(newValue, type, bound, fileName);
            return true;
        }
        return false;
    }
    public static bool IsBaseScope(string v) => v == "ROOT" || v == "PREV" || v == "THIS" || v == "FROM";
    public static bool IsLogicalScope(string v) => v == "NOT" || v == "AND" || v == "OR";
    public static bool IsDefaultScope(string v) => v == "REB" || v == "NAT" || v == "PIR";
    public static string StringCompile(string file, string fileName, CompileType type, ArcObject? bound, bool preprocessor = false)
    {
        if (preprocessor)
            file = Parser.Preprocessor(file);

        return Compile(type, Parser.ParseCode(file, fileName), bound);
    }
    
    public static List<(string, NewCommand)> NewTriggers = new();
    public static List<(string, NewCommand)> NewEffects = new();
    public static List<(string, NewCommand)> NewModifiers = new();
    public static bool NewFunctions(Walker g, ref Block result, List<(string, NewCommand)> newList)
    {
        Word key = g.Current;
        IEnumerable<(string, NewCommand)> ThisFunctions = from c in newList where c.Item1 == key select c;
        if (ThisFunctions.Any())
        {
            (string, NewCommand) LastClickedEffect = ThisFunctions.Last();

            List<string> Errors = new();

            g = Args.GetArgs(g, out Args args);
            foreach ((string, NewCommand) effect in ThisFunctions)
            {
                try
                {
                    effect.Item2.Call(args, ref result);
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
    
    
    [GeneratedRegex("{([^}]+)}", RegexOptions.Compiled)]
    public static partial Regex TranspiledString();
}