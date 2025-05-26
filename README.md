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

```
🤝 Contributing
We welcome contributions! If you’d like to report a bug, request a feature, or contribute code, please submit an issue or pull request.

```
