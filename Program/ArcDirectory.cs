using Microsoft.VisualBasic.FileIO;
using Pastel;
using System.Collections;
using System.IO;

namespace Arc;
public class VDirType : IEnumerable<string>
{
    List<string> files = new();
    public void Add(string path)
    {
        path = path.Replace("\\", "/");
        files.Add(path);
    }
    public bool Contains(string path) => files.Contains(path);
    public IEnumerator<string> GetEnumerator() => files.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public int Count => files.Count;

}
public static class ArcDirectory
{
    public static string directory = AppDomain.CurrentDomain.BaseDirectory;
    public static VDirType ScriptVDir = new();
    public static VDirType UnsortedVDir = new();
    public static VDirType MapVDir = new();
    public static VDirType GfxVDir = new();
    public static VDirType ExtraFiles = new();
    public static void CheckFolder(string path)
    {
        if (Path.GetRelativePath(directory, path).Replace('\\', '/').StartsWith($"{Program.TranspileTarget}/.")) return;

        string[] subPaths = GetDirectories(path);
        foreach (string subPath in subPaths)
        {
            CheckFolder(subPath);
        }

        string[] files = GetFiles(path);
        foreach (string file in files)
        {
            string aPath = Path.GetRelativePath(directory, file).Replace('\\', '/');

            if (ScriptVDir.Contains(aPath)) continue;
            if (UnsortedVDir.Contains(aPath)) continue;
            if (MapVDir.Contains(aPath)) continue;
            if (GfxVDir.Contains(aPath)) continue;
            
            ExtraFiles.Add(aPath);
        }
    }
    public static void TryDelete(string path)
    {
        if (File.Exists(Path.Combine(directory, path))) File.Delete(Path.Combine(directory, path));
    }
    public static void Copy(string origin, string destination)
    {
        origin = Path.Combine(directory, origin);
        destination = Path.Combine(directory, destination);

        File.Copy(origin, destination, true);
    }
    public static void VDirPopulate(string[] args)
    {
        string[] VDirCache;
        
        if (args.Contains("script") || args.Contains("all")) Program.OverwriteFile(".arc/script.vdir", string.Join('\n', ScriptVDir), false);
        else
        {
            VDirCache = GetFile(".arc/script.vdir");
            foreach(string vDir in VDirCache) 
            {
                if (!ScriptVDir.Contains(vDir)) ScriptVDir.Add(vDir);
            }
        }

        if (args.Contains("unsorted") || args.Contains("all")) Program.OverwriteFile(".arc/unsorted.vdir", string.Join('\n', UnsortedVDir), false);
        else
        {
            VDirCache = GetFile(".arc/unsorted.vdir");
            foreach(string vDir in VDirCache) 
            {
                if (!UnsortedVDir.Contains(vDir)) UnsortedVDir.Add(vDir);
            }
        }

        if (args.Contains("map") || args.Contains("all")) Program.OverwriteFile(".arc/map.vdir", string.Join('\n', MapVDir), false);
        else
        {
            VDirCache = GetFile(".arc/map.vdir");
            foreach(string vDir in VDirCache) 
            {
                if (!MapVDir.Contains(vDir)) MapVDir.Add(vDir);
            }
        }

        if (args.Contains("gfx") || args.Contains("all")) Program.OverwriteFile(".arc/gfx.vdir", string.Join('\n', GfxVDir), false);
        else
        {
            VDirCache = GetFile(".arc/gfx.vdir");
            foreach(string vDir in VDirCache) 
            {
                if (!GfxVDir.Contains(vDir)) GfxVDir.Add(vDir);
            }
        }
    }
    public static string[] GetDirectories(string path)
    {
        if (!path.StartsWith(directory)) path = Path.Combine(directory, path);

        return Directory.GetDirectories(path).OrderBy(d => d).ToArray();
    }
    public static string[] GetFiles(string path, bool includeSubdirectories = false)
    {
        if (!Path.Exists(Path.Combine(directory, path))) return new string[] { };
        if (!path.StartsWith(directory)) path = Path.Combine(directory, path);

        if (includeSubdirectories) return Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories).OrderBy(d => d).ToArray();

        return Directory.GetFiles(path).OrderBy(d => d).ToArray();
    }
    public static string[] GetFile(string path)
    {
        if (!Path.Exists(Path.Combine(directory, path))) return new string[] { };
        if (!path.StartsWith(directory)) path = Path.Combine(directory, path);

        return File.ReadAllLines(path);
    }
    public static IEnumerable<string> GetFolders(string path)
    {
        string location = Path.Combine(directory, path);

        return from s in GetDirectories(location) select Path.GetRelativePath(directory, s);
    }

}