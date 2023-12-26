using Python.Runtime;


namespace Retrofit.Services;

/// <summary>
/// Wrapper service to call Python ML Interop.
/// </summary>
public class RetrofitImageCaptioningService
{
    private RetrofitStatusService retrofitStatusService;
    private PyModule scope;

    public RetrofitImageCaptioningService(RetrofitStatusService retrofitStatusService)
    {
        this.retrofitStatusService = retrofitStatusService;

        // Initialize the Python model
        PythonEngine.Initialize();
        scope = Py.CreateScope();
        dynamic os = scope.Import("os");
        dynamic sys = scope.Import("sys");
        sys.path.append(os.getcwd());
        scope.Import("models.caption_model", "caption_model");
        scope.Exec("caption_model = caption_model.RetrofitCaptionModel()");
    }
}