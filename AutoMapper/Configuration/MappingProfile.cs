using AdvanceMapper.Interface;

namespace AdvanceMapper.Configuration
{
    /// <summary>
    /// Base class for mapping profiles
    /// </summary>
    public abstract class MappingProfile : IMappingProfile
    {
        /// <summary>
        /// Configure mappings for this profile
        /// </summary>
        public abstract void Configure(IMapperConfiguration config);
    }
}
