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
This package relies on [MediatR](https://www.nuget.org/packages/MediatR) to set the foundation for infrastructure-decoupled applications.

## Usage
### 1. Define Some Queries
```csharp
// Queries/CustomersByRegion/CustomersByRegionQuery.cs
// using ...
using Flowsy.Mediation;
// using ...

public class CustomersByRegionQuery : Request<IEnumerable<CustomerDto>>
{
    public string CountryId { get; set; }
    public string StateId { get; set; }
}
```

### 2. Define Some Query Handlers
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
        // If InitializationBehavior is registered, every request contains the current user and culture.
    
        var customers = await _customerRepository.GetManyAsync<Customer>(request.CountryId, request.StateId, cancellationToken);
        
        return customers.Select(c => {
            var customerDto = new CustomerDto();
            // Copy properties from c to customerDto
            return customerDto;
            }); 
    }
}
```

### 3. Define Some Commands
```csharp
// Commands/CreateCustomer/CreateCustomerCommand.cs
// using ...
using Flowsy.Mediation;
// using ...

public class CreateCustomerCommand : Request<CreateCustomerCommandResult>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}
```

### 4. Define Some Command Handlers
```csharp
// Commands/CreateCustomer/CreateCustomerCommandResult.cs
public class CreateCustomerCommandResult
{
    public int CustomerId { get; set; }
}
```

### 5. Define Some Command Handlers
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
        // If InitializationBehavior is registered, every request contains the current user and culture.
        
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

### 6. Register Queries and Commands
Add a reference to the assemblies containing the application logic and place this code in the Program.cs file.

```csharp
// Program.cs
// using ...
using Flowsy.Mediation;
// using ...

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediation(
    true, // Register InitializationBehavior to set the current user and culture for every request
    typeof(CustomersByRegionQuery).Assembly, // Register queries and commands from this assembly
    typeof(CreateCustomerCommand).Assembly // Register queries and commands from this assembly
    );

// Add other services

var app = builder.Build();

// Use services

app.Run();
```