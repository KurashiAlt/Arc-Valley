
using Pastel;
using System.IO;
using System.Text;

namespace Arc;
public class ReligionGroup : IArcObject
{
    public static readonly Dict<ReligionGroup> ReligionGroups = new();
    public bool IsObject() => true;
    public ArcString Name { get; set; }
    public ArcString Id { get; set; }
    public ArcBool DefenderOfFaith { get; set; }
    public ArcBool CanFormPersonalUnions { get; set; }
    public Province CenterOfReligion { get; set; }
    public ArcString CrusadeName { get; set; }
    public ArcInt? FlagsWithEmblemPercentage { get; set; }
    public ArcBlock? FlagsEmblemIndexRange { get; set; }
    public Dict<IVariable?> KeyValuePairs { get; set; }
    public ReligionGroup(ArcString name, ArcString id, ArcBool defenderOfFaith, ArcBool canFormPersonalUnions, Province centerOfReligion, ArcString crusadeName, ArcInt? flagsWithEmblemPercentage, ArcBlock? flagsEmblemIndexRange)
    {
        Name = name;
        Id = id;
        DefenderOfFaith = defenderOfFaith;
        CanFormPersonalUnions = canFormPersonalUnions;
        CenterOfReligion = centerOfReligion;
        CrusadeName = crusadeName;
        FlagsWithEmblemPercentage = flagsWithEmblemPercentage;
        FlagsEmblemIndexRange = flagsEmblemIndexRange;
        KeyValuePairs = new()
        {
            { "name", Name },
            { "id", Id },
            { "defender_of_faith", DefenderOfFaith },
            { "can_form_personal_unions", CanFormPersonalUnions },
            { "center_of_religion", CenterOfReligion },
            { "crusade_name", CrusadeName },
            { "flags_with_emblem_percentage", FlagsEmblemIndexRange },
            { "flag_emblem_index_range", FlagsEmblemIndexRange }
        };
    }

    public bool CanGet(string indexer) => KeyValuePairs.CanGet(indexer);
    public IVariable? Get(string indexer) => KeyValuePairs.Get(indexer);
    public static Walker Call(Walker i)
    {
        if (!i.MoveNext()) throw new Exception();

        string id = Compiler.GetId(i.Current);

        i = Args.GetArgs(i, out Args args);
        ReligionGroup ReligionGroup = new(
            args.Get(ArcString.Constructor, "name"),
            new(id),
            args.Get(ArcBool.Constructor, "defender_of_faith", new(false)),
            args.Get(ArcBool.Constructor, "can_form_personal_unions", new(false)),
            args.GetFromList(Province.Provinces, "center_of_religion"),
            args.Get(ArcString.Constructor, "crusade_name", new("Crusade")),
            args.Get(ArcInt.Constructor, "flags_with_emblem_percentage", null),
            args.Get(ArcCode.Constructor, "flag_emblem_index_range", null)
        );

        ReligionGroups.Add(id, ReligionGroup);

        return i;
    }
    public static string Transpile()
    {
        string countryreligionview = ((ArcString)((Dict<IValue>)Compiler.global["interface"]).Get("countryreligionview")).Value;
        string church_aspects = ((ArcString)((Dict<IValue>)Compiler.global["interface"]).Get("church_aspects")).Value;

        StringBuilder sa = new();

        StringBuilder sb = new();
        foreach (ReligionGroup religionGroup in ReligionGroups.Values())
        {
            Program.Localisation.Add($"{religionGroup.Id}", religionGroup.Name.Value);
            Program.Localisation.Add($"{religionGroup.Id}_crusade", religionGroup.CrusadeName.Value);

            sb.Append($"{religionGroup.Id} = {{ ");
            if (religionGroup.DefenderOfFaith) sb.Append("defender_of_faith = yes ");
            if (religionGroup.CanFormPersonalUnions) sb.Append("can_form_personal_unions = yes ");
            sb.Append($"center_of_religion = {religionGroup.CenterOfReligion.Id} ");
            sb.Append($"crusade_name = {religionGroup.Id}_crusade ");
            if (religionGroup.FlagsWithEmblemPercentage != null) sb.Append($"flags_with_emblem_percentage = {religionGroup.FlagsWithEmblemPercentage} ");
            if (religionGroup.FlagsEmblemIndexRange != null) sb.Append($"flag_emblem_index_range = {{ {religionGroup.FlagsEmblemIndexRange} }} ");

            foreach (Religion religion in from rel in Religion.Religions where rel.Value.ReligionGroup == religionGroup select rel.Value)
            {
                religion.Transpile(sb);


                if (religion.UsesChurchPower)
                {
                    sa.Append(church_aspects.Replace("$id$", religion.Id.Value).Trim('`'));
                    sa.Append(" ");
                }
            }
            sb.Append("} ");
        }
        countryreligionview = countryreligionview.Replace("$church_aspects$", sa.ToString()).Trim('`');

        Program.OverwriteFile($"{Program.TranspileTarget}/interface/countryreligionview.gui", countryreligionview, false);

        Program.OverwriteFile($"{Program.TranspileTarget}/common/religions/arc.txt", sb.ToString());
        return "Religions";
    }
    public override string ToString() => Name.Value;
    public Walker Call(Walker i, ref Block result) { result.Add(Id.Value.ToString()); return i; }
}
