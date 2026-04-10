using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace my_project_main.Tests.user_account_service.Tests.Support;

internal sealed class NeverChangesChangeToken : IChangeToken
{
    public static readonly NeverChangesChangeToken Instance = new();

    public bool HasChanged => false;

    public bool ActiveChangeCallbacks => false;

    public IDisposable RegisterChangeCallback(Action<object?> callback, object? state) => NoopDisposable.Instance;
}

internal sealed class NoopDisposable : IDisposable
{
    public static readonly NoopDisposable Instance = new();

    public void Dispose()
    {
    }
}

/// <summary>
/// Configuration plate pour <see cref="user_account_service.Config.JwtTokenService"/> dans les tests.
/// </summary>
internal sealed class TestConfiguration : IConfiguration
{
    private readonly IReadOnlyDictionary<string, string?> _values;

    public TestConfiguration(IReadOnlyDictionary<string, string?> values) => _values = values;

    public string? this[string key]
    {
        get => _values.TryGetValue(key, out var v) ? v : null;
        set => throw new NotSupportedException();
    }

    public IEnumerable<IConfigurationSection> GetChildren() => Array.Empty<IConfigurationSection>();

    public IChangeToken GetReloadToken() => NeverChangesChangeToken.Instance;

    public IConfigurationSection GetSection(string key) => new TestConfigurationSection(this, key);
}

internal sealed class TestConfigurationSection : IConfigurationSection
{
    private readonly IConfiguration _root;
    private readonly string _path;

    public TestConfigurationSection(IConfiguration root, string path)
    {
        _root = root;
        _path = path;
    }

    public string? this[string childKey]
    {
        get => _root[$"{_path}:{childKey}"];
        set => throw new NotSupportedException();
    }

    public string Key => _path.Split(':').Last();

    public string Path => _path;

    public string? Value
    {
        get => _root[_path];
        set => throw new NotSupportedException();
    }

    public IEnumerable<IConfigurationSection> GetChildren() => Array.Empty<IConfigurationSection>();

    public IChangeToken GetReloadToken() => NeverChangesChangeToken.Instance;

    public IConfigurationSection GetSection(string childKey) =>
        new TestConfigurationSection(_root, $"{_path}:{childKey}");
}
