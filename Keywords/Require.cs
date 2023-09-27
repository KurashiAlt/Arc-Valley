namespace Arc;

public partial class Compiler
{
    public static T GetVariable<T>(string locator)
    {
        if(TryGetVariable(locator, out IVariable? var)) 
        {
            if (var == null) throw new Exception($"Variable {locator} is null");
            return (T)var;
        }
        else
        {
            throw new Exception($"Could not find variable {locator}");
        }
    }
}
