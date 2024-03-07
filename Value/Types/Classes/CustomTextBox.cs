using Arc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CustomTextBox : ArcObject
{
    public static readonly Dict<CustomTextBox> CustomTextBoxs = new();
    public CustomTextBox(string id)
    {
        CustomTextBoxs.Add(id, this);
    }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public override string ToString() => Get("id").ToString();
    public static CustomTextBox Constructor(string id, Args args) => new(id)
    {
        { "id", new ArcString(id) },
        { "potential", args.Get(ArcTrigger.Constructor, "potential", new ArcTrigger()) },
        { "name", args.Get(ArcString.Constructor, "name") },
        { "tooltip", args.Get(ArcString.Constructor, "tooltip", null) },
    };
    public void Transpile(ref Block b)
    {
        string id = ToString();
        Program.Localisation.Add(id, Get("name").ToString());
        b.Add(
            "custom_text_box", "=", "{",
                "name", "=", id,
                Get<ArcTrigger>("potential").Compile("potential")
        );
        ArcString? Tooltip = GetNullable<ArcString>("tooltip");
        if (Tooltip != null)
        {
            b.Add("tooltip", "=", $"{id}_tt");
            Program.Localisation.Add($"{id}_tt", Tooltip.ToString());
        }
        b.Add(
            "}"
        );
    }
    public static string Transpile()
    {
        Block b = new();
        foreach (CustomTextBox CustomTextBox in CustomTextBoxs.Values())
        {
            CustomTextBox.Transpile(ref b);
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/common/custom_gui/custom_text_boxes.txt", b.ToString());
        return "Custom Text Boxes";
    }
}