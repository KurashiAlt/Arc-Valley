
using Pastel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Arc;
public class GovernmentReform : IArcObject
{
    public static readonly Dict<GovernmentReform> GovernmentReforms = new();
    public bool IsObject() => true;
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }
    public ArcString Icon { get; set; }
    public ArcTrigger Potential { get; set; }
    public ArcTrigger Trigger { get; set; }
    public ArcModifier Modifier { get; set; }
    public ArcEffect Effect { get; set; }
    public ArcCode? CustomAttributes { get; set; }
    //public ArcList<ArcBlock>? Conditionals { get; set; }
    public Dict<ArcCode> Attributes { get; set; } //Used for non implemented Attributes
    /*
    public ArcBool? BasicReform { get; set; }
    public ArcBool? Monarchy { get; set; }
    public ArcBool? Republic { get; set; }
    public ArcBool? Religion { get; set; }
    public ArcBool? Tribal { get; set; }
    public ArcBool? Monastic { get; set; }
    public ArcBool? Dictatorship { get; set; }
    public ArcBool? Nomad { get; set; }
    public ArcBool? Papacy { get; set; }
    public ArcBool? HasMeritocracy { get; set; }
    public ArcBool? HasDevotion { get; set; }
    public ArcBool? RazeProvince { get; set; }
    public ArcBool? HasTermElection { get; set; }
    public ArcBool? IsElective { get; set; }
    public ArcBool? FreeCity { get; set; }
    public ArcBool? IsTradingCity { get; set; }
    public ArcBool? ValidForNewCountry { get; set; }
    public ArcBool? AllowConvert { get; set; }
    public ArcBool? AllowNormalConversion { get; set; }
    public ArcBool? LockLevelWhenSelected { get; set; }
    public ArcBool? LockedGovernmentType{ get; set; }
    public ArcInt? FixedRank { get; set; }
    public ArcBool? Queen { get; set; }
    public ArcBool? Heir { get; set; }
    public ArcBool? RoyalMarriage { get; set; }
    public ArcBlock? NationDesignerTrigger { get; set; }
    public ArcBool? ValidForNationDesigner { get; set; }
    public ArcInt? NationDesignerCost { get; set; }
    public ArcBool? RepublicanName { get; set; }
    public ArcInt? Duration { get; set; }
    public ArcBool? MaintainDynasty { get; set; }
    public ArcBool? HasParliament { get; set; }
    public ArcBool? HasHarem { get; set; }
    public ArcBool? HasPashas { get; set; }
    public ArcBool? ForeignSlaveRulers { get; set; }
    public ArcBool? AllowVassalWar { get; set; }
    public ArcBool? AllowVassalAlliance { get; set; }
    public ArcBool? AllowForceTributary { get; set; }
    public ArcBool? ClaimStates { get; set; }
    public ArcBool? DifferentReligionAcceptance { get; set; }
    public ArcBool? DifferentReligionGroupAcceptance { get; set; }
    public ArcBool? BoostIncome { get; set; }
    public ArcBool? CanUseTradePost { get; set; }
    public ArcBool? CanFormTradeLeague { get; set; }
    public GovernmentReform? TradeCityReform { get; set; }
    public ArcBool? NativeMechanic { get; set; }
    public ArcBool? AllowMigration { get; set; }
    public ArcBool? RulersCanBeGenerals { get; set; }
    public ArcBool? HeirsCanBeGenerals { get; set; }
    public ArcInt? MinAutonomy { get; set; }
    public ArcInt? StartTerritoryToEstates { get; set; }
    public ArcList<Faction>? Factions { get; set; }
    public Dict<ArcBlock>? AssimilationCultures { get; set; }
    public Dict<ArcBlock>? StatesGeneralMechanic { get; set; }
    */
    public ArcList<GovernmentMechanic>? GovernmentAbilities { get; set; }
    public static string[] ImplementedAttributes = new string[]
    {
        "name", "desc", "icon", "potential", "trigger", "modifier", "effect", "custom_attributes", "government_abilities"
    };
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public GovernmentReform(string id, ArcString name, ArcString desc, ArcString icon, ArcTrigger potential, ArcTrigger trigger, ArcModifier modifier, ArcCode? customAttributes, Dict<ArcCode> attributes, ArcList<GovernmentMechanic>? governmentAbilities, ArcEffect effect)
    {
        Id = new(id);
        Name = name;
        Desc = desc;
        Icon = icon;
        Potential = potential;
        Trigger = trigger;
        Modifier = modifier;
        CustomAttributes = customAttributes;
        Attributes = attributes;
        GovernmentAbilities = governmentAbilities;
        Effect = effect;
        KeyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "desc", Desc },
            { "icon", Icon },
            { "potential", Potential },
            { "trigger", Trigger },
            { "modifier", Modifier },
            { "custom_attributes", CustomAttributes },
            { "attributes", Attributes },
            { "effect", Effect },
        };

        GovernmentReforms.Add(id, this);
    }

    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        i.ForceMoveNext();

        string id = Compiler.GetId(i.Current);

        i = Args.GetArgs(i, out Args args);

        GovernmentReform reform = Constructor(id, args);

        Regex regex = new Regex("^tier_(\\d+)");
        Match match = regex.Match(id);
        if (match.Success)
        {
            int tier = int.Parse(match.Groups[1].Value);
            AddToReformLevel("monarchy");
            AddToReformLevel("republic");
            AddToReformLevel("tribal");
            AddToReformLevel("native");
            AddToReformLevel("theocracy");

            void AddToReformLevel(string type)
            {
                ReformLevel b = Government.Governments[type].Get<ArcList<ReformLevel>>("reform_levels").Values[tier - 1] ?? throw new Exception();
                b.Get<ArcList<GovernmentReform>>("reforms").Values.Add(reform);
            }
        }

        return i;

    }
    public static GovernmentReform Constructor(string id, Args args)
    {
        return new GovernmentReform(id, 
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc", new("")),
            args.Get(ArcString.Constructor, "icon"),
            args.Get(ArcTrigger.Constructor, "potential", new()),
            args.Get(ArcTrigger.Constructor, "trigger", new()),
            args.Get(ArcModifier.Constructor, "modifier", new()),
            args.Get(ArcCode.Constructor, "custom_attributes", null),
            args.GetAttributes(ImplementedAttributes),
            args.Get(ArcList<GovernmentMechanic>.GetConstructor(GovernmentMechanic.GovernmentMechanics), "government_abilities", null),
            args.Get(ArcEffect.Constructor, "effect", new())
        );
    }
    public static string Transpile()
    {
        Block file = new();
        foreach(GovernmentReform reform in GovernmentReforms.Values())
        {
            file.Add(reform.Id, "=", "{");
            file.Add("icon", "=", reform.Icon);
            reform.Potential.Compile("potential", ref file);
            reform.Trigger.Compile("trigger", ref file);
            reform.Modifier.Compile("modifiers", ref file);
            reform.Effect.Compile("effect", ref file);
            if(reform.GovernmentAbilities != null)
            {
                file.Add("government_abilities", "=", "{");
                foreach(GovernmentMechanic? mechanic in reform.GovernmentAbilities.Values)
                {
                    if (mechanic == null) continue;
                    file.Add(mechanic.Id);
                }
                file.Add("}");
            }
            if(reform.CustomAttributes != null) reform.CustomAttributes.Compile("custom_attributes", ref file);
            foreach(var v in reform.Attributes)
            {
                v.Value.Compile(v.Key, ref file, true, true);
            }
            file.Add("}");

            Program.Localisation.Add($"{reform.Id}", reform.Name.Value);
            Program.Localisation.Add($"{reform.Id}_desc", reform.Desc.Value);
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/common/government_reforms/reforms.txt", string.Join(' ', file));
        return "Government Reforms";
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}
