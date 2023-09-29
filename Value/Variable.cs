namespace Arc;
public interface IVariable
{
    public virtual Walker Call(Walker i, ref Block result) => throw new NotImplementedException();
}