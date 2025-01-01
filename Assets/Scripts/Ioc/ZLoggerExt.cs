using Microsoft.Extensions.Logging;
using VContainer;
using ZLogger.Unity;

namespace Ioc;

public static class ZLoggerExt
{
  public static ILoggerFactory LoggerFactory { get; private set; }

  public static ILogger Logger { get; private set; }

  public static ILogger<T> GetLogger<T>() where T : class
  {
    return LoggerFactory.CreateLogger<T>();
  }

  public static IContainerBuilder AddZLogger(this IContainerBuilder builder)
  {
    LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(logging =>
    {
      logging.SetMinimumLevel(LogLevel.Trace);
      logging.AddZLoggerUnityDebug();
    });

    Logger = LoggerFactory.CreateLogger("global");

    // 全局日志工厂
    builder.Register(_ => LoggerFactory, Lifetime.Singleton);

    return builder;
  }
}