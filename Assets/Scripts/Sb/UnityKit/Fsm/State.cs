using System;

namespace Sb.UnityKit.Fsm
{
  public interface IState<T>
  {
    /// <summary>
    ///   进入时
    /// </summary>
    public Action<Fsm<T>>? OnEnter { get; set; }


    /// <summary>
    ///   Update
    /// </summary>
    public Action<Fsm<T>>? OnUpdate { get; set; }


    /// <summary>
    ///   LateUpdate
    /// </summary>
    public Action<Fsm<T>>? OnLateUpdate { get; set; }


    /// <summary>
    ///   FixedUpdate
    /// </summary>
    public Action<Fsm<T>>? OnFixedUpdate { get; set; }


    /// <summary>
    ///   退出时
    /// </summary>
    public Action<Fsm<T>>? OnExit { get; set; }

    /// <summary>
    ///   Key
    /// </summary>
    public T Key { get; }
  }

  public class State<T> : IState<T>
  {
    public State(T vEnum)
    {
      Key = vEnum;
    }

    public Action<Fsm<T>>? OnEnter { get; set; }
    public Action<Fsm<T>>? OnUpdate { get; set; }
    public Action<Fsm<T>>? OnLateUpdate { get; set; }
    public Action<Fsm<T>>? OnFixedUpdate { get; set; }
    public Action<Fsm<T>>? OnExit { get; set; }
    public T Key { get; }
  }
}