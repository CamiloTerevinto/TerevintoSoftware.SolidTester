namespace SampleLibrary;

public interface ISingleDependencyService
{
    Task ExecuteOperationAsync();
}

public class SingleDependencyService : ISingleDependencyService
{
    private readonly ISomeDependencyService _someDependencyService;

    public SingleDependencyService(ISomeDependencyService someDependencyService)
    {
        _someDependencyService = someDependencyService;
    }

    public async Task ExecuteOperationAsync()
    {
        // no-op
        await _someDependencyService.ExecuteOperationAsync("test");
    }
}
