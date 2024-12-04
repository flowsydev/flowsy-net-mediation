using System.Reflection;
using Flowsy.Mediation.Test.Simulation;
using Flowsy.Mediation.Test.Simulation.Commands.AddNumbers;
using Flowsy.Mediation.Test.Simulation.Commands.MultiplyNumbers;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;
using Xunit.Abstractions;
using Xunit.Priority;

namespace Flowsy.Mediation.Test;

public class MediationTest
{
    private readonly ITestOutputHelper _output;
    private readonly IServiceProvider _serviceProvider;
    
    public MediationTest(ITestOutputHelper output)
    {
        _output = output;
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(new JsonFormatter())
            .CreateLogger();
        
        var services = new ServiceCollection();
        
        services.AddLogging(config =>
        {
            config.ClearProviders();
            config.AddSerilog();
        });
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        services
            .AddMediation(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            })
            .UseRequestContext<OperationContextProvider>()
            .UseRequestLogging()
            .UseRequestValidation();
        
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    [Priority(1)]
    public async Task Should_Add_Numbers()
    {
        // Arrange
        await using var scope = _serviceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var addNumbersCommand = new AddNumbersCommand(1, 2);

        _output.WriteLine("Adding numbers {0} and {1}", addNumbersCommand.FirstNumber, addNumbersCommand.SecondNumber);
        
        // Act
        var result = await mediator.Send(addNumbersCommand, CancellationToken.None);
        _output.WriteLine("Result: {0}", result.Value);
        _output.WriteLine("Added by: {0}", result.AddedBy);
        
        // Assert
        Assert.Equal(3, result.Value);
        Assert.Equal("john.doe@example.com", result.AddedBy);
    }
    
    [Fact]
    [Priority(2)]
    public async Task Should_Validate_Numbers_When_Adding()
    {
        // Arrange
        await using var scope = _serviceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var addNumbersCommand = new AddNumbersCommand(1, -2);

        _output.WriteLine("Adding numbers {0} and {1}", addNumbersCommand.FirstNumber, addNumbersCommand.SecondNumber);
        
        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => mediator.Send(addNumbersCommand, CancellationToken.None));
        var errorList = exception.Errors.ToList();
        errorList.ForEach(error =>
        {
            _output.WriteLine("Property: {0}, Error: {1}", error.PropertyName, error.ErrorMessage);
        });
        
        // Assert
        Assert.Single(errorList, e => e.PropertyName == "SecondNumber" && e.ErrorMessage == "Second number must be greater than or equal to 0.");
    }

    [Fact]
    [Priority(3)]
    public async Task Should_Validate_Context_When_Multiplying()
    {
        // Arrange
        await using var scope = _serviceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var multiplyNumbersCommand = new MultiplyNumbersCommand(25, 3);

        _output.WriteLine("Multiplying numbers {0} and {1}", multiplyNumbersCommand.FirstNumber, multiplyNumbersCommand.SecondNumber);
        
        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => mediator.Send(multiplyNumbersCommand, CancellationToken.None));
        var errorList = exception.Errors.ToList();
        errorList.ForEach(error =>
        {
            _output.WriteLine("Property: {0}, Error: {1}", error.PropertyName, error.ErrorMessage);
        });
        
        // Assert
        Assert.Single(errorList, e => e.PropertyName == "Context" && e.ErrorMessage == "Context user email is required.");
    }
}