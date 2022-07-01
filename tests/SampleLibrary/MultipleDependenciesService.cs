namespace SampleLibrary;

public class SuperOptions
{
    public string UserId { get; set; }
}

public interface IMultipleDependenciesService
{
    Task ExecuteOperationAsync();
}

public class MultipleDependenciesService : IMultipleDependenciesService
{
    private readonly INoDependenciesService _dependency;
    private readonly SuperOptions _options;

    public MultipleDependenciesService(INoDependenciesService noDependenciesService, SuperOptions options)
    {
        _dependency = noDependenciesService;
        _options = options;
    }

    public async Task ExecuteOperationAsync()
    {
        await _dependency.NoOpAsyncMethod(_options.UserId);
    }

    public static void SomeStaticMethod()
    {
        Console.WriteLine("That does nothing useful");
    }
}
