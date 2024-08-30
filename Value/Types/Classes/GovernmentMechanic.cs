using Arc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
public class GovernmentMechanicInteraction : IArcObject
{
    public static readonly Dict<GovernmentMechanicInteraction> GovernmentMechanicInteractions = new();
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }
    public ArcCode? Gui { get; set; }
    public ArcBool? Center { get; set; }
    public ArcString? Icon { get; set; }
    public ArcString? CostType { get; set; }
    public ArcInt? Cost { get; set; }
    public ArcTrigger? Potential { get; set; }
    public ArcTrigger? Trigger { get; set; }
    public ArcEffect Effect { get; set; }
    public ArcInt? CooldownYears { get; set; }
    public ArcString? CooldownToken { get; set; }
    public ArcString? CooldownDesc { get; set; }
    public ArcCode AiChance { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public GovernmentMechanicInteraction(
        string id, 
        ArcString name, 
        ArcString desc,
        ArcCode? gui, 
        ArcBool? center, 
        ArcString? icon, 
        ArcString? costType, 
        ArcInt? cost, 
        ArcTrigger? potential, 
        ArcTrigger? trigger, 
        ArcEffect effect, 
        ArcInt? cooldownYears, 
        ArcString? cooldownToken, 
        ArcString? cooldownDesc, 
        ArcCode aiChance
    ) {
        Id = new(id);
        Name = name;
        Desc = desc;
        Gui = gui;
        Center = center;
        Icon = icon;
        CostType = costType;
        Cost = cost;
        Potential = potential;
        Trigger = trigger;
        Effect = effect;
        CooldownYears = cooldownYears;
        CooldownToken = cooldownToken;
        CooldownDesc = cooldownDesc;
        AiChance = aiChance;

        KeyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "desc", Desc },
            { "gui", Gui },
            { "center", Center },
            { "icon", Icon },
            { "cost_type", CostType },
            { "cost", Cost },
            { "potential", Potential },
            { "trigger", Trigger },
            { "effect", Effect },
            { "cooldown_years", CooldownYears },
            { "cooldown_token", CooldownToken },
            { "cooldown_desc", CooldownDesc },
            { "ai_chance", AiChance },
        };

        GovernmentMechanicInteractions.Add(id, this);
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static GovernmentMechanicInteraction Constructor(string id, Args args)
    {
        return new GovernmentMechanicInteraction(id,
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc", new("")),
            args.Get(ArcCode.Constructor, "gui", null),
            args.Get(ArcBool.Constructor, "center", null),
            args.Get(ArcString.Constructor, "icon", null),
            args.Get(ArcString.Constructor, "cost_type", null),
            args.Get(ArcInt.Constructor, "cost", null),
            args.Get(ArcTrigger.Constructor, "potential", null),
            args.Get(ArcTrigger.Constructor, "trigger", null),
            args.Get(ArcEffect.Constructor, "effect"),
            args.Get(ArcInt.Constructor, "cooldown_years", null),
            args.Get(ArcString.Constructor, "cooldown_token", null),
            args.Get(ArcString.Constructor, "cooldown_desc", null),
            args.Get(ArcCode.Constructor, "ai_chance", new("factor", "=", "1"))
        );
    }
    public void Transpile(ref Block b, ref Block g)
    {
        Program.Localisation.Add($"{Id}", Name.Value);
        Program.Localisation.Add($"{Id}_desc", Desc.Value);

        b.Add(Id, "=", "{");
        if (Gui != null)
        {
            g.Add(
                "windowType", "=", "{",
                    "name", "=", $"\"{Id}_mechanic_window\"",
                    string.Join(' ', Gui), 
                "}"
            );
            b.Add("gui", "=", $"{Id}_mechanic_window");
        }
        if (Center != null) b.Add("center", "=", Center);
        if (Icon != null) b.Add("icon", "=", Icon);
        if (CostType != null) b.Add("cost_type", "=", CostType);
        if (Cost != null) b.Add("cost", "=", Cost);
        Potential?.Compile("potential", ref b);
        Trigger?.Compile("trigger", ref b);
        Effect.Compile("effect", ref b);
        if (CooldownYears != null) b.Add("cooldown_years", "=", CooldownYears);
        if (CooldownToken != null) b.Add("cooldown_token", "=", CooldownToken);
        if (CooldownDesc != null)
        {
            Program.Localisation.Add($"{Id}_cooldown_desc", CooldownDesc.Value);
            b.Add("cooldown_desc", "=", $"{Id}_cooldown_desc");
        }
        b.Add("}");
    }
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}

