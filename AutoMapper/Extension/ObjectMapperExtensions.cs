using AdvanceMapper.Interface;

namespace AdvanceMapper.Extension
{
    /// <summary>
    /// Extension methods for easier object mapping
    /// </summary>
    public static class ObjectMapperExtensions
    {
        /// <summary>
        /// Map an object to the specified destination type
        /// </summary>
        public static TDestination MapTo<TDestination>(this object source, IMapper mapper)
            where TDestination : new()
        {
            return mapper.Map<TDestination>(source);
        }

        /// <summary>
        /// Map a collection to the specified destination type
        /// </summary>
        public static List<TDestination> MapToList<TSource, TDestination>(this IEnumerable<TSource> source, IMapper mapper)
            where TDestination : new()
        {
            return mapper.Map<TSource, TDestination>(source);
        }
    }
}
