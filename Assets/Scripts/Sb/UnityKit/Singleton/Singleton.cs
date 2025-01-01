using System;
using System.Threading;
using JetBrains.Annotations;

namespace Sb.UnityKit.Singleton
{
  public interface ISingleton<T> where T : class
  {
    [UsedImplicitly] public static T Instance { get; } = null!;
  }


  public class Singleton<T> : ISingleton<T> where T : class, new()
  {
    // ReSharper disable once InconsistentNaming
    private static readonly Lazy<T> instance = new(() => new T());
    protected SemaphoreSlim SlimLock = new(1, 1);

    public static T Instance => instance.Value;
  }
}