using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Sb.UnityKit.NativeArray;

public static class NativeArrayExt
{
  /// <summary>
  ///   转换到 NativeArray
  /// </summary>
  /// <param name="span"></param>
  /// <param name="allocator"></param>
  /// <typeparam name="T"></typeparam>
  /// <returns></returns>
  public static unsafe NativeArray<T> AsNativeArray<T>(this ReadOnlySpan<T> span,
    Allocator allocator = Allocator.Persistent) where T : unmanaged
  {
    fixed (void* source = span)
    {
      var data = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(source, span.Length, allocator);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref data, AtomicSafetyHandle.Create());
#endif
      return data;
    }
  }

  /// <summary>
  ///   转换到 NativeArray
  /// </summary>
  /// <param name="span"></param>
  /// <param name="allocator"></param>
  /// <typeparam name="T"></typeparam>
  /// <returns></returns>
  public static unsafe NativeArray<T> AsNativeArray<T>(this Span<T> span,
    Allocator allocator = Allocator.Persistent) where T : unmanaged
  {
    fixed (void* source = span)
    {
      var data = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(source, span.Length, allocator);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref data, AtomicSafetyHandle.Create());
#endif
      return data;
    }
  }

  
}