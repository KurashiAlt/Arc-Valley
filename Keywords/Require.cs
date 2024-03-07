namespace Arc;

public partial class Compiler
{
    public static T GetVariable<T>(string locator)
    {
        if(TryGetVariable(locator, out IVariable? var)) 
        {
            if (var == null) throw ArcException.Create($"Variable {locator} is null", locator);
            return (T)var;
        }
        else
        {
            throw ArcException.Create($"Could not find variable {locator}", locator);
        }
    }
}
