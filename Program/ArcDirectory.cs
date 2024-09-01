using Pastel;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Arc;
/// <summary>
/// Represents a virtual directory that stores a list of file paths.
/// </summary>
public class VDirType : IEnumerable<string>
{
    readonly List<string> files = new();
    public void Add(ArcPath path)
    {
        files.Add(path);
    }
    public bool Contains(string path) => files.Contains(path);
    public IEnumerator<string> GetEnumerator() => files.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public int Count => files.Count;
}

/// <summary>
/// An abstraction layer that makes paths always use '/' intead of '\'
/// </summary>
public class ArcPath
{
    public string value;
    public ArcPath(string path)
    {
        value = path.NormalizePath();
    }
    public static implicit operator string(ArcPath path) => path.value;
    public static implicit operator ArcPath(string path) => new(path);
}

/// <summary>
/// Provides methods for directory management and file operations, including population and cleanup.
/// </summary>
public static class ArcDirectory
{
    /// <summary>
    /// Base directory of the application.
    /// </summary>
    public static string directory = AppDomain.CurrentDomain.BaseDirectory;

    /// <summary>
    /// Virtual directories to manage different types of files.
    /// </summary>
    public static readonly (string Name, VDirType VDir)[] VDirs = 
    {
        ("script", new()),
        ("unsorted", new()),
        ("map", new()),
        ("gfx", new()),
        ("extra", new()),
    };

    /// <summary>
    /// Properties to access specific virtual directories by name.
    /// </summary>
    public static VDirType ScriptVDir => VDirs[0].VDir;
    public static VDirType UnsortedVDir => VDirs[1].VDir;
    public static VDirType MapVDir => VDirs[2].VDir;
    public static VDirType GfxVDir => VDirs[3].VDir;
    public static VDirType ExtraFiles => VDirs[4].VDir;

    /// <summary>
    /// Normalizes path separators to use forward slashes.
    /// </summary>
    /// <param name="path">The path to normalize.</param>
    /// <returns>The normalized path.</returns>
    public static string NormalizePath(this string path)
    {
        path = path.Replace('\\', '/');
        return path;
    }

    /// <summary>
    /// Creates all required subdirectories up to the specified directory path.
    /// </summary>
    /// <param name="path">The relative path for which to create directories.</param>
    public static void CreateTillDirectory(ArcPath path)
    {
        string? DirPath = Path.GetDirectoryName(path);
        if (DirPath == null) return;

        if (!Directory.Exists(DirPath))
        {
            Console.WriteLine($"\tCreating {Path.GetRelativePath(directory, DirPath)}".Pastel(ConsoleColor.Magenta));
            Directory.CreateDirectory(DirPath);
        }
    }

    /// <summary>
    /// Tries to delete a file at the specified relative path.
    /// </summary>
    /// <param name="path">Relative path of the file to delete.</param>
    public static void TryDelete(ArcPath path)
    {
        ArcPath fullPath = Path.Combine(directory, path);
        if (File.Exists(fullPath)) File.Delete(fullPath);
    }

    /// <summary>
    /// Copies a file from origin to destination within the base directory.
    /// </summary>
    /// <param name="origin">Relative origin path.</param>
    /// <param name="destination">Relative destination path.</param>
    public static void Copy(ArcPath origin, ArcPath destination)
    {
        origin = Path.Combine(directory, origin);
        destination = Path.Combine(directory, destination);

        CreateTillDirectory(destination);

        File.Copy(origin, destination, true);
    }

