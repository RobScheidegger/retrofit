using Microsoft.EntityFrameworkCore;
using Python.Runtime;


namespace Retrofit.Services;

/// <summary>
/// Wrapper service to call Python ML Interop.
/// </summary>
public class RetrofitImageCaptioningService
{
    private RetrofitStatusService retrofitStatusService;
    private IDbContextFactory<RetrofitDbContext> contextFactory;
    private PyModule scope;

    public RetrofitImageCaptioningService(RetrofitStatusService retrofitStatusService, IDbContextFactory<RetrofitDbContext> contextFactory)
    {
        this.retrofitStatusService = retrofitStatusService;
        this.contextFactory = contextFactory;

        // Initialize the Python model
        PythonEngine.Initialize();
        scope = Py.CreateScope();
        dynamic os = scope.Import("os");
        dynamic sys = scope.Import("sys");
        sys.path.append(os.getcwd());
        scope.Import("models.caption_model", "caption_model");
        scope.Exec("caption_model = caption_model.RetrofitCaptionModel()");

    }

    /// <summary>
    /// Captions all of the un-captioned images 
    /// </summary>
    /// <returns></returns>
    public async Task CaptionUncaptionedImages()
    {
        if (!retrofitStatusService.TryChangeState(RetrofitServiceStatusEnum.Idle, RetrofitServiceStatusEnum.Captioning))
            return;

        retrofitStatusService.Progress = 0;

        using var context = contextFactory.CreateDbContext();
        var entries = await context.Entries.Where(e => e.Caption == null).ToListAsync();
        var total = entries.Count;
        var current = 0;

        foreach (var entry in entries)
        {
            var caption = CaptionImage(entry.Path);
            entry.Caption = caption;
            await context.SaveChangesAsync();
            current++;
            retrofitStatusService.Progress = (float)current / (float)total;
        }

        retrofitStatusService.TryChangeState(RetrofitServiceStatusEnum.Captioning, RetrofitServiceStatusEnum.Idle);
    }

    public string CaptionImage(string imagePath, string format = "jpg")
    {
        PyString image_path = new(imagePath);
        PyString image_format = new(format);

        var caption = scope.Get("caption_model").InvokeMethod("generate_image_caption", image_path, image_format);
        return caption.ToString()!;
    }
}