using System.Buffers;
using System.IO;
using Cysharp.Threading.Tasks;
using VYaml.Serialization;

namespace Sb.UnityKit.VYaml;

public static class VYamlExt
{
  /// <summary>
  ///   保存到文件
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="path"></param>
  /// <param name="data"></param>
  /// <param name="options"></param>
  public static void WriteToFile<T>(string path, T data, YamlSerializerOptions? options = null)
  {
    var bytes = YamlSerializer.Serialize(data, options);
    File.WriteAllBytes(path, bytes.ToArray());
  }

  /// <summary>
  ///   保存到文件
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="path"></param>
  /// <param name="data"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static async UniTask WriteToFileAsync<T>(string path, T data, YamlSerializerOptions? options = null)
  {
    var writer = new ArrayBufferWriter<byte>(8192);
    await UniTask.RunOnThreadPool(() => YamlSerializer.Serialize(writer, data, options));
    await using var fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);
    fs.Seek(0, SeekOrigin.Begin);
    fs.SetLength(0);
    await fs.WriteAsync(writer.WrittenMemory);
    await fs.FlushAsync();
  }

  /// <summary>
  ///   从文件读取
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="path"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static T? ReadFromFile<T>(string path, YamlSerializerOptions? options = null)
  {
    if (!File.Exists(path)) return default;

    var bytes = File.ReadAllBytes(path);
    return YamlSerializer.Deserialize<T?>(bytes, options);
  }

  /// <summary>
  ///   从文件读取
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="path"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static async UniTask<T?> ReadFromFileAsync<T>(string path, YamlSerializerOptions? options = null)
  {
    if (!File.Exists(path)) return default;

    await using var fs = File.OpenRead(path);
    return await YamlSerializer.DeserializeAsync<T?>(fs);
  }
}