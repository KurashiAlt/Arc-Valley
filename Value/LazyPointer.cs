using Arc;

public class LazyPointer<T> : IVariable, IArcObject where T : IVariable
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

    public Walker Call(Walker i, ref Block result) => Get().Call(i, ref result);
    public bool CanGet(string indexer) => ((Get() as IArcObject)?.CanGet(indexer)) ?? false;
    IVariable? IArcObject.Get(string indexer) => (Get() as IArcObject)?.Get(indexer);
}