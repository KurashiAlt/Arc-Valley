namespace Arc;

public class IPath
{
    //You shouldn't be allowed to interact with files outside of here
    public static string FullDirectory = Program.directory;
    //You shouldn't be allowed to manipulate files outside of these
    public static List<IPath> AllowedToModify = new();
    
    private string _path;
    public IPath()
    {
        _path = "";
    }
    public IPath(string path)
    {
        _path = "";
        Mutate(path);
    }

    public IPath Mutate(string path, bool allowDirectoryCreation = false)
    {
        ArcDirectory.NormalizePath(path); // So it works on all operating systems

        if (path == "..")
            return GetParent();

        _path = Path.Combine(path, _path);

        if(IsValid()) return this;
        if (!allowDirectoryCreation) throw new ArgumentException($"{_GetFullPath()} as a path does not exist");
        Directory.CreateDirectory(_path);
        return this;
    }
    public IPath this[string path]
    {
        get => Mutate(path);
    }
    
    private bool IsValid()
    {
        string fullPath = _GetFullPath();
        if (Directory.Exists(fullPath) || File.Exists(fullPath))
            return true;
        return false;
    }
    private string _GetFullPath() => Path.Combine(FullDirectory, _path);
    
    // Get
    public IPath GetParent()
    {
        int lastIndex = _path.LastIndexOf(Path.DirectorySeparatorChar);
        if (lastIndex == -1) return this;
        
        _path = _path[..lastIndex];
        return this;
    }
        // GetFiles
        // GetDirectories
    // Modify
        // WriteFile
        // CopyFile
        // DeleteFile
    // Read
        // ReadFile
    // Logic
        // IsPartOf

}