using Cysharp.Threading.Tasks;
using UnityEngine;

// ReSharper disable Unity.IncorrectMethodSignature
#pragma warning disable UNT0006

namespace Sb.UnityKit.SbGameObject;

public abstract class BaseSbGameObject : MonoBehaviour
{
  [field: SerializeField] public string[] Keys { get; protected set; } = { };
  public Vector3 Position0 { get; protected set; }
  public Vector3 LocalPosition0 { get; protected set; }
  public Vector3 EulerAngles0 { get; protected set; }
  public Vector3 LocalEulerAngles0 { get; protected set; }
  public Quaternion Rotation0 { get; protected set; }
  public Quaternion LocalRotation0 { get; protected set; }

  private async UniTaskVoid Awake()
  {
    Position0 = transform.position;
    LocalPosition0 = transform.localPosition;
    EulerAngles0 = transform.eulerAngles;
    LocalEulerAngles0 = transform.localEulerAngles;
    Rotation0 = transform.rotation;
    LocalRotation0 = transform.localRotation;

    OnAwake();
    await AfterAwakeAsync();
  }

  private async UniTaskVoid Start()
  {
    OnStart();
    await AfterStartAsync();
  }

  protected virtual void OnAwake()
  {
  }

  protected virtual async UniTask AfterAwakeAsync()
  {
    await UniTask.CompletedTask;
  }

  protected virtual void OnStart()
  {
  }

  protected virtual async UniTask AfterStartAsync()
  {
    await UniTask.CompletedTask;
  }
}