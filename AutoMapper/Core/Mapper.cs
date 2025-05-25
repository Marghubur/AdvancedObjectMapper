using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using AdvanceMapper.Attribute;
using AdvanceMapper.Interface;

namespace AdvanceMapper.Core
{
    /// <summary>
    /// Fluent configuration for member mapping
    /// </summary>
    public class MemberConfigurationExpression<TSource, TDestination, TMember>
    {
        private readonly TypeMappingExpression<TSource, TDestination> _parent;
        private readonly string _memberName;

        internal MemberConfigurationExpression(TypeMappingExpression<TSource, TDestination> parent, string memberName)
        {
            _parent = parent;
            _memberName = memberName;
        }

        /// <summary>
        /// Map from a source member
        /// </summary>
        public TypeMappingExpression<TSource, TDestination> MapFrom<TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember)
        {
            var memberName = GetMemberName(sourceMember);
            _parent.AddMemberMapping(_memberName, memberName, null);
            return _parent;
        }

        /// <summary>
        /// Map using a custom value resolver
        /// </summary>
        public TypeMappingExpression<TSource, TDestination> MapFrom(Func<TSource, TMember> valueResolver)
        {
            _parent.AddMemberMapping(_memberName, null, obj => valueResolver((TSource)obj));
            return _parent;
        }

        /// <summary>
        /// Ignore this member during mapping
        /// </summary>
        public TypeMappingExpression<TSource, TDestination> Ignore()
        {
            _parent.AddMemberMapping(_memberName, null, null, true);
            return _parent;
        }

