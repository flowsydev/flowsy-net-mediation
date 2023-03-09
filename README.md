# Flowsy Mediation

## Concepts
The mediation pattern allows us to separate the core of our application from the infrastructure that supports it.
A good way to achieve this separation is to think of our application as a model of the business processes from the real world, in which
users ask for something to be done and expect some kind of result from the requested operation.

This way, a common application flow would be:
1. User requests some action to be executed.
2. The application validates the request and returns some error information to the user in case of invalid input.
3. The application executes the requested operation.
4. The application returns to the user the result of the operation or some error information if something went wrong during the process.

To go even further, we can split those requests in two types:
* Query: A request for reading data with no alteration of the application's current state. 
* Command: A request for creating, updating or removing data, thus altering the application's current state.

## Dependencies
This package relies on other packages to set the foundation for infrastructure-decoupled applications:
* [MediatR](https://www.nuget.org/packages/MediatR)
* [FluentValidation](https://www.nuget.org/packages/FluentValidation)
* [Serilog](https://www.nuget.org/packages/Serilog)

## Usage
### 1. Define Some Queries
```csharp
// Queries/CustomersByRegion/CustomersByRegionQuery.cs
// using ...
using Flowsy.Mediation;
// using ...

public class CustomersByRegionQuery : Request<IEnumerable<CustomerDto>>
{
    public string CountryId { get; set; } = string.Empty;
    public string? StateId { get; set; }
}
```

### 2. Define Some Query Validators
```csharp
// Queries/CustomersByRegion/CustomersByRegionQueryValidator.cs
// using ...
using Flowsy.Mediation;
using FluentValidation;
// using ...

public class CustomersByRegionQueryValidator : AbstractValidator<CustomersByRegionQuery>
{
    public CustomersByRegionQueryValidator()
    {
        RuleFor(query => query.CountryId)
            .NotEmpty()
            .WithMessage("Country identifier is required.");
    }
}
```


### 3. Define Some Query Handlers
```csharp
// Queries/CustomersByRegion/CustomersByRegionQueryHandler.cs
// using ...
using Flowsy.Mediation;
// using ...

public class CustomersByRegionQueryHandler : RequestHandler<CustomerByIdQuery, IEnumerable<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    
    public CustomersByRegionQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    protected override async Task<IEnumerable<CustomerDto>> HandleAsync(CustomersByRegionQuery request, CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetManyAsync<Customer>(request.CountryId, request.StateId, cancellationToken);
        
        return customers.Select(c => {
            var customerDto = new CustomerDto();
            // Copy properties from c to customerDto
            return customerDto;
            }); 
    }
}
```

### 4. Define Some Commands
```csharp
// Commands/CreateCustomer/CreateCustomerCommand.cs
// using ...
using Flowsy.Mediation;
// using ...

public class CreateCustomerCommand : Request<CreateCustomerCommandResult>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
```

### 5. Define Some Command Validators
```csharp
// Commands/CreateCustomer/CreateCustomerCommandValidator.cs
// using ...
using Flowsy.Mediation;
using FluentValidation;
// using ...

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(command => command.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.");
            
        RuleFor(command => command.Email)
            .EmailAddress()
            .WithMessage("Invalid email address")
            .DependentRules(() => 
            {
                RuleFor(command => command.Email)
                    .NotEmpty()
                    .WithMessage("Email is required.")
                    .MaximumLength(320)
                    .WithMessage("Up to 320 characters.");
            });
    }
}
```

### 6. Define Some Command Results
```csharp
// Commands/CreateCustomer/CreateCustomerCommandResult.cs
public class CreateCustomerCommandResult
{
    public int CustomerId { get; set; }
}
```

### 7. Define Some Command Handlers
```csharp
// Commands/CreateCustomer/CreateCustomerCommandHandler.cs
// using ...
using Flowsy.Mediation;
// using ...

public class CreateCustomerCommandHandler : RequestHandler<CreateCustomerCommand, CreateCustomerCommandResult>
{
    private readonly ICustomerRepository _customerRepository;
    
    public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    protected override async Task<CreateCustomerCommandResult> HandleAsync(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer();
        
        // Copy properties from request to customer
    
        await _customerRepository.CreateAsync(customer, cancellationToken);
        
        return new CreateCustomerCommandResult
        {
            CustomerId = customer.CustomerId
        }; 
    }
}
```

### 8. Resolve the Current User
The current user must be resolved from the context of every request being executed.

The following example shows the common use case for a web application.

```csharp
// HttpRequestUserResolver.cs
// using ...
using Flowsy.Mediation;
// using ...

public class HttpRequestUserResolver : IRequestUserResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public HttpRequestUserResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public Task<ClaimsPrincipal?> GetUserAsync<TRequest, TResult>(TRequest request, CancellationToken cancellationToken)
        where TRequest : Request<TResult>, IRequest<TResult>
    {
        return Task.Run<ClaimsPrincipal>((Func<ClaimsPrincipal>) (() => _httpContextAccessor.HttpContext?.User), cancellationToken);
    }
}
```

### 9. Register Queries and Commands
Add a reference to the assemblies containing the application logic and place this code in the Program.cs file.

```csharp
// Program.cs
// using ...
using Flowsy.Mediation;
// using ...

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IRequestUserResolver, HttpRequestUserResolver>();

builder.Services
    .AddMediation(
        typeof(CustomersByRegionQuery).Assembly, // Register queries and commands from this assembly
        typeof(CreateCustomerCommand).Assembly // Register queries and commands from this assembly
        // Register queries and commands from others assemblies
    )
    // Registers RequestUserResolutionBehavior to set the current user for every request
    .AddRequestUser()
    // Registers RequestValidationBehavior to validate every request
    .AddValidation()
    // Registers RequestLoggingBehavior to log information for every request and its result
    .AddLogging();

// Add other services

var app = builder.Build();

// Use services

app.Run();
```