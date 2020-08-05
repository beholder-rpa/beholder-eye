namespace beholder_eye_tests
{
    using Microsoft.Extensions.Logging;
    using System;

    public interface ILoggerAdapter<T>
    {
        void LogInformation(string message);

        void LogError(Exception ex, string message, params object[] args);
    }

    public class LoggerAdapter<T> : ILoggerAdapter<T>
    {
        private readonly ILogger<T> _logger;

        public LoggerAdapter(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogError(Exception ex, string message, params object[] args)
        {
            Console.WriteLine($"{ex} - {message} {args}");
            _logger.LogError(ex, message, args);
        }

        public void LogInformation(string message)
        {
            Console.WriteLine(message);
            _logger.LogInformation(message);
        }
    }
}
