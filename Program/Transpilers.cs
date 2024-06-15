using Arc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static partial class TranspilerClass
{
    public static string TranspileCustomTextBoxes()
    {
        if (!Compiler.global.CanGet("custom_text_boxes")) return "";

        Block b = new();
        foreach (ArcObject obj in Compiler.GetVariable<Dict<IVariable>>(new Word("custom_text_boxes")).Values())
        {
            string id = obj.Get("id").ToString();
            Program.Localisation.Add(id, obj.Get("name").ToString());
            b.Add(
                "custom_text_box", "=", "{",
                    "name", "=", id,
                    obj.Get<ArcTrigger>("potential").Compile("potential")
            );
            ArcString? Tooltip = obj.GetNullable<ArcString>("tooltip");
            if (Tooltip != null)
            {
                b.Add("tooltip", "=", $"{id}_tt");
                Program.Localisation.Add($"{id}_tt", Tooltip.ToString());
            }
            b.Add(
                "}"
            );
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/common/custom_gui/custom_text_boxes.txt", b.ToString());
        return "Custom Text Boxes";
    }
}