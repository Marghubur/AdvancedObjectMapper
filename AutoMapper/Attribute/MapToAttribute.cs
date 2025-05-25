namespace AdvanceMapper.Attribute
{
    /// <summary>
    /// Attribute to specify custom property mapping
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MapToAttribute : System.Attribute
    {
        /// <summary>
        /// Target property name to map to
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Initialize with target property name
        /// </summary>
        public MapToAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
