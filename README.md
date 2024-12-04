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

## Usage
Although you can directly use the `IRequest`, `IRequest<TResponse>`, `IRequestHandler<TRequest>` and `IRequestHandler<TRequest, TResponse>`
interfaces from **MediatR** to define your requests and request handlers, this package provides some base classes to help you define **contextual requests** and **contextual request handlers**. 

### 1. Define a Request Context
We can provide our requests with relevant information about the context in which they are being executed.
For instance, we can create a class to store information associated with the current user or service account executing our requests:
```csharp
public sealed class OperationContext
{
    public OperationContext(string serviceAccountId, string userId)
    {
        ServiceAccountId = serviceAccountId;
        UserId = userId;
    }
    
    // Identifies the application or service executing the request
    public string ServiceAccountId { get; } 
    
    // Identifies the user executing the request
    public string UserId { get; }
}
```


### 2. Define Some Queries
```csharp
// Queries/CustomersByRegion/CustomersByRegionQuery.cs
// using ...
using Flowsy.Mediation;
// using ...

public class CustomersByRegionQuery : ContextualRequest<OperationContext, IEnumerable<CustomerDto>>
{
    public string CountryId { get; set; } = string.Empty;
    public string? StateId { get; set; }
}
```

### 3. Define Some Query Validators
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
        RuleFor(query => query.Context)
            .NotNull()
            .WithMessage("Query context is required.");
        
        RuleFor(query => query.CountryId)
            .NotEmpty()
            .WithMessage("Country identifier is required.");
    }
}
```


### 4. Define Some Query Handlers
```csharp
// Queries/CustomersByRegion/CustomersByRegionQueryHandler.cs
// using ...
using Flowsy.Mediation;
// using ...

public class CustomersByRegionQueryHandler : ContextualRequestHandler<OperationContext, CustomerByIdQuery, IEnumerable<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    
    public CustomersByRegionQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    protected override async Task<IEnumerable<CustomerDto>> HandleAsync(CustomersByRegionQuery request, CancellationToken cancellationToken)
    {
        var context = request.Context;
        var serviceAccountId = context.ServiceAccountId;
        var userId = context.UserId;
        
        // Do something with serviceAccountId and userId, maybe check user or service account permissions
        
        var customers = await _customerRepository.GetManyAsync<Customer>(request.CountryId, request.StateId, cancellationToken);
        
        return customers.Select(c => {
            var customerDto = new CustomerDto();
            // Copy properties from c to customerDto
            return customerDto;
            }); 
    }
}
```

### 5. Define Some Commands
```csharp
// Commands/CreateCustomer/CreateCustomerCommand.cs
// using ...
using Flowsy.Mediation;
// using ...

public class CreateCustomerCommand : ContextualRequest<OperationContext, CreateCustomerCommandResult>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
```

### 6. Define Some Command Validators
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
        RuleFor(command => command.Context.UserId)
            .NotEmpty()
            .WithMessage("Request User ID is required.");
        
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

### 7. Define Some Command Results
```csharp
// Commands/CreateCustomer/CreateCustomerCommandResult.cs
public class CreateCustomerCommandResult
{
    public int CustomerId { get; set; }
}
```

### 8. Define Some Command Handlers
```csharp
// Commands/CreateCustomer/CreateCustomerCommandHandler.cs
// using ...
using Flowsy.Mediation;
// using ...

public class CreateCustomerCommandHandler : ContextualRequestHandler<OperationContext, CreateCustomerCommand, CreateCustomerCommandResult>
{
    private readonly ICustomerRepository _customerRepository;
    
    public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    protected override async Task<CreateCustomerCommandResult> HandleAsync(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var context = request.Context;
        var serviceAccountId = context.ServiceAccountId;
        var userId = context.UserId;
        
        // Do something with serviceAccountId and userId, maybe check user or service account permissions
        
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

### 9. Provide the Request Context
In order to provide our requests with their corresponding context, we must implement the **IRequestContextProvider** interface.

```csharp
// HttpRequestContextProvider.cs
// using ...
using Flowsy.Mediation;
// using ...

public sealed class HttpRequestContextProvider : IRequestContextProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public HttpRequestContextProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public object ProvideContext<TRequest>(TRequest request) where TRequest : IContextualRequest
    {
        var serviceAccountId = "";
        var userId = "";
        var user = _httpContextAccessor.HttpContext?.User;
        
        // Read information from user to resolve the service account ID and user ID
        
        return new OperationContext(serviceAccountId, userId);
    }
    
    public Task<object> ProvideContextAsync<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : IContextualRequest
        => Task.Run(() => ProvideContext(request), cancellationToken);
}
```

### 10. Register Queries and Commands
Add a reference to the assemblies containing the application logic and place this code in the Program.cs file.

```csharp
// Program.cs
// using ...
using Flowsy.Mediation;
using FluentValidation;
// using ...

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services
    .AddMediation(config => 
    {
        // Register requests from the required assemblies
        config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    })
    // Registers RequestContextResolutionBehavior to add context information to
    // every request using the specified implementation of IRequestContextProvider
    .UseRequestContext<HttpRequestContextProvider>()
    // Registers RequestValidationBehavior to validate every request
    .UseRequestValidation()
    // Registers RequestLoggingBehavior to log information for every request and its result
    .UseRequestLogging();

// Add other services

var app = builder.Build();

// Use services

app.Run();
```