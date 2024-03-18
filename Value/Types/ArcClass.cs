using Arc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class ArcNull : IVariable
{

}
public class ArcClass : ArcObject
{
    public static Dict<ArcClass> Classes = new();
    public Func<string, IVariable> Define;
    public Func<Args, string, IVariable> Init;
    public ArcCode OnCreate;
    public ArcClass(string id, Args args)
    {
        Dict<IVariable> List;
        if (args.keyValuePairs.ContainsKey("list"))
        {
            List = new();
            Compiler.global.Add(args.Get("list").ToString(), List);
        }
        else List = Compiler.global;
        string idConst = args.Get("id", new("`{this:id}`")).ToString();
        Args? Default;
        Block? DefaultBlock = args.GetNullable("default");
        if (DefaultBlock != null) Default = Args.GetArgs(DefaultBlock);
        else Default = null;

        OnCreate = args.Get(ArcCode.NamelessConstructor, "on_create", new() { ShouldBeCompiled = false });
        Define = (string s) =>
        {
            ArcNull n = new();
            List.Add(s, n);
            return n;
        };
        Init = (Args s, string obj) =>
        {
            if (Default != null) s.Inherit(Default);
            if (s.keyValuePairs != null) s.keyValuePairs.Add("id", new Block(obj));
            Compiler.global.Add("this", new ArgsObject(s));
            ArcType t = ArcType.Constructor(args.Get("args"));
            IVariable v = t.ThisConstructor(s.block);
            if (v is ArcObject ob)
            {
                ob.Add("id", new ArcString(idConst));
            }
            List[obj] = v;

            OnCreate.Compile();

            Compiler.global.Delete("this");
            return v;
        };
        ArcType.Types.Add(id, new (List.Get));

        Classes.Add(id, this);
    }
    public static ArcClass Constructor(string id, Args args) => new(id, args)
    {
        { "id", new ArcString(id) },
    };
    public static Walker Call(Walker i) => Call(i, Constructor);
}