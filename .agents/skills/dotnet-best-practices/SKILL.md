---
description: Ensure .NET/C# code meets best practices for the solution/project.
name: dotnet-best-practices
---
# .NET/C# Best Practices

C# coding guidelines and best practices. MUST follow these rules. Use when reviewing/writing C# code or dotnet tasks.

## Documentation & Structures

- Create comprehensive XML documentation comments for all public classes, interfaces, methods, and properties using the csharp-docs skill.
- Include parameter descriptions and return value descriptions in XML comments
- Follow the established namespace structure: {Core|Console|App|Service}.{Feature}

## Design Patterns & Architecture

- Use primary constructor syntax for dependency injection (e.g., `public class MyClass(IDependency dependency)`)
- Implement the Command Handler pattern with generic base classes (e.g., `CommandHandler<TOptions>`)
- Use interface segregation with clear naming conventions (prefix interfaces with 'I')
- Follow the Factory pattern for complex object creation.

## Syntax & Language Features

- Prefer Records for DTOs and messages to ensure immutability by default.
- NEVER mix multiple classes or DTOs in a single file, even if they are small. Filename ALWAYS matches the class or DTO name, for example UserDto.cs for a UserDto class.
- Use record structs and init accessors so that you can clone objects using the with keyword rather than tracking state mutations.
- Leverage Pattern Matching with switch expressions for cleaner logic flow.
- Utilize Collection Expressions and Span<T> for high-performance data handling.
- Use expression bodies (arrow syntax) for methods, properties, and constructors when the logic is a simple transformation.
- For conditional assignments, replace bulky, nested if-else blocks or legacy switch statements with switch expressions.
- Use [ and ] for initializing arrays, lists, and spans. This offers a uniform, clean syntax across all collection types and eliminates redundant type declarations.
- One Class Per File: Ensure a strict structure with matching filenames.
- Use var for local variables when type is obvious.
- ALWAYS use sealed accessor for classes or records if not intended for inheritance.
- ALWAYS use internal accessor for internal classes or records.
- Don't use Regions in very small code files.
- Use Regions in large code files to group public properties.
- Use Regions in large code files to group event logic together.

## Dependency Injection & Services

- Use constructor dependency injection with null checks via ArgumentNullException
- Register services with appropriate lifetimes (Singleton, Scoped, Transient)
- Use Microsoft.Extensions.DependencyInjection patterns
- Implement service interfaces for testability

## Resource Management & Localization

- Use ResourceManager for localized messages and error strings
- Separate LogMessages and ErrorMessages resource files
- Access resources via `_resourceManager.GetString("MessageKey")`

## Async/Await Patterns

- Use async/await for all I/O operations and long-running tasks
- Return Task or Task<T> from async methods
- Use ConfigureAwait(false) where appropriate
- Handle async exceptions properly

## Testing Standards

- Use XUnit framework for unit tests.
- Follow AAA pattern (Arrange, Act, Assert)
- Use Moq for mocking dependencies
- Test both success and failure scenarios
- Include null parameter validation tests

## Configuration & Settings

- Use strongly-typed configuration classes with data annotations
- Implement validation attributes (Required, NotEmptyOrWhitespace)
- Use IConfiguration binding for settings
- Support appsettings.json configuration files

## Semantic Kernel & AI Integration

- Use Microsoft.SemanticKernel for AI operations
- Implement proper kernel configuration and service registration
- Handle AI model settings (ChatCompletion, Embedding, etc.)
- Use structured output patterns for reliable AI responses

## Error Handling & Logging

- Use structured logging with Microsoft.Extensions.Logging or NLog
- Prefer NLog if it is already used
- ALWAYS Structured logging to log, no string interpolation and no other logging methods.
- Include scoped logging with meaningful context
- Throw specific exceptions with descriptive messages
- Use try-catch blocks for expected failure scenarios
- Use early exists with ArgumentNullException.ThrowIfNull(argument) instead of custom if (argument == null) checks.

## Performance & Security

- Use C# 12+ features and .NET 8+ optimizations where applicable
- Implement proper input validation and sanitization
- Use parameterized queries for database operations
- Follow secure coding practices for AI/ML operations

## Code Quality

- Ensure SOLID principles compliance
- Avoid code duplication through base classes and utilities
- Use meaningful names that reflect domain concepts
- Keep methods focused and cohesive
- Implement proper disposal patterns for resources

## WPF

- UI Agnosticism: Never use WPF types (e.g., MessageBox, Panel, TextBox) inside the ViewModel. 
  All UI interactions must go through interfaces or services (e.g., IDialogService).
- No Code-Behind: Only use XAML and Microsoft.Xaml.Behaviors.Wpf (EventToCommand) to trigger ViewModel logic. 
  Code-behind must be strictly reserved for View-specific visual setups.
- One-to-One Binding: Enforce one View-Model per View and rely purely on data binding (e.g., {Binding PropertyName}) 
  rather than accessing UI elements by their x:Name

## Winforms

- Do not create any new UI using Winforms.
- Legacy Winforms UI can be maintained.
