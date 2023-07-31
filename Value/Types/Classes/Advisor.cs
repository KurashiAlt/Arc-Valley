using ArcInstance;
using Pastel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace Arc;
public class AdvisorType : IArcObject
{
    public static readonly Dict<AdvisorType> AdvisorTypes = new();
    public bool IsObject() => true;
    public string Class => "AdvisorType";
    public ArcString Name { get; set; }
    public ArcString Desc { get; set; }
    public ArcBlock Modifier { get; set; }
    public ArcBool AllowOnlyMale { get; set; }
    public ArcBool AllowOnlyFemale { get; set; }
    public ArcBool AllowOnlyOwnerReligion { get; set; }
    public ArcBlock Chance { get; set; }
    public ArcBlock AiWillDo { get; set; }
    public ArcString MonarchPower { get; set; }
    public ArcString Id { get; set; }
    public Dict<IValue> KeyValuePairs { get; set; }
    public AdvisorType(ArcString name, ArcString desc, ArcBlock modifier, ArcBlock aiWillDo, ArcString id, ArcBool allowOnlyMale, ArcBool allowOnlyFemale, ArcBool allowOnlyOwnerReligion, ArcBlock chance, ArcString monarchPower)
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
            args.Get(ArcBlock.Constructor, "modifier"),
            args.Get(ArcBlock.Constructor, "ai_will_do", new("factor = 1")),
            new(id),
            args.Get(ArcBool.Constructor, "allow_only_male", new(false)),
            args.Get(ArcBool.Constructor, "allow_only_female", new(false)),
            args.Get(ArcBool.Constructor, "allow_only_owner_religion", new(false)),
            args.Get(ArcBlock.Constructor, "chance", new("factor = 1")),
            args.Get(ArcString.Constructor, "monarch_power")
        );

        AdvisorTypes.Add(id, AdvisorType);

        return i;
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref List<string> result, Compiler comp) { result.Add(Id.Value); return i; }
    public static void Transpile()
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
        Console.WriteLine($"Finished Transpiling Advisor Types".Pastel(ConsoleColor.Cyan));
    }
}