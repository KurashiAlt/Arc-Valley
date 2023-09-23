namespace Arc;
public interface IVariable
{
    public Walker Call(Walker i, ref Block result);
}