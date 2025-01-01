#if SBKIT_VYAML_ENABLE && SBKIT_ULID_ENABLE
using System;
using System.Buffers.Text;
using VYaml.Emitter;
using VYaml.Parser;
using VYaml.Serialization;

namespace Sb.UnityKit.VYaml.Formatters;

public class UlidVYamlFormatter : IYamlFormatter<Ulid>
{
  public static readonly UlidVYamlFormatter Instance = new();

  private readonly byte[] _buffer = new byte[26];

  public void Serialize(ref Utf8YamlEmitter emitter, Ulid value, YamlSerializationContext context)
  {
    if (value.TryWriteStringify(_buffer))
    {
      emitter.WriteScalar(_buffer);
      return;
    }

    throw new YamlSerializerException($"Cannot serialize {value}");
  }

  public Ulid Deserialize(ref YamlParser parser, YamlDeserializationContext context)
  {
    if (parser.TryGetScalarAsSpan(out var span))
    {
      parser.Read();
      if (Ulid.TryParse(span, out var ulid)) return ulid;

      // 兼容 guid
      if (Utf8Parser.TryParse(span, out Guid guid, out var bytesConsumed) &&
          bytesConsumed == span.Length) return new Ulid(guid);
    }

    throw new YamlSerializerException(
      $"Cannot detect a scalar value of Ulid : {parser.CurrentEventType} {parser.GetScalarAsString()}");
  }
}
#endif