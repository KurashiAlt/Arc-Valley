using Arc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CustomIcon : ArcObject
{
    public static readonly Dict<CustomIcon> CustomIcons = new();
    public CustomIcon(string id)
    {
        CustomIcons.Add(id, this);
    }
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public override string ToString() => Get("id").ToString();
    public static ArcObject FrameConstructor(Block b)
    {
        Args args = Args.GetArgs(b);
        return new ArcObject()
        {
            { "frame", args.Get(ArcInt.Constructor, "frame") },
            { "trigger", args.Get(ArcTrigger.Constructor, "trigger") },
        };
    }
    public static CustomIcon Constructor(string id, Args args) => new(id)
    {
        { "id", new ArcString(id) },
        { "potential", args.Get(ArcTrigger.Constructor, "potential", new ArcTrigger()) },
        { "tooltip", args.Get(ArcString.Constructor, "tooltip", null) },
        { "frames", args.Get(ArcList<ArcObject>.GetConstructor(FrameConstructor), "frames", new(FrameConstructor)) }
    };
    public void Transpile(ref Block b)
    {
        string id = ToString();
        b.Add(
            "custom_icon", "=", "{",
                "name", "=", id,
                Get<ArcTrigger>("potential").Compile("potential")
        );
        ArcString? Tooltip = GetNullable<ArcString>("tooltip");
        if (Tooltip != null)
        {
            b.Add("tooltip", "=", $"{id}_tt");
            Program.Localisation.Add($"{id}_tt", Tooltip.ToString());
        }
        
        foreach (ArcObject? frame in Get<ArcList<ArcObject>>("frames").Values)
        {
            if (frame == null) continue;
            b.Add(
                "frame", "=", "{",
                    "number", "=", frame.Get("frame"),
                    frame.Get<ArcTrigger>("trigger").Compile("trigger"),
                "}"
            );
        }

        b.Add("}");
    }
    public static string Transpile()
    {
        Block b = new();
        foreach (CustomIcon CustomIcon in CustomIcons.Values())
        {
            CustomIcon.Transpile(ref b);
        }
        Program.OverwriteFile($"{Program.TranspileTarget}/common/custom_gui/custom_icons.txt", b.ToString());
        return "Custom Icons";
    }
    public override Walker Call(Walker i, ref Block result) => throw new Exception();
}