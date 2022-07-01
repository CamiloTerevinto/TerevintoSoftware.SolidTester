namespace SampleLibrary;

public interface ISingleDependencyService
{
    Task ExecuteOperationAsync();
}

public class SingleDependencyService : ISingleDependencyService
{
    private readonly INoDependenciesService _dependency;

    public SingleDependencyService(INoDependenciesService someDependencyService)
    {
        _dependency = someDependencyService;
    }

    public async Task ExecuteOperationAsync()
    {
        // no-op
        await _dependency.NoOpAsyncMethod("test");
    }
}
