```markdown
# AdvancedObjectMapper

A powerful, AutoMapper-like object mapping library for .NET 8+ applications with fluent configuration and dependency injection support.


## Features

- ✅ Fluent Configuration API
- ✅ Dependency Injection Support
- ✅ Profile-based Configuration
- ✅ Custom Value Resolvers
- ✅ Deep Object Mapping
- ✅ Collection Mapping
- ✅ Attribute-based Mapping
- ✅ Type Safety with IntelliSense


## Installation

```bash
Install-Package AdvancedObjectMapper
```

## 🚀 Quick Start

### 1. Create Mapping Profile
```csharp
public class UserMappingProfile : MappingProfile
{
    public override void Configure(IMapperConfiguration config)
    {
        config.CreateMap<Employee, User>()
            .ForMember(u => u.UserId, opt => opt.MapFrom(e => e.EmployeeId))
            .ForMember(u => u.FullName, opt => opt.MapFrom(e => $"{e.FirstName} {e.LastName}"));
    }
}
```

### 2. Register in Program.cs
```csharp
builder.Services.AddAdvancedObjectMapper(typeof(UserMappingProfile));
```

### 3. Use in Controllers/Services
```csharp
public class UserService
{
    private readonly IMapper _mapper;

    public UserService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public User GetUser(Employee employee)
    {
        return _mapper.Map<Employee, User>(employee);
    }
}
```

```
## 🧩 Supported Mapping Scenarios

| Feature                   | Supported      |
| ------------------------- | -------------- |
| Basic Property Mapping    | ✅              |
| Property Name Overrides   | ✅              |
| Nested Object Mapping     | ✅              |
| List & Collection Mapping | ✅              |
| Custom Value Resolvers    | ✅              |
| Reverse Mapping           | 🚧 Coming Soon |
| Attributes-Based Mapping  | 🚧 Coming Soon |
```

```
## 📁 Project Structure Suggestion

/YourApp
│
├── MappingProfiles/
│   └── UserMappingProfile.cs
│
├── Services/
│   └── UserService.cs
│
├── Models/
│   ├── Employee.cs
│   └── User.cs

```

## License

MIT License
```

This structure provides:

1. **Clean Library Architecture** - Proper separation of concerns
2. **NuGet Package Ready** - Complete `.csproj` with package metadata
3. **Dependency Injection Integration** - Full .NET Core 8 support
4. **Profile-based Configuration** - Clean, organized mapping setup
5. **Type Safety** - Full IntelliSense and compile-time checking
6. **Easy Registration** - Simple `Program.cs` setup
7. **Production Ready** - Proper interfaces, error handling, and documentation

To create the NuGet package, run:
```bash
dotnet pack --configuration Release
```

```
🤝 Contributing
We welcome contributions! If you’d like to report a bug, request a feature, or contribute code, please submit an issue or pull request.

```

The package will be ready for publishing to NuGet.org or your private feed!