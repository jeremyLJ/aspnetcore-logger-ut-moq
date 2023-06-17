using Microsoft.Extensions.Logging;

namespace aspnetcore_logger_ut_moq;

public class Demo
{
    private readonly ILogger<Demo> _logger;

    public Demo(ILogger<Demo> logger)
    {
        _logger = logger;
    }

    public void DoSomething()
    {
        _logger.LogDebug("This is a debug log");
        
        _logger.LogInformation("this is a info log");
        
        _logger.LogWarning("this is a warning log");
    }
}