using ArcInstance;
using Pastel;
using System.IO;
using System.Linq;
using System.Text;

namespace Arc;
public class GovernmentReform : IArcObject
{
    public static readonly Dict<GovernmentReform> GovernmentReforms = new();
    public bool IsObject() => true;
    public ArcString Id { get; set; }
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }
    public ArcString Icon { get; set; }
    public ArcBlock Potential { get; set; }
    public ArcBlock Trigger { get; set; }
    public ArcBlock Modifier { get; set; }
    public ArcBlock? CustomAttributes { get; set; }
    //public ArcList<ArcBlock>? Conditionals { get; set; }
    public Dict<ArcBlock> Attributes { get; set; } //Used for non implemented Attributes
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
    public ArcBlock? GovernmentAbilities { get; set; }
    */
    public static string[] ImplementedAttributes = new string[]
    {
        "name", "desc", "icon", "potential", "trigger", "modifier", "custom_attributes"
    };
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public GovernmentReform(string id, ArcString name, ArcString desc, ArcString icon, ArcBlock potential, ArcBlock trigger, ArcBlock modifier, ArcBlock? customAttributes, Dict<ArcBlock> attributes)
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
        };

        GovernmentReforms.Add(id, this);
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
    public static GovernmentReform Constructor(string id, Args args)
    {
        return new GovernmentReform(id, 
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc"),
            args.Get(ArcString.Constructor, "icon"),
            args.Get(ArcBlock.Constructor, "potential", new()),
            args.Get(ArcBlock.Constructor, "trigger", new()),
            args.Get(ArcBlock.Constructor, "modifier", new()),
            args.Get(ArcBlock.Constructor, "custom_attributes", null),
            args.GetAttributes(ImplementedAttributes)
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
            if(reform.CustomAttributes != null) reform.CustomAttributes.Compile("custom_attributes", ref file);
            foreach(var v in reform.Attributes)
            {
                v.Value.Compile(v.Key, ref file, true);
            }
            file.Add("}");

            Instance.Localisation.Add($"{reform.Id}", reform.Name.Value);
            Instance.Localisation.Add($"{reform.Id}_desc", reform.Desc.Value);
        }
        Instance.OverwriteFile("target/common/government_reforms/arc.txt", string.Join(' ', file));
        return "Government Reforms";
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result, Compiler comp) { result.Add(Id.Value.ToString()); return i; }
}
