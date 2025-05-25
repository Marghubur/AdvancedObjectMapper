using AdvanceMapper.Core;

namespace AdvanceMapper.Interface
{
    /// <summary>
    /// Interface for mapping profile configuration
    /// </summary>
    public interface IMappingProfile
    {
        /// <summary>
        /// Configure mappings for this profile
        /// </summary>
        void Configure(IMapperConfiguration config);
    }

    /// <summary>
    /// Interface for mapper configuration
    /// </summary>
    public interface IMapperConfiguration
    {
        /// <summary>
        /// Create a mapping between source and destination types
        /// </summary>
        TypeMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>();
    }
}
