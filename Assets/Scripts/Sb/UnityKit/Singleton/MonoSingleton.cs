using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sb.UnityKit.Singleton;

public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton<T> where T : MonoBehaviour
{
  private static T _instance = null!;

  // ReSharper disable once InconsistentNaming
  // ReSharper disable once StaticMemberInGenericType
  private static readonly object _lock = new();

  // ReSharper disable once StaticMemberInGenericType
  private static bool _applicationIsQuitting;

  protected virtual bool DontDestroyInstanceOnLoad { get; } = false;

  public static T Instance
  {
    get
    {
      lock (_lock)
      {
        if (_instance) return _instance;

        _instance = FindAnyObjectByType<T>();

        if (_instance) return _instance;

        var singleton = new GameObject
        {
          name = "(singleton) " + typeof(T)
        };
        _instance = singleton.AddComponent<T>();

        return _instance;
      }
    }
  }

  // ReSharper disable once Unity.IncorrectMethodSignature
#pragma warning disable UNT0006
  private async UniTaskVoid Awake()
#pragma warning restore UNT0006
  {
    if (DontDestroyInstanceOnLoad) DontDestroyOnLoad(this);
    // ReSharper disable once MethodHasAsyncOverload
    OnAwake();
    await OnAwakeAsync();
  }


  public void OnDestroy()
  {
    _applicationIsQuitting = true;
  }

  protected virtual void OnAwake()
  {
  }

  protected virtual UniTask OnAwakeAsync()
  {
    return UniTask.CompletedTask;
  }

  public static bool IsDestroy()
  {
    return _applicationIsQuitting;
  }
}