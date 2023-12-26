namespace Retrofit.Services;

public enum RetrofitServiceStatusEnum
{
    Idle,
    Ingesting,
    Captioning
}

public class RetrofitStatusService
{
    private Mutex mutex = new Mutex();
    public RetrofitServiceStatusEnum Status { get; private set; } = RetrofitServiceStatusEnum.Idle;

    public bool TryChangeState(RetrofitServiceStatusEnum state)
    {
        mutex.WaitOne();
        if (Status == RetrofitServiceStatusEnum.Idle)
        {
            Status = state;
            mutex.ReleaseMutex();
            return true;
        }
        mutex.ReleaseMutex();
        return false;
    }
}