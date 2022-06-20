namespace SampleLibrary;

public class SuperOptions
{
    public string UserId { get; set; }
}

public interface ISomeDependencyService
{
    Task ExecuteOperationAsync(string userId);
}

public class SomeDependencyService : ISomeDependencyService
{
    public async Task ExecuteOperationAsync(string userId) { await Task.Delay(userId.Length); }
}

public interface ISuperUsefulService
{
    Task ExecuteOperationAsync();
}

public class SuperUsefulService : ISuperUsefulService
{
    private readonly ISomeDependencyService _someDependencyService;
    private readonly SuperOptions _options;

    public SuperUsefulService(ISomeDependencyService someDependencyService, SuperOptions options)
    {
        _someDependencyService = someDependencyService;
        _options = options;
    }

    public async Task ExecuteOperationAsync()
    {
        await _someDependencyService.ExecuteOperationAsync(_options.UserId);
    }
}
