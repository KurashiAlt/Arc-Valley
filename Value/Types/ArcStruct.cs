using Arc;
using System.Diagnostics;
using System.Runtime.Serialization;

public class ArcStruct : ArcType
{
    public static Dict<ArcStruct> Structs = new();

    //Properties
    Dictionary<Word, (ArcType type, Block? def)> Structure = new();
    ArcString? Id;
    public ArcStruct(string? id, Args args)
    {
        if (id != null)
        {
            Id = new(id);
            Structs.Add(id, this);
        }

        ParseStructure(args);

        ThisConstructor = (Block block) =>
        {
            if (!Parser.HasEnclosingBrackets(block))
            {
                block.AddFirst(new Word("{", block.First.Value));
                block.AddLast(new Word("}", block.Last.Value));
            }

            Args arguments = Args.GetArgs(block);
            ArcObject obj = new();

            foreach (var kvp in Structure)
            {
                AddArgumentToObject(arguments, obj, kvp);
            }

            return obj;
        };

        RegisterType(id);
    }

    /// <summary>
    /// Parses the structure from the provided arguments and populates the Structure dictionary.
    /// </summary>
    /// <param name="args">Arguments containing the block to parse.</param>
    private void ParseStructure(Args args)
    {
        Walker walker = new(args.block);
        do
        {
            Block type = walker.GetScope();
            walker.ForceMoveNext();

            if (walker == "=")
            {
                // This means it 's the old structure, meaning Type is the name and the next word is the Type.
                walker.ForceMoveNext();

                Block realType = walker.GetScope();
                Structure.Add(type.ToString(), (ArcType.Constructor(realType), null));
                // In the old structure you can't define default values here
            }
            else
            {
                Word name = walker.Current;
                Block? defaultValue = GetDefaultValue(walker);

                Structure.Add(name, (ArcType.Constructor(type), defaultValue));
            }
        } while (walker.MoveNext());
    }

    /// <summary>
    /// Determines the default value for the current element in the walker if an "=" sign is present.
    /// </summary>
    /// <param name="walker">The walker object iterating through the arguments block.</param>
    /// <returns>The default value block if "=" is present; otherwise, null.</returns>
    private Block? GetDefaultValue(Walker walker)
    {
        if (!walker.MoveNext())
            return null;

        if (walker.Current == "=")
        {
            walker.ForceMoveNext();
            return walker.GetScope();
        }

        walker.MoveBack();
        return null;
    }

    /// <summary>
    /// Registers the type in the ArcType.Types dictionary with a constructor that creates an ArcObject based on the provided block.
    /// </summary>
    /// <param name="id">The identifier for the type being registered.</param>
    private void RegisterType(string? id)
    {
        if (id != null)
        {
            ArcType.Types.Add(id, new(ThisConstructor));
        }
    }

    /// <summary>
    /// Adds an argument to the ArcObject based on the key-value pair from the Structure dictionary and the provided arguments.
    /// </summary>
    /// <param name="arguments">Arguments containing the values to be added to the ArcObject.</param>
    /// <param name="obj">The ArcObject to which the argument is added.</param>
    /// <param name="kvp">The key-value pair representing the structure's element.</param>
    private void AddArgumentToObject(Args arguments, ArcObject obj, KeyValuePair<Word, (ArcType type, Block? def)> kvp)
    {
        Block? arg = arguments.GetNullable(kvp.Key);

        if (arg == null)
        {
            HandleMissingArgument(obj, kvp);
            return;
        }

        IVariable? value;
        if (!Compiler.TryGetVariable(arg.ToWord(), out value))
        {
            if (arg.ToString().Contains("grain")) Debugger.Break();
            value = kvp.Value.type.ThisConstructor(arg);
        }

        obj.Add(kvp.Key, value);
    }

    /// <summary>
    /// Handles the case when an argument is missing by either assigning a default value or throwing an exception if required.
    /// </summary>
    /// <param name="obj">The ArcObject to which the default value is added if available.</param>
    /// <param name="kvp">The key-value pair representing the structure's element.</param>
    /// <exception cref="ArcException">Thrown if a required argument is missing and no default value is provided.</exception>
    private void HandleMissingArgument(ArcObject obj, KeyValuePair<Word, (ArcType type, Block? def)> kvp)
    {
        bool isNullable = kvp.Value.type.Nullable;
        Block? defaultValue = kvp.Value.def;

        if (defaultValue != null)
        {
            IVariable? value = kvp.Value.type.ThisConstructor(defaultValue);
            obj.Add(kvp.Key, value);
            return;
        }

        if (!isNullable)
        {
            throw ArcException.Create(
                $"Missing {kvp.Key} for struct", obj, isNullable, defaultValue, kvp.Value.type
            );
        }
    }

    public static Walker Call(Walker i)
    {
        i.ForceMoveNext();

        string id = Compiler.GetId(i.Current);

        i = Args.GetArgs(i, out Args args);

        _ = new ArcStruct(id, args);

        return i;
    }
    public IVariable? Get(string indexer) => indexer == "id" ? Id : null;
    public bool CanGet(string indexer) => indexer == "id" && Id != null;
}
