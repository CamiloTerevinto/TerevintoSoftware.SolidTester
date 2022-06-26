namespace SampleLibrary;

public interface INoDependenciesService
{
    Task NoOpAsyncMethod();
    void NoOpSyncMethod();
}

public class NoDependenciesService : INoDependenciesService
{
    public async Task NoOpAsyncMethod()
    {
        // no-op
        await Task.Delay(0);
    }

    public void NoOpSyncMethod()
    {

    }
}