public class GovernmentMechanicPower : IArcObject
{
    public static readonly Dict<GovernmentMechanicPower> GovernmentMechanicPowers = new();
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }
    public ArcCode? Gui { get; set; }
    public ArcInt? Min { get; set; }
    public ArcInt Max { get; set; }
    public ArcInt? Default { get; set; }
    public ArcBool? ResetOnNewRuler { get; set; }
    public ArcFloat? BaseMonthlyGrowth { get; set; }
    public ArcFloat? DevelopmentScaledMonthlyGrowth { get; set; }
    public ArcString? MonarchPower { get; set; }
    public ArcBool? ShowBeforeInteractions { get; set; }
    public ArcCode? ScaledModifier { get; set; }
    public ArcCode? ReverseScaledModifier { get; set; }
    public ArcCode? RangeModifier { get; set; }
    public ArcEffect? OnMaxReached { get; set; }
    public ArcEffect? OnMinReached { get; set; }
    public ArcBool? IsGood { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public GovernmentMechanicPower(
        string id, 
        ArcString name, 
        ArcString desc,
        ArcCode? gui, 
        ArcInt? min, 
        ArcInt max, 
        ArcInt? @default, 
        ArcBool? resetOnNewRuler, 
        ArcFloat? baseMonthlyGrowth, 
        ArcFloat? developmentScaledMonthlyGrowth, 
        ArcString? monarchPower, 
        ArcBool? showBeforeInteractions, 
        ArcCode? scaledModifier, 
        ArcCode? reverseScaledModifier, 
        ArcCode? rangeModifier,
        ArcEffect? onMaxReached, 
        ArcEffect? onMinReached, 
        ArcBool? isGood
    ) {
        Id = new(id);

        GovernmentMechanicPowers.Add(id, this);
        Name = name;
        Desc = desc;
        Gui = gui;
        Min = min;
        Max = max;
        Default = @default;
        ResetOnNewRuler = resetOnNewRuler;
        BaseMonthlyGrowth = baseMonthlyGrowth;
        DevelopmentScaledMonthlyGrowth = developmentScaledMonthlyGrowth;
        MonarchPower = monarchPower;
        ShowBeforeInteractions = showBeforeInteractions;
        ScaledModifier = scaledModifier;
        ReverseScaledModifier = reverseScaledModifier;
        RangeModifier = rangeModifier;
        OnMaxReached = onMaxReached;
        OnMinReached = onMinReached;
        IsGood = isGood;

        KeyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "desc", Desc },
            { "gui", Gui },
            { "min", Min },
            { "max", Max },
            { "default", Default },
            { "reset_on_new_ruler", ResetOnNewRuler },
            { "base_monthly_growth", BaseMonthlyGrowth },
            { "development_scaled_monthly_growth", DevelopmentScaledMonthlyGrowth },
            { "monarch_power", MonarchPower },
            { "show_before_interactions", ShowBeforeInteractions },
            { "scaled_modifier", ScaledModifier },
            { "reverse_scaled_modifier", ReverseScaledModifier },
            { "range_modifier", RangeModifier },
            { "on_max_reached", OnMaxReached },
            { "on_min_reached", OnMinReached },
            { "is_good", IsGood },
        };
    }

    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static GovernmentMechanicPower Constructor(string id, Args args) => new(id,
        args.Get(ArcString.Constructor, "name"),
        args.Get(ArcString.Constructor, "desc", new("")),
        args.Get(ArcCode.Constructor, "gui", null),
        args.Get(ArcInt.Constructor, "min", null),
        args.Get(ArcInt.Constructor, "max"),
        args.Get(ArcInt.Constructor, "default", null),
        args.Get(ArcBool.Constructor, "reset_on_new_ruler", null),
        args.Get(ArcFloat.Constructor, "base_monthly_growth", null),
        args.Get(ArcFloat.Constructor, "development_scaled_monthly_growth", null),
        args.Get(ArcString.Constructor, "monarch_power", null),
        args.Get(ArcBool.Constructor, "show_before_interactions", null),
        args.Get(ArcCode.Constructor, "scaled_modifier", null),
        args.Get(ArcCode.Constructor, "reverse_scaled_modifier", null),
        args.Get(ArcCode.Constructor, "range_modifier", null),
        args.Get(ArcEffect.Constructor, "on_max_reached", null),
        args.Get(ArcEffect.Constructor, "on_min_reached", null),
        args.Get(ArcBool.Constructor, "is_good", null)
    );
    public void Transpile(ref Block b, ref Block g)
    {
        Program.Localisation.Add($"{Id}", Name.Value);
        Program.Localisation.Add($"{Id}_desc", Desc.Value);
        Program.Localisation.Add($"monthly_{Id}", $"Monthly {Name}");
        Program.Localisation.Add($"{Id}_gain_modifier", $"{Name} Gain Modifier");

        b.Add(Id, "=", "{");
        if (Gui != null)
        {
            g.Add(
                "windowType", "=", "{",
                    "name", "=", $"\"{Id}_mechanic_window\"",
                    string.Join(' ', Gui),
                "}"
            );
            b.Add("gui", "=", $"{Id}_mechanic_window");
        }
        if (Min != null) b.Add("min", "=", Min);
        b.Add("max", "=", Max);
        if (Default != null) b.Add("default", "=", Default);
        if (ResetOnNewRuler != null) b.Add("reset_on_new_ruler", "=", ResetOnNewRuler);
        if (BaseMonthlyGrowth != null) b.Add("base_monthly_growth", "=", BaseMonthlyGrowth);
        if (DevelopmentScaledMonthlyGrowth != null) b.Add("development_scaled_monthly_growth", "=", DevelopmentScaledMonthlyGrowth);
        if (MonarchPower != null) b.Add("monarch_power", "=", MonarchPower);
        if (ShowBeforeInteractions != null) b.Add("show_before_interactions", "=", ShowBeforeInteractions);
        ScaledModifier?.Compile("scaled_modifier", ref b);
        ReverseScaledModifier?.Compile("reverse_scaled_modifier", ref b);
        RangeModifier?.Compile("range_modifier", ref b);
        OnMaxReached?.Compile("on_max_reached", ref b);
        OnMinReached?.Compile("on_min_reached", ref b);
        if (IsGood != null) b.Add("is_good", "=", IsGood);
        b.Add("}");
    }
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}

