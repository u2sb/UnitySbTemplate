using VContainer;
using VContainer.Unity;
using ZeroMessenger.VContainer;

namespace Ioc;

public class GameLifetimeScope : LifetimeScope
{
  protected override void Configure(IContainerBuilder builder)
  {
    builder.AddZeroMessenger();
    builder.AddZLogger();
  }
}