using ArcInstance;
using Pastel;
using System.IO;
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
    public ArcBlock CanSelect { get; set; }
    public ArcBlock ProvincesToHighlight { get; set; }
    public ArcBlock SelectionWeight { get; set; }
    public ArcBlock PreEffect { get; set; }
    public ArcBlock ImmediateEffect { get; set; }
    public ArcBlock TaskRequirements { get; set; }
    public ArcBlock TaskCompletedEffect { get; set; }
    public ArcBlock FailingEffect { get; set; }
    public ArcBlock FailIf { get; set; }
    public ArcBlock OnInvalid { get; set; }
    public ArcBlock InvalidTrigger { get; set; }
    public ArcBlock Modifier { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public EstateAgenda(string id, ArcString name, ArcString desc, ArcFloat? maxDaysActive, ArcBlock canSelect, ArcBlock provincesToHighlight, ArcBlock selectionWeight, ArcBlock preEffect, ArcBlock immediateEffect, ArcBlock taskRequirements, ArcBlock taskCompletedEffect, ArcBlock failingEffect, ArcBlock failIf, ArcBlock onInvalid, ArcBlock invalidTrigger, ArcBlock modifier)
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
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);

        new EstateAgenda(
            id,
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc"),
            args.Get(ArcFloat.Constructor, "max_days_active", null),
            args.Get(ArcBlock.Constructor, "can_select"),
            args.Get(ArcBlock.Constructor, "provinces_to_highlight"),
            args.Get(ArcBlock.Constructor, "selection_weight"),
            args.Get(ArcBlock.Constructor, "pre_effect"),
            args.Get(ArcBlock.Constructor, "immediate_effect"),
            args.Get(ArcBlock.Constructor, "task_requirements"),
            args.Get(ArcBlock.Constructor, "task_completed_effect"),
            args.Get(ArcBlock.Constructor, "failing_effect"),
            args.Get(ArcBlock.Constructor, "fail_if"),
            args.Get(ArcBlock.Constructor, "on_invalid"),
            args.Get(ArcBlock.Constructor, "invalid_trigger"),
            args.Get(ArcBlock.Constructor, "modifier")
        );

        return i;
    }
    public static string Transpile()
    {
        Block b = new();

        foreach(EstateAgenda agenda in EstateAgendas.Values())
        {
            Instance.Localisation.Add($"{agenda.Id}", agenda.Name.Value);
            Instance.Localisation.Add($"{agenda.Id}_desc", agenda.Desc.Value);

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
            agenda.ProvincesToHighlight.Compile("provines_to_highlight", ref b);
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

        Instance.OverwriteFile("target/common/estate_agendas/arc.txt", string.Join(' ', b));
        return "Agendas";
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result, Compiler comp) { result.Add(Id.Value.ToString()); return i; }
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
    public ArcBlock IsValid { get; set; }
    public ArcBlock CanSelect { get; set; }
    public ArcBlock OnGranted { get; set; }
    public ArcBlock CanRevoke { get; set; }
    public ArcBlock OnRevoked { get; set; }
    public ArcBlock OnInvalid { get; set; }
    public ArcBlock Penalties { get; set; }
    public ArcBlock Benefits { get; set; }
    public ArcList<ArcBlock> ConditionalModifiers { get; set; }
    public ArcBlock ModifierByLandOwnership { get; set; }
    public ArcBlock Mechanics { get; set; }
    public ArcInt CooldownYears { get; set; }
    public ArcBlock OnCooldownExpires { get; set; }
    public ArcBlock AiWillDo { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public EstatePrivilege(string id, ArcString name, ArcString desc, ArcString icon, ArcFloat landShare, ArcFloat maxAbsolutism, ArcFloat loyalty, ArcFloat influence, ArcBlock isValid, ArcBlock canSelect, ArcBlock onGranted, ArcBlock canRevoke, ArcBlock onInvalid, ArcBlock onRevoked, ArcBlock penalties, ArcBlock benefits, ArcList<ArcBlock> conditionalModifiers, ArcBlock modifierByLandOwnership, ArcBlock mechanics, ArcInt cooldownYears, ArcBlock onCooldownExpires, ArcBlock aiWillDo)
    {
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
        };

        EstatePrivileges.Add(id, this);
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);

        Constructor(id, args);

        return i;
    }
    public static EstatePrivilege Constructor(string estate, string key, Args args) => Constructor($"estate_{estate}_{key}", args);
    public static EstatePrivilege Constructor(string key, Args args) => new EstatePrivilege(
        key,
        args.Get(ArcString.Constructor, "name"),
        args.Get(ArcString.Constructor, "desc"),
        args.Get(ArcString.Constructor, "icon"),
        args.Get(ArcFloat.Constructor, "land_share", new(0)),
        args.Get(ArcFloat.Constructor, "max_absolutism", new(0)),
        args.Get(ArcFloat.Constructor, "loyalty", new(0)),
        args.Get(ArcFloat.Constructor, "influence", new(0)),
        args.Get(ArcBlock.Constructor, "is_valid", new()),
        args.Get(ArcBlock.Constructor, "can_select", new()),
        args.Get(ArcBlock.Constructor, "on_granted", new()),
        args.Get(ArcBlock.Constructor, "can_revoke", new()),
        args.Get(ArcBlock.Constructor, "on_revoked", new()),
        args.Get(ArcBlock.Constructor, "on_invalid", new()),
        args.Get(ArcBlock.Constructor, "penalties", new()),
        args.Get(ArcBlock.Constructor, "benefits", new()),
        args.Get((Block s) => new ArcList<ArcBlock>(s, (Block s) => ArcBlock.Constructor(s)), "conditional_modifiers", new()),
        args.Get(ArcBlock.Constructor, "modifier_by_land_ownership", new()),
        args.Get(ArcBlock.Constructor, "mechanics", new()),
        args.Get(ArcInt.Constructor, "cooldown_years", new(10)),
        args.Get(ArcBlock.Constructor, "on_cooldown_expires", new()),
        args.Get(ArcBlock.Constructor, "ai_will_do", new("factor", "=", "1"))
    );
    public static string Transpile()
    {
        Block b = new();

        foreach (EstatePrivilege privilege in EstatePrivileges.Values())
        {
            Instance.Localisation.Add($"{privilege.Id}", privilege.Name.Value);
            Instance.Localisation.Add($"{privilege.Id}_desc", privilege.Desc.Value);

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
            foreach(ArcBlock? s in privilege.ConditionalModifiers.Values)
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

        Instance.OverwriteFile("target/common/estate_privileges/arc.txt", string.Join(' ', b));
        return "Privileges";
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result, Compiler comp) { result.Add(Id.Value.ToString()); return i; }
}
public class Estate : IArcObject
{
    public static readonly Dict<Estate> Estates = new();
    public bool IsObject() => true;
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }

    public ArcString Id { get; set; }
    public ArcInt Icon { get; set; }
    public ArcBlock Color { get; set; }
    public ArcBlock Trigger { get; set; }
    public ArcBlock CountryModifierHappy { get; set; }
    public ArcBlock CountryModifierNeutral { get; set; }
    public ArcBlock CountryModifierAngry { get; set; }
    public ArcBlock LandOwnershipModifier { get; set; }
    public ArcBlock ProvinceIndependenceWeight { get; set; }
    public ArcFloat BaseInfluence { get; set; }
    public ArcList<ArcBlock> InfluenceModifiers { get; set; }
    public ArcList<ArcBlock> LoyaltyModifiers { get; set; }
    public ArcList<ArcBlock> CustomNames { get; set; }
    public ArcBool ContributesToCuriaTreasury { get; set; }
    public ArcList<EstatePrivilege> Privileges { get; set; }
    public ArcList<EstateAgenda> Agendas { get; set; }
    public ArcFloat InfluenceFromDevModifier { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public Estate(string id, ArcString name, ArcString desc, ArcInt icon, ArcBlock color, ArcBlock trigger, ArcBlock countryModifierHappy, ArcBlock countryModifierNeutral, ArcBlock countryModifierAngry, ArcBlock landOwnershipModifier, ArcBlock provinceIndependenceWeight, ArcFloat baseInfluence, ArcList<ArcBlock> influenceModifiers, ArcList<ArcBlock> loyaltyModifiers, ArcList<ArcBlock> customNames, ArcBool contributesToCuriaTreasury, ArcList<EstatePrivilege> privileges, ArcList<EstateAgenda> agendas, ArcFloat influenceFromDevModifier)
    {
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
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);

        Estate estate = new(
            id,
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc"),
            args.Get(ArcInt.Constructor, "icon"),
            args.Get(ArcBlock.Constructor, "color"),
            args.Get(ArcBlock.Constructor, "trigger"),
            args.Get(ArcBlock.Constructor, "country_modifier_happy"),
            args.Get(ArcBlock.Constructor, "country_modifier_neutral"),
            args.Get(ArcBlock.Constructor, "country_modifier_angry"),
            args.Get(ArcBlock.Constructor, "land_ownership_modifier"),
            args.Get(ArcBlock.Constructor, "province_independence_weight", new("factor", "=", "1")),
            args.Get(ArcFloat.Constructor, "base_influence"),
            args.Get((Block s) => new ArcList<ArcBlock>(s, (Block s) => ArcBlock.Constructor(s)), "influence_modifiers", new()),
            args.Get((Block s) => new ArcList<ArcBlock>(s, (Block s) => ArcBlock.Constructor(s)), "loyalty_modifiers", new()),
            args.Get((Block s) => new ArcList<ArcBlock>(s, (Block s) => ArcBlock.Constructor(s)), "custom_names", new()),
            args.Get(ArcBool.Constructor, "contributes_to_curia_treasury", new(false)),
            args.Get((Block s) => new ArcList<EstatePrivilege>(s, (string key, Args s) => EstatePrivilege.Constructor(id, key, s), EstatePrivilege.EstatePrivileges), "privileges", new()),
            args.Get((Block s) => new ArcList<EstateAgenda>(s, EstateAgenda.EstateAgendas), "agendas", new()),
            args.Get(ArcFloat.Constructor, "influence_from_dev_modifier", new(1))
        );

        return i;
    }
    public static string Transpile()
    {
        Block b = new()
        {
            "estate_special", "=", "{",
                "icon", "=", "1",
                "trigger", "=", "{", "always", "=", "no", "}",
                "country_modifier_happy", "=", "{", "}",
                "country_modifier_neutral", "=", "{", "}",
                "country_modifier_angry", "=", "{", "}",
                "land_ownership_modifier", "=", "{", "}",
                "land_ownership_modifier", "=", "{", "factor", "=", "0", "}",
                "base_influence", "=", "0",
                "color", "=", "{", "0", "0", "0", "}",
                "privileges", "=", "{", "}",
                "agendas", "=", "{", "}",
                "influence_from_dev_modifier", "=", "0.0",
            "}"
        };
        Block c = new()
        {
            "estate_special", "=", "{",
                "modifier_definition", "=", "{",
                    "type", "=", "privileges",
                    "key", "=", "max_terms",
                    "trigger", "=", "{",
                        "has_government_attribute", "=", "has_limited_terms",
                    "}",
                "}",
            "}"
        };

        foreach (Estate estate in Estates.Values())
        {
            Instance.Localisation.Add($"{estate.Id}", estate.Name.ToString());
            Instance.Localisation.Add($"{estate.Id}_desc", estate.Desc.ToString());

            b.Add(estate.Id, "=", "{");
            b.Add("icon", "=", estate.Icon);
            estate.Color.Compile("color", ref b);
            estate.Trigger.Compile("trigger", ref b);
            estate.CountryModifierHappy.Compile("country_modifier_happy", ref b);
            estate.CountryModifierNeutral.Compile("country_modifier_neutral", ref b);
            estate.CountryModifierAngry.Compile("country_modifier_angry", ref b);
            estate.LandOwnershipModifier.Compile("land_ownership_modifier", ref b);
            estate.ProvinceIndependenceWeight.Compile("province_independence_weight", ref b);
            b.Add("base_influence", "=", estate.BaseInfluence);
            foreach(ArcBlock? s in estate.InfluenceModifiers.Values)
            {
                if (s == null) continue;
                s.Compile("influence_modifier", ref b);
            }
            foreach(ArcBlock? s in estate.LoyaltyModifiers.Values)
            {
                if (s == null) continue;
                s.Compile("loyalty_modifier", ref b);
            }
            foreach(ArcBlock? s in estate.CustomNames.Values)
            {
                if (s == null) continue;
                s.Compile("custom_name", ref b);
            }
            b.Add("contributes_to_curia_treasury", "=", estate.ContributesToCuriaTreasury);
            b.Add("privileges", "=", "{");
            foreach(EstatePrivilege? privilege in estate.Privileges.Values)
            {
                if(privilege == null) continue;
                b.Add(privilege.Id);
            }
            b.Add("}");
            b.Add("agendas", "=", "{");
            foreach (EstateAgenda? agenda in estate.Agendas.Values)
            {
                if (agenda == null) continue;
                b.Add(agenda.Id);
            }
            b.Add("}");
            b.Add("influence_from_dev_modifier", "=", estate.InfluenceFromDevModifier);
            b.Add("}");

            c.Add(estate.Id, "=", "{");
            c.Add("modifier_definition", "=", "{");
            c.Add("type", "=", "loyalty");
            c.Add("key", "=", $"{estate.Id.ToString()[7..]}_loyalty_modifier");
            c.Add("trigger", "=", "{", "has_estate", "=", estate.Id, "}");
            c.Add("}");
            c.Add("modifier_definition", "=", "{");
            c.Add("type", "=", "influence");
            c.Add("key", "=", $"{estate.Id.ToString()[7..]}_influence_modifier");
            c.Add("trigger", "=", "{", "has_estate", "=", estate.Id, "}");
            c.Add("}");
            c.Add("}");
        }

        Instance.OverwriteFile("target/common/estates/arc.txt", string.Join(' ', b));
        Instance.OverwriteFile("target/common/estates_preload/arc.txt", string.Join(' ', c));
        return "Estates";
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result, Compiler comp) { result.Add(Id.Value.ToString()); return i; }
}
