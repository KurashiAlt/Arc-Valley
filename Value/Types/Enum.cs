namespace Arc;

/// <summary>
/// Represents an enumeration type in Arc with a set of possible values.
/// </summary>
public class ArcEnum : ArcType
{
    private readonly List<string> possibleValues;
    private readonly string? id;

    /// <summary>
    /// Creates an instance of <see cref="ArcEnum"/> with the given block of possible values.
    /// <list type="bullet">
    /// <item><param name="block">A block containing possible values for the enum.</param></item>
    /// </list>
    /// </summary>
    /// <returns>An instance of <see cref="ArcEnum"/>.</returns>
    public ArcEnum(string? id, Block block)
    {
        block.RemoveEnclosingBlock();

        possibleValues = block.Select(w => (string)w).ToList();
        this.id = id;

        InstanceConstructor = (Block valueBlock) =>
        {
            var value = valueBlock.ToString();
            int index = possibleValues.IndexOf(value);

            if (index != -1)
            {
                return new Instance(this, index);
            }

            throw ArcException.Create($"ArcEnum.Instance is not a possible value for the specified ArcEnum: '{value}'", valueBlock);
        };

        if (id != null)
        {
            ArcType.Types.Add(id, new(InstanceConstructor));
        }
    }

    /// <summary>
    /// Calling the type in the object declaration scope.
    /// <list type="bullet">
    /// <item><param name="walker">The walker used for traversing.</param></item>
    /// </list>
    /// </summary>
    /// <returns>The updated <see cref="Walker"/>.</returns>
    public static Walker Call(Walker i)
    {
        i.ForceMoveNext();

        string id = Compiler.GetId(i.Current);

        i = Args.GetArgs(i, out Args args);

        _ = new ArcEnum(id, args.block);

        return i;
    }

    /// <summary>
    /// Represents an instance of an <see cref="ArcEnum"/> with a specific value.
    /// </summary>
    public class Instance : IVariable, IArcObject
    {
        private readonly ArcEnum arcEnum;
        private readonly int valueIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="Instance"/> class.
        /// <list type="bullet">
        /// <item><param name="arcEnum">The <see cref="ArcEnum"/> that this instance belongs to.</param></item>
        /// <item><param name="valueIndex">The index of the value in the possible values list.</param></item>
        /// </list>
        /// </summary>
        public Instance(ArcEnum arcEnum, int valueIndex)
        {
            this.arcEnum = arcEnum;
            this.valueIndex = valueIndex;
        }
        /// <summary>
        /// Returns the string representation of the enum instance.
        /// </summary>
        /// <returns>The string representation of the enum value.</returns>
        public override string ToString()
        {
            return arcEnum.possibleValues[valueIndex];
        }
        /// <summary>
        /// Calls this instance in a code scope.
        /// <list type="bullet">
        /// <item><param name="walker">The walker used for traversing.</param></item>
        /// <item><param name="result">The block where the result will be added.</param></item>
        /// </list>
        /// </summary>
        /// <returns>The updated <see cref="Walker"/>.</returns>
        public Walker Call(Walker walker, ref Block result)
        {
            result.Add(ToString());
            return walker;
        }
        /// <summary>
        /// Calls this instance in a logical scope
        /// <list type="bullet">
        /// <item><param name="walker">The walker used for traversing.</param></item>
        /// </list>
        /// </summary>
        /// <returns>Whether this LogicalCall passes or not.</returns>
        public bool LogicalCall(ref Walker walker)
        {
            return new ArcString(ToString()).LogicalCall(ref walker);
        }
        /// <summary>
        /// Gets a property or method associated with this instance.
        /// <list type="bullet">
        /// <item><param name="property">The name of the property or method to retrieve.</param></item>
        /// </list>
        /// </summary>
        /// <returns>The corresponding <see cref="IVariable"/> if the property exists; otherwise, <c>null</c>.</returns>
        /// <exception cref="NotImplementedException">Thrown when the property is not implemented.</exception>
        public IVariable? Get(string property)
        {
            return property switch
            {
                "to_int" => new ArcInt(valueIndex),
                _ => throw new NotImplementedException($"Property '{property}' is not implemented."),
            };
        }
        /// <summary>
        /// Determines whether the specified property can be retrieved.
        /// <list type="bullet">
        /// <item><param name="property">The name of the property to check.</param></item>
        /// </list>
        /// </summary>
        /// <returns><c>true</c> if the property can be retrieved; otherwise, <c>false</c>.</returns>
        public bool CanGet(string property)
        {
            return property switch
            {
                "to_int" => true,
                _ => false,
            };
        }
    }
}