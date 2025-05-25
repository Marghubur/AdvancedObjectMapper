namespace AdvanceMapper.Core
{
    /// <summary>
    /// Configuration for type-to-type mapping
    /// </summary>
    public class TypeMapping
    {
        /// <summary>
        /// Source type
        /// </summary>
        public Type SourceType { get; set; } = null!;

        /// <summary>
        /// Destination type
        /// </summary>
        public Type DestinationType { get; set; } = null!;

        /// <summary>
        /// Member-specific mappings
        /// </summary>
        public Dictionary<string, MemberMapping> MemberMappings { get; set; } = new();

        /// <summary>
        /// Mapping options for this type mapping
        /// </summary>
        public MappingOptions Options { get; set; } = new();
    }
}
