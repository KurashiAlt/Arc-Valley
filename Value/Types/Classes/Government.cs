using Arc;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class ReformLevel : ArcObject
{
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static ReformLevel Constructor(string id, Args args) => new()
    {
        { "id", new ArcString(id) },
        { "name", new ArcString(string.Join(' ', args.Get())) },
        { "reforms", new ArcList<GovernmentReform>(GovernmentReform.GovernmentReforms) }
    };
    public void Transpile(string id, ref Block b)
    {
        string levelId = $"{id}_{Get<ArcString>("id").Value}";
        Program.Localisation.Add(levelId, Get<ArcString>("name").Value);
        b.Add(
            levelId, "=", "{",
                "reforms", "=", "{");
        foreach(GovernmentReform? s in Get<ArcList<GovernmentReform>>("reforms").Values)
        {
            if (s == null) continue;
            b.Add(s.Id);
        }
        b.Add("}",
            "}");
    }
    public override Walker Call(Walker i, ref Block result) {
        i.ForceMoveNext();
        if (i.Current != "+=") throw new Exception();
        i.ForceMoveNext();
        if (i.Current == "new")
        {
            i.ForceMoveNext();

            string id = Compiler.GetId(i.Current);

            i = Args.GetArgs(i, out Args args);

            Get<ArcList<GovernmentReform>>("reforms").Values.Add(GovernmentReform.Constructor(id, args));
        }
        else
        {
            Get<ArcList<GovernmentReform>>("reforms").Values.Add(GovernmentReform.GovernmentReforms[i.Current]);
        }
        return i;
    }
}
public class Government : ArcObject
{
    public static readonly Dict<Government> Governments = new();
    public Government(string id)
    {
        Governments.Add(id, this);
    }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static Government Constructor(string id, Args args) => new(id)
    {
        { "id", new ArcString(id) },
        { "name", args.Get(ArcString.Constructor, "name") },
        { "basic_reform", args.Get(ArcCode.Constructor, "basic_reform") },
        { "color", args.Get(ArcCode.Constructor, "color") },
        { "reform_levels", args.Get(ArcList<ReformLevel>.GetConstructor(ReformLevel.Constructor), "reform_levels") }
    };
    public void Transpile(ref Block a, ref Block b)
    {
        string id = Get<ArcString>("id").Value;
        Program.Localisation.Add(id, Get<ArcString>("name").Value);
        Program.Localisation.Add($"{id}_name", Get<ArcString>("name").Value);

        a.Add(
            id, "=", "{",
            "basic_reform", "=", $"{id}_mechanic"
        );
        Get<ArcCode>("color").Compile("color", ref a, false);
        a.Add("reform_levels", "=", "{");
        foreach (ReformLevel? level in Get<ArcList<ReformLevel>>("reform_levels").Values)
        {
            if (level == null) continue;
            level.Transpile(id, ref a);
        }
        a.Add("}", "}");

        Get<ArcCode>("basic_reform").Compile($"{id}_mechanic", ref b);
    }
    public static string Transpile()
    {
        Block a = new();
        Block b = new();
        Compiler.global.Get<ArcCode>("default_reform").Compile("defaults_reform", ref b);
        foreach(Government gov in Governments.Values())
        {
            gov.Transpile(ref a, ref b);
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/common/governments/arc.txt", string.Join(' ', a));
        Program.OverwriteFile($"{Program.TranspileTarget}/common/government_reforms/basic_reforms.txt", string.Join(' ', b));
        return "Governments";
    }
}
public class GovernmentNames : ArcObject
{
    public static readonly Dict<GovernmentNames> GovernmentNameDict = new();
    public GovernmentNames(string id)
    {
        GovernmentNameDict.Add(id, this);
    }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public static GovernmentNames Constructor(string id, Args args) => new(id)
    {
        { "id", new ArcString(id) },
        { "rank", args.Get(Dict<ArcString>.Constructor(ArcString.Constructor), "rank", new()) },
        { "ruler_male", args.Get(Dict<ArcString>.Constructor(ArcString.Constructor), "ruler_male", new()) },
        { "ruler_female", args.Get(Dict<ArcString>.Constructor(ArcString.Constructor), "ruler_female", new()) },
        { "consort_male", args.Get(Dict<ArcString>.Constructor(ArcString.Constructor), "consort_male", new()) },
        { "consort_female", args.Get(Dict<ArcString>.Constructor(ArcString.Constructor), "consort_female", new()) },
        { "heir_male", args.Get(Dict<ArcString>.Constructor(ArcString.Constructor), "heir_male", new()) },
        { "heir_female", args.Get(Dict<ArcString>.Constructor(ArcString.Constructor), "heir_female", new()) },
        { "trigger", args.Get(ArcTrigger.Constructor, "trigger") }
    };
    public void Transpile(ref Block a)
    {
        string id = Get<ArcString>("id").Value;
        a.Add(id, "=", "{");
        v(ref a, "rank");
        v(ref a, "ruler_male");
        v(ref a, "ruler_female");
        v(ref a, "consort_male");
        v(ref a, "consort_female");
        v(ref a, "heir_male");
        v(ref a, "heir_female");
        Get<ArcTrigger>("trigger").Compile("trigger", ref a);
        a.Add("}");

        void v(ref Block a, string type)
        {
            a.Add(type, "=", "{");
            foreach (KeyValuePair<string, ArcString> kvp in Get<Dict<ArcString>>(type))
            {
                string locKey = $"{id}_{type}_{kvp.Key}";
                a.Add(kvp.Key, "=", locKey);
                Program.Localisation.Add(locKey, kvp.Value.Value);
            }
            a.Add("}");
        }
    }
    public static string Transpile()
    {
        Block a = new();
        foreach(GovernmentNames gv in GovernmentNameDict.Values())
        {
            gv.Transpile(ref a);
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/common/government_names/arc.txt", string.Join(' ', a));
        return "Government Names";
    }

}