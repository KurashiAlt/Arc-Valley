using Arc;

public class LazyPointer<T> : IVariable where T : IVariable
{
    public readonly Func<T> Get;
    public LazyPointer(Func<T> get)
    {
        Get = get;
    }
    public LazyPointer(Dict<T> dict, string key)
    {
       Get = () => dict[key];
    }

    public Walker Call(Walker i, ref Block result, Compiler comp) => Get().Call(i, ref result, comp);
}