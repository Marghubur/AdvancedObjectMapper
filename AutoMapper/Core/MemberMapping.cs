namespace AdvanceMapper.Core
{
    /// <summary>
    /// Configuration for individual member mapping
    /// </summary>
    public class MemberMapping
    {
        /// <summary>
        /// Destination member name
        /// </summary>
        public string DestinationMember { get; set; } = string.Empty;

        /// <summary>
        /// Source member name
        /// </summary>
        public string? SourceMember { get; set; }

        /// <summary>
        /// Custom value resolver function
        /// </summary>
        public Func<object, object>? ValueResolver { get; set; }

        /// <summary>
        /// Whether to ignore this member
        /// </summary>
        public bool Ignore { get; set; }
    }
}
