using Spectre.Console.Cli;

namespace PartyCli;

public sealed class MyTypeResolver : ITypeResolver, IDisposable
{
    private readonly IServiceProvider _provider;

    public MyTypeResolver(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public void Dispose()
    {
        if (_provider is IDisposable disposable) disposable.Dispose();
    }

    public object Resolve(Type type)
    {
        if (type == null) return null;

        var service = _provider.GetService(type);

        if (service == null) throw new InvalidOperationException($"Could not resolve type '{type.FullName}'.");

        return _provider.GetService(type);
    }
}