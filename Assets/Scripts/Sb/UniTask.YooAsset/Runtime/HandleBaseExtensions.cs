// #if UNITY_2020_1_OR_NEWER && ! UNITY_2021
// #define UNITY_2020_BUG
// #endif

// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace

using System;
using System.Runtime.CompilerServices;
using YooAsset;
using static Cysharp.Threading.Tasks.Internal.Error;

namespace Cysharp.Threading.Tasks;

public static class HandleBaseExtensions
{
  public static UniTask.Awaiter GetAwaiter(this HandleBase handle)
  {
    return ToUniTask(handle).GetAwaiter();
  }

  public static UniTask ToUniTask(this HandleBase handle,
    IProgress<float> progress = null,
    PlayerLoopTiming timing = PlayerLoopTiming.Update)
  {
    ThrowArgumentNullException(handle, nameof(handle));

    if (!handle.IsValid) return UniTask.CompletedTask;

    return new UniTask(
      HandleBaserConfiguredSource.Create(
        handle,
        timing,
        progress,
        out var token
      ),
      token
    );
  }

  private sealed class HandleBaserConfiguredSource : IUniTaskSource,
    IPlayerLoopItem,
    ITaskPoolNode<HandleBaserConfiguredSource>
  {
    private static TaskPool<HandleBaserConfiguredSource> pool;

    private readonly Action<HandleBase> continuationAction;
    private bool completed;
    private UniTaskCompletionSourceCore<AsyncUnit> core;
    private HandleBase handle;

    private HandleBaserConfiguredSource nextNode;
    private IProgress<float> progress;

    static HandleBaserConfiguredSource()
    {
      TaskPool.RegisterSizeGetter(typeof(HandleBaserConfiguredSource), () => pool.Size);
    }

    private HandleBaserConfiguredSource()
    {
      continuationAction = Continuation;
    }

    public bool MoveNext()
    {
      if (completed)
      {
        TryReturn();
        return false;
      }

      if (handle.IsValid) progress?.Report(handle.Progress);

      return true;
    }

    public ref HandleBaserConfiguredSource NextNode => ref nextNode;

    public UniTaskStatus GetStatus(short token)
    {
      return core.GetStatus(token);
    }

    public void OnCompleted(Action<object> continuation, object state, short token)
    {
      core.OnCompleted(continuation, state, token);
    }

    public void GetResult(short token)
    {
      core.GetResult(token);
    }

    public UniTaskStatus UnsafeGetStatus()
    {
      return core.UnsafeGetStatus();
    }

    public static IUniTaskSource Create(HandleBase handle,
      PlayerLoopTiming timing,
      IProgress<float> progress,
      out short token)
    {
      if (!pool.TryPop(out var result)) result = new HandleBaserConfiguredSource();

      result.handle = handle;
      result.progress = progress;
      result.completed = false;
      TaskTracker.TrackActiveTask(result, 3);

      if (progress != null) PlayerLoopHelper.AddAction(timing, result);

      // BUG 在 Unity 2020.3.36 版本测试中, IL2Cpp 会报 如下错误
      // BUG ArgumentException: Incompatible Delegate Types. First is System.Action`1[[YooAsset.AssetHandle, YooAsset, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]] second is System.Action`1[[YooAsset.HandleBase, YooAsset, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
      // BUG 也可能报的是 Action '1' Action '1' 的 InvalidCastException
      // BUG 此处不得不这么修改, 如果后续 Unity 修复了这个问题, 可以恢复之前的写法 
#if UNITY_2020_BUG
      switch (handle)
      {
        case AssetHandle asset_handle:
          asset_handle.Completed += result.AssetContinuation;
          break;
        case SceneHandle scene_handle:
          scene_handle.Completed += result.SceneContinuation;
          break;
        case SubAssetsHandle sub_asset_handle:
          sub_asset_handle.Completed += result.SubContinuation;
          break;
        case RawFileHandle raw_file_handle:
          raw_file_handle.Completed += result.RawFileContinuation;
          break;
      }
#else
      switch (handle)
      {
        case AssetHandle asset_handle:
          asset_handle.Completed += result.continuationAction;
          break;
        case SceneHandle scene_handle:
          scene_handle.Completed += result.continuationAction;
          break;
        case SubAssetsHandle sub_asset_handle:
          sub_asset_handle.Completed += result.continuationAction;
          break;
        case RawFileHandle raw_file_handle:
          raw_file_handle.Completed += result.continuationAction;
          break;
      }
#endif
      token = result.core.Version;

      return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void BaseContinuation()
    {
      if (completed)
      {
        TryReturn();
      }
      else
      {
        completed = true;
        if (handle.Status == EOperationStatus.Failed)
          core.TrySetException(new Exception(handle.LastError));
        else
          core.TrySetResult(AsyncUnit.Default);
      }
    }

    private void Continuation(HandleBase _)
    {
      switch (handle)
      {
        case AssetHandle asset_handle:
          asset_handle.Completed -= continuationAction;
          break;
        case SceneHandle scene_handle:
          scene_handle.Completed -= continuationAction;
          break;
        case SubAssetsHandle sub_asset_handle:
          sub_asset_handle.Completed -= continuationAction;
          break;
        case RawFileHandle raw_file_handle:
          raw_file_handle.Completed -= continuationAction;
          break;
      }

      BaseContinuation();
    }

    private bool TryReturn()
    {
      TaskTracker.RemoveTracking(this);
      core.Reset();
      handle = null;
      progress = null;
      return pool.TryPush(this);
    }
#if UNITY_2020_BUG
    private void AssetContinuation(AssetHandle handle)
    {
      handle.Completed -= AssetContinuation;
      BaseContinuation();
    }

    private void SceneContinuation(SceneHandle handle)
    {
      handle.Completed -= SceneContinuation;
      BaseContinuation();
    }

    private void SubContinuation(SubAssetsHandle handle)
    {
      handle.Completed -= SubContinuation;
      BaseContinuation();
    }

    private void RawFileContinuation(RawFileHandle handle)
    {
      handle.Completed -= RawFileContinuation;
      BaseContinuation();
    }
#endif
  }
}