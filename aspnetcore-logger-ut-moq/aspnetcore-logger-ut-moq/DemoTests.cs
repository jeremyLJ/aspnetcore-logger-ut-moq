using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace aspnetcore_logger_ut_moq;

public class DemoTests
{
    [Fact]
    public void TestDoSomething()
    {
        // Given
        Mock<ILogger<Demo>> loggerMock = new();
        var sut = new Demo(loggerMock.Object);

        // When
        sut.DoSomething();

        // Then
        // try #1 - failed
        // loggerMock.Verify(m => m.LogDebug(It.IsAny<string>()));

        // try #2 - failed
        // loggerMock.Verify(m => m.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(),
        //     It.IsAny<Exception?>(), It.IsAny<Func<FormattedLogValues, Exception?, string>>(), Times.Once()));
        
        // try #3 - succeed
        loggerMock.Verify(m => m.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        
        // try #4 - succeed - enhanced
        loggerMock.Verify(m => m.Log(
            It.Is<LogLevel>(level => level == LogLevel.Debug), 
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((obj, type) => obj.ToString() == "This is a debug log" && type.Name == "FormattedLogValues"),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        
        // try #5 - succeed - enhanced
        loggerMock.VerifyLogging("This is a debug log")
            .VerifyLogging("this is a info log", LogLevel.Information)
            .VerifyLogging("this is a warning log", LogLevel.Warning, Times.Once());
    }
}

public static class LogAssert
{
    public static Mock<ILogger<T>> VerifyLogging<T>(this Mock<ILogger<T>> loggerMock, string expectLogMessage, LogLevel expectLogLevel = LogLevel.Debug, Times? times = null)
    {
        times ??= Times.Once();

        Func<object, Type, bool> state = (obj, type) => obj.ToString()!.Equals(expectLogMessage);
        
        loggerMock.Verify(m => m.Log(
            It.Is<LogLevel>(level => level == expectLogLevel), 
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((obj, type) => state(obj, type)),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), (Times)times);

        return loggerMock;
    }
}