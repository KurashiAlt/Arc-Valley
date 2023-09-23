using ArcInstance;
using Pastel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace Arc;
public class Advisor : IArcObject
{
    public static readonly Dict<Advisor> Advisors = new();
    public bool IsObject() => true;
    public ArcInt Id { get; set; }
    public ArcString Name { get; set; }
    public Province Location { get; set; }
    public ArcBool Discount { get; set; }
    public ArcInt Skill { get; set; }
    public AdvisorType Type { get; set; }
    public ArcString Date { get; set; }
    public ArcString DeathDate { get; set; }
    public Dict<IVariable> KeyValuePairs { get; set; }
    public Advisor(
        string key,
        ArcString name,
        Province location,
        ArcBool discount,
        ArcInt skill,
        AdvisorType type,
        ArcString date,
        ArcString deathDate
    ) {
        Advisors.Add(key, this);
        Id = new(Advisors.Count + 1);
        Name = name;
        Location = location;
        Discount = discount;
        Skill = skill;
        Type = type;
        Date = date;
        DeathDate = deathDate;
        KeyValuePairs = new()
        {
            { "id", Id },
            { "name", Name },
            { "location", Location },
            { "discount", Discount },
            { "type", Type },
            { "date", Date },
            { "death_date", DeathDate },
        };
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string key = i.Current;

        i = Args.GetArgs(i, out Args args);

        Advisor Advisor = new(
            key,
            args.Get(ArcString.Constructor, "name"),
            args.GetFromList(Province.Provinces, "location"),
            args.Get(ArcBool.Constructor, "discount", new(false)),
            args.Get(ArcInt.Constructor, "skill", new(1)),
            args.GetFromList(AdvisorType.AdvisorTypes, "type"),
            args.Get(ArcString.Constructor, "date"),
            args.Get(ArcString.Constructor, "death_date")
        );

        return i;
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value); return i; }
    public void Transpile(ref Block s)
    {
        s.Add(
            "advisor", "=", "{",
                "advisor_id", "=", Id,
                "name", "=", $"\"{Name}\"",
                "location", "=", Location.Id,
                "discount", "=", Discount,
                "skill", "=", Skill,
                "type", "=", Type.Id,
                "date", "=", Date,
                "death_date", "=", DeathDate,
            "}"
        );
    }
    public static string Transpile()
    {
        Block s = new();
        foreach (Advisor Advisor in Advisors.Values())
        {
            Advisor.Transpile(ref s);
        }
        Instance.OverwriteFile("target/history/advisors/arc.txt", string.Join(' ', s));
        return "Advisor Types";
    }
}

public class AdvisorType : IArcObject
{
    public static readonly Dict<AdvisorType> AdvisorTypes = new();
    public bool IsObject() => true;
    public string Class => "AdvisorType";
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }
    public ArcModifier Modifier { get; set; }
    public ArcBool AllowOnlyMale { get; set; }
    public ArcBool AllowOnlyFemale { get; set; }
    public ArcBool AllowOnlyOwnerReligion { get; set; }
    public ArcCode Chance { get; set; }
    public ArcCode AiWillDo { get; set; }
    public ArcString MonarchPower { get; set; }
    public ArcString Id { get; set; }
    public Dict<IValue> KeyValuePairs { get; set; }
    public AdvisorType(ArcString name, ArcString desc, ArcModifier modifier, ArcCode aiWillDo, ArcString id, ArcBool allowOnlyMale, ArcBool allowOnlyFemale, ArcBool allowOnlyOwnerReligion, ArcCode chance, ArcString monarchPower)
    {
        Name = name;
        Desc = desc;
        Modifier = modifier;
        AiWillDo = aiWillDo;
        Id = id;
        AllowOnlyMale = allowOnlyMale;
        AllowOnlyFemale = allowOnlyFemale;
        AllowOnlyOwnerReligion = allowOnlyOwnerReligion;
        Chance = chance;
        MonarchPower = monarchPower;
        KeyValuePairs = new Dict<IValue>()
        {
            { "name", Name },
            { "desc", Desc },
            { "modifier", Modifier },
            { "ai_will_do", AiWillDo },
            { "id", Id },
            { "allow_only_male", AllowOnlyMale },
            { "allow_only_female", AllowOnlyFemale },
            { "allow_only_owner_religion", AllowOnlyOwnerReligion },
            { "chance", Chance },
            { "monarch_power", MonarchPower },
        };
    }
    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = i.Current;

        i = Args.GetArgs(i, out Args args);

        AdvisorType AdvisorType = new(
            args.Get(ArcString.Constructor, "name"),
            args.Get(ArcString.Constructor, "desc"),
            args.Get(ArcModifier.Constructor, "modifier"),
            args.Get(ArcCode.Constructor, "ai_will_do", new("factor = 1")),
            new(id),
            args.Get(ArcBool.Constructor, "allow_only_male", new(false)),
            args.Get(ArcBool.Constructor, "allow_only_female", new(false)),
            args.Get(ArcBool.Constructor, "allow_only_owner_religion", new(false)),
            args.Get(ArcCode.Constructor, "chance", new("factor = 1")),
            args.Get(ArcString.Constructor, "monarch_power")
        );

        AdvisorTypes.Add(id, AdvisorType);

        return i;
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value); return i; }
    public static string Transpile()
    {
        StringBuilder sb = new("");
        foreach (AdvisorType AdvisorType in AdvisorTypes.Values())
        {
            sb.Append($"{AdvisorType.Id} = {{ monarch_power = {AdvisorType.MonarchPower} ");
            if (AdvisorType.AllowOnlyFemale) sb.Append("allow_only_female = yes ");
            if (AdvisorType.AllowOnlyMale) sb.Append("allow_only_male = yes ");
            if (AdvisorType.AllowOnlyOwnerReligion) sb.Append("allow_only_owner_religion = yes ");
            sb.Append($"{AdvisorType.Modifier.Compile()} {AdvisorType.AiWillDo.Compile("ai_will_do")} {AdvisorType.Chance.Compile("chance")} }} ");
            Instance.Localisation.Add($"{AdvisorType.Id}", AdvisorType.Name.Value);
            Instance.Localisation.Add($"{AdvisorType.Id}_desc", AdvisorType.Desc.Value);
        }
        Instance.OverwriteFile("target/common/advisortypes/arc.txt", sb.ToString());
        return "Advisor Types";
    }
}