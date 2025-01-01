#if SBKIT_VYAML_ENABLE
using Sb.UnityKit.VYaml.Formatters;
using UnityEngine;
using VYaml.Serialization;

namespace Sb.UnityKit.VYaml.Resolvers;

public class VYamlSbUnityResolver
{
  private static readonly IYamlFormatterResolver Resolver = CompositeResolver.Create(
    new IYamlFormatter[]
    {
#if SBKIT_ULID_ENABLE
      UlidVYamlFormatter.Instance
#endif
    },
    StandardResolver.DefaultResolvers);

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
  public static void Init()
  {
    YamlSerializer.DefaultOptions.Resolver = Resolver;
  }
}
#endif