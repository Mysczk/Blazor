using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class ImageService
{
    private readonly string _imageFolderPath;

    public ImageService(string imageFolderPath)
    {
        _imageFolderPath = imageFolderPath;
    }

    public Task<List<string>> GetImagePathsAsync()
    {
        var images = new List<string>();

        if (Directory.Exists(_imageFolderPath))
        {
            var files = Directory.GetFiles(_imageFolderPath, "*.jpg");
            foreach (var file in files)
            {
                images.Add($"images/gallery/{Path.GetFileName(file)}");
            }
        }

        return Task.FromResult(images);
    }
}
