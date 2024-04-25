namespace Arc;

public interface IArcObject : IVariable
{
    IVariable? Get(string indexer);
    bool CanGet(string indexer);
    bool Delete(string indexer) => throw new NotImplementedException();
    
}