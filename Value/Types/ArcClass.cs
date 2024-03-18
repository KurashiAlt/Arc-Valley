using Arc;
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
        if (args.keyValuePairs == null) throw ArcException.Create(id, args);

        Dict<IVariable> List;
        if (args.keyValuePairs.ContainsKey("list"))
        {
            List = new();
            Compiler.global.Add(args.Get("list").ToString(), List);
        }
        else List = Compiler.global;

        ArcType.Types.Add(id, new(List.Get));
        Classes.Add(id, this);

        string idConst = args.Get("id", new("`{this:id}`")).ToString();
        Args? Default;
        Block? DefaultBlock = args.GetNullable("default");
        if (DefaultBlock != null) Default = Args.GetArgs(DefaultBlock);
        else Default = null;

        Dictionary<string, NewCommand> functions = new();
        foreach (KeyValuePair<string, Block> pair in args.keyValuePairs)
        {
            if (pair.Key == "list") continue;
            if (pair.Key == "id") continue;
            if (pair.Key == "args") continue;
            if (pair.Key == "default") continue;
            if (pair.Key == "on_create") continue;
            functions.Add(pair.Key, new NewCommand(pair.Key, Args.GetArgs(pair.Value), CompileType.Block));
        }

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
                ob.functions = functions;
            }
            List[obj] = v;

            OnCreate.Compile();

            Compiler.global.Delete("this");
            return v;
        };
    }
    public static ArcClass Constructor(string id, Args args) => new(id, args)
    {
        { "id", new ArcString(id) },
    };
    public static Walker Call(Walker i) => Call(i, Constructor);
}