        private static string GetMemberName<T>(Expression<Func<TSource, T>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            throw new ArgumentException("Expression must be a member expression");
        }
    }

    /// <summary>
    /// Fluent configuration for type mapping
    /// </summary>
    public class TypeMappingExpression<TSource, TDestination>
    {
        private readonly TypeMapping _typeMapping;

        internal TypeMappingExpression(TypeMapping typeMapping)
        {
            _typeMapping = typeMapping;
        }

        /// <summary>
        /// Configure mapping for a specific member
        /// </summary>
        public MemberConfigurationExpression<TSource, TDestination, TMember> ForMember<TMember>(
            Expression<Func<TDestination, TMember>> destinationMember)
        {
            var memberName = GetMemberName(destinationMember);
            return new MemberConfigurationExpression<TSource, TDestination, TMember>(this, memberName);
        }

        /// <summary>
        /// Configure options for this type mapping
        /// </summary>
        public TypeMappingExpression<TSource, TDestination> WithOptions(Action<MappingOptions> configureOptions)
        {
            configureOptions(_typeMapping.Options);
            return this;
        }

        internal void AddMemberMapping(string destinationMember, string? sourceMember, Func<object, object>? valueResolver, bool ignore = false)
        {
            _typeMapping.MemberMappings[destinationMember] = new MemberMapping
            {
                DestinationMember = destinationMember,
                SourceMember = sourceMember,
                ValueResolver = valueResolver,
                Ignore = ignore
            };
        }

        private static string GetMemberName<T>(Expression<Func<TDestination, T>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            throw new ArgumentException("Expression must be a member expression");
        }
    }

    /// <summary>
    /// Mapper configuration implementation
    /// </summary>
    public class MapperConfiguration : IMapperConfiguration
    {
        internal Dictionary<string, TypeMapping> TypeMappings { get; } = new();

        /// <summary>
        /// Create a mapping between source and destination types
        /// </summary>
        public TypeMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            var key = GetMappingKey(typeof(TSource), typeof(TDestination));

            var typeMapping = new TypeMapping
            {
                SourceType = typeof(TSource),
                DestinationType = typeof(TDestination)
            };

            TypeMappings[key] = typeMapping;
            return new TypeMappingExpression<TSource, TDestination>(typeMapping);
        }

        private static string GetMappingKey(Type sourceType, Type destinationType)
        {
            return $"{sourceType.FullName} -> {destinationType.FullName}";
        }
    }

    public class Mapper : IMapper
    {
        private readonly Dictionary<string, TypeMapping> _typeMappings;
        private readonly Dictionary<Type, PropertyInfo[]> _propertyCache = new();

        public Mapper(MapperConfiguration configuration)
        {
            _typeMappings = configuration.TypeMappings;
        }

        /// <inheritdoc />
        public TDestination Map<TDestination>(object source) where TDestination : new()
        {
            if (source == null) return default!;

            var key = GetMappingKey(source.GetType(), typeof(TDestination));
            var typeMapping = _typeMappings.GetValueOrDefault(key);

            var destination = new TDestination();
            MapProperties(source, destination, typeMapping);
            return destination;
        }

        /// <inheritdoc />
        public TDestination Map<TSource, TDestination>(TSource source) where TDestination : new()
        {
            if (source == null) return default!;

            var destination = new TDestination();
            return Map(source, destination);
        }

        /// <inheritdoc />
        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            if (source == null) return destination;

            var key = GetMappingKey(typeof(TSource), typeof(TDestination));
            var typeMapping = _typeMappings.GetValueOrDefault(key);

            MapProperties(source, destination, typeMapping);
            return destination;
        }

        /// <inheritdoc />
        public List<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source) where TDestination : new()
        {
            if (source == null) return new List<TDestination>();

            return source.Select(item => Map<TSource, TDestination>(item)).ToList();
        }

        // [Rest of the implementation methods remain the same as in the previous version]
        // MapProperties, ConvertValue, ConvertCollection, and helper methods...

        private void MapProperties(object source, object destination, TypeMapping? typeMapping)
        {
            if (source == null || destination == null) return;

            var sourceType = source.GetType();
            var destinationType = destination.GetType();
            var options = typeMapping?.Options ?? new MappingOptions();

            var sourceProperties = GetCachedProperties(sourceType);
            var destinationProperties = GetCachedProperties(destinationType)
                .Where(p => p.CanWrite && !HasIgnoreMapAttribute(p))
                .ToDictionary(p => p.Name, p => p,
                    options.IgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

            foreach (var sourceProp in sourceProperties.Where(p => p.CanRead))
            {
                if (HasIgnoreMapAttribute(sourceProp)) continue;

                var sourceValue = sourceProp.GetValue(source);
                if (!options.MapNullValues && sourceValue == null) continue;

                var destinationPropertyName = sourceProp.Name;
                MemberMapping? memberMapping = null;

                if (typeMapping?.MemberMappings != null)
                {
                    memberMapping = typeMapping.MemberMappings.Values
                        .FirstOrDefault(m => m.SourceMember == sourceProp.Name);

                    if (memberMapping != null)
                    {
                        destinationPropertyName = memberMapping.DestinationMember;
                    }
                    else
                    {
                        typeMapping.MemberMappings.TryGetValue(sourceProp.Name, out memberMapping);
                    }
                }

                if (memberMapping?.Ignore == true) continue;

                var mapToAttribute = sourceProp.GetCustomAttribute<MapToAttribute>();
                if (mapToAttribute != null)
                {
                    destinationPropertyName = mapToAttribute.PropertyName;
                }

                if (options.CustomMappings.TryGetValue(sourceProp.Name, out var customMapping))
                {
                    destinationPropertyName = customMapping;
                }

                if (!destinationProperties.TryGetValue(destinationPropertyName, out var destinationProp))
                    continue;

                try
                {
                    object? valueToSet;

                    if (memberMapping?.ValueResolver != null)
                    {
                        valueToSet = memberMapping.ValueResolver(source);
                    }
                    else
                    {
                        valueToSet = ConvertValue(sourceValue, destinationProp.PropertyType, options);
                    }

                    destinationProp.SetValue(destination, valueToSet);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error mapping property {sourceProp.Name}: {ex.Message}");
                }
            }

            if (typeMapping?.MemberMappings != null)
            {
                foreach (var mapping in typeMapping.MemberMappings.Values)
                {
                    if (mapping.Ignore || mapping.ValueResolver == null) continue;

                    if (destinationProperties.TryGetValue(mapping.DestinationMember, out var destProp))
                    {
                        try
                        {
                            var value = mapping.ValueResolver(source);
                            destProp.SetValue(destination, value);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error applying value resolver for {mapping.DestinationMember}: {ex.Message}");
                        }
                    }
                }
            }
        }

        private object? ConvertValue(object? value, Type targetType, MappingOptions options)
        {
            if (value == null) return GetDefaultValue(targetType);

            var sourceType = value.GetType();

            if (sourceType == targetType || targetType.IsAssignableFrom(sourceType))
                return value;

            var underlyingType = Nullable.GetUnderlyingType(targetType);
            if (underlyingType != null)
            {
                if (value == null) return null;
                return ConvertValue(value, underlyingType, options);
            }

            if (IsCollection(targetType) && IsCollection(sourceType))
            {
                return ConvertCollection(value, targetType, options);
            }

            if (IsComplexType(targetType) && IsComplexType(sourceType))
            {
                if (!options.DeepCopy) return value;

                var destination = Activator.CreateInstance(targetType);
                var key = GetMappingKey(sourceType, targetType);
                var typeMapping = _typeMappings.GetValueOrDefault(key);
                MapProperties(value, destination, typeMapping);
                return destination;
            }

            if (targetType.IsPrimitive || targetType == typeof(string) || targetType == typeof(DateTime) ||
                targetType == typeof(decimal) || targetType == typeof(Guid))
            {
                return Convert.ChangeType(value, targetType);
            }

            if (targetType.IsEnum)
            {
                if (sourceType == typeof(string))
                    return Enum.Parse(targetType, value.ToString()!, true);
                return Enum.ToObject(targetType, value);
            }

            return value;
        }

        private object? ConvertCollection(object sourceCollection, Type targetCollectionType, MappingOptions options)
        {
            if (sourceCollection == null) return null;

            var sourceEnumerable = (IEnumerable)sourceCollection;
            var targetElementType = GetCollectionElementType(targetCollectionType);

            if (targetElementType == null) return sourceCollection;

            var convertedItems = new List<object?>();

            foreach (var item in sourceEnumerable)
            {
                var convertedItem = ConvertValue(item, targetElementType, options);
                convertedItems.Add(convertedItem);
            }

            if (targetCollectionType.IsArray)
            {
                var array = Array.CreateInstance(targetElementType, convertedItems.Count);
                for (int i = 0; i < convertedItems.Count; i++)
                {
                    array.SetValue(convertedItems[i], i);
                }
                return array;
            }

            if (targetCollectionType.IsGenericType)
            {
                var listType = typeof(List<>).MakeGenericType(targetElementType);
                var list = (IList)Activator.CreateInstance(listType)!;

                foreach (var item in convertedItems)
                {
                    list.Add(item);
                }

                return list;
            }

            return convertedItems;
        }

        private static string GetMappingKey(Type sourceType, Type destinationType)
        {
            return $"{sourceType.FullName} -> {destinationType.FullName}";
        }

        private PropertyInfo[] GetCachedProperties(Type type)
        {
            if (_propertyCache.TryGetValue(type, out var properties))
                return properties;

            properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            _propertyCache[type] = properties;
            return properties;
        }

        private static bool HasIgnoreMapAttribute(PropertyInfo property)
        {
            return property.GetCustomAttribute<IgnoreMapAttribute>() != null;
        }

        private static bool IsCollection(Type type)
        {
            return type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
        }

        private static bool IsComplexType(Type type)
        {
            return !type.IsPrimitive && !type.IsEnum && type != typeof(string) &&
                   type != typeof(DateTime) && type != typeof(decimal) && type != typeof(Guid) &&
                   !IsCollection(type);
        }

        private static Type? GetCollectionElementType(Type collectionType)
        {
            if (collectionType.IsArray)
                return collectionType.GetElementType();

            if (collectionType.IsGenericType)
            {
                var genericArgs = collectionType.GetGenericArguments();
                if (genericArgs.Length > 0)
                    return genericArgs[0];
            }

            return null;
        }

        private static object? GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
