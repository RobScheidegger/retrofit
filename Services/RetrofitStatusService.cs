namespace Retrofit.Services;

public enum RetrofitServiceStatusEnum
{
    Idle,
    Ingesting,
    Captioning
}

public class RetrofitStatusService
{
    private readonly Mutex mutex = new();
    public RetrofitServiceStatusEnum Status { get; private set; } = RetrofitServiceStatusEnum.Idle;
    public float Progress { get; set; } = 0.0f;
    public string Operation { get; set; } = "None";

    public bool TryChangeState(RetrofitServiceStatusEnum oldState, RetrofitServiceStatusEnum newState)
    {
        mutex.WaitOne();
        if (Status == oldState)
        {
            Status = newState;
            mutex.ReleaseMutex();
            return true;
        }
        mutex.ReleaseMutex();
        return false;
    }
}