public class GovernmentMechanic : IArcObject
{
    public static readonly Dict<GovernmentMechanic> GovernmentMechanics = new();
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcString? AlertIconGfx { get; set; }
    public ArcInt? AlertIconIndex { get; set; }
    public ArcTrigger? Available { get; set; }
    public Dict<GovernmentMechanicPower>? Powers { get; set; }
    public Dict<GovernmentMechanicInteraction>? Interactions { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public GovernmentMechanic(string id, ArcString name, ArcString? alertIconGfx, ArcInt? alertIconIndex, ArcTrigger? available, Dict<GovernmentMechanicPower>? powers, Dict<GovernmentMechanicInteraction>? interactions)
    {
        Id = new(id);
        Name = name;
        AlertIconGfx = alertIconGfx;
        AlertIconIndex = alertIconIndex;
        Available = available;
        Powers = powers;
        Interactions = interactions;

        KeyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "alert_icon_gfx", AlertIconGfx },
            { "alert_icon_index", AlertIconIndex },
            { "available", Available },
            { "powers", Powers },
            { "interactions", Interactions },
        };

        GovernmentMechanics.Add(id, this);
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
    public static GovernmentMechanic Constructor(string id, Args args) => new(id,
        args.Get(ArcString.Constructor, "name"),
        args.Get(ArcString.Constructor, "alert_icon_gfx", null),
        args.Get(ArcInt.Constructor, "alert_icon_index", null),
        args.Get(ArcTrigger.Constructor, "available", new("always", "=", "yes")),
        args.Get(Dict<GovernmentMechanicPower>.Constructor(GovernmentMechanicPower.Constructor), "powers", null),
        args.Get(Dict<GovernmentMechanicInteraction>.Constructor(GovernmentMechanicInteraction.Constructor), "interactions", null)
    );
    public void Transpile(ref Block b, ref Block g)
    {
        Program.Localisation.Add($"ability_{Id}", Name.Value);

        b.Add(
            Id, "=", "{"
        );
        if (AlertIconGfx != null) b.Add("alert_icon_gfx", "=", AlertIconGfx);
        if (AlertIconIndex != null) b.Add("alert_icon_gfx", "=", AlertIconIndex);
        Available?.Compile("available", ref b);
        
        if(Powers != null)
        {
            b.Add("powers", "=", "{");
            foreach(GovernmentMechanicPower power in Powers.Values())
            {
                power.Transpile(ref b, ref g);
            }
            b.Add("}");
        }

        if(Interactions != null)
        {
            b.Add("interactions", "=", "{");
            foreach(GovernmentMechanicInteraction interaction in Interactions.Values())
            {
                interaction.Transpile(ref b, ref g);
            }
            b.Add("}");
        }

        b.Add(
            "}"
        );
    }
    public static string Transpile()
    {
        Block b = new();
        Block g = new("guiTypes", "=", "{");

        foreach(GovernmentMechanic mechanic in GovernmentMechanics.Values()) { 
            mechanic.Transpile(ref b, ref g); 
        }

        g.Add("}");
        Program.OverwriteFile($"{Program.TranspileTarget}/interface/government_mechanics/arc.gui", string.Join(' ', g));
        Program.OverwriteFile($"{Program.TranspileTarget}/common/government_mechanics/arc.txt", string.Join(' ', b));
        return "Government Mechanics";
    }
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}
