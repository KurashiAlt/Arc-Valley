using Microsoft.CodeAnalysis;
using Pastel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace Arc;
public class Advisor : ArcObject
{
    public static readonly Dict<Advisor> Advisors = new();
    public Advisor(string key) { Advisors.Add(key, this); }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static Advisor Constructor(string id, Args args) => new(id)
    {
        { "id", new ArcInt(Advisors.Count) },
        { "name", args.Get(ArcString.Constructor, "name") },
        { "location", args.GetFromList(Province.Provinces, "location") },
        { "discount", args.Get(ArcBool.Constructor, "discount", new(false)) },
        { "skill", args.Get(ArcInt.Constructor, "skill", new(1)) },
        { "type", args.GetFromList(AdvisorType.AdvisorTypes, "type") },
        { "date", args.Get(ArcString.Constructor, "date") },
        { "death_date", args.Get(ArcString.Constructor, "death_date") }
    };
    public override string ToString() => Get("id").ToString();
    public override Walker Call(Walker i, ref Block result) { result.Add(ToString()); return i; }
    public void Transpile(ref Block s)
    {
        s.Add(
            "advisor", "=", "{",
                "advisor_id", "=", Get("id"),
                "name", "=", $"\"{Get("name")}\"",
                "location", "=", Get("location"),
                "discount", "=", Get("discount"),
                "skill", "=", Get("skill"),
                "type", "=", Get("type"),
                "date", "=", Get("date"),
                "death_date", "=", Get("death_date"),
            "}"
        );
    }
    public static string Transpile()
    {
        Block s = new();
        foreach (KeyValuePair<string, Advisor> Advisor in Advisors)
        {
            Advisor.Value.Transpile(ref s);
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/history/advisors/arc.txt", string.Join(' ', s));
        return "Advisor Types";
    }
}

public class AdvisorType : ArcObject
{
    public static readonly Dict<AdvisorType> AdvisorTypes = new();
    public AdvisorType(string id) { AdvisorTypes.Add(id, this); }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static ArcObject SkillScaledModifierConstructor(Block b)
    {
        Args gArgs = Args.GetArgs(b);
        return new ArcObject()
        {
            { "trigger", gArgs.Get(ArcTrigger.Constructor, "trigger") },
            { "modifier", gArgs.Get(ArcModifier.Constructor, "modifier") },
        };
    }
    public static AdvisorType Constructor(string id, Args args) => new(id)
    {
        { "id", new ArcString(id) },
        { "name", args.Get(ArcString.Constructor, "name") },
        { "desc", args.Get(ArcString.Constructor, "desc") },
        { "modifier", args.Get(ArcModifier.Constructor, "modifier") },
        { "ai_will_do", args.Get(ArcCode.Constructor, "ai_will_do", new("factor = 1")) },
        { "allow_only_male", args.Get(ArcBool.Constructor, "allow_only_male", new(false)) },
        { "allow_only_female", args.Get(ArcBool.Constructor, "allow_only_female", new(false)) },
        { "allow_only_owner_religion", args.Get(ArcBool.Constructor, "allow_only_owner_religion", new(false)) },
        { "chance", args.Get(ArcCode.Constructor, "chance", new("factor = 1")) },
        { "monarch_power", args.Get(ArcString.Constructor, "monarch_power") },
        { "skill_scaled_modifiers", args.Get(ArcList<ArcObject>.GetConstructor(SkillScaledModifierConstructor), "skill_scaled_modifier", new(SkillScaledModifierConstructor)) },
    };
    public override string ToString() => Get("id").ToString();
    public override Walker Call(Walker i, ref Block result) { result.Add(ToString()); return i; }
    public void Transpile(ref Block s)
    {
        s.Add(Get("id"), "=", "{");
        s.Add("monarch_power", "=", Get("monarch_power"));
        if (Get("allow_only_female") is ArcBool allowOnlyFemale && allowOnlyFemale == true) s.Add("allow_only_female", "=", allowOnlyFemale);
        if (Get("allow_only_male") is ArcBool allowOnlyMale && allowOnlyMale == true) s.Add("allow_only_male", "=", allowOnlyMale);
        if (Get("allow_only_owner_religion") is ArcBool allowOnlyOwnerReligion && allowOnlyOwnerReligion == true) s.Add("allow_only_owner_religion", "=", allowOnlyOwnerReligion);
        foreach (ArcObject skillScaledModifier in Get<ArcList<ArcObject>>("skill_scaled_modifiers"))
        {
            s.Add(
                "skill_scaled_modifier", "=", "{",
                    skillScaledModifier.Get<ArcTrigger>("trigger").Compile("trigger"),
                    skillScaledModifier.Get<ArcModifier>("modifier").Compile("modifier"),
                "}"
            );
        }
        Get<ArcModifier>("modifier").Compile(ref s);
        Get<ArcCode>("ai_will_do").Compile("ai_will_do", ref s);
        Get<ArcCode>("chance").Compile("chance", ref s);
        s.Add("}");
        Program.Localisation.Add($"{Get("id")}", Get("name").ToString());
        Program.Localisation.Add($"{Get("id")}_desc", Get("desc").ToString());
    }
    public static string Transpile()
    {
        Block s = new();
        foreach (KeyValuePair<string, AdvisorType> Advisor in AdvisorTypes)
        {
            Advisor.Value.Transpile(ref s);
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/common/advisortypes/arc.txt", string.Join(' ', s));
        return "Advisor Types";
    }
}