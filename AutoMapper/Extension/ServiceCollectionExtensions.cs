using AdvanceMapper.Core;
using AdvanceMapper.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace AdvanceMapper.Extension
{
    /// <summary>
    /// Extension methods for IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add AdvancedObjectMapper to the service collection
        /// </summary>
        public static IServiceCollection AddAdvancedObjectMapper(this IServiceCollection services,
            Action<IMapperConfiguration> configureMapping)
        {
            var configuration = new MapperConfiguration();
            configureMapping(configuration);

            services.AddSingleton(configuration);
            services.AddSingleton<IMapper, Mapper>();

            return services;
        }

        /// <summary>
        /// Add AdvancedObjectMapper with profiles to the service collection
        /// </summary>
        public static IServiceCollection AddAdvancedObjectMapper(this IServiceCollection services,
            params Type[] profileTypes)
        {
            var configuration = new MapperConfiguration();

            foreach (var profileType in profileTypes)
            {
                if (!typeof(IMappingProfile).IsAssignableFrom(profileType))
                    throw new ArgumentException($"Type {profileType.Name} must implement {nameof(IMappingProfile)}");

                var profile = (IMappingProfile)Activator.CreateInstance(profileType)!;
                profile.Configure(configuration);
            }

            services.AddSingleton(configuration);
            services.AddSingleton<IMapper, Mapper>();

            return services;
        }

        /// <summary>
        /// Add AdvancedObjectMapper with profile instances to the service collection
        /// </summary>
        public static IServiceCollection AddAdvancedObjectMapper(this IServiceCollection services,
            params IMappingProfile[] profiles)
        {
            var configuration = new MapperConfiguration();

            foreach (var profile in profiles)
            {
                profile.Configure(configuration);
            }

            services.AddSingleton(configuration);
            services.AddSingleton<IMapper, Mapper>();

            return services;
        }
    }
}
