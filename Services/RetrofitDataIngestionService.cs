
using Retrofit.Data;

namespace Retrofit.Services;
/// <summary>
/// Data service to handle the ingestion of data from the filesystem to the database.
/// </summary>
public class RetrofitDataIngestionService
{
    private readonly RetrofitDbContext _context;
    private readonly RetrofitImageCaptioningService _captioningService;

    public RetrofitDataIngestionService(RetrofitDbContext context, RetrofitImageCaptioningService captioningService)
    {
        _context = context;
        _captioningService = captioningService;
    }

    public async Task IngestData(string path)
    {
        var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var extension = Path.GetExtension(file).ToLower();
            var name = Path.GetFileNameWithoutExtension(file);
            var type = extension switch
            {
                ".jpg" => RetrofitType.Image,
                ".png" => RetrofitType.Image,
                ".mp4" => RetrofitType.Video,
                _ => RetrofitType.Unknown
            };

            int width = 0;
            int height = 0;

            if (type == RetrofitType.Image)
            {
                using var image = SixLabors.ImageSharp.Image.Load(file);
                width = image.Width;
                height = image.Height;
            }

            var entry = new RetrofitEntry
            {
                Width = width,
                Height = height,
                Name = name,
                Path = file,
                Type = type,
                Extension = extension
            };

            _context.Entries.Add(entry);
        }

        await _context.SaveChangesAsync();
    }
}