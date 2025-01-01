using UnityEngine;

namespace Sb.UnityKit.Singleton
{
  public class MonoSingleton<T> : MonoBehaviour, ISingleton<T> where T : MonoBehaviour
  {
    private static T _instance = null!;

    // ReSharper disable once InconsistentNaming
    // ReSharper disable once StaticMemberInGenericType
    private static readonly object _lock = new();

    // ReSharper disable once StaticMemberInGenericType
    private static bool _applicationIsQuitting;

    public static T Instance
    {
      get
      {
        lock (_lock)
        {
          if (_instance == null)
          {
            _instance = (T)FindObjectOfType(typeof(T));

            if (FindObjectsOfType(typeof(T)).Length > 1) return _instance;

            if (_instance == null)
            {
              var singleton = new GameObject()
              {
                name = "(singleton) " + typeof(T)
              };
              _instance = singleton.AddComponent<T>();

              DontDestroyOnLoad(singleton);
            }
          }

          return _instance;
        }
      }
    }

    public void OnDestroy()
    {
      _applicationIsQuitting = true;
    }

    public static bool IsDestroy()
    {
      return _applicationIsQuitting;
    }
  }
}