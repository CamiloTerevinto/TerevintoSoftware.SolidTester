namespace SampleLibrary;

public interface INoDependenciesService
{
    Task NoOpAsyncMethod(string userId);
    void NoOpSyncMethod();
}

public class NoDependenciesService : INoDependenciesService
{
    public async Task NoOpAsyncMethod(string userId)
    {
        // no-op
        await Task.Delay(userId.Length);
    }

    public void NoOpSyncMethod()
    {

    }
}
