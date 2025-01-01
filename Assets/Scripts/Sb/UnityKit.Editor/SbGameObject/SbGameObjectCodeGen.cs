using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Sb.UnityKit.SbGameObject;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utf8StringInterpolation;

namespace Sb.UnityKit.Editor.SbGameObject;

public class SbGameObjectCodeGen : UnityEditor.Editor
{
  [MenuItem("U2SB/SbGameObject/Scene GenCode")]
  public static void GenCode()
  {
    Debug.Log("正在生成缓存文件");

    var dir = Path.Combine(Application.dataPath, "Scripts", "SbGameObjectKeys");
    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

    var className = $"{SceneManager.GetActiveScene().name}SbGameObjectCode";
    var outputFile = Path.Combine(dir, $"{className}.cs");

    outputFile = new Uri(outputFile).LocalPath;

    var sbGameObjects =
      FindObjectsByType<BaseSbGameObject>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

    var kp = new Dictionary<string, List<string>>();

    if (sbGameObjects is not { Length: > 0 }) return;

    foreach (var sbGameObject in sbGameObjects)
    foreach (var key in sbGameObject.Keys)
    {
      var path = GetFullPath(sbGameObject.transform);
      if (kp.ContainsKey(key))
        kp[key].Add(path);
      else
        kp.Add(key, new List<string> { path });
    }

    ArrayBufferWriter<byte> stringBufferWriter = new(1024 * 8);
    var writer = Utf8String.CreateWriter(stringBufferWriter);

    writer.AppendLineLF("// ReSharper disable CheckNamespace"u8);
    writer.AppendLineLF("// ReSharper disable InconsistentNaming"u8);
    writer.AppendLineLF("// ReSharper disable IdentifierTypo"u8);
    writer.AppendLineLF("// ReSharper disable UnusedMember.Global"u8);
    writer.AppendLineLF();

    writer.AppendLineLF("using System.Collections.Generic;"u8);
    writer.AppendLineLF("namespace SbGameObjectKeys;"u8);
    writer.AppendLineLF();
    writer.AppendUtf8("public static class "u8);
    writer.AppendLineLF(CleanString(className));
    writer.AppendLineLF("{");

    foreach (var (key, value) in kp)
    {
      writer.AppendUtf8("  public static List<string> "u8);
      writer.Append(CleanString(key));
      writer.AppendLineLF(" { get; } = new()"u8);
      writer.AppendLineLF("  {"u8);
      foreach (var v in value)
      {
        writer.AppendUtf8("    \""u8);
        writer.Append(v);
        writer.AppendLineLF("\","u8);
      }

      writer.AppendLineLF("  };"u8);
    }

    writer.AppendLineLF("}"u8);

    writer.AppendLineLF();

    writer.AppendUtf8("public static class "u8);
    writer.Append(CleanString(className));
    writer.AppendLineLF("Key"u8);
    writer.AppendLineLF("{"u8);

    foreach (var (key, _) in kp)
    {
      writer.AppendUtf8("  public const string "u8);
      writer.Append(CleanString(key));
      writer.AppendUtf8(" = \""u8);
      writer.Append(key);
      writer.AppendLineLF("\";"u8);
    }

    writer.AppendLineLF("}"u8);

    writer.Flush();

    using (var fs = File.OpenWrite(outputFile))
    {
      fs.SetLength(0);
      fs.Write(stringBufferWriter.WrittenSpan);
    }

    writer.Dispose();

    Debug.Log(outputFile);
    Debug.Log("缓存文件生成成功");
  }


  #region 工具类

  private static readonly Regex LeadingDigitsRegex = new(@"^\d+", RegexOptions.Compiled);

  private static readonly Regex SpecialCharsRegex =
    new(@"[\s!@#$%^&*()_+{}|:<>?,./;' \[\] \\=\-~`]+", RegexOptions.Compiled);

  private static readonly Regex MultipleUnderscoresRegex = new(@"_+", RegexOptions.Compiled);

  /// <summary>
  ///   变量名清除
  /// </summary>
  /// <param name="input"></param>
  /// <returns></returns>
  private static string CleanString(string input)
  {
    // 去掉开头的数字
    input = LeadingDigitsRegex.Replace(input, "");

    // 替换特殊字符、空格和运算符为下划线
    input = SpecialCharsRegex.Replace(input, "_");

    // 移除多余的下划线
    input = MultipleUnderscoresRegex.Replace(input, "_");

    return input;
  }

  /// <summary>
  ///   获取完整路径
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  private static string GetFullPath(Transform obj)
  {
    var path = obj.name;
    var current = obj.transform;
    while (current.parent != null)
    {
      current = current.parent;
      path = current.name + "/" + path;
    }

    return path;
  }

  #endregion
}