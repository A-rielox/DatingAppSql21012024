using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using DatingAppSql21012024.Helpers;
using DatingAppSql21012024.Interfaces;
using Microsoft.Extensions.Options;

namespace DatingAppSql21012024.Services;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;
    // para pasar la configuracion cuando esta a traves de una class
    public PhotoService(IOptions<CloudinarySettings> config)
    {
        var acc = new Account
            (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

        _cloudinary = new Cloudinary(acc);
    }

    ///////////////////////////////////////
    /// ///////////////////////////////////////
    /// 
    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();

        if (file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                Folder = "datingAppSql21012024"
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;
    }

    ///////////////////////////////////////
    /// ///////////////////////////////////////
    /// 
    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);

        var result = await _cloudinary.DestroyAsync(deleteParams);

        return result;
    }
}
