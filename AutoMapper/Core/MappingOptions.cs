namespace AdvanceMapper.Core
{
    /// <summary>
    /// Configuration options for mapping behavior
    /// </summary>
    public class MappingOptions
    {
        /// <summary>
        /// Whether to ignore case when matching property names
        /// </summary>
        public bool IgnoreCase { get; set; } = true;

        /// <summary>
        /// Whether to map null values
        /// </summary>
        public bool MapNullValues { get; set; } = true;

        /// <summary>
        /// Whether to perform deep copy of complex objects
        /// </summary>
        public bool DeepCopy { get; set; } = true;

        /// <summary>
        /// Custom property name mappings
        /// </summary>
        public Dictionary<string, string> CustomMappings { get; set; } = new();
    }
}
