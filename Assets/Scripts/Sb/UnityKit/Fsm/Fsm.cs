using System.Collections.Generic;
using UnityEngine;

namespace Sb.UnityKit.Fsm
{
  public abstract class Fsm<T> : MonoBehaviour
  {
    protected T? CurrentState;

    protected virtual Dictionary<T, IState<T>?> States { get; } = new();

    /// <summary>
    ///   状态
    /// </summary>
    public T? State
    {
      get => CurrentState;
      set
      {
        Get(CurrentState)?.OnExit?.Invoke(this);
        CurrentState = value;
        Get(CurrentState)?.OnEnter?.Invoke(this);
      }
    }

    public IState<T>? this[T key] => Get(key);

    public IState<T>? Get(T? key)
    {
      return CurrentState == null ? null : States.GetValueOrDefault(key!);
    }

    public void Set(IState<T> state)
    {
      if (States.TryAdd(state.Key, state)) return;
      States[state.Key] = state;
    }

    #region 生命周期

    private void Update()
    {
      Get(CurrentState)?.OnUpdate?.Invoke(this);
    }

    private void LateUpdate()
    {
      Get(CurrentState)?.OnLateUpdate?.Invoke(this);
    }

    private void FixedUpdate()
    {
      Get(CurrentState)?.OnFixedUpdate?.Invoke(this);
    }

    #endregion
  }
}