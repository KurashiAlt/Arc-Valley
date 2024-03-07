using Arc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

public class CustomizableLocalization : ArcList<ArcObject>
{
    public static readonly Dict<CustomizableLocalization> CustomizableLocalizations = new();
    public ArcString Id;
    public override IVariable? Get(string indexer)
    {
        switch (indexer)
        {
            case "id": return Id;
        }
        if (int.TryParse(indexer, out int res))
        {
            res -= 1;
            return Values[res];
        }
        throw new Exception();
    }
    public override bool CanGet(string indexer)
    {
        switch (indexer)
        {
            case "id": return true;
        }
        if (int.TryParse(indexer, out int res))
        {
            res -= 1;
            if (res < 0) return false;
            if (res >= Values.Count) return false;
            return true;
        }
        return false;
    }
    public CustomizableLocalization(string id, Block value) : base(value, (Block b) =>
    {
        Args args = Args.GetArgs(b);
        return new ArcObject()
        {
            { "text", args.Get(ArcString.Constructor, "text") },
            { "trigger", args.Get(ArcTrigger.Constructor, "trigger") },
        };
    })
    {
        Id = new(id);
        CustomizableLocalizations.Add(id, this);
    }
    protected static Walker Call<T>(Walker i, Func<string, Args, T> func)
    {
        i.ForceMoveNext();

        string id = Compiler.GetId(i.Current);

        i = Args.GetArgs(i, out Args args);

        func(id, args);

        return i;
    }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public override string ToString() => Get("id").ToString();
    public static CustomizableLocalization Constructor(string id, Args args) => new(id, args.block);
    public void Transpile(ref Block sb)
    {
        sb.Add(
            "defined_text", "=", "{",
                "name", "=", Id
        );
        int i = 1;
        foreach (ArcObject? item in Values)
        {
            if (item == null) continue;
            Program.Localisation.Add($"{Id}_text_{i}", item.Get("text").ToString());
            sb.Add(
                "text", "=", "{",
                    "localisation_key", "=", $"{Id}_text_{i}",
                    item.Get<ArcBlock>("trigger").Compile("trigger"),
                "}"
            );
            i++;
        }
        sb.Add("}");
    }
    public static string Transpile()
    {
        Block b = new();
        foreach (CustomizableLocalization cl in CustomizableLocalizations.Values())
        {
            cl.Transpile(ref b);
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/customizable_localization/arc.txt", b.ToString());
        return "Customizable Localization";
    }
}