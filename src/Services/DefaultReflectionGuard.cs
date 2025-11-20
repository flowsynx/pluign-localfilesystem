using FlowSynx.PluginCore.Helpers;

namespace FlowSynx.Plugins.Local.Services;

internal class DefaultReflectionGuard : IReflectionGuard
{
    public bool IsCalledViaReflection() => ReflectionHelper.IsCalledViaReflection();
}