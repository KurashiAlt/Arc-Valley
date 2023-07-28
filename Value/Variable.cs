namespace Arc;
public interface IVariable
{
    public Walker Call(Walker i, ref List<string> result, Compiler comp);
    public bool IsFloat() => false;
    public bool IsInt() => false;
    public bool IsBool() => false;
    public bool IsString() => false;
    public bool IsList() => false;
    public bool IsBlock() => false;
    public bool IsObject() => false;
}