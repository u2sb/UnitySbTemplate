using System;
using System.Buffers;
using Utf8StringInterpolation;

namespace Sb.UnityKit.Editor.SbGameObject;

public static class CodeGenExt
{
  #region Utf8StringWriter拓展

  public static void AppendLineLF<T>(this ref Utf8StringWriter<T> writer, string? s) where T : IBufferWriter<byte>
  {
    writer.Append(s);
    writer.AppendUtf8("\n"u8);
  }

  public static void AppendLineLF<T>(this ref Utf8StringWriter<T> writer, ReadOnlySpan<byte> span)
    where T : IBufferWriter<byte>
  {
    writer.AppendUtf8(span);
    writer.AppendUtf8("\n"u8);
  }

  public static void AppendLineLF<T>(this ref Utf8StringWriter<T> writer) where T : IBufferWriter<byte>
  {
    writer.AppendUtf8("\n"u8);
  }

  #endregion
}