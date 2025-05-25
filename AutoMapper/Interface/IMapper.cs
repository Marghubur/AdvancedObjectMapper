namespace AdvanceMapper.Interface
{
    /// <summary>
    /// Interface for the advanced object mapper
    /// </summary>
    public interface IMapper
    {
        /// <summary>
        /// Maps an object to the specified destination type
        /// </summary>
        TDestination Map<TDestination>(object source) where TDestination : new();

        /// <summary>
        /// Maps an object of the specified source type to the destination type
        /// </summary>
        TDestination Map<TSource, TDestination>(TSource source) where TDestination : new();

        /// <summary>
        /// Maps to an existing destination object
        /// </summary>
        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);

        /// <summary>
        /// Maps a collection of objects
        /// </summary>
        List<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source) where TDestination : new();
    }
}
