using Arc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ProvinceGroup : ArcObject
{
    public static Dict<ProvinceGroup> ProvinceGroups = new();
    public static new Walker Call(Walker i) => Call(i, Constructor);
    public ProvinceGroup(string id, Args args)
    {
        ProvinceGroups.Add(id, this);
    }
    public static ProvinceGroup Constructor(string id, Args args) => new(id, args)
    {
        { "id", new ArcString(id) },
        { "name", args.Get(ArcString.Constructor, "name") },
        { "provinces", args.Get(ArcList<Province>.GetConstructor(Province.Provinces), "provinces") }
    };
    public override Walker Call(Walker i, ref Block result)
    {
        result.Add(Get("id"));
        return i;
    }
    public void Transpile(ref Block b)
    {
        string id = Get("id").ToString();
        if(CanGet("name")) Program.Localisation.Add($"{id}", Get("name").ToString());
        b.Add(id, "=", "{");
        foreach(Province? prov in Get<ArcList<Province>>("provinces").Values)
        {
            if (prov == null) continue;
            b.Add(prov);
        }
        b.Add("}");
    }
    public static string Transpile()
    {
        Block b = new();

        foreach(KeyValuePair<string, ProvinceGroup> p in ProvinceGroups)
        {
            p.Value.Transpile(ref b);
        }

        Program.OverwriteFile($"{Program.TranspileTarget}/map/provincegroup.txt", string.Join(' ', b));
        return "Province Groups";
    }
}