    /// <summary>
    /// Overwrites or creates a file with the specified text, applying formatting and BOM as needed.
    /// </summary>
    /// <param name="path">The relative path of the file to overwrite or create.</param>
    /// <param name="text">The content to write to the file.</param>
    /// <param name="allowFormatting">Indicates whether to apply formatting based on the Format flag.</param>
    /// <param name="addBOM">Indicates whether to add a Byte Order Mark (BOM) to the file.</param>
    /// <param name="vdirOverride">An optional virtual directory to use instead of the default ScriptVDir.</param>
    /// <param name="forceFormatting">Indicates whether to forcefully apply formatting regardless of the allowFormatting flag.</param>
    public static void OverwriteFile(
        ArcPath path,
        string text,
        bool allowFormatting = true,
        bool addBOM = false,
        VDirType? vdirOverride = null,
        bool forceFormatting = false
    ) {
        // Add the path to the appropriate virtual directory
        if (vdirOverride == null)
            ArcDirectory.ScriptVDir.Add(path);
        else
            vdirOverride.Add(path);

        // Replace placeholder blocks in the text
        text = Program.ReplaceBlocks(text);
        text = text
            .Replace("__ARC.FORCE_END_LINE__", "\n")
            .Replace("__ARC.OPEN_BRACKET__", "{")
            .Replace("__ARC.CLOSE_BRACKET__", "}");

        // Add BOM if specified
        if (addBOM)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] bomBytes = Encoding.UTF8.GetPreamble();
            byte[] resultBytes = bomBytes.Concat(textBytes).ToArray();
            text = Encoding.UTF8.GetString(resultBytes);
        }

        // Construct the full path and format the text if needed
        ArcPath originalPath = path;
        path = Path.Combine(ArcDirectory.directory, path);

        // Ensure the directory exists before writing the file
        ArcDirectory.CreateTillDirectory(originalPath);

        if (allowFormatting && Program.Format || forceFormatting)
        {
            try
            {
                text = Parser.FormatCode(text);
            }
            catch
            {
                Console.WriteLine($"Error formatting code for file: {originalPath}");
            }
        }

        // Write the text to the file if it has changed or if it does not exist
        if (File.Exists(path))
        {
            string existingText = File.ReadAllText(path);
            if (text != existingText)
            {
                File.WriteAllText(path, text);
            }
        }
        else
        {
            File.WriteAllText(path, text);
        }
    }

    /// <summary>
    /// Gets the directories at the specified path.
    /// </summary>
    /// <param name="path">Relative path to search for directories.</param>
    /// <returns>Array of directory paths.</returns>
    public static ArcPath[] GetDirectories(ArcPath path)
    {
        if (!path.value.StartsWith(directory)) path = Path.Combine(directory, path);

        return Directory.GetDirectories(path).OrderBy(d => d).Select(s => new ArcPath(s)).ToArray();
    }

    /// <summary>
    /// Combines the two paths.
    /// </summary>
    public static string Combine(ArcPath origin, ArcPath destination)
    {
        ArcPath path = Path.Combine(origin, destination);
        return path;
    }

    /// <summary>
    /// Gets relative path between two paths.
    /// </summary>
    public static string Relative(ArcPath origin, ArcPath destination)
    {
        ArcPath path = Path.GetRelativePath(origin, destination);
        return path;
    }

    /// <summary>
    /// Gets files in a directory, optionally including subdirectories.
    /// </summary>
    /// <param name="path">Relative path to search for files.</param>
    /// <param name="includeSubdirectories">Whether to include files in subdirectories.</param>
    /// <returns>Array of file paths.</returns>
    public static ArcPath[] GetFiles(ArcPath path, bool includeSubdirectories = false)
    {
        ArcPath fullPath = Path.Combine(directory, path);
        if (!Directory.Exists(fullPath)) return Array.Empty<ArcPath>();

        if (!path.value.StartsWith(directory)) path = Path.Combine(directory, path);

        if (includeSubdirectories)
            return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).OrderBy(d => d).Select(s => new ArcPath(s)).ToArray();

        return Directory.GetFiles(path).OrderBy(d => d).Select(s => new ArcPath(s)).ToArray();
    }

    /// <summary>
    /// Reads all lines from a file at the specified relative path.
    /// </summary>
    /// <param name="path">Relative path of the file to read.</param>
    /// <returns>Array of lines read from the file.</returns>
    public static string[] GetFile(ArcPath path)
    {
        ArcPath fullPath = path;

        if (!fullPath.value.StartsWith(directory)) fullPath = Path.Combine(directory, path);

        if (!File.Exists(fullPath)) return Array.Empty<string>();
        
        return File.ReadAllLines(fullPath);
    }

    /// <summary>
    /// Gets the relative paths of folders within the specified path.
    /// </summary>
    /// <param name="path">Relative path to search for folders.</param>
    /// <returns>Array of relative folder paths.</returns>
    public static ArcPath[] GetFolders(ArcPath path)
    {
        ArcPath location = Path.Combine(directory, path);
        return GetDirectories(location).Select(s => new ArcPath(Path.GetRelativePath(directory, s))).ToArray();
    }

    /// <summary>
    /// Recursively checks folders for files that are not part of predefined virtual directories.
    /// </summary>
    /// <param name="path">The root path to start checking.</param>
    public static void CheckFolderForUncategorizedFiles(ArcPath path)
    {
        // Skip if the path is inside a hidden or excluded folder.
        if (path.value.Split('/').Last().StartsWith('.')) return;

        ArcPath[] subPaths = GetDirectories(path);
        foreach (ArcPath subPath in subPaths)
        {
            CheckFolderForUncategorizedFiles(subPath);
        }

        ArcPath[] files = GetFiles(path);
        foreach (ArcPath file in files)
        {
            ArcPath aPath = Path.GetRelativePath(directory, file);
            if (VDirs.Any(vd => vd.VDir.Contains(aPath))) continue;

            // Add to ExtraFiles if not categorized.
            ExtraFiles.Add(aPath);
        }
    }
    
    /// <summary>
    /// Populates virtual directories from cache files or updates them based on arguments.
    /// </summary>
    /// <param name="args">Command line arguments specifying which directories to update.</param>
    public static void VDirPopulate(string[] args)
    {
        foreach (var (name, vDir) in VDirs)
        {
            if (args.Contains(name) || args.Contains("all"))
            {
                Program.OverwriteFile($".arc/{name}.vdir", string.Join('\n', vDir), false);
            }
            else
            {
                string[] VDirCache = GetFile($".arc/{name}.vdir");
                foreach (ArcPath vDirPath in VDirCache)
                {
                    if (!vDir.Contains(vDirPath)) vDir.Add(vDirPath);
                }
            }
        }
    }

}