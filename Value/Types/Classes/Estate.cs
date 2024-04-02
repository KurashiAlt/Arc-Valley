
using Pastel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Arc;
public class EstateAgenda : IArcObject
{
    public static Dict<EstateAgenda> EstateAgendas = new();
    public bool IsObject() => true;
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }

    public ArcString Id { get; set; }
    public ArcFloat? MaxDaysActive { get; set; }
    public ArcTrigger CanSelect { get; set; }
    public ArcTrigger ProvincesToHighlight { get; set; }
    public ArcCode SelectionWeight { get; set; }
    public ArcEffect PreEffect { get; set; }
    public ArcEffect ImmediateEffect { get; set; }
    public ArcTrigger TaskRequirements { get; set; }
    public ArcEffect TaskCompletedEffect { get; set; }
    public ArcEffect FailingEffect { get; set; }
    public ArcTrigger FailIf { get; set; }
    public ArcEffect OnInvalid { get; set; }
    public ArcTrigger InvalidTrigger { get; set; }
    public ArcModifier Modifier { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public EstateAgenda(string id, ArcString name, ArcString desc, ArcFloat? maxDaysActive, ArcTrigger canSelect, ArcTrigger provincesToHighlight, ArcCode selectionWeight, ArcEffect preEffect, ArcEffect immediateEffect, ArcTrigger taskRequirements, ArcEffect taskCompletedEffect, ArcEffect failingEffect, ArcTrigger failIf, ArcEffect onInvalid, ArcTrigger invalidTrigger, ArcModifier modifier)
    {
        Id = new($"{id}_agenda");
        Name = name;
        Desc = desc;
        MaxDaysActive = maxDaysActive;
        CanSelect = canSelect;
        ProvincesToHighlight = provincesToHighlight;
        SelectionWeight = selectionWeight;
        PreEffect = preEffect;
        ImmediateEffect = immediateEffect;
        TaskRequirements = taskRequirements;
        TaskCompletedEffect = taskCompletedEffect;
        FailingEffect = failingEffect;
        FailIf = failIf;
        OnInvalid = onInvalid;
        InvalidTrigger = invalidTrigger;
        Modifier = modifier;
        KeyValuePairs = new()
        {
            { "max_days_active", MaxDaysActive },
            { "can_select", CanSelect },
            { "provinces_to_highlight", ProvincesToHighlight },
            { "selection_weight", SelectionWeight },
            { "pre_effect", PreEffect },
            { "immediate_effect", ImmediateEffect },
            { "task_requirements", TaskRequirements },
            { "task_completed_effect", TaskCompletedEffect },
            { "failing_effect", FailingEffect },
            { "fail_if", FailIf },
            { "on_invalid", OnInvalid },
            { "invalid_trigger", InvalidTrigger },
            { "modifier", Modifier },
        };

        EstateAgendas.Add(id, this);
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        i.ForceMoveNext();

        string id = Compiler.GetId(i.Current);

        i = Args.GetArgs(i, out Args args);

        new EstateAgenda(
            id,
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc"),
            args.Get(ArcFloat.Constructor, "max_days_active", null),
            args.Get(ArcTrigger.Constructor, "can_select"),
            args.Get(ArcTrigger.Constructor, "provinces_to_highlight"),
            args.Get(ArcCode.Constructor, "selection_weight"),
            args.Get(ArcEffect.Constructor, "pre_effect"),
            args.Get(ArcEffect.Constructor, "immediate_effect"),
            args.Get(ArcTrigger.Constructor, "task_requirements"),
            args.Get(ArcEffect.Constructor, "task_completed_effect"),
            args.Get(ArcEffect.Constructor, "failing_effect"),
            args.Get(ArcTrigger.Constructor, "fail_if"),
            args.Get(ArcEffect.Constructor, "on_invalid"),
            args.Get(ArcTrigger.Constructor, "invalid_trigger"),
            args.Get(ArcModifier.Constructor, "modifier")
        );

        return i;
    }
    public static string Transpile()
    {
        Block b = new();

        foreach(EstateAgenda agenda in EstateAgendas.Values())
        {
            Program.Localisation.Add($"{agenda.Id}", agenda.Name.Value);
            Program.Localisation.Add($"{agenda.Id}_desc", agenda.Desc.Value);

            b.Add(agenda.Id);
            b.Add("=");
            b.Add("{");
            if(agenda.MaxDaysActive != null)
            {
                b.Add("max_days_active");
                b.Add("=");
                b.Add(agenda.MaxDaysActive.ToString());
            }
            agenda.CanSelect.Compile("can_select", ref b);
            agenda.ProvincesToHighlight.Compile("provinces_to_highlight", ref b);
            agenda.SelectionWeight.Compile("selection_weight", ref b);
            agenda.PreEffect.Compile("pre_effect", ref b);
            agenda.ImmediateEffect.Compile("immediate_effect", ref b);
            agenda.TaskRequirements.Compile("task_requirements", ref b);
            agenda.TaskCompletedEffect.Compile("task_completed_effect", ref b);
            agenda.FailingEffect.Compile("failing_effect", ref b);
            agenda.FailIf.Compile("fail_if", ref b);
            agenda.OnInvalid.Compile("on_invalid", ref b);
            agenda.InvalidTrigger.Compile("invalid_trigger", ref b);
            agenda.Modifier.Compile("modifier", ref b);
            b.Add("}");
        }

        Program.OverwriteFile($"{Program.TranspileTarget}/common/estate_agendas/arc.txt", string.Join(' ', b));
        return "Agendas";
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}
public class EstatePrivilege : IArcObject
{
    public static Dict<EstatePrivilege> EstatePrivileges = new();
    public bool IsObject() => true;
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }

    public ArcString Id { get; set; }
    public ArcString Icon { get; set; }
    public ArcFloat LandShare { get; set; }
    public ArcFloat MaxAbsolutism { get; set; }
    public ArcFloat Loyalty { get; set; }
    public ArcFloat Influence { get; set; }
    public ArcTrigger IsValid { get; set; }
    public ArcTrigger CanSelect { get; set; }
    public ArcEffect OnGranted { get; set; }
    public ArcTrigger CanRevoke { get; set; }
    public ArcEffect OnRevoked { get; set; }
    public ArcEffect OnInvalid { get; set; }
    public ArcModifier Penalties { get; set; }
    public ArcModifier Benefits { get; set; }
    public ArcList<ArcCode> ConditionalModifiers { get; set; }
    public ArcModifier ModifierByLandOwnership { get; set; }
    public ArcCode Mechanics { get; set; }
    public ArcInt CooldownYears { get; set; }
    public ArcEffect OnCooldownExpires { get; set; }
    public ArcCode AiWillDo { get; set; }
    public ArcEffect? OnGrantedProvince { get; set; }
    public ArcEffect? OnRevokedProvince { get; set; }
    public ArcEffect? OnInvalidProvince { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public EstatePrivilege(
        string id, 
        ArcString name, 
        ArcString desc, 
        ArcString icon, 
        ArcFloat landShare, 
        ArcFloat maxAbsolutism, 
        ArcFloat loyalty, 
        ArcFloat influence, 
        ArcTrigger isValid, 
        ArcTrigger canSelect, 
        ArcEffect onGranted, 
        ArcTrigger canRevoke, 
        ArcEffect onInvalid, 
        ArcEffect onRevoked, 
        ArcModifier penalties, 
        ArcModifier benefits, 
        ArcList<ArcCode> conditionalModifiers, 
        ArcModifier modifierByLandOwnership, 
        ArcCode mechanics, 
        ArcInt cooldownYears,
        ArcEffect onCooldownExpires, 
        ArcCode aiWillDo,
        ArcEffect? onGrantedProvince,
        ArcEffect? onRevokedProvince,
        ArcEffect? onInvalidProvince
    ) {
        Name = name;
        Desc = desc;
        Id = new($"{id}_privelege");
        Icon = icon;
        LandShare = landShare;
        MaxAbsolutism = maxAbsolutism;
        Loyalty = loyalty;
        Influence = influence;
        IsValid = isValid;
        CanSelect = canSelect;
        OnGranted = onGranted;
        CanRevoke = canRevoke;
        OnRevoked = onRevoked;
        OnInvalid = onInvalid;
        Penalties = penalties;
        Benefits = benefits;
        ConditionalModifiers = conditionalModifiers;
        ModifierByLandOwnership = modifierByLandOwnership;
        Mechanics = mechanics;
        CooldownYears = cooldownYears;
        OnCooldownExpires = onCooldownExpires;
        AiWillDo = aiWillDo;
        OnGrantedProvince = onGrantedProvince;
        OnRevokedProvince = onRevokedProvince;
        OnInvalidProvince = onInvalidProvince;
        KeyValuePairs = new()
        {
            { "name", Name },
            { "desc", Desc },
            { "id", Id },
            { "icon", Icon },
            { "land_share", LandShare },
            { "max_absolutism", MaxAbsolutism },
            { "loyalty", Loyalty },
            { "influence", Influence },
            { "is_valid", IsValid },
            { "can_select", CanSelect },
            { "on_granted", OnGranted },
            { "can_revoke", CanRevoke },
            { "on_revoked", OnRevoked },
            { "on_invalid", OnInvalid },
            { "penalties", Penalties },
            { "benefits", Benefits },
            { "conditional_modifiers", ConditionalModifiers },
            { "modifier_by_land_ownership", ModifierByLandOwnership },
            { "mechanics", Mechanics },
            { "cooldown_years", CooldownYears },
            { "on_cooldown_expires", OnCooldownExpires },
            { "ai_will_do", AiWillDo },
            { "on_granted_province", OnGrantedProvince },
            { "on_revoked_province", OnRevokedProvince },
            { "on_invalid_province", OnInvalidProvince }
        };

        EstatePrivileges.Add(id, this);
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        i.ForceMoveNext();

        string id = Compiler.GetId(i.Current);

        i = Args.GetArgs(i, out Args args);

        Constructor(id, args);

        return i;
    }
    public static EstatePrivilege Constructor(string key, Args args) => new EstatePrivilege(
        key,
        args.Get(ArcString.Constructor, "name"),
        args.Get(ArcString.Constructor, "desc", new("")),
        args.Get(ArcString.Constructor, "icon"),
        args.Get(ArcFloat.Constructor, "land_share", new(0)),
        args.Get(ArcFloat.Constructor, "max_absolutism", new(0)),
        args.Get(ArcFloat.Constructor, "loyalty", new(0)),
        args.Get(ArcFloat.Constructor, "influence", new(0)),
        args.Get(ArcTrigger.Constructor, "is_valid", new()),
        args.Get(ArcTrigger.Constructor, "can_select", new()),
        args.Get(ArcEffect.Constructor, "on_granted", new()),
        args.Get(ArcTrigger.Constructor, "can_revoke", new()),
        args.Get(ArcEffect.Constructor, "on_revoked", new()),
        args.Get(ArcEffect.Constructor, "on_invalid", new()),
        args.Get(ArcModifier.Constructor, "penalties", new()),
        args.Get(ArcModifier.Constructor, "benefits", new()),
        args.Get((Block s) => new ArcList<ArcCode>(s, (Block s) => ArcCode.Constructor(s)), "conditional_modifiers", new()),
        args.Get(ArcModifier.Constructor, "modifier_by_land_ownership", new()),
        args.Get(ArcCode.Constructor, "mechanics", new()),
        args.Get(ArcInt.Constructor, "cooldown_years", new(10)),
        args.Get(ArcEffect.Constructor, "on_cooldown_expires", new()),
        args.Get(ArcCode.Constructor, "ai_will_do", new("factor", "=", "1")),
        args.Get(ArcEffect.Constructor, "on_granted_province", null),
        args.Get(ArcEffect.Constructor, "on_revoked_province", null),
        args.Get(ArcEffect.Constructor, "on_invalid_province", null)
    );
    public static string Transpile()
    {
        Block b = new();

        foreach (EstatePrivilege privilege in EstatePrivileges.Values())
        {
            Program.Localisation.Add($"{privilege.Id}", privilege.Name.Value);
            Program.Localisation.Add($"{privilege.Id}_desc", privilege.Desc.Value);

            b.Add(privilege.Id.ToString(), "=", "{");
            b.Add("icon", "=", privilege.Icon.ToString());
            b.Add("land_share", "=", privilege.LandShare.ToString());
            b.Add("max_absolutism", "=", privilege.MaxAbsolutism.ToString());
            b.Add("loyalty", "=", privilege.Loyalty.ToString());
            b.Add("influence", "=", privilege.Influence.ToString());
            privilege.IsValid.Compile("is_valid", ref b);
            privilege.CanSelect.Compile("can_select", ref b);
            privilege.OnGranted.Compile("on_granted", ref b);
            privilege.CanRevoke.Compile("can_revoke", ref b);
            privilege.OnRevoked.Compile("on_revoked", ref b);
            privilege.OnInvalid.Compile("on_invalid", ref b);
            privilege.Penalties.Compile("penalties", ref b);
            privilege.Benefits.Compile("benefits", ref b);
            privilege.OnGrantedProvince?.Compile("on_granted_province", ref b);
            privilege.OnRevokedProvince?.Compile("on_revoked_province", ref b);
            privilege.OnInvalidProvince?.Compile("on_invalid_province", ref b);
            foreach(ArcCode? s in privilege.ConditionalModifiers.Values)
            {
                if (s == null) continue;
                s.Compile("conditional_modifier", ref b);
            }
            privilege.ModifierByLandOwnership.Compile("modifier_by_land_ownership", ref b);
            privilege.Mechanics.Compile("mechanics", ref b);
            b.Add("cooldown_years", "=", privilege.CooldownYears.ToString());
            privilege.OnCooldownExpires.Compile("on_cooldown_expires", ref b);
            privilege.AiWillDo.Compile("ai_will_do", ref b);
            b.Add("}");
        }

        Program.OverwriteFile($"{Program.TranspileTarget}/common/estate_privileges/arc.txt", string.Join(' ', b));
        return "Privileges";
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}
public class Estate : IArcObject
{
    public static readonly Dict<Estate> Estates = new();
    public bool IsObject() => true;
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }

    public ArcString Id { get; set; }
    public ArcInt Icon { get; set; }
    public ArcCode Color { get; set; }
    public ArcTrigger Trigger { get; set; }
    public ArcModifier CountryModifierHappy { get; set; }
    public ArcModifier CountryModifierNeutral { get; set; }
    public ArcModifier CountryModifierAngry { get; set; }
    public ArcModifier LandOwnershipModifier { get; set; }
    public ArcCode ProvinceIndependenceWeight { get; set; }
    public ArcFloat BaseInfluence { get; set; }
    public ArcList<ArcCode> InfluenceModifiers { get; set; }
    public ArcList<ArcCode> LoyaltyModifiers { get; set; }
    public ArcList<ArcCode> CustomNames { get; set; }
    public ArcBool ContributesToCuriaTreasury { get; set; }
    public PrivilegeList Privileges { get; set; }
    public ArcList<EstateAgenda> Agendas { get; set; }
    public ArcFloat InfluenceFromDevModifier { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public Estate(
        string id, 
        ArcString name, 
        ArcString desc, 
        ArcInt icon, 
        ArcCode color, 
        ArcTrigger trigger, 
        ArcModifier countryModifierHappy, 
        ArcModifier countryModifierNeutral, 
        ArcModifier countryModifierAngry, 
        ArcModifier landOwnershipModifier, 
        ArcCode provinceIndependenceWeight, 
        ArcFloat baseInfluence, 
        ArcList<ArcCode> influenceModifiers, 
        ArcList<ArcCode> loyaltyModifiers, 
        ArcList<ArcCode> customNames, 
        ArcBool contributesToCuriaTreasury, 
        PrivilegeList privileges, 
        ArcList<EstateAgenda> agendas, 
        ArcFloat influenceFromDevModifier
    ) {
        Id = new($"estate_{id}");
        Icon = icon;
        Name = name; 
        Desc = desc; 
        Color = color;
        Trigger = trigger;
        CountryModifierHappy = countryModifierHappy;
        CountryModifierNeutral = countryModifierNeutral;
        CountryModifierAngry = countryModifierAngry;
        LandOwnershipModifier = landOwnershipModifier;
        ProvinceIndependenceWeight = provinceIndependenceWeight;
        BaseInfluence = baseInfluence;
        InfluenceModifiers = influenceModifiers;
        LoyaltyModifiers = loyaltyModifiers;
        CustomNames = customNames;
        ContributesToCuriaTreasury = contributesToCuriaTreasury;
        Privileges = privileges;
        Agendas = agendas;
        InfluenceFromDevModifier = influenceFromDevModifier;
        KeyValuePairs = new()
        {
            { "id", Id },
            { "icon", Icon },
            { "name", Name },
            { "desc", Desc },
            { "color", Color },
            { "trigger", Trigger },
            { "country_modifier_happy", CountryModifierHappy },
            { "country_modifier_neutral", CountryModifierNeutral },
            { "country_modifier_angry", CountryModifierAngry },
            { "land_ownership_modifier", LandOwnershipModifier },
            { "province_independence_weight", ProvinceIndependenceWeight },
            { "base_influence", BaseInfluence },
            { "influence_modifiers", InfluenceModifiers },
            { "loyalty_modifiers", LoyaltyModifiers },
            { "custom_names", CustomNames },
            { "contributes_to_curia_treasury", ContributesToCuriaTreasury },
            { "privileges", Privileges },
            { "agendas", Agendas },
            { "influence_from_dev_modifier", InfluenceFromDevModifier },
        };

        Estates.Add(id, this);
    }

    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        i.ForceMoveNext();

        string id = Compiler.GetId(i.Current);

        i = Args.GetArgs(i, out Args args);

        Estate estate = new(
            id,
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc"),
            args.Get(ArcInt.Constructor, "icon"),
            args.Get(ArcCode.Constructor, "color"),
            args.Get(ArcTrigger.Constructor, "trigger"),
            args.Get(ArcModifier.Constructor, "country_modifier_happy"),
            args.Get(ArcModifier.Constructor, "country_modifier_neutral"),
            args.Get(ArcModifier.Constructor, "country_modifier_angry"),
            args.Get(ArcModifier.Constructor, "land_ownership_modifier"),
            args.Get(ArcCode.Constructor, "province_independence_weight", new("factor", "=", "1")),
            args.Get(ArcFloat.Constructor, "base_influence"),
            args.Get((Block s) => new ArcList<ArcCode>(s, (Block s) => ArcCode.Constructor(s)), "influence_modifiers", new()),
            args.Get((Block s) => new ArcList<ArcCode>(s, (Block s) => ArcCode.Constructor(s)), "loyalty_modifiers", new()),
            args.Get((Block s) => new ArcList<ArcCode>(s, (Block s) => ArcCode.Constructor(s)), "custom_names", new()),
            args.Get(ArcBool.Constructor, "contributes_to_curia_treasury", new(false)),
            args.Get((Block s) => new PrivilegeList(s, (string key, Args s) => EstatePrivilege.Constructor(key, s), EstatePrivilege.EstatePrivileges), "privileges", new()),
            args.Get((Block s) => new ArcList<EstateAgenda>(s, EstateAgenda.EstateAgendas), "agendas", new()),
            args.Get(ArcFloat.Constructor, "influence_from_dev_modifier", new(1))
        );

        return i;
    }
    public void Transpile(ref Block estateFile, ref Block preloadFile)
    {
        Program.Localisation.Add($"{Id}", Name.ToString());
        Program.Localisation.Add($"{Id}_desc", Desc.ToString());

        estateFile.Add(Id, "=", "{");
        estateFile.Add("icon", "=", Icon);
        Color.Compile("color", ref estateFile);
        Trigger.Compile("trigger", ref estateFile);
        CountryModifierHappy.Compile("country_modifier_happy", ref estateFile, false);
        CountryModifierNeutral.Compile("country_modifier_neutral", ref estateFile, false);
        CountryModifierAngry.Compile("country_modifier_angry", ref estateFile, false);
        LandOwnershipModifier.Compile("land_ownership_modifier", ref estateFile, false);
        ProvinceIndependenceWeight.Compile("province_independence_weight", ref estateFile);
        estateFile.Add("base_influence", "=", BaseInfluence);
        foreach (ArcCode? s in InfluenceModifiers.Values)
        {
            if (s == null) continue;
            s.Compile("influence_modifier", ref estateFile);
        }
        foreach (ArcCode? s in LoyaltyModifiers.Values)
        {
            if (s == null) continue;
            s.Compile("loyalty_modifier", ref estateFile);
        }
        foreach (ArcCode? s in CustomNames.Values)
        {
            if (s == null) continue;
            s.Compile("custom_name", ref estateFile);
        }
        estateFile.Add("contributes_to_curia_treasury", "=", ContributesToCuriaTreasury);
        estateFile.Add("privileges", "=", "{");
        foreach (EstatePrivilege? privilege in Privileges.Values)
        {
            if (privilege == null) continue;
            estateFile.Add(privilege.Id);
        }
        estateFile.Add("}");
        estateFile.Add("agendas", "=", "{");
        foreach (EstateAgenda? agenda in Agendas.Values)
        {
            if (agenda == null) continue;
            estateFile.Add(agenda.Id);
        }
        estateFile.Add("}");
        estateFile.Add("influence_from_dev_modifier", "=", InfluenceFromDevModifier);
        estateFile.Add("}");

        Program.Localisation.Add($"{Id.ToString()[7..]}_loyalty_modifier", $"{Name} Loyalty Equilibrium");
        Program.Localisation.Add($"{Id.ToString()[7..]}_influence_modifier", $"{Name} Influence");
        Program.Localisation.Add($"{Id.ToString()[7..]}_privilege_slots", $"{Name} Max Privileges");

        preloadFile.Add(
            Id, "=", "{",
                "modifier_definition", "=", "{",
                    "type", "=", "loyalty",
                    "key", "=", $"{Id.ToString()[7..]}_loyalty_modifier",
                    "trigger", "=", "{", "has_estate", "=", Id, "}",
                "}",
                "modifier_definition", "=", "{",
                    "type", "=", "influence",
                    "key", "=", $"{Id.ToString()[7..]}_influence_modifier",
                    "trigger", "=", "{", "has_estate", "=", Id, "}",
                "}",
                "modifier_definition", "=", "{",
                    "type", "=", "privileges",
                    "key", "=", $"{Id.ToString()[7..]}_privilege_slots",
                    "trigger", "=", "{", "has_estate", "=", Id, "}",
                "}",
            "}"
        );
    }
    public static string Transpile()
    {
        Block estateFile = new()
        {
            "estate_special", "=", "{",
                "icon", "=", "1",
                "trigger", "=", "{", "always", "=", "no", "}",
                "country_modifier_happy", "=", "{", "}",
                "country_modifier_neutral", "=", "{", "}",
                "country_modifier_angry", "=", "{", "}",
                "land_ownership_modifier", "=", "{", "}",
                "province_independence_weight", "=", "{", "factor", "=", "0", "}",
                "base_influence", "=", "0",
                "color", "=", "{", "0", "0", "0", "}",
                "privileges", "=", "{", "}",
                "agendas", "=", "{", "}",
                "influence_from_dev_modifier", "=", "0.0",
            "}"
        };
        Block preloadFile = new()
        {
            "estate_special", "=", "{"
        };
        if (Compiler.TryGetVariable(new("modifier_definitions"), out IVariable? tvar))
        {
            if (tvar == null) throw new Exception();
            foreach (IVariable vr in ((Dict<IVariable>)tvar).Values())
            {
                preloadFile.Add("modifier_definition", "=", "{");

                if (vr is ArcObject modifierDefinition)
                {
                    string id = modifierDefinition.Get<ArcString>("id").Value;
                    string name = modifierDefinition.Get<ArcString>("name").Value;
                    Program.Localisation.Add(id, name);
                    bool isPercentage = modifierDefinition.Get<ArcBool>("is_percentage").Value;

                    preloadFile.Add("type", "=");
                    if (isPercentage) preloadFile.Add("loyalty");
                    else preloadFile.Add("privileges");

                    preloadFile.Add("key", "=", id);

                    modifierDefinition.Get<ArcTrigger>("trigger").Compile("trigger", ref preloadFile, false);
                }
                else throw ArcException.Create("Trying to transpile modifier definition, but vr wasn't an object", preloadFile, estateFile, vr);

                preloadFile.Add("}");
            }
        }
        preloadFile.Add("}");
        Block SpawnRebelsFromUnhappyEstate = new()
        {
            "spawn_rebels_from_unhappy_estate", "=", "{",
                "random_list", "=", "{"
        };
        Block HasAnyEstate = new()
        {
            "has_any_estates", "=", "{",
                "custom_trigger_tooltip", "=", "{",
                    "tooltip", "=", "has_any_estates_tt",
                    "OR", "=", "{"
        };

        foreach (KeyValuePair<string, Estate> estate in Estates)
        {
            estate.Value.Transpile(ref estateFile, ref preloadFile);
            SpawnRebelsFromUnhappyEstate.Add(
                "1", "=", "{",
                    "trigger", "=", "{",
                        "owner", "=", "{",
                            "has_estate", "=", estate.Value.Id,
                            "NOT", "=", "{",
                                "estate_loyalty", "=", "{",
                                    "estate", "=", estate.Value.Id,
                                    "loyalty", "=", "30",
                                "}",
                            "}",
                        "}",
                    "}",
                    "spawn_rebels", "=", "{",
                        "type", "=", "noble_rebels",
                        "size", "=", "$size$",
                        "estate", "=", estate.Value.Id,
                        "as_if_faction", "=", "yes",
                    "}",
                "}"
            );
            HasAnyEstate.Add("has_estate", "=", estate.Value.Id);
        }

        SpawnRebelsFromUnhappyEstate.Add(
                "}",
            "}"
        );
        HasAnyEstate.Add(
                    "}",
                "}",
            "}"
        );

        Program.OverwriteFile($"{Program.TranspileTarget}/common/estates/arc.txt", string.Join(' ', estateFile));
        Program.OverwriteFile($"{Program.TranspileTarget}/common/estates_preload/arc.txt", string.Join(' ', preloadFile));
        Program.OverwriteFile($"{Program.TranspileTarget}/common/scripted_effects/arc.txt", string.Join(' ', new Block(
            string.Join(' ', SpawnRebelsFromUnhappyEstate)
        )));
        Program.OverwriteFile($"{Program.TranspileTarget}/common/scripted_triggers/arc.txt", string.Join(' ', new Block(
            string.Join(' ', HasAnyEstate)
        )));
        return "Estates";
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}
public class PrivilegeList : ArcList<EstatePrivilege>
{
    public PrivilegeList()
    {
        Values = new();
    }
    public PrivilegeList(Block value, Func<string, Args, EstatePrivilege> Constructor, Dict<EstatePrivilege> Dictionary, Func<string, string>? KeyMorpher = null)
    {
        if (Parser.HasEnclosingBrackets(value)) Compiler.RemoveEnclosingBrackets(value);

        Values = new();
        constructor = Constructor;
        dict = Dictionary;

        if (value.Count == 0) return;

        Walker i = new(value);
        do
        {
            if (i.Current == "new")
            {
                i.ForceMoveNext();
                string key = i.Current;
                i = Args.GetArgs(i, out Args args);

                if (KeyMorpher != null) key = KeyMorpher(key);

                Values.Add(Constructor(key, args));
            }
            else
                Values.Add((EstatePrivilege?)Dictionary.Get(i.Current));
        } while (i.MoveNext());
    }
    public override IVariable? Get(string indexer)
    {
        var a = from privilege in Values where privilege.Id.Value == (indexer + "_privelege") select privilege;
        if (a.Count() == 1) return a.First();
        throw ArcException.Create(indexer, a);
    }

    public override bool CanGet(string indexer)
    {
        var a = from privilege in Values where privilege.Id.Value == (indexer + "_privelege") select privilege;
        if (a.Count() == 1) return true;
        return false;
    }
}